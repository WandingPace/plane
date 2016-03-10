using UnityEngine;
using System.Collections;

public class BalloonTutorial : MonoBehaviour {
	public GameObject ballonObj;
	void Start () {
		if(ballonObj){
			ballonObj.AddComponent<BalloonVisible>();
		}
		else{
			//Debug.LogError("Can't find ballon ref on junction. Check to make sure balloon obj is referenced in oz_ww_cliffs_balloonjunctionLR_a_prefab or call Eyal");
		}
	}
	
}
