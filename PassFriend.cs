using UnityEngine;
using System.Collections;

public class PassFriend : MonoBehaviour {
	
	public UILabel label;
	private int friendsPassed = 0;
	public Transform body;
	
	void Start(){
		friendsPassed = 0;
	}
	
	IEnumerator OnTriggerEnter(Collider other)
	{
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Passfreind);
		
		yield return new WaitForSeconds(1f);
		
		//If it hasnt moved, turn the label off.
		//if(transform.position==start)
		//	GetComponent<UILabel>().enabled = false;
		gameObject.SetActive(false);
	}
	
	public void SetText(string name){
		if(friendsPassed == 0){
			friendsPassed++;
			//Debug.Log("AndroidFontName " + label.AndroidFontName);
			//Debug.Log("AppleFontName " + label.AppleFontName);
		}
		label.text = name;
		//int count = name.Length;
		//body.localScale = new Vector3(0.25f + count * 0.15f, 1f, 1f);
	}
	
	public IEnumerator SetPosition(Vector3 pos, Vector3 dir){
		yield return new WaitForSeconds(0.1f);
		float width = (float)label.transform.localScale.x;
		body.localScale = new Vector3(0.1f + 0.7f * (float)width / 1000f, 1f, 1f);
		
		transform.forward = -dir;
		transform.position = pos + Vector3.up * 2f - transform.right * (body.localScale.x - 0.5f) ;
	}
	
}
