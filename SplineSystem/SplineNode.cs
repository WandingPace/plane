using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class SplineNode : MonoBehaviour
{
	public SplineNode next;
	public SplineNode last;
	
	public SplineNode side;
	
	public Vector3 cpNext = new Vector3(3,0,0);
	public Vector3 cpLast = new Vector3(-3,0,0);
	
	public bool LockControlPoints = true;
	public bool LockNormalization = true;
	
	
	public Vector3 Pos {
		get {
			return transform.position;
		}
	}
	
	public Vector3 NextPos {
		get {
			return transform.TransformPoint(cpNext);
		}
	}
	public Vector3 LastPos {
		get {
			return transform.TransformPoint(cpLast);
		}
	}
	
	
	private const float CURVE_RESOLUTION = 25f;
	
	
	public SplineNode CreateNew()
	{
		GameObject go = new GameObject("SplineNode");
		go.transform.position = transform.position + cpNext.normalized*3f;
		go.transform.rotation = transform.rotation;
		go.transform.parent = transform.parent;
		
		next = go.AddComponent<SplineNode>();
		next.last = this;
		next.cpNext = cpNext;
		next.cpLast = cpLast;
		
		return next;
	}
	
	
	public static void Connect(SplineNode sp1, SplineNode sp2)
	{
		sp1.Connect(sp2);
	}
	
	public void Connect(SplineNode nxt)
	{
		next = nxt;
		next.last = this;
	}
	
	
	
	public Vector3 Bezier(float t)
	{
		if(next==null)	return transform.position;
		return Bezier(transform.position,NextPos,next.LastPos,next.transform.position,t);
	}
	
	public Vector3 LocalBezier(float t)
	{
		if(next==null)	return transform.position;
		return Bezier(Vector3.zero,cpNext,next.cpLast,transform.TransformPoint(next.transform.position),t);
	}
	
	public Vector3 BezierTangent(float t)
	{
		if(next==null)	return transform.position;
		return BezierTangent(transform.position,NextPos,next.LastPos,next.transform.position,t);
	}
	
	
	public static float Bezier(float x0, float x1, float x2, float x3, float t)
	{
		return ( ( (x3 - 3*x2 + 3*x1 - x0)*t + (3*x2 - 6*x1 + 3*x0))*t + (3*x1 - 3*x0))*t + x0;
	}
	
	public static Vector3 Bezier(Vector3 p1, Vector3 c1, Vector3 c2, Vector3 p2, float t)
	{
		return new Vector3(Bezier(p1.x,c1.x,c2.x,p2.x,t),Bezier(p1.y,c1.y,c2.y,p2.y,t),Bezier(p1.z,c1.z,c2.z,p2.z,t));
	}
	
	public static Vector3 Bezier(SplineNode sn1, SplineNode sn2, float t)
	{
		return Bezier(sn1.Pos,sn1.NextPos,sn2.LastPos,sn2.Pos,t);
	}
	
	public static float BezierTangent(float x0,float x1,float x2,float x3,float t)
	{
		return ( (3*x3 - 9*x2 + 9*x1 - 3*x0)*t + (6*x2 - 12*x1 + 6*x0))*t + (3*x1 - 3*x0);
	}
	
	public static Vector3 BezierTangent(Vector3 p1, Vector3 c1, Vector3 c2, Vector3 p2, float t)
	{
		return new Vector3(BezierTangent(p1.x,c1.x,c2.x,p2.x,t),BezierTangent(p1.y,c1.y,c2.y,p2.y,t),BezierTangent(p1.z,c1.z,c2.z,p2.z,t));
	}
	
	public static Vector3 BezierTangent(SplineNode sn1, SplineNode sn2, float t)
	{
		return BezierTangent(sn1.Pos,sn1.NextPos,sn2.LastPos,sn2.Pos,t);
	}
	
	
	
	public void NormalizeFront()
	{
		if(next!=null)
			cpNext = cpNext.normalized * (next.transform.position-transform.position).magnitude/Mathf.PI;
	}
	
	public void NormalizeBack()
	{
		if(last!=null)
			cpLast = cpLast.normalized * (last.transform.position-transform.position).magnitude/Mathf.PI;
	}
	
	public void FrontMatchBack()
	{
		cpNext = cpNext.magnitude * -cpLast.normalized;
	}
	
	public void BackMatchFront()
	{
		cpLast = cpLast.magnitude * -cpNext.normalized;
	}
	
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
			
		if(last==null)	Gizmos.color = new Color(0.65f,0.65f,0.65f);
		Gizmos.DrawSphere(transform.position,0.25f);
		
		Gizmos.color = Color.black;
		
		if(next!=null)
		{
			DrawCurve();
		}
		
		if(side!=null)
		{
			DrawDottedLine(transform.position,side.transform.position);
		}
			
		if(Selection.activeGameObject==gameObject)
		{
			Gizmos.color = new Color(0.5f,0.5f,0.5f);
			Gizmos.DrawLine(transform.position, transform.TransformPoint(cpNext));
			Gizmos.DrawLine(transform.position, transform.TransformPoint(cpLast));
			
			Gizmos.DrawSphere(transform.TransformPoint(cpNext),0.15f);
			Gizmos.DrawSphere(transform.TransformPoint(cpLast),0.15f);
		}
		
		//Correct connections, using "next" as the determining factor
		if(next!=null && next.last!=this)
			next.last = this;
		
		if(last!=null && last.next!=this)
			last = null;
		
		if(side!=null && side.side!=this)
			side.side = this;
	}
	
	
	void DrawCurve()
	{
		for(float t = 0f; t<1.0f; t+= 1f/CURVE_RESOLUTION)
		{
			if(t<1f-1f/CURVE_RESOLUTION)
				Gizmos.DrawLine(Bezier(t), Bezier(t+1f/CURVE_RESOLUTION));
			else
				GizmoUtils.DrawArrow(Bezier(t), Bezier(t+1f/CURVE_RESOLUTION),0.5f);
		}
	}
	
	
	void DrawDottedLine(Vector3 start, Vector3 end)
	{
		Vector3 dir = end-start;
		for(float t = 0f; t<1.0f; t+= 2f/21f)
		{
			Gizmos.DrawLine(transform.position+dir*t, transform.position+dir*(t+1f/21f));
		}
	}
#endif
	
	
}
