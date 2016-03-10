using UnityEngine;
using System.Collections;

public class DragMap : MonoBehaviour
{

    public Transform draggable;
    public UITexture leftfog,rightfog;

    //public float speed = 3f;
    //阻力
    public float dragpower = 0f;

    public float sensitivity = 0.01f;
    private float mEndAngle = 0f;
    private float minAngle = -20.9f;
    private float maxAngel = 20.9f;
    private float t;
    private float dragEndspeed = 0f;
    private float maxInertiaSpeed = 0.3f;
    private bool FogTrigger = true; //触发雾动作
    private float leavescene1Angel = -8.18f, leavescene2Angel = 6.34f;
    void Awake()
    {


    }

    // Use this for initialization
    void Start()
    {
        mEndAngle = minAngle;
    }

    void Update()
    {

        //惯性移动 待优化
        if (dragEndspeed != 0f)
        {

            if (dragEndspeed < 0)
                dragEndspeed = Mathf.Lerp(dragEndspeed, 0, Time.deltaTime * dragpower);
            //dragEndspeed += dragpower * Time.deltaTime;
            else if (dragEndspeed > 0)
                dragEndspeed = Mathf.Lerp(dragEndspeed, 0, Time.deltaTime * dragpower);
            //dragEndspeed -= dragpower * Time.deltaTime;

            if (Mathf.Abs(dragEndspeed) < 0.1f)
            {
                dragEndspeed = 0;
            }

            //Debug.Log(dragEndspeed);
            mEndAngle = (mEndAngle + dragEndspeed) % 360;
        }

        //欧拉角范围0-360和检视面板不符合
        if (mEndAngle % 360 > 180f)
        {
            mEndAngle = mEndAngle % 360 - 360f;
        }
        else if (mEndAngle % 360 <= 180)
        {
            mEndAngle = mEndAngle % 360;

        }
        //角度限制范围
        mEndAngle = Mathf.Clamp(mEndAngle, minAngle, maxAngel);

        draggable.transform.localEulerAngles = new Vector3(0, 0, mEndAngle);

        if (IsEnterFog())
        {

            if (FogTrigger)
            {
                UIDynamically.instance.LeftToScreen(leftfog.gameObject, -848f,-63f,1f);
                UIDynamically.instance.LeftToScreen(rightfog.gameObject, 765f, 16f, 1f);
                FogTrigger = false;
            }


        }
        else
        {

            if (!FogTrigger)
            {
                UIDynamically.instance.LeftToScreen(leftfog.gameObject, -848f, -63f, 1f, true);
                UIDynamically.instance.LeftToScreen(rightfog.gameObject, 765f, 16f, 1f, true);
                FogTrigger = true;
            }
            //fog.alpha = 0;
        }
    }

    //是否进入雾效
    private bool IsEnterFog()
    {
        if (ObjectivesManager.LevelObjectives.Count <= 20)
        {
            if (Eulerangel2Inspector(draggable.transform.localEulerAngles.z) > leavescene1Angel)
            {

                return true;
            }
            else
            {

                return false;
            }
        }
        if (ObjectivesManager.LevelObjectives.Count <= 40)
        {
            if (Eulerangel2Inspector(draggable.transform.localEulerAngles.z) > leavescene2Angel)
            {

                return true;
            }
            else
            {

                return false;
            }
        }

        return false;
    }
    //是否进入关卡场景 true 返回进入场景 false返回进入过渡区
    //private bool IsEnterScene(float curangle)
    //{

    //    if (curangle > leaveFogAngel[0] && curangle <= enterFogAngel[1]) //场景2
    //    {
    //        enterAngel = leaveFogAngel[0];
    //        levelAngel = enterFogAngel[1];
    //        return true;
    //    }
    //    else if (curangle > leaveFogAngel[1] && curangle <= enterFogAngel[2]) //场景3
    //    {
    //        enterAngel = leaveFogAngel[1];
    //        levelAngel = enterFogAngel[2];
    //        return true;
    //    }
    //    else if (curangle > enterFogAngel[0] && curangle <= leaveFogAngel[0]) //过渡海1
    //    {
    //        enterAngel = enterFogAngel[0];
    //        levelAngel = leaveFogAngel[0];
    //        return false;
    //    }
    //    else if (curangle > enterFogAngel[1] && curangle <= leaveFogAngel[1]) //过渡海2
    //    {
    //        enterAngel = enterFogAngel[1];
    //        levelAngel = leaveFogAngel[1];
    //        return false;
    //    }
    //    else if (curangle > enterFogAngel[2] && curangle <= leaveFogAngel[2]) //过渡海3
    //    {
    //        enterAngel = enterFogAngel[2];
    //        levelAngel = leaveFogAngel[2];
    //        return false;
    //    }
    //    else
    //        return false;

    //}
    void OnDragStart()
    {
        //ResetmaxAngel();
    }
    void OnDrag(Vector2 delta)
    {

        dragEndspeed = 0;
        t = delta.x * sensitivity;
        if (!UIManagerOz.SharedInstance.worldOfOzVC.worldList.isPlaneMoving)
        {


            if (delta.x < 0)
            {

                mEndAngle = ((draggable.localEulerAngles.z) + Mathf.Abs(t)) % 360;


            }
            else
            {

                mEndAngle = ((draggable.localEulerAngles.z) - Mathf.Abs(t)) % 360;


            }
        }
    }

    void OnDragEnd()
    {
        dragEndspeed = -t;
        dragEndspeed = Mathf.Clamp(dragEndspeed, -maxInertiaSpeed, maxInertiaSpeed);
    }
    //欧拉角转检视面板角度
    private float Eulerangel2Inspector(float angel)
    {
        if (angel % 360 > 180f)
        {
            angel = angel % 360 - 360f;
        }
        else if (angel % 360 <= 180)
        {
            angel = angel % 360;
        }
        return angel;
    }
    //计算夹角的角度 0~360
    float angleBtVector(Vector3 from_, Vector3 to_)
    {

        Vector3 v3 = Vector3.Cross(from_, to_);

        if (v3.z > 0)

            return Vector3.Angle(from_, to_);

        else

            return 360 - Vector3.Angle(from_, to_);
    }
}
