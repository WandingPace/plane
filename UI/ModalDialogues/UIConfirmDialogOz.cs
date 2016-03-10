using UnityEngine;
using System;
using System.Collections;

public class UIConfirmDialogOz : UIModalDialogOz	//MonoBehaviour
{
	public delegate void voidClickedHandler();
	public static event voidClickedHandler onNegativeResponse;
	public static event voidClickedHandler onPositiveResponse;
	
    //normal
    public GameObject NormalTip;
	public GameObject LeftButton;
    public GameObject RightButton;
	public UILocalize LeftButtonText;
	public UILocalize RightButtonText;
	public UILocalize DescriptionText;

    //extra
    public GameObject ExtraTip;
    public GameObject ExLeftButton;
    public GameObject ExRightButton;
    public UILocalize ExDescriptionText;
    public UILocalize ExLeftButtonText;
    public UISprite   ExLeftIconSprite;
    public UILabel    ExLeftCountText;
    public UILocalize ExRightButtonText;
    //

	protected override void Awake()
	{
		base.Awake();
	}

    protected override void Start ()
    {
        base.Start ();
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        UIEventListener.Get(LeftButton).onClick = OnLeftButtonPress;
        UIEventListener.Get(RightButton).onClick = OnRightButtonPress;
        UIEventListener.Get(ExLeftButton).onClick = OnLeftButtonPress;
        UIEventListener.Get(ExRightButton).onClick = OnRightButtonPress;
    }
	
	public void ShowConfirmDialog(string description, string negativeButtonText, string positiveButtonText)	//, string price = null)  
	{
		LeftButtonText.SetKey(negativeButtonText);	// localize the left button text
		RightButtonText.SetKey(positiveButtonText);	// localize the right button text		
		DescriptionText.SetKey(description);		// localize the description	
        NormalTip.SetActive(true);
        ExtraTip.SetActive(false);
		NGUITools.SetActive(this.gameObject, true);
	}
	
	private void OnLeftButtonPress(GameObject obj)
	{
        NGUITools.SetActive(this.gameObject, false);
		if (onNegativeResponse != null)
			onNegativeResponse();
	}
	
	private void OnRightButtonPress(GameObject obj) 
	{
        NGUITools.SetActive(this.gameObject, false);
		if (onPositiveResponse != null)
			onPositiveResponse();
	}
	
	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;
		OnLeftButtonPress(gameObject);
	}


    //extra
    public void ShowConfirmDialog(string description, string leftButtonText, string rightButtonText,int price,CostType type = CostType.Special)  
    {
        ExDescriptionText.SetKey(description);        // localize the description 
        ExLeftButtonText.SetKey(leftButtonText);  // localize the left button text
        ExRightButtonText.SetKey(rightButtonText); // localize the right button text       
        ExLeftIconSprite.spriteName = UIManagerOz.SharedInstance.inventoryVC.GetCostIconNameByType(type);
        ExLeftCountText.text = price.ToString();

        NormalTip.SetActive(false);
        ExtraTip.SetActive(true);
        NGUITools.SetActive(this.gameObject, true);
    }





}
