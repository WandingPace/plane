using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBoss : MonoBehaviour
{
    public GameObject Ripslinger;
    public GameObject Zed;
    public GameObject Ned;
    public ParticleSystem AlarmFx1;
    public ParticleSystem AlarmFx2;
    public float playerSpeedBefore;     //开始前玩家速度
    public float playerMaxSpeedBefore;  //开始前玩家最大速度
    public GameObject Bomb;
    public GameObject Bomb2;
    public ParticleSystem BombFx;

    private BoxCollider _collider = null;
    private GameObject _Ripslinger;
    private GameObject _Zed;
    private GameObject _Ned;
    private ParticleSystem _BombFx;

    private float safeDistance = 24f;
    private int battleStep;
    private int loopCount;
    private int hurtCount;
    private GamePlayer player;
    private bool willDie;
    private SpawnPool pool;
    private List<GameObject> Enemies = new List<GameObject>();
    private float nextStepTick;

    private Queue<BattleAction> battle;
    private BattleAction curBattleAction;
    public class BattleAction
    {
        public float delay;
        public System.Action action;

        public BattleAction(float _delay, System.Action _action)
        {
            delay = _delay;
            action = _action;
        }
    }


    private bool CanRunning
    {
        get
        {
            if (!enabled || Time.timeScale == 0f)
                return false;

            if (player.Hold || player.Dying)
                return false;

            return true;
        }
    }
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
    private void Update()
    {
        if (!CanRunning)
            return;

        if (GamePlayer.SharedInstance.Dying)
        {
            return;
        }

        if (!enabled || Time.timeScale == 0f)
            return;

        var velocity = player.getRunVelocity();
        // 与主角同速度向前飞行
        if (battleStep == 1)
        {
            //更新位置信息
            _Ripslinger.transform.position = Vector3.Lerp(_Ripslinger.transform.position,
                _Ripslinger.transform.position + _Ripslinger.transform.forward*velocity,
                Time.deltaTime);
            _Zed.transform.position = Vector3.Lerp(_Zed.transform.position,
                _Zed.transform.position + _Zed.transform.forward*velocity,
                Time.deltaTime);
            _Ned.transform.position = Vector3.Lerp(_Ned.transform.position,
                _Ned.transform.position + _Ned.transform.forward*velocity,
                Time.deltaTime);
        }
        

        if (battleStep == 2)
        {
            _Ripslinger.transform.position = Vector3.Lerp(_Ripslinger.transform.position,
                _Ripslinger.transform.position + _Ripslinger.transform.forward*velocity,
                Time.deltaTime);

            nextStepTick += Time.deltaTime;
            if (nextStepTick > curBattleAction.delay)
            {
                curBattleAction.action.Invoke();
                if (battle.Count > 0)
                    curBattleAction = battle.Dequeue();
            }
            return;
        }

        if (battleStep == 3)
        {
            PlayBossSound(2);

            battleStep++;
            nextStepTick = 0.3f;

            return;
        }

        nextStepTick -= Time.deltaTime;

        if (battleStep == 4)
        {
            //nextStepTick = 2f; //减速持续时间
            _Ripslinger.transform.position = Vector3.Lerp(_Ripslinger.transform.position,
                player.OriginalPosition + player.CachedTransform.forward*15, Time.deltaTime*2f);

            if (nextStepTick <= -2f)
            {
                battleStep++;
                loopCount = 0;
            }
            else
                return;
        }

        if (battleStep == 5)
        {
            _Ripslinger.transform.position = Vector3.Lerp(_Ripslinger.transform.position,
                _Ripslinger.transform.position + _Ripslinger.transform.forward*velocity,
                Time.deltaTime);

            if(nextStepTick < 0)
            {
                loopCount++;
                nextStepTick = loopCount > 4 ? 1f : 1.2f;

                if (loopCount == 1 || loopCount == 3)
                {
                    StartCoroutine(throwBomb(Bomb, GameController.SharedInstance.xOffset, false));
                }
                else if (loopCount == 2 || loopCount == 4)
                {
                    List<float> offset = new List<float>() {-1, 0, 1};
                    float _offset = GameController.SharedInstance.xOffset;
                    StartCoroutine(throwBomb(Bomb, _offset, false));

                    offset.RemoveAt((int) _offset + 1);
                    _offset = offset[Random.Range(0, 2)];
                    StartCoroutine(throwBomb(Bomb, _offset, false));
                }
                else if (loopCount == 5)
                {
                    List<float> offset = new List<float>() {-1, 0, 1};
                    float _offset = GameController.SharedInstance.xOffset;
                    StartCoroutine(throwBomb(Bomb, _offset, false));

                    offset.RemoveAt((int) _offset + 1);
                    _offset = offset[Random.Range(0, 2)];
                    StartCoroutine(throwBomb(Bomb2, _offset, true));
                }
                else if (loopCount == 6)
                {
                    List<float> offset = new List<float>() {-1, 0, 1};
                    float _offset = GameController.SharedInstance.xOffset;
                    StartCoroutine(throwBomb(Bomb2, _offset, true));

                    offset.RemoveAt((int) _offset + 1);
                    _offset = offset[Random.Range(0, 2)];
                    StartCoroutine(throwBomb(Bomb, _offset, false));
                }
                else if (loopCount == 7)
                {
                    List<float> offset = new List<float>() {-1, 0, 1};
                    float _offset = offset[Random.Range(0, 3)];
                    StartCoroutine(throwBomb(Bomb2, _offset, true));
                }

                if (loopCount > 6)
                    loopCount = 0;
            }

            return;
        }


        if (battleStep == 6)
        {
            battleStep++;
            //if (loopCount > 5)
            //    StartCoroutine(Exit(false));
            //else if (hurtCount > 2)
                StartCoroutine(Exit(true));
        }

        //if (battleStep == 15)
        //{
        //    _Ripslinger.transform.position = Vector3.Lerp(_Ripslinger.transform.position,
        //        _Ripslinger.transform.position + _Ripslinger.transform.forward*30,
        //        Time.deltaTime);

        //    return;
        //}

    }

    public void Start(TrackPiece tp)
    {
        if (pool == null)
            pool = PoolManager.Pools["npc"];

        GameController.SharedInstance.CurrentMaxSpeed = 15f;
        GamePlayer.SharedInstance.SetMaxRunVelocity(15);
        GamePlayer.SharedInstance.SetPlayerVelocity(15);

        battleStep = 0;
        hurtCount = loopCount = 0;
        player = GamePlayer.SharedInstance;

        transform.position = tp.transform.position;
        transform.localRotation = tp.transform.localRotation;
        transform.localScale = player.transform.localScale;

        //if (_collider == null)
        //    _collider = gameObject.AddComponent<BoxCollider>();
        //_collider.size = new Vector3(10, 10, 1);
        //_collider.enabled = true;
        if (Enemies == null)
            Enemies = new List<GameObject>();

        gameObject.SetActive(true);
        StartCoroutine(waitToMove());
        UIManagerOz.SharedInstance.inGameVC.HidePauseButton();
    }

    private void OnTriggerEnter(Collider other)
    {
        _collider.enabled = false;

        StartCoroutine(waitToMove());
    }

    private IEnumerator waitToMove()
    {
        PlayBossSound(1);

        transform.Rotate(Vector3.up, 180f);

        _Ripslinger = Instantiate(Ripslinger) as GameObject;
        _Ripslinger.transform.SetParent(transform);
        _Ripslinger.transform.ResetTransformation();
        _Ripslinger.transform.localScale = Vector3.one * 4f;
        //_Ripslinger.transform.SetLocalPositionY(-3f);

        //GameObject collider = new GameObject();
        //collider.layer = LayerMask.NameToLayer("stumbleColliders");
        //collider.transform.SetParent(_Ripslinger.transform);
        //collider.transform.ResetTransformation();
        //var c = collider.AddComponent<BoxCollider>();
        //c.size = Vector3.one;
        //c.center = Vector3.up * 0.5f;

        _BombFx = Instantiate(BombFx) as ParticleSystem;
        _BombFx.transform.SetParent(_Ripslinger.transform);
        _BombFx.transform.ResetTransformation();
        _BombFx.transform.SetLocalPositionY(0.6f);
        _BombFx.Stop(true);
        _BombFx.Clear(true);

        _Zed = pool.Spawn(Zed.transform).gameObject;
        iTween.Stop(_Zed);
        _Zed.transform.SetParent(transform);
        _Zed.transform.ResetTransformation();

        _Ned = pool.Spawn(Ned.transform).gameObject;
        iTween.Stop(_Ned);
        _Ned.transform.SetParent(transform);
        _Ned.transform.ResetTransformation();

        _Zed.transform.SetLocalPositionX(1f);
        _Ripslinger.transform.SetLocalPositionX(0f);
        _Ned.transform.SetLocalPositionX(-1f);

        doPlayAnimation(_Ripslinger, GamePlayer.AnimType.kRun);
        doPlayAnimation(_Zed, GamePlayer.AnimType.kRun);
        doPlayAnimation(_Ned, GamePlayer.AnimType.kRun);

        transform.position = transform.position - transform.forward * 5f + transform.up * 3; //身后5米远3米高

//        //如果是第一次开始这个模式,则显示教程
//        bool showTutorial = true;
//        //当教程显示状态为true时则一直等待
//        while (showTutorial) 
//        {
//            yield return new WaitForEndOfFrame();
//            showTutorial = false;
//        }
        //2秒后启动教程引导
        iTween.ValueTo(gameObject,iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time",0.1f,
            "delay",2f,
            "ignoretimescale", true,
            "onupdate","ShowBossGuide",
            "onupdatetarget", UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.gameObject
            ));

        //缓冲阶段从缓冲区起点移动到追逐区前方30米(4秒角色移动距离+安全距25离)
        var distance = player.getRunVelocity() * 4f + safeDistance;
        iTween.MoveTo(gameObject,
             iTween.Hash("position", player.CurrentPosition + player.transform.forward * distance,
                 "time", 4f,
                 "oncomplete", "BuffOver",
                 "oncompletetarget", gameObject,
                 "easetype", iTween.EaseType.linear));

        OzGameCamera.SharedInstance.Shake(0.015f, 1.5f, 1.5f, 1.5f);

        yield break;
    }

    private void BuffOver()
    {
        battleStep = 1;
        //开始3秒倒数开始boss战
        StartCoroutine(countDown(3f));
    }

    private IEnumerator countDown(float num)
    {
        float _num = num;

        //倒数时保持与角色同速移动
        while (_num > 0)
        {
            UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowCountDown(Mathf.CeilToInt(_num).ToString());
            _num -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        GameController.SharedInstance.Hold = false;
        UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowGo();
        UIManagerOz.SharedInstance.inGameVC.ShowPauseButton();

        battle = new Queue<BattleAction>();
        battleStep = 2;

        Battle1();
    }

    private void Battle1()
    {
        //旋转180度后逆向飞行
        _Zed.transform.Rotate(Vector3.up, 180f);
        _Ned.transform.Rotate(Vector3.up, 180f);

        // 飞到玩家身后10米位置
        float distance = safeDistance;
        // 等待时间需要加上飞行时间
        iTween.MoveTo(_Zed,
            iTween.Hash("position", _Zed.transform.position + _Zed.transform.forward*distance,
                "time", 2,
                "oncomplete", "MoveOver",
                "oncompletetarget", gameObject,
                "oncompleteparams", _Zed,
                "easetype", iTween.EaseType.linear));
        iTween.MoveTo(_Ned,
            iTween.Hash("position", _Ned.transform.position + _Ned.transform.forward*distance,
                "time", 2,
                "oncomplete", "MoveOver",
                "oncompletetarget", gameObject,
                "oncompleteparams", _Ned,
                "easetype", iTween.EaseType.linear));

        int _xOffset = 0;
        
        battle.Enqueue(new BattleAction(2, delegate()
        {
            _xOffset = RandomOffSet(true);
            StartCoroutine(alarmForEnemy(Zed, AlarmFx1, _xOffset));

            _xOffset = RandomOffSet();
            StartCoroutine(alarmForEnemy(Ned, AlarmFx2, _xOffset));
        }));

        battle.Enqueue(new BattleAction(3.5f, delegate()
        {
            _xOffset = RandomOffSet();
            distance = 30f;
            var position = player.OriginalPosition + player.transform.right*_xOffset +
                           player.transform.forward*distance;
            StartCoroutine(MoveTowardPlayer(Zed, position, distance));
        }));

        battle.Enqueue(new BattleAction(5, delegate()
        {
            _xOffset = RandomOffSet();
            StartCoroutine(alarmForEnemy(Ned, AlarmFx2, _xOffset));
        }));

        battle.Enqueue(new BattleAction(6f, delegate()
        {
            distance = 30f;

            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Zed, position, distance));

            _xOffset = RandomOffSet();
            position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Ned, position, distance));

            Battle2();
        }));

        curBattleAction = battle.Dequeue();
        nextStepTick = 0;
    }

    private float t1, t2;
    float distance, _xOffset;
    private void Battle2()
    {
        PlayBossSound(1);
        t1 = 1.2f;
        t2 = 0.8f;
        distance = 30f;

        battle.Enqueue(new BattleAction(0, delegate()
        {
            _xOffset = RandomOffSet(true);
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Zed, position, distance));

            _xOffset = RandomOffSet();
            position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Ned, position, distance));
        }));

        battle.Enqueue(new BattleAction(t1, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Zed, position, distance));

            _xOffset = RandomOffSet();
            position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Ned, position, distance));
        }));

        battle.Enqueue(new BattleAction(2 * t1, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Zed, position, distance));
        }));

        battle.Enqueue(new BattleAction(2 * t1+t2, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Ned, position, distance));
        }));

        battle.Enqueue(new BattleAction(2 * t1 + 2* t2, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Zed, position, distance));
        }));
        battle.Enqueue(new BattleAction(2 * t1 + 3 * t2, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
            StartCoroutine(MoveTowardPlayer(Ned, position, distance));
        }));

        battle.Enqueue(new BattleAction(2 * t1 + 3 * t2, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Zed, position, distance));

            _xOffset = RandomOffSet();
            position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Ned, position, distance));
        }));

        battle.Enqueue(new BattleAction(3 * t1 + 3 * t2, delegate()
        {
            _xOffset = RandomOffSet();
            var position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Zed, position, distance));

            _xOffset = RandomOffSet();
            position = player.OriginalPosition + player.transform.right * _xOffset +
                           player.transform.forward * distance;
           StartCoroutine( MoveTowardPlayer(Ned, position, distance));

           battleStep++;
        }));

        nextStepTick = -2;
    }

    private IEnumerator Battle3()
    {
        PlayBossSound(2);

        yield return new WaitForSeconds(0.3f);

        var velocity = player.getRunVelocity();
        iTween.MoveTo(_Ripslinger,
            iTween.Hash("position", _Ripslinger.transform.position +
                                    _Ripslinger.transform.forward*(velocity*2 - safeDistance + 15f),
                "time", 2,
                "easetype", iTween.EaseType.linear));

        yield return new WaitForSeconds(2f);


    }

    private List<int> offset =new List<int>();

    private int RandomOffSet(bool reset =false)
    {
        if (offset.Count == 0 || reset)
            offset.AddRange(new int[] {-1, 0, 1});

        int index = Random.Range(0, offset.Count);
        int value = (int) offset[index];
        offset.RemoveAt(index);
        return value;
    }

    private IEnumerator delayTodo(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log(Time.time);
        while (!CanRunning)
            yield return null;

        action();
    }

    private IEnumerator alarmForEnemy(GameObject g, ParticleSystem fx, float xOffset, float duration = 2f)
    {
        //等待时间需要加上同向冲锋跟警报的时间
        fx.gameObject.SetActive(true);

        fx.transform.position = player.OriginalPosition +
                                (player.transform.forward * player.getRunVelocity() / 2) + player.transform.right * xOffset;

        yield return new WaitForSeconds(1f);

        fx.gameObject.SetActive(false);

        GameObject enemy = pool.Spawn(g.transform).gameObject;
        enemy.transform.ResetTransformation();
        enemy.transform.Rotate(Vector3.up, 180f);
        enemy.transform.position = fx.transform.position;
        doPlayAnimation(enemy, GamePlayer.AnimType.kRun);
        iTween.Stop(enemy);
        iTween.MoveTo(enemy,
            iTween.Hash("position", enemy.transform.position + enemy.transform.forward * 30 * duration,
                "time", duration,
                 "oncomplete", "MoveOver",
                 "oncompletetarget", gameObject,
                 "oncompleteparams",enemy,
                "easetype", iTween.EaseType.linear));
    }

    private void MoveOver(GameObject g)
    {
        g.transform.ResetTransformation();
        pool.Despawn(g.transform, null);
    }

    private IEnumerator MoveTowardPlayer(GameObject g, Vector3 pos, float distance, float duration = 2f)
    {
        GameObject enemy = pool.Spawn(g.transform).gameObject;
        enemy.transform.position = pos;
        enemy.transform.localScale = Vector3.one;
        doPlayAnimation(enemy, GamePlayer.AnimType.kRun);
        yield return null;
        iTween.Stop(enemy);
        iTween.MoveTo(enemy,
            iTween.Hash("position", pos + enemy.transform.forward * distance,
                "time", duration,
                "oncomplete", "MoveOver",
                "oncompletetarget", gameObject,
                "oncompleteparams", enemy,
                "easetype", iTween.EaseType.linear));
    }

    private IEnumerator throwBomb(GameObject g, float xOffset, bool canClick, float duration = 0.5f)
    {
        GameObject bomb = PoolManager.Pools["Enemies"].Spawn(g.transform).gameObject;

        if (canClick && bomb.GetComponent<ClickObstacle>() == null)
            bomb.AddComponent<ClickObstacle>();

        var from = _Ripslinger.transform.position + _Ripslinger.transform.forward * 15;
        //ArrayList offset = new ArrayList(new int[] { -1, 0, 1 });
        //int xOffset = (int)offset[Random.Range(1, offset.Count + 1) - 1];
        var to = _Ripslinger.transform.right*xOffset - _Ripslinger.transform.forward*5;

        Vector3[] paths = new Vector3[3];
        paths[0] = from;
        paths[1] = (from + to*0.5f + Vector3.up*2f);
        paths[2] = (from + to);

        iTween.MoveTo(bomb, iTween.Hash("path", paths, "time", duration, "easeType", iTween.EaseType.linear));

        yield return new WaitForEndOfFrame();

        //Enemies.Add(bomb);
        PoolManager.Pools["Enemies"].Despawn(bomb.transform, 3f, null);
    }

    private void doPlayAnimation(GameObject enemy, GamePlayer.AnimType animType)
    {
        var anim = enemy.GetComponentInChildren<Animation>();
        string animStr = player.GetAnimName(animType);

        anim.Play(animStr);
    }

    private IEnumerator Exit(bool isWin)
    {
        PlayBossSound(isWin ? 3 : 4);
        
        GamePlayer.SharedInstance.StartInvincibility(2f);

        Animation model = _Ripslinger.GetComponentInChildren<Animation>();
        if (model)
            iTween.RotateBy(model.gameObject, new Vector3(0, 0, 1), 2f);
        //iTween.MoveTo(_Ripslinger,
        //    iTween.Hash("position", _Ripslinger.transform.position + _Ripslinger.transform.forward * 15f,
        //        "time", 1f,
        //        "easetype", iTween.EaseType.linear));

        //yield return new WaitForSeconds(1f);
        iTween.MoveTo(_Ripslinger,
            iTween.Hash("position", _Ripslinger.transform.position + _Ripslinger.transform.forward*60f,
                "time", 2f,// "delay", 1f,
                "easetype", iTween.EaseType.linear));

        for (int i = 0; i < Enemies.Count; i++)
        {
            GameObject g = Enemies[i];
            g.SetActive(false);
        }
        Enemies.Clear();

        if (isWin) //完成boss模式
        {
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.BossModeFinished, 1);
        }

        yield return new WaitForSeconds(2f);
        //显示胜利提示
        UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowWin(isWin);

        GameController.SharedInstance.WaitToExitEventMode(1f);
        GamePlayer.SharedInstance.StartInvincibility(4f);
        Clear();
    }
    
    public void Clear()
    {
        GameController.SharedInstance.CurrentMaxSpeed = playerMaxSpeedBefore;
        GamePlayer.SharedInstance.SetMaxRunVelocity(playerMaxSpeedBefore);
        GamePlayer.SharedInstance.SetPlayerVelocity(playerSpeedBefore);

        if (_Ripslinger)
            Destroy(_Ripslinger);

        //for (int i = 0; i < Enemies.Count; i++)
        //{
        //    GameObject g = Enemies[i];
        //    if(g.activeSelf)
        //        PoolManager.Pools["Enemies"].Despawn(g.transform, null);
        //}
        //Enemies.Clear();

        gameObject.SetActive(false);
    }

    public void Hurt(GameObject g)
    {
        var pos = g.transform.position;
        Vector3[] paths = new Vector3[3];
        paths[0] = pos;
        paths[1] = _Ripslinger.transform.position + _Ripslinger.transform.forward*7f + Vector3.up*2f;
        paths[2] = _Ripslinger.transform.position + _Ripslinger.transform.forward*15;

        iTween.MoveTo(g,
            iTween.Hash("path", paths, "time", 0.5, "easeType", iTween.EaseType.linear, "oncomplete", "BombHit",
                "oncompletetarget", g));

        StopCoroutine("checkBombFx");
        hurtCount++;

        if (hurtCount > 2)
        {
            battleStep++;
        }

        _BombFx.Play();
        StartCoroutine("checkBombFx");

        //GameCamera.SharedInstance.Shake(0.15f, 1.0f, 1.0f);
        StartCoroutine(Shake(_Ripslinger, 0.1f, 1f, 0.5f));
    }

    private IEnumerator Shake(GameObject g, float magnitude, float duration, float freqMult)
    {
        var ShakeDamperRate = (magnitude/duration);
        var ShakeMagnitude = magnitude;

        Transform anim = g.GetComponentInChildren<Animation>().transform;
        Vector3 cachePos = anim.localPosition;
        while (ShakeMagnitude > 0.0f || duration > 0)
        {
            duration -= Time.smoothDeltaTime;
            ShakeMagnitude -= (Time.smoothDeltaTime*ShakeDamperRate);

            yield return null;

            float cameraShakeOffsetX = Mathf.Sin(duration*35.0f*freqMult)*ShakeMagnitude;
            float cameraShakeOffsetY = Mathf.Sin(duration*50.0f*freqMult)*ShakeMagnitude;
            anim.Translate(cameraShakeOffsetX, cameraShakeOffsetY, 0);
        }
        anim.localPosition = cachePos;
    }

    IEnumerator checkBombFx()
    {
        yield return new WaitForSeconds(1f);

        _BombFx.Stop(true);
        _BombFx.Clear(true);
    }

    void OnGUI()
    {
        //if (GUI.Button(new Rect(220, 0, 100, 20), "Shake"))
        //    StartCoroutine(Shake(_Ripslinger, 0.1f, 0.5f, 0.5f));
    }

    private void PlayBossSound(int step)
    {
        UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowBossPrompt(2f);

        if(!GamePlayer.SharedInstance.EnableDubbed)
            return;

        switch (step)
        {
            case 1:
                AudioManager.SharedInstance.PlayAnimatedSound(AudioManager.Effects.game_boss_1);
                //Debug.Log("UI提示Ripslinger头像横制屏幕上方，音效同步挑衅玩家（Ripslinger-Boss对战说话1：你休想赢我！");
                break;
            case 2:
                AudioManager.SharedInstance.PlayAnimatedSound(AudioManager.Effects.game_boss_2);
                //Debug.Log("UI提示Ripslinger头像横制屏幕上方，音效同步挑衅玩家（Ripslinger-Boss对战说话2：尝尝我的厉害！");
                break;
            case 3:
                AudioManager.SharedInstance.PlayAnimatedSound(AudioManager.Effects.game_boss_3);
                //Debug.Log("UI提示Ripslinger头像横制屏幕上方，音效同步挑衅玩家（Ripslinger-Boss对战失败：什么？这不可能！？");
                break;
            case 4:
                AudioManager.SharedInstance.PlayAnimatedSound(AudioManager.Effects.game_boss_4);
                //Debug.Log("UI提示Ripslinger头像横制屏幕上方，音效同步挑衅玩家（Ripslinger-Boss对战胜利：哈哈！");
                break;
        }
    }
    void OnEnable()
    {
        UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ResetPos();
    }
}
