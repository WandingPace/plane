using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingObstacle : TrackPieceAttatchment
{
    public enum MoveDirection
    {
        Same = 1,           //同向空艇
        Relative = -1,      //迎面飞机
        Follow = 3,         //背后冲出
        Train =4,           //迎面火车
    }

    private bool hasInitialize;
    private bool isMoving;
    private SpawnPool pool;
    private float duration;
    private float endTime = 2.5f;
    private float DelayToAlarm = 1f;
    private GameObject[] _enemy;

    public MoveDirection Direction;
    public GameObject[] enemy;
    public int enemyLimit = 2;
    public ParticleSystem[] fx;
    public GameObject fximg;
    public float TriggerDistance;

    private void Start()
    {
        //Initialize();
    }

    public override void OnEnable()
    {
        //Initialize();
        base.OnEnable();
    }

    public void Initialize()
    {
        if (hasInitialize)
            return;
        hasInitialize = true;

        if (Direction == MoveDirection.Train)
        {
            if (pool == null)
                pool = PoolManager.Pools["Enemies"];

            for (var i = 0; i < enemyLimit; i++)
            {
                var _gameObject = pool.Spawn(enemy[0].transform).gameObject;
                _gameObject.transform.SetParent(transform);
                _gameObject.transform.ResetTransformation();
                _gameObject.transform.localPosition = new Vector3(0f, 0f, -(i + 1)*4f);
            }

            return;
        }

        if (pool == null)
            pool = PoolManager.Pools["npc"];
        _enemy = new GameObject[2];

        if (Direction == MoveDirection.Follow)
        {
            transform.Rotate(Vector3.up, 180f);

            //for (int i = 0; i < enemy.Length; i++)
            //{
            //    enemy[i].SetActive(false);
            //}

            var c = gameObject.GetComponent<BoxCollider>();
            if (c == null)
            {
                c = gameObject.AddComponent<BoxCollider>();
                c.size = new Vector3(10, 10, 1);
                c.center = GameController.SharedInstance.IsTutorialMode
                    ? new Vector3(0f, 0f, -5)
                    : new Vector3(0f, 0f, -10);
            }
            c.enabled = true;

            foreach (var system in fx)
                system.gameObject.SetActive(false);

            return;
        }

        var offset = new ArrayList(new[] {-1, 0, 1});
        var enemyIndex = new ArrayList();
        for (var i = 0; i < enemy.Length; i++)
            enemyIndex.Add(i);

        //随机障碍最大数
        var count = Random.Range(1, enemyLimit + 1);
        bool isColin;
        for (var i = 0; i < count; i++)
        {
            var index = Random.Range(1, enemyIndex.Count + 1) - 1;
            var _gameObject = pool.Spawn(enemy[(int) enemyIndex[index]].transform).gameObject;
            _gameObject.transform.SetParent(transform);
            _gameObject.transform.ResetTransformation();
            _gameObject.SetActive(false);
            enemyIndex.RemoveAt(index);

            EnableAudio(_gameObject);
            //如果障碍是Colin则不能出现在最左边(xoffset不能为1)
            isColin = (_gameObject.name.ToLower().Contains("colin"));
            if (isColin)
                offset.RemoveAt(2);

            index = Random.Range(1, offset.Count + 1) - 1;
            var xOffset = (int) offset[index];
            offset.RemoveAt(index);

            _gameObject.transform.localPosition = new Vector3(xOffset, 0f, 0f);
            _enemy[i] = _gameObject;
        }

        //因为基准点默认朝向正z方向.所以物体默认绕y轴旋转180度,而作为对向的障碍则不用反转
        if (Direction == MoveDirection.Same)
        {
            transform.Rotate(Vector3.up, 180f);
            TriggerDistance = -TriggerDistance;
        }

        var collider = gameObject.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(10, 10, 1);
            collider.center = new Vector3(0f, 0f, TriggerDistance);
        }
    }

    public override void OnDespawned()
    {
        isMoving =  false;
        if (Direction == MoveDirection.Train)
            return;

        hasInitialize = false;
        transform.ResetTransformation();
        duration = 0;

        if (_enemy !=null)
        {
            for (var i = 0; i < _enemy.Length; i++)
            {
                if (_enemy[i] != null)
                {
                    _enemy[i].transform.SetParent(pool.transform);
                    _enemy[i].transform.ResetTransformation();
                    //_enemy[i].SetActive(false);

                    pool.Despawn(_enemy[i].transform, null);
                    _enemy[i] = null;
                }
            }
        }

        if (Direction == MoveDirection.Relative) //躲避对向飞机
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.PassTheRelativePlanes, 1);
        else if (Direction == MoveDirection.Follow) //躲避战斗机
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.PassTheFollowPlanes, 1);

        base.OnDespawned();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        bool disableAudio = false;

        if (!enabled || Time.timeScale == 0f)
            disableAudio = true;

        if (GamePlayer.SharedInstance.Hold || GamePlayer.SharedInstance.Dying)
            disableAudio = true;
        if (_enemy != null)
        {
            for (var i = 0; i < _enemy.Length; i++)
            {
                if (_enemy[i])
                {
                    if (!_enemy[i].activeSelf)
                        _enemy[i].SetActive(true);

                    if (canPlayAudio == disableAudio)
                        EnableAudio(_enemy[i], !disableAudio);
                }
            }
        }
        canPlayAudio = !disableAudio;
        if(disableAudio)
            return;

        var _velocity = GamePlayer.SharedInstance.getRunVelocity()/3;
        if (Direction == MoveDirection.Relative)
            _velocity *= 2;
        else if (Direction == MoveDirection.Follow)
            _velocity = 30f;
        else if (Direction == MoveDirection.Train)
            _velocity = GamePlayer.SharedInstance.getRunVelocity()/TriggerDistance;

        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward*_velocity,
            Time.deltaTime);

        duration += Time.deltaTime;
        if (Direction == MoveDirection.Follow && duration > endTime)
            OnDespawned();
    }

    private IEnumerator WaitToMove()
    {
        var offset = new ArrayList(new[] {-1, 0, 1});

        var enemyIndex = new ArrayList();
        for (var i = 0; i < enemy.Length; i++)
            enemyIndex.Add(i);

        if (GameController.SharedInstance.IsTutorialMode)
        {
            enemyLimit = 1;
            offset = new ArrayList(new[] {0});
        }

        //随机障碍最大数
        var count = Random.Range(1, enemyLimit + 1);
        var alarm = new int[count];
        for (var i = 0; i < count; i++)
        {
            var index = Random.Range(1, enemyIndex.Count + 1) - 1;
            var _gameObject = pool.Spawn(enemy[(int) enemyIndex[index]].transform).gameObject;
            _gameObject.transform.SetParent(transform);
            _gameObject.transform.ResetTransformation();
            _gameObject.SetActive(false);
            enemyIndex.RemoveAt(index);

            EnableAudio(_gameObject);
            StartCoroutine(WaitToActive(_gameObject, DelayToAlarm));

            index = Random.Range(1, offset.Count + 1) - 1;
            var xOffset = (int) offset[index];
            alarm[i] = xOffset;
            offset.RemoveAt(index);

            _gameObject.transform.localPosition = new Vector3(xOffset, 0f, 0f);
            fx[i].transform.SetLocalPositionX(xOffset);
            fx[i].gameObject.SetActive(true);
            fx[i].Play(false);

            _enemy[i] = _gameObject;
        }

        if (GameController.SharedInstance.IsTutorialMode)
        {
            //出现警报后暂停显示教学
            GameController.SharedInstance.waitCommand = true;
            fximg.SetActive(true);
            while (GameController.SharedInstance.waitCommand)
            {
                yield return new WaitForEndOfFrame();
            }
            fximg.SetActive(false);
        }

        yield return new WaitForSeconds(DelayToAlarm);

        foreach (var system in fx)
            system.gameObject.SetActive(false);

        isMoving = true;
    }

    private IEnumerator WaitToActive(GameObject g, float time)
    {
        yield return new WaitForSeconds(time);
        g.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.Contains("Player"))
            return;

        if (Direction == MoveDirection.Follow)
        {
            StartCoroutine(WaitToMove());
            var collider = gameObject.GetComponent<BoxCollider>();
            collider.enabled = false;
        }
        else
            isMoving = true;
    }

    public override void OnPlayerEnteredPreviousTrackPiece()
    {
        if (Direction == MoveDirection.Follow)
            return;

        isMoving = true;
    }

    private bool canPlayAudio;
    private void EnableAudio(GameObject g, bool enable = true)
    {
        var audioSource = g.GetComponentsInChildren<AudioSource>(true);
        for (var i = 0; i < audioSource.Length; i++)
            audioSource[i].enabled = enable;

        if (enable)
            canPlayAudio = true;
    }
}