using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldOfOzList : MonoBehaviour
{
    private List<GameObject> childLevels = new List<GameObject>();
    private List<ObjectiveProtoData> dataList = new List<ObjectiveProtoData>();
    private bool IsInitialized;
    [HideInInspector] 
    public bool isPlaneMoving = false;
    [HideInInspector]
    public bool isUnlockNewLevel;
//    public GameObject readyTag;
    public List<Transform> levelCotains = new List<Transform>();
    public GameObject plane;
    [HideInInspector] public GameObject PlaneModel;
    public Transform PlaneRotAngle;
    public GameObject prefab;
    public GameObject tutorialFinger;

    private void Awake()
    {
    }

    //3D点换算成NGUI屏幕上的2D点。
    public Vector3 WorldToUI(Vector3 point)
    {
        var pt = UIManagerOz.SharedInstance.UICamera.WorldToScreenPoint(point);
        //我发现有时候UICamera.currentCamera 有时候currentCamera会取错，取的时候注意一下啊。
        var ff = UICamera.currentCamera.ScreenToWorldPoint(pt);
        //UI的话Z轴 等于0 
        //  ff.z = 0;
        return ff;
    }

    private void InitialPlaneModelPos()
    {
       
        if (isUnlockNewLevel)
        {
            unLocklevel();

            isUnlockNewLevel = false;
            // MovePlaneModel(pos);
        }
    }

    //================关卡解锁流程=====================
    private void unLocklevel()
    {   
            childLevels[ObjectivesManager.LevelObjectives.Count - 1].GetComponent<WorldOfOzCellData>().IntoLevelClicked(gameObject);
        
    }
    //---显示关卡提示---
    private void MovePlane()
    {
        childLevels[ObjectivesManager.LevelObjectives.Count - 1].GetComponent<WorldOfOzCellData>()
            .IntoLevelClicked(gameObject);
        isUnlockNewLevel = false;
    }

    //-----------------



    public void Refresh()
    {
        if (PlaneModel == null)
            CreatModel();
        if (!IsInitialized) //&& Initializer.IsBuildVersionPassThreshold())
        {
            PopulateTaskData();
        }
        else
        {
            RefreshCells();
        }
    }
    public void RefreshMapCellDate()
    {
        //ObjectiveProtoData obj;
        for (int i = 0; i < childLevels.Count; i++)
        {
            //obj = i < ObjectivesManager.LevelObjectives.Count ? ObjectivesManager.LevelObjectives[i] : dataList[i];
            childLevels[i].GetComponent<WorldOfOzCellData>().ResetCellData();

        }
    }
    public void PopulateTaskData()
    {
        dataList = ObjectivesManager.QuickAccessLevelObjectives;

        if (dataList.Count > 0)
        {
//            dataList.Sort((a1, a2) => a1._id.CompareTo(a2.id));

            if (!IsInitialized)
            {
                Initialize();
            }
            //if (GameController.SharedInstance.upgradeTutorialPlayed
            //    && !GameController.SharedInstance.levelTutorialPlayed)
            if (GameController.SharedInstance.GetTutorialIDforSys() == 1)
            {
                childLevels[0].GetComponent<WorldOfOzCellData>().btnOverLevel.GetComponent<UISprite>().depth = 90;
                //plane.GetComponent<UISprite>().depth = 99;
                tutorialFinger.SetActive(true);
                var tutorialTS = TweenScale.Begin(tutorialFinger, 1.0f, Vector3.one);
                tutorialTS.to = Vector3.one*0.8f;
                tutorialTS.style = UITweener.Style.Loop;
            }
            else
            {
                tutorialFinger.SetActive(false);
            }
            RefreshCells();
        }
    }

    public void RefreshCells()
    {
//        MarkLastestLevel();
     
        ObjectiveProtoData obj;
        for (int i = 0; i < childLevels.Count; i++)
        {
            obj = i < ObjectivesManager.LevelObjectives.Count ? ObjectivesManager.LevelObjectives[i] : dataList[i];
            childLevels[i].GetComponent<WorldOfOzCellData>().SetData(obj);

        }

        bool IsnextBiglevel = true;
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i]._conditionList[0]._isBigLevel)
            {
                Transform biglevelFX = levelCotains[i].transform.FindChild("hongseqizi_tx");
                if (biglevelFX != null)
                {
                    if (IsnextBiglevel && dataList[i]._id >= ObjectivesManager.LevelObjectives.Count-1)
                    {
                        biglevelFX.gameObject.SetActive(true);
                        IsnextBiglevel = false;
                    }
                    else
                    {
                        biglevelFX.gameObject.SetActive(false);
                    }
                }

            }

        }
            //在图标更新完后执行
            InitialPlaneModelPos();
    }

    private string planesprite(int index)
    {
        switch (index)
        {
            case 0:
                return "level_dusty";
            case 1:
                return "level_skipper";
            case 2:
                return "level_ElChupacabra";
            case 3:
                return "level_Ishani";
            case 4:
                return "level_Rochelle";
            case 5:
                return "level_Bulldog";
            case 6:
                return "level_Ripslinger";
            default:
                return "level_dusty";
        }
    }

    public void Initialize()
    {
        ClearGrid(); // kill all old objects under grid, prior to initialization
        childLevels = CreateCells(); // create cell GameObjects for all objectives

        var pos =
            childLevels[GameProfile.SharedInstance.Player.activeLevel].GetComponent<WorldOfOzCellData>()
                .planeModel.transform.position;
        plane.transform.position = pos;
       
        IsInitialized = true;
    }

    //缓冲池生成模型
    public void CreatModel()
    {
        var orderIndex = GameProfile.SharedInstance.Player.teamIndexsOrder[0];
        PlaneModel = SpawnModelByOrderIndex(orderIndex);
        PlaneModel.transform.parent = plane.transform;
        PlaneModel.transform.ResetTransformation();
        //改成ui层 存在两层ui之间
        PlaneModel.layer = 23;
        foreach (Transform tran in PlaneModel.GetComponentsInChildren<Transform>()) //遍历当前物体及其所有子物体
            tran.gameObject.layer = 23;
        PlayAnim(PlaneModel, "Idle");
    }
    //模型放回缓冲池
    public void DespawnModel()
    {

        DespawnModel(PlaneModel);
    
    }
    public void PlayAnim(GameObject modelGO, string animstr)
    {
        Animation anim = modelGO.GetComponentInChildren<Animation>();
        if (anim != null)
        {
            AnimationClip idle = anim.GetClip(animstr);
            if (idle != null)
                anim.CrossFade(idle.name);
            
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

    private void ClearGrid()
    {
        foreach (var obj in childLevels)
        {
            obj.transform.parent = null;
                // unparent first to remove bug when calling NGUI's UIGrid.Reposition(), because Destroy() is not immediate!
            Destroy(obj);
        }
    }

    private List<GameObject> CreateCells()
    {
        var newObjs = new List<GameObject>();

        for (int i = 0; i < levelCotains.Count; i++)
            newObjs.Add(CreateObjectivePanel(i));

        return newObjs;
    }

    private GameObject CreateObjectivePanel(int index) //string _title, string _description)
    {
        var obj = (GameObject) Instantiate(prefab);
        obj.transform.parent = levelCotains[index];
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.name = (index + 1).ToString();
        return obj;
    }
    public void  disappear()
    {
        //重制原始层次
        PlaneModel.layer = 15;
        foreach (Transform tran in PlaneModel.GetComponentsInChildren<Transform>()) //遍历当前物体及其所有子物体
            tran.gameObject.layer = 15;
    }
    void OnDisable()
    {


        DespawnModel();
        PlaneModel = null;
    }
}