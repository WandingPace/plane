using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnCoinForClimb : TrackPieceAttatchment
{

    [System.Serializable]
    public class CoinSpawnData
    {
        public Vector2 pos;
        public bool isMedal;
    }

    [System.Serializable]
    public class SpList
    {
        public List<CoinSpawnData> coinWay = new List<CoinSpawnData>();

        public bool _showFoldOut = false;
        public string title;
    }

    private int decision = -1;
    public List<SpList> SpawnPointLists = new List<SpList>();

    public SpList Decision
    {
        get
        {
            if (decision <= -1) 
                DecideOnSpawnList();
            if (decision >= 0) 
                return SpawnPointLists[decision];
            return null;
        }
    }

    private void DecideOnSpawnList()
    {
        TrackPiece tp = null;
        Transform cur = transform;
        while (cur != null && tp == null)
        {
            tp = cur.GetComponent<TrackPiece>();
            cur = cur.parent;
        }

        int choice = Random.Range(0, SpawnPointLists.Count);

        decision = choice;
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
