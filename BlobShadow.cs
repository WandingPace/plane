using UnityEngine;
using System.Collections;

public class BlobShadow : MonoBehaviour {

	public Transform Caster;
	public LayerMask Mask;
	
	
	void LateUpdate()
	{
		if(Caster==null)	{Destroy(gameObject); return;}
		
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(Caster.position,Vector3.down*10f,out hit,100f,Mask))
		{
			transform.position = hit.point + Vector3.up*0.5f;
		}
	}
}
