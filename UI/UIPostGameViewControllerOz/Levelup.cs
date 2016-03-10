using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Levelup : MonoBehaviour
{
    public GameObject LevelUpClose;
    public Transform LevelUpModelpos;
    public UILabel LevelUpnextRewardTxt; //下一级能力
    public GameObject LevelUppanel;
    public UILabel LevelUprewardTxt; //本级能力
    //升级界面
    public GameObject LevelUproot;
    public List<UILabel> LevelUpTaskDecList = new List<UILabel>();
    public UILabel LevelUpTxt, nextLevelUpTxt; //1->2
    public GameObject camerafade;
    private GameObject Model;
    private void Start()
    {
        UIEventListener.Get(LevelUpClose).onClick = OnLevelUpCloseClose;
    }

    private void OnLevelUpCloseClose(GameObject obj) //关闭升级界面
    {
        Destroy(Model);
        AudioManager.SharedInstance.StopFX();
        UIManagerOz.SharedInstance.postGameVC.postAcount.Buttonshieldmask.SetActive(false);
        ShowunlockChallengeTutorial();
        LevelUproot.SetActive(false);
    }
    public void ShowunlockChallengeTutorial()
    {
        //if (GameController.SharedInstance.unlockrole2PosTutorialPlayed && !GameController.SharedInstance.unlockChallengeTutorialPlayed && GameProfile.SharedInstance.Player.GetIsChallengeUnlock()) //挑战教学
        if (GameController.SharedInstance.GetTutorialIDforSys() == 5 && GameProfile.SharedInstance.Player.GetIsChallengeUnlock())
        {
            if (UIManagerOz.SharedInstance.ObjectivesVC.gameObject.activeSelf)
            {
                UIManagerOz.SharedInstance.ObjectivesVC.tutorialCharacter.SetActive(true);
                var tutorialTS = TweenScale.Begin(UIManagerOz.SharedInstance.ObjectivesVC.tutorialCharacter, 1.0f, Vector3.one);
                tutorialTS.to = Vector3.one * 0.8f;
                tutorialTS.style = UITweener.Style.Loop;
            }
            else if (UIManagerOz.SharedInstance.postGameVC.gameObject.activeSelf)
            {
                UIManagerOz.SharedInstance.postGameVC.tutorialUpgrade.SetActive(false);
                UIManagerOz.SharedInstance.postGameVC.tutorialHome.SetActive(true);
                var tutorialTS = TweenScale.Begin(UIManagerOz.SharedInstance.postGameVC.tutorialHome, 1.0f, Vector3.one);
                tutorialTS.to = Vector3.one * 0.8f;
                tutorialTS.style = UITweener.Style.Loop;
            }
        }


    }
    //---升级
    public void PopupLevelUpUI()
    {
       
       // iTween.ColorTo(camerafade, Color.black, 0.5f);

        iTween.ColorTo(camerafade, iTween.Hash(
         "color", Color.white,
         "time", 0.8f,
         "oncomplete", "camerafadeEnd",
         "oncompletetarget", gameObject
         ));

        
    }
    private void camerafadeEnd()
    {
        iTween.ColorTo(camerafade,Color.black,0.5f);

        //==========升级变化数据========
        LevelUpTxt.text = "等级：" + (GameProfile.SharedInstance.Player.playerLv - 1) + "->";
        nextLevelUpTxt.text = GameProfile.SharedInstance.Player.playerLv.ToString();
        UIDynamically.instance.Blink(nextLevelUpTxt.gameObject, 0.2f);
        //更新角色特性数据
        LevelUprewardTxt.text = "角色移动倍数加成：" + (GameProfile.SharedInstance.Player.playerLv - 1) + "->";
        LevelUpnextRewardTxt.text = GameProfile.SharedInstance.Player.playerLv.ToString();
        UIDynamically.instance.Blink(LevelUpnextRewardTxt.gameObject, 0.2f);
        //==========================
        //ui
        LevelUproot.SetActive(true);
        LevelUppanel.SetActive(true);
        LevelUpClose.SetActive(true);

        LevelUppanel.transform.localScale = Vector3.zero;
        for (var i = 0; i < LevelUpTaskDecList.Count; i++)
        {
            LevelUpTaskDecList[i].gameObject.SetActive(false);
            LevelUpTaskDecList[i].transform.localScale = Vector3.zero;
        }
        iTween.ScaleTo(LevelUppanel, iTween.Hash(
        "islocal", true,
        "scale", Vector3.one,
         "oncomplete", "LevelUpPopupEnd",
         "oncompletetarget", gameObject,
         "time", 1,
          "delay", 0.3f,
          "easyType", iTween.EaseType.easeInOutBounce
         ));

        //--生成模型---
        var orderIndex = GameProfile.SharedInstance.Player.teamIndexsOrder[0];
        Model = SpawnModelByOrderIndex(orderIndex);

        Model.transform.parent = LevelUpModelpos;
        Model.transform.ResetTransformation();
        Model.SetActive(true);
        Model.layer = 22;
        foreach (var tran in Model.GetComponentsInChildren<Transform>()) //遍历当前物体及其所有子物体
            tran.gameObject.layer = 22;
        //印象
        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.ui_opening_cheer);
        //动作
        var animsInGO = Model.GetComponentsInChildren<Animation>(true)[0];
        if (animsInGO != null)
        {
            var idle = animsInGO.GetClip("Idle");
            var cheer = animsInGO.GetClip("Cheer");
            if (cheer != null)
                animsInGO.Play(cheer.name, PlayMode.StopAll);
            if (idle != null)
                animsInGO.CrossFadeQueued("Idle", 0.3f, QueueMode.CompleteOthers);
        }

    
    
    }

    private GameObject SpawnModelByOrderIndex(int index) //生成模型
    {
        var activeCharacter = UIManagerOz.SharedInstance.chaSelVC.GetCharacterByOrderIndex(index);
        var protoVisual =
            GameProfile.SharedInstance.ProtoCharacterVisuals[activeCharacter.protoVisualIndex];
        var prefab = protoVisual.prefab;
        if (prefab != null)
        {
            var model = PoolManager.Pools["characters"].Spawn(prefab.transform).gameObject;
            return model;
        }

        return null;
    }

    private void DespawnModel(GameObject CharacterModel)
    {
        if (CharacterModel != null && CharacterModel.activeSelf
            && PoolManager.Pools.ContainsKey("characters"))
            PoolManager.Pools["characters"].Despawn(CharacterModel.transform, null);
    }

    private void LevelUpPopupEnd() //弹出框结束
    {

        //更新任务数据
        var dataList = GameProfile.SharedInstance.Player.objectivesMain;
        dataList = dataList.GroupBy(x => x._id).Select(y => y.First()).ToList();
        var index = 0;
        foreach (var pd in dataList)
        {
            LevelUpTaskDecList[index].gameObject.SetActive(true);
            UIDynamically.instance.ZoomZeroToOne(LevelUpTaskDecList[index].gameObject, 0.3f, false);
            LevelUpTaskDecList[index].text = pd._title;

            index++;
        }
    }
}