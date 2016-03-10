//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//
//public class BackgroundColliderDisabler : MonoBehaviour 
//{
//	private Collider[] allActiveColliders, foregroundActiveColliders;
//	private List<Collider> backgroundActiveColliders = new List<Collider>();
//	
//	public void DisableAllBackgroundColliders(GameObject foregroundRootGO, GameObject allUIrootGO)
//	{
//		allActiveColliders = allUIrootGO.GetComponentsInChildren<Collider>(false);				// add active UI colliders
//		foregroundActiveColliders = foregroundRootGO.GetComponentsInChildren<Collider>(false);	// colliders for popup dialog
//		
//		backgroundActiveColliders.Clear();
//		
//		foreach (Collider col in allActiveColliders)
//		{
//			if (!foregroundActiveColliders.Contains(col))	// separate out background colliders by filtering out foreground ones
//			{
//				backgroundActiveColliders.Add(col);			// store references, so they can be re-enabled
//				col.enabled = false;						// disable the collider on background UI element
//			}
//		}
//	}
//
//	public void EnableAllBackgroundColliders()				// re-enable colliders on all previously disabled background UI elements
//	{
//		foreach (Collider col in backgroundActiveColliders)
//			col.enabled = true;
//	}			
//}
