using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class DialogDetailDate
{
    public int _id=-1;
    public int _modelid=-1;
    public Dictionary<int, string> _dialogs = new  Dictionary<int, string>();
#if UNITY_EDITOR
    public bool _showFoldOut;
#endif
    //public void DialogDetailDate(int id, int modelid, string[] dialogs)
    //{
    //    _id = id;
    //    _modelid = modelid;
    //    _dialogs = dialogs;

    //}
    public  DialogDetailDate(int id, int modelid, Dictionary<int, string> dialogs)
    {
        _id=id;
        _modelid =modelid;
        _dialogs = dialogs;
    
    }
    public  DialogDetailDate(Dictionary<string, object> dict)
    {
#if UNITY_EDITOR
        _showFoldOut = false; 
#endif
        _id = -1;
        if (dict.ContainsKey("dialogID"))
            _id = (int)((long)dict["dialogID"]);

        _modelid = -1;
        if (dict.ContainsKey("dialogModelID"))
            _modelid = (int)((long)dict["dialogModelID"]);


        if (dict.ContainsKey("dialogStringList"))
        {
            _dialogs = new Dictionary<int, string>();
            _dialogs.Clear();

            var tempDict = dict["dialogStringList"] as IDictionary;

            if (tempDict != null)
            {
                foreach (var item in tempDict)
                {
                    if (item != null)
                    {
                        var kvp = (DictionaryEntry)item;
                        var k = int.Parse((string)kvp.Key);
                        var v = kvp.Value.ToString();
                        _dialogs.Add(k, v);
                    }
                }
            }

        }
    }
    public Dictionary<string, object> ToDict()
    {
        var d = new Dictionary<string, object>();
        d.Add("dialogID", _id);
        d.Add("dialogModelID", _modelid);

        var dialogStrList = new Dictionary<int, string>();
        if (_dialogs != null)
        {

            dialogStrList=_dialogs;
            
        }
        d.Add("dialogStringList", dialogStrList);
        return d;
    
    }
}
public enum LevelDialogNPC
{
    NPCskipper = 1,
    NPCdottie 
}
public class LevelDialogData : MonoBehaviour {

    public GameObject LevelDialogRoot;
    public GameObject levelDialogs;
    public UILabel levelDiaTxt;
    public GameObject DialogsClose;
    public GameObject npcModel;
    public Transform npcStartPos;
    public Transform npcEndPos;
    private GameObject npcmodelGO;
    private LevelDialogNPC npcIndex = LevelDialogNPC.NPCdottie;
    public static LevelDialogNPC historypnpcIndex;
    public List<Transform> npcmodelPrefab;
    public Dictionary<int, string> dialogs;
    private int diaIndex=-1;
    [HideInInspector]
    public bool bDialogEnd = false;


    void Start() 
    {
        RegisterEvent();
    }
    void RegisterEvent()
    {
        UIEventListener.Get(DialogsClose).onClick = OnDialogsClose;
    
    }
    void OnDialogsClose(GameObject obj)
    {
        if (diaIndex != -1)
        {
            if (diaIndex <= dialogs.Count)
                CloseDialog();
        }
    }

