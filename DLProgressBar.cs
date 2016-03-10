using UnityEngine;
using System.Collections;

public class DLProgressBar : MonoBehaviour 
{
	public UISprite progressFG;
	private float progress = 0.0f;	
	
	void Start() { }
	
	void Update() 
	{
		progress = DownloadManager.GetTotalDownloadProgress();
		progressFG.transform.localScale = new Vector3(100.0f * progress, progressFG.transform.localScale.y, 1.0f);
	}
}
