using System.Collections;
using UnityEngine;

public class WorldOfOzData
{
    public bool available;
    public string Description;
    public string IconName;
    public int ID;
    //public int SortPriority;
    public bool installed;
    public string Title;

    public WorldOfOzData(int id, string title, string description, string iconName, bool inst, bool avail)
    {
        ID = id;
        Title = title;
        Description = description;
        IconName = iconName;
        installed = inst;
        available = avail;
    }
}

public class WorldOfOzCellData : MonoBehaviour
{

//    private int levelId;

    public static int PerfectClearStar = 1;
    public ObjectiveProtoData _data; // reference to data	
    public GameObject btnLockLevel; //未解锁关卡
    public GameObject btnOverLevel; //已通过关卡
    public GameObject lockhead;
    public GameObject locktail;
    //private float normalStarY = 64f;
    public GameObject planeModel;
    private Transform routeline;
    private Transform routelinegrey;
    public GameObject star1; //
    public GameObject star2;
    public GameObject star3;
    public GameObject starCotain; //星星容器
    private int starCount;
    //private float withPlaneStarY = 47f;

    private void Awake()
    {
    }

    private void Start()
    {
        RegisterEvent();

        ResetCellData();
    }
    public void ResetCellData()
    {
        routeline = UIManagerOz.SharedInstance.worldOfOzVC.worldList.levelCotains[_data._id].FindChild("route");

        routelinegrey = UIManagerOz.SharedInstance.worldOfOzVC.worldList.levelCotains[_data._id].FindChild("routegrey");

        if (isUnlock())
        {
            if (routeline != null && _data._id != 0)
                routeline.gameObject.GetComponent<UISlider>().value = 1;
            if (routelinegrey != null)
                routelinegrey.gameObject.SetActive(false);

            if (_data._conditionList[0]._isBigLevel)
            {
                btnOverLevel.GetComponent<UISprite>().spriteName = "level_over_l";

            }

            btnOverLevel.SetActive(true);
            btnOverLevel.collider.enabled = true;


            btnLockLevel.SetActive(false);
        }
        else
        {


            btnOverLevel.SetActive(false);
            btnOverLevel.collider.enabled = false;

            if (_data._conditionList[0]._isBigLevel)
            {

                btnOverLevel.SetActive(true);
                btnOverLevel.GetComponent<UISprite>().spriteName = "level_over_l";
            }


            btnLockLevel.SetActive(true);
        }
    }
    private void RegisterEvent()
    {
        UIEventListener.Get(btnOverLevel).onClick = IntoLevelClicked;
        UIEventListener.Get(btnLockLevel).onClick = LockClicked;

    }

    public void SetData(ObjectiveProtoData data)
    {
        _data = data;
        Refresh(); // populate fields	
    }

    private void Refresh()
    {
        if (_data != null)
        {
            UpdateStar();
            ToggleBtn();
        }
    }

    private void ToggleBtn()
    {
        if (isUnlock())
        {
            if (_data._conditionList[0]._isBigLevel)
            {
                btnOverLevel.GetComponent<UISprite>().spriteName = "level_over_l";               
            }

                btnOverLevel.collider.enabled = true;
                btnOverLevel.SetActive(true);
           

            btnLockLevel.SetActive(false);
        }
        else
        {
            btnLockLevel.SetActive(true);
        }
    }

    private bool isUnlock()
    {
//        Debug.LogError(ObjectivesManager.LevelObjectives.Count+";"+_data._id+":"+ObjectivesManager.LevelObjectives.Contains(_data));

        var flag = false;
//        foreach(ObjectiveProtoData tp in ObjectivesManager.LevelObjectives)
//        {
//            if(tp._id == _data._id)
//            {
//                Debug.LogError(tp._id);
//                flag = true;
//                break;
//            }
//        }

        if (_data._id <= ObjectivesManager.LevelObjectives[ObjectivesManager.LevelObjectives.Count - 1]._id)
            flag = true;

        //if (_data._conditionList[0]._isBigLevel&&_data._id == ObjectivesManager.LevelObjectives[ObjectivesManager.LevelObjectives.Count - 1]._id + 1)
        //    flag = true;

        return flag;
    }

    private void UpdateStar()
    {
        starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(_data);
        if (starCount == 0 && !isUnlock())
        {
            starCotain.SetActive(false);
            return;
        }
        starCotain.SetActive(true);
        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarActive(star1, star2, star3, starCount);
    }