    public void appear(ObjectiveProtoData data)
    {
        //初始化对话框和背景
        DialogsClose.SetActive(true);
        DialogsClose.GetComponent<UISprite>().alpha = 1f;

        int dialogIndex = data._conditionList[0]._DialogIndexForLevel;
       
        if (dialogIndex > 0)
        {


            DialogDetailDate dd = ObjectivesManager.LevelDialogDicData.Find(

                delegate(DialogDetailDate cur)
                {
                    return cur._id == dialogIndex;
                });
            npcIndex = (LevelDialogNPC)dd._modelid;

            if (npcIndex != historypnpcIndex)
            {
                
                DespawnModel();
                npcmodelGO = SpawnModelByOrderIndex((int)npcIndex);
                npcmodelGO.transform.parent = npcModel.transform;
                if (npcIndex == LevelDialogNPC.NPCdottie)
                {
                    npcModel.transform.localEulerAngles = new Vector3(0f, 122f, 0f);
                }
                else
                {
                    npcModel.transform.localEulerAngles = Vector3.zero;
                }
                npcmodelGO.transform.ResetTransformation();
                historypnpcIndex = npcIndex;
            }
            dialogs = dd._dialogs;
            bDialogEnd = false;
            

            LevelDialogRoot.SetActive(true);
            levelDialogs.transform.localScale = Vector3.zero;
            //npc入场
            StartCoroutine(npcAdmission());
        }
        else
        {
            npcModelexitEnd();
        
        }
    
    }
    IEnumerator npcAdmission()
    {
        Animation anim = npcmodelGO.GetComponentInChildren<Animation>();
        if (anim != null)
        {
            AnimationClip idle = anim.GetClip("Speak");
            if (idle != null)
                anim.Play(idle.name);
            anim.wrapMode = WrapMode.Loop;
        }
        npcModel.transform.position = npcStartPos.position;
        iTween.MoveTo(npcModel.gameObject, iTween.Hash(
            "islocal", false,
            "position", npcEndPos,
            "time", 1f,
            "easytype", iTween.EaseType.easeOutQuad,
            "ignoretimescale", false
            ));
        yield return new WaitForSeconds(1f);
        //对话索引
        diaIndex = 1;
        ShowDialog(diaIndex);
        yield break;
    }
    public void ShowDialog(int index)
    {

        levelDiaTxt.text = dialogs[index];

        levelDialogs.transform.localScale = Vector3.zero;
        iTween.ScaleTo(levelDialogs, iTween.Hash(
        "scale", Vector3.one,
        "islocal", true,
        "time", 0.25f,
        "easetype", iTween.EaseType.easeOutBack
        ));


    }
    public void CloseDialog()
    {
        levelDialogs.transform.localScale = Vector3.one;
        iTween.ScaleTo(levelDialogs, iTween.Hash(
            "scale", Vector3.zero,
            "islocal", true,
            "time", 0.1f,
            "easetype", iTween.EaseType.easeInOutSine,
            "oncomplete", "DialogCloseEnd",
            "oncompletetarget", gameObject
            ));

    }
    private void DialogCloseEnd()
    {

        diaIndex++;
        if (diaIndex > dialogs.Count)
        {
            iTween.MoveTo(npcModel.gameObject, iTween.Hash(
             "islocal", false,
              "position", npcStartPos,
              "time", 0.8f,
              "easytype", iTween.EaseType.easeInBounce,
              "oncomplete", "npcModelexitEnd",
              "oncompletetarget", gameObject,
              "ignoretimescale", false
                 ));

        }
        else
            ShowDialog(diaIndex);
    }
    void npcModelexitEnd()
    {
        LevelDialogRoot.SetActive(false);
        DialogsClose.GetComponent<UISprite>().alpha = 0.01f;
        bDialogEnd = true;
    }

    private GameObject SpawnModelByOrderIndex(int index) //生成模型
    {
        if (npcIndex == LevelDialogNPC.NPCskipper)
        {
            if (PoolManager.Pools.ContainsKey("characters"))
            {
                var model = PoolManager.Pools["characters"].Spawn(npcmodelPrefab[index]).gameObject;
                return model;
            
            }
        
        }
        else 
        {
            if (PoolManager.Pools.ContainsKey("npc"))
            {
                GameObject npcmodelGO = PoolManager.Pools["npc"].Spawn(npcmodelPrefab[index]).gameObject;

                return npcmodelGO;
            }
        }

        



        return null;
    }

    private void DespawnModel()
    {
         if (npcmodelGO != null && npcmodelGO.activeSelf)
        {
            if (PoolManager.Pools.ContainsKey("characters") && historypnpcIndex == LevelDialogNPC.NPCskipper)
                PoolManager.Pools["characters"].Despawn(npcmodelGO.transform, null);

            if (PoolManager.Pools.ContainsKey("npc") && historypnpcIndex == LevelDialogNPC.NPCdottie)
                PoolManager.Pools["npc"].Despawn(npcmodelGO.transform, null);
        }
    }
}
