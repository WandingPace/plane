using UnityEngine;
using System.Collections;

public class LineBetweenGOs : MonoBehaviour 
{
	public GameObject targetGO;
	public Color lineColor = Color.yellow;	
	LineRenderer lineRenderer;

    void Awake()
	{
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(lineColor,lineColor);
        lineRenderer.SetWidth(2.0f, 2.0f);
        lineRenderer.SetVertexCount(2);
		lineRenderer.useWorldSpace = false;	
		lineRenderer.SetPosition(0, gameObject.transform.localPosition);		
		lineRenderer.SetPosition(1, gameObject.transform.localPosition);					
    }
	
	void Update()
	{
	}
	
	public void SetTargetGO(GameObject _targetGO)
	{
		lineRenderer.SetPosition(1, _targetGO.transform.localPosition);		
	}
}


	//Transform trans1, trans2;
	
    //public Color c1 = Color.yellow;
    //public Color c2 = Color.red;
    //public int lengthOfLineRenderer = 2;	
	
		//trans1 = new GameObject("trans1").transform;
		//trans1.transform.parent = gameObject.transform;
		//trans2 = new GameObject("trans2").transform;
		//trans2.transform.parent = gameObject.transform;
		
		
		//Vector3 pos1 = new Vector3(0.0f, 0.0f, 0.0f);
		//Vector3 pos2 = new Vector3(100.0f, 100.0f, 0.0f);
		//lineRenderer.SetPosition(0, pos1);	
		//lineRenderer.SetPosition(1, pos2);	

		//Destroy(trans1.gameObject);
		//Destroy(trans2.gameObject);		
		//trans1 = _trans1;
		//trans2 = _trans2;