using UnityEngine;
using System.Collections;

public class UIOkayDialogOz : UIModalDialogOz	//MonoBehaviour
{
	public delegate void voidClickedHandler();
//	public static event voidClickedHandler onNegativeResponse;
	public static event voidClickedHandler onPositiveResponse;
	
	public UISprite CenterButton;
//    public UILabel CenterButtonText;
	public Transform DescriptionText;
	
	private GameObject messageObject;
	
	protected override void Awake()
	{
		base.Awake();
	}

     protected override void Start ()
    {
        base.Start ();
        UIEventListener.Get(CenterButton.gameObject).onClick = OnCenterButtonPress;
    }

	
	public void ShowOkayDialog(string description, string positiveButtonText, bool useSysFont = false, GameObject msgObject = null) 
	{		
		SetupNotify();	//Sometimes, this is called before 'notify' is set up
		notify.Debug("ShowOkayDialog {0} {1} {2} {3}" , description, positiveButtonText, useSysFont, msgObject);	
		messageObject = msgObject;

//		if (Localization.HasMainFontBeenUpdated() == false)
//		{
//			// force SysFont in this case, we are so early we haven't loaded the main font texture
//			notify.Debug("ShowOkayDialog forcing sysfont");
//			useSysFont = true;
//		}
		{
			DescriptionText.gameObject.GetComponent<UILabel>().enabled = true;
			DescriptionText.gameObject.GetComponent<UILocalize>().SetKey(description);			// localize the description
         
//			CenterButtonText.gameObject.GetComponent<UILocalize>().SetKey(positiveButtonText);	// localize the center button text
//			CenterButtonText.gameObject.GetComponent<UILabel>().enabled = true;
		}
		
		NGUITools.SetActive(this.gameObject, true);
		
	
	}
	
	private void OnCenterButtonPress(GameObject obj)
	{
		if (onPositiveResponse != null)
			onPositiveResponse();

		if (messageObject)
			messageObject.SendMessage("OnOkayDialogClosed");
		
		NGUITools.SetActive(this.gameObject, false);		
	}
	
	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;
		
		OnCenterButtonPress(gameObject);// left always??!!
	}	
}
