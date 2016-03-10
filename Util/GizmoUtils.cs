using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class GizmoUtils
{
	private Mesh m;
	public static void DrawArrow(Vector3 start, Vector3 end)
	{
#if UNITY_EDITOR
		DrawArrow(start,end,(end-start).magnitude/15f);
#endif
	}
	
	public static void DrawArrow(Vector3 start, Vector3 end, float arrowSize)
	{
#if UNITY_EDITOR
		if(SceneView.lastActiveSceneView!=null)
		{
			Camera sceneCamera = SceneView.lastActiveSceneView.camera;
			
			if(sceneCamera!=null)
			{
				Vector3 viewDir = (end - sceneCamera.transform.position).normalized;
				Vector3 lineDir = (end-start).normalized;
				float lineMag = (end-start).magnitude;
				Vector3 marksDir = Vector3.Cross(lineDir,viewDir).normalized;
				
				Vector3 pt1 = end + (start-end).normalized*arrowSize + (marksDir*lineMag).normalized*arrowSize*1f/2f;
				Vector3 pt2 = end + (start-end).normalized*arrowSize - (marksDir*lineMag).normalized*arrowSize*1f/2f;
				
				Gizmos.DrawLine(start,(pt1+pt2)/2f);
				Gizmos.DrawLine(end, pt1);
				Gizmos.DrawLine(end, pt2);
				Gizmos.DrawLine(pt1, pt2);
				
			}
		}
#endif
	}
	
}
