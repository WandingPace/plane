using UnityEngine;
using System;
using System.Collections;

public class UIModalDialogOz : MonoBehaviour 
{
	private GameObject alphaBGforModalDialogs;
	
	protected static Notify notify;
	
	protected virtual void Awake()
	{
		SetupNotify();
	}
	
	protected void SetupNotify()
	{
		if(notify==null)
		{
			notify = new Notify(this.GetType().Name);
		}
	}
	
	
    protected virtual void Start()
	{
		alphaBGforModalDialogs = (GameObject)Instantiate(Resources.Load("Oz/Prefabs/AlphaBGforModalDialogs"));
        alphaBGforModalDialogs.transform.parent = gameObject.transform;
        alphaBGforModalDialogs.transform.localPosition = Vector3.zero;
        alphaBGforModalDialogs.transform.localScale = Vector3.one;
	}
	
	void OnEnable()		// disable background colliders
	{
		UIManagerOz.SharedInstance.SetUICameraLayerMask(true);		// only modal dialog layer should receive events
		UIManagerOz.SharedInstance.AddToActiveList(this as UIModalDialogOz);
	}
	
	void OnDisable()	// re-enable all background colliders
	{
		UIManagerOz.SharedInstance.SetUICameraLayerMask(false);		// all layers will now receive events again
		UIManagerOz.SharedInstance.RemoveFromActiveList(this as UIModalDialogOz);

	}
}
