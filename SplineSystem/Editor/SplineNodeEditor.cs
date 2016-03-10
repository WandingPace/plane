using UnityEngine;
using UnityEditor;
using System.Collections;




[CustomEditor(typeof(SplineNode))]
public class SplineNodeEditor : Editor
{
	
	
	private static GameObject SplineParent;
	
	
#if UNITY_EDITOR
	[MenuItem("Spline Tool/New Node/At Camera")]
	public static SplineNode NewNode()
	{
		Camera[] cams = SceneView.GetAllSceneCameras();		
		if(cams.Length == 0)	{ Debug.LogWarning("Please open a SceneView to create a node!"); return null; }
		
		Vector3 pos = cams[0].transform.position + cams[0].transform.forward * 10f;
		
		GameObject go = new GameObject("SplineNode");
		GameObject gopar = new GameObject("SplineParent");
		gopar.transform.position = pos;
		go.transform.parent = gopar.transform;
		go.transform.position = pos;
		
		go.AddComponent<SplineNode>();
		
		Selection.activeGameObject = go;
		
		return go.GetComponent<SplineNode>();
	}
	[MenuItem("Spline Tool/New Node/At Origin")]
	public static SplineNode NewNodeAtOrigin()
	{
		SplineNode sn = NewNode();
		sn.transform.position = Vector3.zero;
		sn.transform.parent.position = Vector3.zero;
		return sn;
	}
	[MenuItem("Spline Tool/New Node/At Z=0")]
	public static SplineNode NewNodeAtZEqualsZero()
	{
		SplineNode sn = NewNode();
		Vector3 pos = sn.transform.position;
		pos.z = 0f;
		sn.transform.parent.position = pos;
		sn.transform.position = pos;
		return sn;
	}
	
#endif
	
	
	
	
	private bool connectMode = false;
	private bool crossConnectMode = false;
	
	
	private const float CURVE_RESOLUTION = 25f;

	
	private SplineNode Target
	{
		get
		{
			return (SplineNode)target;
		}
	}
	
	Texture mapTex;
	Texture upRtTex;
	Texture lowRtTex;
	Texture lowLftTex;
	Texture nodeTex;
	
	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
			if(connectMode==false) {
				if(GUILayout.Button("Connect") && !crossConnectMode) {
					connectMode = true;
				}
			} else {
				if(GUILayout.Button("CONNECTING...")) {
					connectMode = false;
				}
				
			}
		
			if(crossConnectMode==false) {
				if(GUILayout.Button("Cross-Connect") && !connectMode) {
					crossConnectMode = true;
				}
			} else {
				if(GUILayout.Button("CROSS-CONNECTING...")) {
					crossConnectMode = false;
				}
			}
		
		EditorGUILayout.EndHorizontal();
		
		if(mapTex==null)	mapTex = (Texture)Resources.Load("SplineToolImages/SplineMap");
		if(upRtTex==null)	upRtTex = (Texture)Resources.Load("SplineToolImages/UpperRight");
		if(lowRtTex==null)	lowRtTex = (Texture)Resources.Load("SplineToolImages/LowerRight");
		if(lowLftTex==null)	lowLftTex = (Texture)Resources.Load("SplineToolImages/LowerLeft");
		if(nodeTex==null)	nodeTex = (Texture)Resources.Load("SplineToolImages/Node");
		
		Rect imgRect = GUILayoutUtility.GetRect(160,120);
		GUI.Label(imgRect,mapTex);
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal();
		
