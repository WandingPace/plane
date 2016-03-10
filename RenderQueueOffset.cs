using UnityEngine;
using System.Collections;

public class RenderQueueOffset : MonoBehaviour {

    public int renderQueue = 3000;
	
	// Use this for initialization
	void Start () {
		SetRenderQueue(transform);
	}

	private void SetRenderQueue(Transform root) {
		
        Renderer[] rd=  gameObject.GetComponentsInChildren<Renderer>();
        for(int i=0;i<rd.Length;i++)
        {
            rd[i].renderer.sharedMaterial.renderQueue=renderQueue;
        }
	}
	
	

}