    private IEnumerator MovePlaneModel(Vector3 toPos)
    {
        if (routeline != null)
        {
            if (routeline.gameObject.GetComponent<UISprite>().fillAmount != 1)
            {

                btnLockLevel.SetActive(true);
                if (!_data._conditionList[0]._isBigLevel)
                    btnOverLevel.SetActive(false);
                //屏蔽层
                UIManagerOz.SharedInstance.worldOfOzVC.levelDialogsData.DialogsClose.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                //对话过程//
                if (_data._conditionList[0]._earnedStatValue == 0)
                {
                    UIManagerOz.SharedInstance.worldOfOzVC.levelDialogsData.appear(_data);
                    while (!UIManagerOz.SharedInstance.worldOfOzVC.levelDialogsData.bDialogEnd)
                        yield return null;

                }
                //解锁动画//
                UIDynamically.instance.ShakePosition(btnLockLevel.gameObject, 0.8f);
                yield return new WaitForSeconds(0.8f);
                UIDynamically.instance.TopToScreen(locktail.gameObject, locktail.transform.localPosition.y,
                    locktail.transform.localPosition.y + 10f, 0.5f);
                UIDynamically.instance.TopToScreen(lockhead.gameObject, lockhead.transform.localPosition.y,
                    lockhead.transform.localPosition.y - 10f, 0.5f);
                yield return new WaitForSeconds(0.5f);
                btnLockLevel.SetActive(false);
                btnOverLevel.SetActive(true);

                if (!_data._conditionList[0]._isBigLevel)
                UIDynamically.instance.ZoomOutToOne(btnOverLevel, new Vector3(0.2f, 0.2f, 1f), 0.4f);
             
                //路径动画//
                UIDynamically.instance.AnimateProgressBar(routeline.gameObject, 0, 1f, 1f);
                yield return new WaitForSeconds(1f);
                routelinegrey.gameObject.SetActive(false);
            }
        }

        //如果是关卡教学不继续后面的流程
        if (GameController.SharedInstance.GetTutorialIDforSys() == 2 || GameController.SharedInstance.GetTutorialIDforSys() == 3)
        {

            UIManagerOz.SharedInstance.worldOfOzVC.btnBack.GetComponent<UISprite>().depth = 101;
            UIManagerOz.SharedInstance.worldOfOzVC.tutorialFinger.SetActive(true);
            var tutorialTS = TweenScale.Begin(UIManagerOz.SharedInstance.worldOfOzVC.tutorialFinger, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;
            UIManagerOz.SharedInstance.worldOfOzVC.levelDialogsData.DialogsClose.gameObject.SetActive(false);
          
            yield break;
        }

        UIManagerOz.SharedInstance.worldOfOzVC.worldList.isPlaneMoving = true;


        Vector3 mStartAngle = UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel.transform.localEulerAngles;
        UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel.transform.LookAt(toPos);
        float mTargetYAngle = UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel.transform.localEulerAngles.y;
        UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel.transform.localEulerAngles = mStartAngle;

        UIManagerOz.SharedInstance.worldOfOzVC.worldList.plane.transform.rotation = UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneRotAngle.rotation;

        iTween.RotateTo(UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel, iTween.Hash(
            "islocal", true,
            "rotation", new Vector3(mStartAngle.x, mTargetYAngle, mStartAngle.z),
            "time", 0.7f,
            "easytype", iTween.EaseType.linear
            ));
        yield return new WaitForSeconds(0.8f);
        UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlayAnim(UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel, "Flight");
        iTween.MoveTo(UIManagerOz.SharedInstance.worldOfOzVC.worldList.plane, iTween.Hash(
            "position", toPos,
            "time", 0.8f,
            "easytype", iTween.EaseType.easeInOutCubic,
            "oncomplete", "IntoLevel",
            "oncompletetarget", gameObject
            ));
        yield break;
    }
    public void IntoLevelClicked(GameObject obj)
    {
        if (_data._conditionList[0]._isBigLevel)
            btnOverLevel.GetComponent<UISprite>().spriteName = "level_over_l";


            //btnOverLevel.collider.enabled = false;


        if (
            Mathf.Abs(Vector3.Distance(UIManagerOz.SharedInstance.worldOfOzVC.worldList.plane.transform.position,
                planeModel.transform.position)) > 0.05f)
        {
            StartCoroutine(MovePlaneModel(planeModel.transform.position));
            //MovePlaneModel(planeModel.transform.position);
        }
        else
        {
            IntoLevel();
        }
    }

    public void IntoLevel()
    {
        UIManagerOz.SharedInstance.worldOfOzVC.worldList.isPlaneMoving = false;
        UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlayAnim(UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel, "Idle");
        UIManagerOz.SharedInstance.worldOfOzVC.levelDialogsData.DialogsClose.gameObject.SetActive(false);
         
        //UIManagerOz.SharedInstance.worldOfOzVC.worldList.plane.SetActive(false);

        //btnOverLevel.collider.enabled = true;
        if (_data._conditionList[0]._isBigLevel)
        {
            btnOverLevel.GetComponent<UISprite>().spriteName = "level_over_l";
            UIManagerOz.SharedInstance.worldOfOzVC.biglevelInfo.appear(_data, starCount);
            //btnBigLevel.collider.enabled = true;
        }
        else
        {
            UIManagerOz.SharedInstance.worldOfOzVC.levelInfo.appear(_data, starCount);
            
        }
    }

    private void LockClicked(GameObject obj)
    {
        //if ((_data._id + 1)%biglevelInterval == 0)
        //{
        //    UIConfirmDialogOz.onPositiveResponse += CancelUnLock;
        //    UIConfirmDialogOz.onNegativeResponse += SureUnLock;
        //    UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("花费18砖石解锁改关卡", "解锁", "确定", 18);
        //}
        //else
        //{
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("此关卡未解锁", "Btn_Ok");
       // }
    }


}