			GUI.enabled = Target.next==null;
			if(Target.next!=null)	GUI.Label(imgRect,upRtTex);
			if(GUI.Button(new Rect(imgRect.x+160,imgRect.y+10,60,30),"Add"))
			{
				AddNodeFrom(Target);
			}
			GUI.enabled = Target.side!=null && Target.side.next==null && Target.next==null;
			if(GUI.Button(new Rect(imgRect.x+150,imgRect.y+45,80,30),"Add Both"))
			{
				AddNodeFrom(Target);
				if(Target.side!=null)
					AddNodeFrom(Target.side);
			}
			GUI.enabled = Target.side!=null && Target.side.next==null;
			if(Target.side!=null && Target.side.next!=null)	GUI.Label(imgRect,lowRtTex);
			if(GUI.Button(new Rect(imgRect.x+160,imgRect.y+80,60,30),"Add"))
			{
				if(Target.side!=null)
					AddNodeFrom(Target.side);
			}
			GUI.enabled = Target.side==null;
			if(Target.side!=null)	GUI.Label(imgRect,lowLftTex);
			if(GUI.Button(new Rect(imgRect.x+10,imgRect.y+120,60,30),"Add Side"))
			{
				GameObject go = new GameObject();
				SplineNode sn = go.AddComponent<SplineNode>();
				go.transform.position = Target.transform.TransformPoint(Vector3.Cross(Target.cpNext,Vector3.forward)*3f);
				sn.cpLast = -Target.cpNext;
				sn.cpNext = Target.cpNext;
				
				go.name = "SplineNode";
			
				if(Target.last!=null && Target.last.side!=null) {
					sn.last = Target.last.side;
					Target.last.side.next = sn;
				}
				if(Target.next!=null && Target.next.side!=null) {
					Target.next.side.last = sn;
					sn.next = Target.next.side;
				}
				Target.side = sn;
				sn.side = Target;
			
				go.transform.parent = Target.transform.parent;
			
				Selection.activeGameObject = go;
			}
			GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Normalize Back"))
			{
				Target.NormalizeBack();
				SceneView.RepaintAll();
			}
			if(GUILayout.Button("Normalize Front"))
			{
				Target.NormalizeFront();
				SceneView.RepaintAll();
			}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Back Match Front"))
			{
				Target.BackMatchFront();
				SceneView.RepaintAll();
			}
			if(GUILayout.Button("Front Match Back"))
			{
				Target.FrontMatchBack();
				SceneView.RepaintAll();
			}
		EditorGUILayout.EndHorizontal();
		
		
		Target.cpNext = EditorGUILayout.Vector3Field("Control Point - Next",Target.cpNext);
		Target.cpLast = EditorGUILayout.Vector3Field("Control Point - Last",Target.cpLast);
		
		Target.LockControlPoints = EditorGUILayout.Toggle("Lock Control Points",Target.LockControlPoints);
		Target.LockNormalization = EditorGUILayout.Toggle("Lock Normalization",Target.LockNormalization);
		
		
		if(GUI.changed)
		{
			SceneView.RepaintAll();
			
			EditorUtility.SetDirty(target);
			
			//Undo...
		}
		
		//DrawDefaultInspector();
	}
	
	static SplineNode AddNodeFrom(SplineNode targ)
	{
		GameObject go = new GameObject();
		SplineNode sn = go.AddComponent<SplineNode>();
		go.transform.position = targ.transform.TransformPoint(targ.cpNext*3f);
		sn.cpLast = -targ.cpNext;
		sn.cpNext = targ.cpNext;
		
		go.name = "SplineNode";
		sn.last = targ;
		sn.next = targ.next;
		targ.next = sn;
	
		if(targ.side!=null)
			sn.side = targ.side.next;
	
		go.transform.parent = targ.transform.parent;
	
		Selection.activeGameObject = go;
		
		return sn;
	}
	
	public void OnSceneGUI()
	{
		if(Target!=null)
		{
			Target.cpNext = Target.transform.InverseTransformPoint(Handles.PositionHandle(Target.transform.TransformPoint(Target.cpNext),
																						Target.transform.rotation));
			if(Target.LockControlPoints)	Target.BackMatchFront();
			
			Target.cpLast = Target.transform.InverseTransformPoint(Handles.PositionHandle(Target.transform.TransformPoint(Target.cpLast),
																						Target.transform.rotation));
			if(Target.LockControlPoints)	Target.FrontMatchBack();
			
			if(Target.LockNormalization)	{
				Target.NormalizeFront();
				Target.NormalizeBack();
				if(Target.next!=null && Target.next.LockNormalization)
					Target.next.NormalizeBack();
				if(Target.last!=null && Target.last.LockNormalization)
					Target.last.NormalizeFront();
			}
			
			if(GUI.changed)
			{
				SceneView.RepaintAll();
				
				EditorUtility.SetDirty(target);
				
				//Undo...
			}
		}
	}
	
	
	void OnDisable()
	{
		if((connectMode || crossConnectMode)
						&& Selection.activeGameObject!=null 
						&& Selection.activeGameObject.GetComponent<SplineNode>()!=null
						&& Selection.activeGameObject != Target.gameObject)
		{
			if(connectMode)
			{
				Target.next = Selection.activeGameObject.GetComponent<SplineNode>();
				Selection.activeGameObject.GetComponent<SplineNode>().last = Target;
			} else {
				Target.side = Selection.activeGameObject.GetComponent<SplineNode>();
				Selection.activeGameObject.GetComponent<SplineNode>().side = Target;
			}
			
			Selection.activeGameObject.transform.parent = Target.transform.parent;
			
			
			EditorUtility.SetDirty(target);
			
			EditorUtility.SetDirty(Selection.activeGameObject);
		}
		connectMode = false;
		crossConnectMode = false;
	}
	
	
	
	
	
	
}
