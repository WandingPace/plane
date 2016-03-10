using UnityEngine;
using System.Collections;

public class BalloonVisible : MonoBehaviour {


	public void OnBecameVisible(){
		GameController.SharedInstance.StartBalloonTutorial();
	}

}
