using UnityEngine;
using System.Collections;

public class ReplaceMaterial : MonoBehaviour {
	
	public Material material;
	
	// Use this for initialization
	void Start () {
		UIWidget ui = GetComponent<UIWidget>();
		Material mat = ui.material;
		ui.material = material;
		ui.material.mainTexture = mat.mainTexture;
		Debug.Log (" new mat " + ui.material.name + " shader " + ui.material.shader + " texture " + ui.material.mainTexture);
		
	
	}

}
