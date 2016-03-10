using UnityEngine;
using System.Collections;

public class TrLogoScale : MonoBehaviour {
	
	public Transform trRoot;
	public Vector3 trRoot_iphone5_scale;
	public Vector3 trRoot_ipad_scale;
	public Transform oz;
	public Vector3 oz_iphone5_scale;
	public Vector3 oz_ipad_scale;
	public Vector3 oz_iphone5_pos;
	public Vector3 oz_ipad_pos;
	public Transform disney;
	public Vector3 disney_iphone5_scale;
	public Vector3 disney_ipad_scale;
	public Vector3 disney_iphone5_pos;
	public Vector3 disney_ipad_pos;
	
	// Use this for initialization
	public void Start () {
		float ratio = (float)Screen.width / (float)Screen.height;
		float ratioMin = 640f / 1136f; 
		float ratioMax = 768f / 1024f; 
		
		float current = (ratio - ratioMin) / (ratioMax - ratioMin);
		current = Mathf.Clamp01(current);
		
		Vector3 sc = new Vector3( Mathf.Lerp(trRoot_iphone5_scale.x, trRoot_ipad_scale.x, current), Mathf.Lerp(trRoot_iphone5_scale.y, trRoot_ipad_scale.y, current), 1f);
		trRoot.localScale = sc;
		
		sc = new Vector3( Mathf.Lerp(oz_iphone5_scale.x, oz_ipad_scale.x, current),  Mathf.Lerp(oz_iphone5_scale.y, oz_ipad_scale.y, current), 1f);
		oz.localScale = sc;
		sc = new Vector3( Mathf.Lerp(oz_iphone5_pos.x, oz_ipad_pos.x, current), Mathf.Lerp(oz_iphone5_pos.y, oz_ipad_pos.y, current), 0f);
		oz.localPosition = sc;

        if (!disney)
            return;
		sc = new Vector3( Mathf.Lerp(disney_iphone5_scale.x, disney_ipad_scale.x, current), Mathf.Lerp(disney_iphone5_scale.y, disney_ipad_scale.y, current), 1f);
		disney.localScale = sc;
		sc = new Vector3( Mathf.Lerp(disney_iphone5_pos.x, disney_ipad_pos.x, current), Mathf.Lerp(disney_iphone5_pos.y, disney_ipad_pos.y, current), 0f);
		disney.localPosition = sc;	
		
	}
	
	
}
