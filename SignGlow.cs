using UnityEngine;
using System.Collections;

public class SignGlow : MonoBehaviour {
	
	public Material glowMaterial;
	public GameObject glowObj1;
	public GameObject glowObj2;
	
	
	void Start () {
		//Debug.Log("replace materials for glow sign");
		if(glowMaterial){
			if(glowObj1){
				glowObj1.renderer.material = glowMaterial;
			}
			if(glowObj2){
				glowObj2.renderer.material = glowMaterial;
			}
		}
	
	}
	
}
