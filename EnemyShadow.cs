using UnityEngine;
using System.Collections;

public class EnemyShadow : MonoBehaviour
{
    public bool isMoving;
    public float velocity;
    private float baseVelocity;         //影子基础速度
    //public float chaseDuration = 60f;   //事件持续时长
    //public float maxDistance = 90f;     //追逐失败距离
    public float playerSpeedBefore;     //开始前玩家速度
    public float playerMaxSpeedBefore;  //开始前玩家最大速度
    public TrackPiece tpBefore;         //开始前玩家所在位置
    public Transform planeShadow;
    public GameObject start;
    public GameObject end;

    //private float durationleft;
    private BoxCollider _collider;
    //private int chaseCount;
    //public float targetVelocity;        //影子目标速度
    //private bool speedUp;
    //private bool speedDown;
    //private float accelDuration;         //变速持续时间
    private float chaseDistance = 30f;  //追逐初始距离
    private float chaseLegth = 450f;  //追逐初始距离
    private Vector3 movePos;
    //private float[] targetSpeeds = new float[] {21, 20, 18};

    // Use this for initialization
    void Start()
    {

    }
    // Update is called once per frame
    private void Update()
    {
        if (GamePlayer.SharedInstance.Dying)
        {
            UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowCountDown("");
            return;
        }

        if (!enabled || Time.timeScale == 0f)
            return;

        if (GamePlayer.SharedInstance.Hold)
            return;

        if (isMoving)
        {
            //durationleft -= Time.deltaTime;
            ////调用UI显示倒计时
            ////UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowCountDown(Mathf.FloorToInt(durationleft).ToString());

            //var distance = Vector3.Distance(transform.position, GamePlayer.SharedInstance.transform.position);

            //if (chaseCount < 3 && distance < 5)
            //{
            //    if (!speedUp && !speedDown)
            //    {
            //        speedUp = true;
            //        accelDuration = 1f;
            //        targetVelocity = targetSpeeds[chaseCount++];
            //    }

            //    //var speed = GamePlayer.SharedInstance.getRunVelocity() + 8 - chaseCount*2;
            //    //var target = GamePlayer.SharedInstance.transform.position +
            //    //             GamePlayer.SharedInstance.transform.forward*(speed*5f);
            //    //target.x = 0f;

            //    //iTween.MoveTo(gameObject,
            //    //    iTween.Hash("position",target,
            //    //        //"speed", 30f,
            //    //        "time",5f,
            //    //        "oncomplete", "Chase",
            //    //        "oncompletetarget", gameObject,
            //    //        "easetype", iTween.EaseType.linear));

            //    //isMoving = false;
            //    //return;
            //}

            //if (speedUp || speedDown)
            //{
            //    accelDuration -= Time.deltaTime;
            //    velocity = Mathf.Lerp(velocity, targetVelocity, speedDown ? Time.time/2 : Time.deltaTime);
            //}
            //else if (!speedUp && accelDuration > 0)
            //{
            //    accelDuration -= Time.deltaTime;
            //    if (accelDuration < 0)
            //    {
            //        speedDown = true;
            //        accelDuration = 2f;
            //    }
            //}

            movePos = GamePlayer.SharedInstance.IsJumping ? planeShadow.forward*baseVelocity : planeShadow.forward*velocity;
            //更新位置信息
            planeShadow.position = Vector3.Lerp(planeShadow.position, planeShadow.position - movePos,
                Time.deltaTime);

            //if (speedUp && accelDuration < 0)
            //{
            //    speedUp = false;
            //    accelDuration = 5f;
            //    targetVelocity = baseVelocity;
            //}
            //else if (speedDown && accelDuration < 0)
            //{
            //    speedDown = false;
            //}

            //if (durationleft < 0)
            //{
            //    ExitShadowMode(false);
            //}
        }
    }

    void Chase()
    {
        isMoving = true;
        //chaseCount++;
    }

    private void BuffOver()
    {
        Vector3 pos = new Vector3(0, 0, 90);
        start.transform.localPosition = pos;
        start.SetActive(true);

        //缓冲结束后在自身后方创建一个碰撞物用以触发追逐
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.size = new Vector3(10, 10, 1);
            _collider.center = pos;
        }
    }

    public void Start(TrackPiece startTP,TrackPiece waitToMoveTP)
    {
        UIManagerOz.SharedInstance.inGameVC.HidePauseButton();

        start.SetActive(false);
        end.SetActive(false);

        GameController .SharedInstance.CurrentMaxSpeed = 15f;
        GamePlayer.SharedInstance.SetMaxRunVelocity(15);
        GamePlayer.SharedInstance.SetPlayerVelocity(15);
        
        isMoving = false;
        //speedUp = speedDown = false;

        MoveToTrackPiece(startTP);
        tpBefore = waitToMoveTP;
        gameObject.SetActive(true);
        //accelDuration = chaseCount = 0;

        //2秒后启动教程引导
        iTween.ValueTo(gameObject,iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time",0.1f,
            "delay",2f,
            "ignoretimescale", true,
            "onupdate","ShowShadowGuide",
            "onupdatetarget", UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.gameObject
            ));

        //缓冲阶段从缓冲区起点移动到追逐区前方30米(3段30米空白区+追逐距离)
        iTween.MoveTo(planeShadow.gameObject,
             iTween.Hash("position", planeShadow.position - planeShadow.forward * (3 * 30 + chaseDistance),
                 "time", 4f,
                 //"oncomplete", "BuffOver",
                 //"oncompletetarget", gameObject,
                 "easetype", iTween.EaseType.linear));

        BuffOver();
        //确定影子速度
        baseVelocity = 11f;
            velocity = 16f;
    }

    private void MoveToTrackPiece(TrackPiece tp)
    {
        planeShadow.position = transform.position = tp.transform.position;
        //transform.rotation = tp.transform.rotation;
        //transform.Rotate(Vector3.up, 180f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isMoving)
        {
            if (planeShadow.position.z < GamePlayer.SharedInstance.CurrentPosition.z)
                ExitShadowMode(false);
            else
               ExitShadowMode(true);
        }
        else
        {
            //开始追逐
            UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowGo();
            UIManagerOz.SharedInstance.inGameVC.ShowPauseButton();
            GameController.SharedInstance.Hold = false;
            //durationleft = chaseDuration;
            isMoving = true;
            //重定碰撞体位置,放置于前方待再次碰撞结束事件
            _collider.center += new Vector3(0f, 0f, chaseLegth);
            end.transform.localPosition = _collider.center;
            end.SetActive(true);
        }
    }

    private void ExitShadowMode(bool isWin)
    {
        if (isWin)
        {
            GamePlayer.SharedInstance.StartInvincibility(3f);
            //完成影子模式
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.ShadowModeFinished, 1);   
        }

        UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowWin(isWin);

        GameController.SharedInstance.WaitToExitEventMode(1f);

        Clear();
    }

    public void Clear()
    {
        //UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ShowCountDown("");

        GameController.SharedInstance.CurrentMaxSpeed = playerMaxSpeedBefore;
        GamePlayer.SharedInstance.SetMaxRunVelocity(playerMaxSpeedBefore);
        GamePlayer.SharedInstance.SetPlayerVelocity(playerSpeedBefore);

        //durationleft = 0f;
        Destroy(_collider);

        Invoke("Disable", 1f);
    }

    void Disable()
    {
        isMoving = false;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        UIManagerOz.SharedInstance.inGameVC.mUIShadowInstance.ResetPos();
    }
}