using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CoinLanePermission
{
    None,
    All,
    LeftOnly,
    RightOnly,
    //新增
    CenteOnly, //沿中轴线排布
    Left, //左侧上方越过或绕开障碍排布
    Right, //右侧上方越过或绕开障碍排布
    Center, //从上方越过或左右绕开排布
    LeftAndCenter, //绕开障碍排布
    RightAndCenter //绕开障碍排布
}

public enum CoinSpawnWay
{
    Default,
    Jump,
    BypassLeft,
    BypassRight
}

public class SpawnEnemyFromPiece : TrackPieceAttatchment
{
    private static SpawnPool EnemyPool;
    private static int lastSpawnIndex;
    private int decision = -1;
    public bool ParentToPiece = true;
    private List<GameObject> spawnedObjects;
    public List<SpList> SpawnPointLists = new List<SpList>();
    private List<Transform> spawns;
    public bool WaitToEnable = true;

    public SpList Decision
    {
        get
        {
            if (decision <= -1) DecideOnSpawnList();
            if (decision >= 0) return SpawnPointLists[decision];
            return null;
        }
    }

    public CoinLanePermission CurrentCoinPermission
    {
        get
        {
            var dec = Decision;
            if (dec != null)
                return dec.coinLanes;
            return CoinLanePermission.All;
        }
    }

    public List<float> CurrentCoinArcs
    {
        get
        {
            var dec = Decision;
            if (dec != null)
                return dec.CoinArcs;
            return new List<float>();
        }
    }

    public List<CoinSpawnWay> CurrentDodgeWay
    {
        get
        {
            var dec = Decision;
            if (dec != null)
                return dec.DodgeWay;
            return new List<CoinSpawnWay>();
        }
    }

    public SplineNode CoinSpline { get; private set; }

    public void OnSpawned()
    {
        //SpawnEnemies();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (EnemyPool == null)
            EnemyPool = PoolManager.Pools["Enemies"];

        //SpawnEnemies();

//		for(int i=0;i<spawnedObjects.Count;i++)
//		{
//			if(spawnedObjects[i]!=null)
//				spawnedObjects[i].SetActive(!WaitToEnable);
//		}

//		if(spawnedObjects==null || spawns==null)	return;
    }

