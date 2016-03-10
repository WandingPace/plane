using UnityEngine;
using System.Collections;

public class PassedObstacle : MonoBehaviour {

    private ObjectiveType passedType = ObjectiveType.JumpOverPassed;
    public int passTrack = 1;

	// Use this for initialization
	void Start () {

        BoxCollider c = gameObject.GetComponent<BoxCollider>();
        if (c == null)
        {
            c = gameObject.AddComponent<BoxCollider>();

            switch (passTrack)
            {
                case 2:
                    c.size = new Vector3(2f, 1.5f, 0f);
                    c.center = new Vector3(0.5f, 2f, 0f);
                    break;
                case 3:
                    c.size = new Vector3(3f, 1.5f, 0f);
                    c.center = new Vector3(1f, 2f, 0f);
                    break;
                default:
                    c.size = new Vector3(1, 1.5f, 0);
                    c.center = new Vector3(0f, 2f, 0f);
                    break;
            }
        }
        c.enabled = true;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.Contains("Player"))
            return;

        if (GamePlayer.SharedInstance.LevelItem != null &&
            GamePlayer.SharedInstance.LevelItem.Type.Equals("DoubleJump"))
            ObjectivesDataUpdater.AddToGenericStat(passedType, GamePlayer.SharedInstance.LevelItem.Value);
        else
            ObjectivesDataUpdater.AddToGenericStat(passedType, 1);
    }
}
