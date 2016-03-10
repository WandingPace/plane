using UnityEngine;
using System.Collections;

public class BalloonPrePiece : MonoBehaviour {
	
	
	private TrackPiece tp;
	public GameObject balloon;
	
	void Start () {
		//Debug.Log ("BalloonPrePiece Start");
		tp = transform.parent.GetComponent<TrackPiece>();
		if(tp){
			//Debug.Log ("we found trackpiece");
		
		}
	}
	
	public void HideBalloon(){
		if(balloon){
			//balloon.SetActive(false);
			Renderer[] renders = balloon.GetComponentsInChildren<Renderer>();
			foreach(Renderer r in renders){
				r.enabled = false;
				//Debug.Log ("turn off " + r.name);
			}
		}
	}
	public void ShowBalloon(){
		if(balloon){
			//balloon.SetActive(true);
			Renderer[] renders = balloon.GetComponentsInChildren<Renderer>();
			foreach(Renderer r in renders){
				r.enabled = true;
				//Debug.Log ("turn on " + r.name);
			}
		}
	}
	
}