    private void Kill()
    {
        if (spawnedObjects != null)
        {
            for (var i = 0; i < spawnedObjects.Count; i++)
            {
                spawnedObjects[i].BroadcastMessage("Kill", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void DecideOnSpawnList()
    {
        TrackPiece tp = null;
        var cur = transform;
        while (cur != null && tp == null)
        {
            tp = cur.GetComponent<TrackPiece>();
            cur = cur.parent;
        }

        var possibilityList = new List<int>();
        //int difficulty = (tp==null || !tp.IsBalloon()) ? TrackBuilder.SharedInstance.MaxTrackPieceDifficulty :
        //                                                GamePlayer.SharedInstance.balloonDifficulty;
        for (var i = 0; i < SpawnPointLists.Count; i++)
        {
            //if(SpawnPointLists[i].Difficulty <= difficulty)
            if (TrackBuilder.SharedInstance.DifficultyList.Contains(SpawnPointLists[i].Difficulty))
                possibilityList.Add(i);
        }

        if (possibilityList.Count == 0)
        {
            notify.Warning("No obstacle set with the correct difficulty found!");
            return;
        }

        //If this choice was chosen last time, try once again.
        var choice = Random.Range(0, possibilityList.Count);
        if (choice == lastSpawnIndex) choice = Random.Range(0, possibilityList.Count);

        decision = possibilityList[choice];

        lastSpawnIndex = choice;
    }

    public void SpawnEnemies(bool deferActivate = false)
    {
        if ((spawnedObjects == null || spawnedObjects.Count == 0) && SpawnPointLists.Count != 0)
        {
            spawnedObjects = new List<GameObject>();

            if (decision <= -1)
                DecideOnSpawnList();

            if (Decision == null) return;

            spawns = Decision.list;
            for (var i = 0; i < spawns.Count; i++)
            {
                spawnedObjects.Add(EnemyPool.Spawn(Decision.prefabs[i].transform, deferActivate).gameObject);

                //spawnedObjects[spawnedObjects.Count-1].SetActive(!WaitToEnable);

                spawnedObjects[i].transform.position = spawns[i].transform.position;
                spawnedObjects[i].transform.rotation = spawns[i].transform.rotation;
            }

            foreach (var _data in Decision.data)
            {
                var g = EnemyPool.Spawn(_data.prefab.transform, deferActivate).gameObject;
                spawnedObjects.Add(g);
                g.transform.position = _data.target.position;
                g.transform.rotation = _data.target.rotation;

                var mo = g.GetComponent<MovingObstacle>();
                if (mo)
                    mo.Initialize();
            }

            var nodes = GetComponentsInChildren<SplineNode>(true);

            if (nodes.Length >= 1)
            {
                if (Decision.CoinSplineFull != null && Decision.CoinSplineFull.Count >= 1)
                {
                    SplineNode latestNode = null;
                    for (var i = 0; i < Decision.CoinSplineFull.Count; i++)
                    {
                        SplineNode newNode;
                        if (nodes.Length > i)
                            newNode = nodes[i];
                        else
                            newNode = latestNode.CreateNew();

                        if (i > 0)
                            latestNode.Connect(newNode);

                        latestNode = newNode;

                        latestNode.transform.position = Decision.CoinSplineFull[i].transform.position;
                        latestNode.transform.rotation = Decision.CoinSplineFull[i].transform.rotation;
                        latestNode.cpNext = Vector3.forward;
                        latestNode.cpLast = -Vector3.forward;
                        //latestNode.cpNext = Decision.CoinSplineFull[i].transform.forward;
                        //latestNode.cpLast = -Decision.CoinSplineFull[i].transform.forward;

                        if (latestNode.last != null)
                        {
                            latestNode.last.NormalizeFront();
                            latestNode.NormalizeBack();
                        }
                    }
                    latestNode.next = null;
                }
                else if (nodes.Length >= 2)
                {
                    if (Decision.CoinStart != null && Decision.CoinEnd != null)
                    {
                        nodes[0].transform.position = Decision.CoinStart.position + Vector3.up*2f;
                        nodes[1].transform.position = Decision.CoinEnd.position + Vector3.up*2f;
                    }
                }

                CoinSpline = nodes[0];

                if (!GameController.SharedInstance.HasSpawnedGemInBalloon
                    && Decision.GemSpawns != null
                    && Decision.GemSpawns.Count > 0
                    &&
                    GameController.SharedInstance.DistanceTraveled >
                    TrackBuilder.SharedInstance.MinDistanceBetweenBalloons)
                {
                    var gemChance =
                        Mathf.Clamp(
                            GameProfile.SharedInstance.BalloonGemSpawnChance +
                            (GamePlayer.SharedInstance.balloonDifficulty*0.01f), 0.05f, 0.2f);
                    var v = Random.value;

                    //Debug.Log ("Random Roll: " + v);
                    if (v < gemChance)
                    {
                        var index = Random.Range(0, Decision.GemSpawns.Count);
                        var item = BonusItem.Create(BonusItem.BonusItemType.Gem);
                        transform.parent.parent.GetComponent<TrackPiece>().BonusItems.Add(item);
                        item.transform.rotation = Decision.GemSpawns[index].rotation;
                        item.transform.position = Decision.GemSpawns[index].position + Vector3.up*2f;
                        item.transform.Rotate(0, 180, 0);
                        GameController.SharedInstance.HasSpawnedGemInBalloon = true;
                    }
                }
            }
        }
    }

    public List<GameObject> SpawnEnemies(int _decision, bool deferActivate = false)
    {
        decision = _decision;
        SpawnEnemies(deferActivate);

        return spawnedObjects;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        decision = -1;

        //HACK! since we disable right after spawning, set the position then.
        if (spawnedObjects == null || spawns == null) return;

        for (var i = 0; i < spawns.Count && i < spawnedObjects.Count; i++)
        {
            //	Debug.Log(i);
            if (spawnedObjects[i] != null && spawns[i] != null)
            {
                spawnedObjects[i].transform.position = spawns[i].transform.position;
                spawnedObjects[i].transform.rotation = spawns[i].transform.rotation;
            }
            //if(spawnedObjects[i]!=null)
            //spawnedObjects[i].SetActive(false);
        }
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        if (spawnedObjects != null)
        {
            for (var i = 0; i < spawnedObjects.Count; i++)
            {
                if (EnemyPool != null && spawnedObjects[i] != null && spawnedObjects[i].activeSelf)
                {
                    EnemyPool.Despawn(spawnedObjects[i].transform, null);
                }
            }
            spawnedObjects.Clear();
            spawnedObjects = null;
        }
    }

    public override void OnPlayerEnteredPreviousTrackPiece()
    {
        if (spawnedObjects != null)
        {
            for (var i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] == null) continue;
                //		Debug.Log(spawnedObjects[i].name);
                spawnedObjects[i].BroadcastMessage("OnPlayerEnteredPreviousTrackPiece",
                    SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public override void OnPlayerEnteredTrackPiece()
    {
        if (WaitToEnable)
        {
            SpawnEnemies(false);
        }

        if (spawnedObjects == null || GamePlayer.SharedInstance.HasBoost) return;

        for (var i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
                continue;

            //spawnedObjects[i].transform.position = spawns[i].transform.position;
            //spawnedObjects[i].transform.rotation = spawns[i].transform.rotation;

            //NOTE: This was commented out, but the object needs to be active for BroadcastMessage to work. We need a different solution.
            //this and the broadcast message are very expensive - we should find a way to minimize these calls
            //I changed the SpawnEnemies call to take a flag for deferred activate. So the call above activates the spawns.
            //The check for active below should catch any strays that were spawned with defer but not yet activated
            //Could we add a flag to the spawned object to say if it needs the begin attack message?
//            if (!spawnedObjects[i].activeSelf)
//                spawnedObjects[i].SetActive(true);
            //this is not ideal - but fits current usage. Ideally there would ba a flag if attack message is needed
            //if(WaitToEnable)
            //
            if (spawnedObjects[i].activeSelf)
                spawnedObjects[i].BroadcastMessage("OnPlayerEnteredTrackPiece", SendMessageOptions.DontRequireReceiver);
            //}
            //Not doing this anymore.
//			if(spawnedObjects[i].GetComponent<FollowObject>()!=null)
//			{
//				Transform oldParent = spawnedObjects[i].transform.parent;
//				spawnedObjects[i].transform.parent = transform;
//				
//				spawnedObjects[i].GetComponent<FollowObject>().Target = GameController.SharedInstance.Player.transform;
//				spawnedObjects[i].GetComponent<FollowObject>().offset = spawnedObjects[i].transform.localPosition;
//				
//				spawnedObjects[i].transform.parent = oldParent;
//			}
        }
    }

    public static void SpawnPools(int envset)
    {
        if (EnemyPool == null)
            EnemyPool = PoolManager.Pools["Enemies"];

        var envSetData = EnvironmentSetManager.SharedInstance.LocalDict[envset];
        var prefabPath = envSetData.EnemyPoolPath;

        var prefab = ResourceManager.Load(prefabPath) as GameObject;

        if (prefab != null)
        {
            var newEnemyPool = Instantiate(prefab) as GameObject;
            var _pool = newEnemyPool.GetComponent<SpawnPool>();
            if (_pool)
            {
                newEnemyPool.name = "Enemies_Env";
                newEnemyPool.transform.SetParent(EnemyPool.transform.parent);

                GameController.SharedInstance.StartCoroutine(EnemyPool.InitializRange(_pool._perPrefabPoolOptions));
            }
        }
    }

    public static void DeSpawnPools()
    {
        if (!EnemyPool)
            return;

        SpawnPool enemiesEnvPool = null;
        if (!PoolManager.Pools.TryGetValue("Enemies_Env", out enemiesEnvPool))
            return;

        foreach (var _prefabPool in enemiesEnvPool._perPrefabPoolOptions)
        {
            var withSuffix = _prefabPool.prefab.name;

            if (EnemyPool.prefabs.ContainsKey(withSuffix))
            {
                var onePrefab = EnemyPool.prefabs[withSuffix];
                var prefabPool = EnemyPool.GetPrefabPool(onePrefab);
                if (prefabPool != null)
                {
                    prefabPool.SelfDestruct();
                    EnemyPool.DeletePrefabPool(prefabPool, withSuffix);
                }
            }
        }

        Destroy(enemiesEnvPool.gameObject);
    }

    [Serializable]
    public class SpawnData
    {
        public GameObject prefab;
        public Transform target;
    }

    [Serializable]
    public class CoinSpawnData
    {
        public int count;
        public bool isMedal;
        public Vector3 pos;
        public CoinSpawnWay way;
    }

    [Serializable]
    public class ItemSpawnData
    {
        public Transform target;
        public BonusItem.BonusItemType Type = BonusItem.BonusItemType.Random;
    }

    [Serializable]
    public class SpList
    {
        public bool _showFoldOut = false;
        public List<ItemSpawnData> bonusItem = new List<ItemSpawnData>();
        public List<float> CoinArcs = new List<float>();
        public Transform CoinEnd;
        public CoinLanePermission coinLanes = CoinLanePermission.All;
        public List<Transform> CoinSplineFull = new List<Transform>();
        public Transform CoinStart;
        public List<CoinSpawnData> coinWay = new List<CoinSpawnData>();
        public List<SpawnData> data = new List<SpawnData>();
        public int Difficulty = 0;
        public List<CoinSpawnWay> DodgeWay = new List<CoinSpawnWay>();
        public List<Transform> GemSpawns = new List<Transform>();
        public List<Transform> list = new List<Transform>();
        public List<GameObject> prefabs = new List<GameObject>();
        public string title;

        public Transform this[int index]
        {
            get { return list[index]; }
        }
    }
}