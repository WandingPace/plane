using System;
using UnityEngine;

public class StoreCellData : MonoBehaviour
{
    public GameObject btnPay;
    public UILabel costLabel;
    public UISprite iconSprite;
    private NotificationIcons notificationIcons;
    private NotificationSystem notificationSystem;
    private string symbol;
    public UILabel symbolLabel;
    public UILabel titleLabel;
    public IAP_DATA _data { get; private set; }

    private void Awake()
    {
        notificationIcons = gameObject.GetComponent<NotificationIcons>();

        symbol = symbolLabel.text;
        symbolLabel.text = "";
    }

    private void Start()
    {
        notificationSystem = Services.Get<NotificationSystem>();
        Refresh();
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        UIEventListener.Get(btnPay).onClick = OnStoreBuySucceed;
    }

    public void SetData(IAP_DATA data)
    {
        _data = data;
        Refresh();
    }

    public void Refresh() // populate fields
    {
        SetNotificationIcon();
        titleLabel.gameObject.GetComponent<UILocalize>().SetKey(_data.title);
        costLabel.text = symbol + _data.price;
        iconSprite.spriteName = _data.iconname;
    }

    public void SetNotificationIcon()
    {
        if (notificationSystem == null)
            notificationSystem = Services.Get<NotificationSystem>();

        var enable = false;
        notificationIcons.SetNotification(0, (enable) ? 0 : -1);
    }

    public void OnStoreBuyPressed()
    {
        var str = transform.FindChild("CellContents").FindChild("txt_cost").GetComponent<UILabel>().text;
        var price = Convert.ToDouble(str.Substring(str.IndexOf(" ")));
        Android.callAndroidJava("pay", gameObject.name, "OnStoreBuySucceed", "shibai", "quxiao", price);
    }

    public void OnStoreBuySucceed(GameObject obj)
    {
        OnStoreBuySucceed();
    }

    public void OnStoreBuySucceed()
    {
        //成功后
        GameProfile.SharedInstance.Player.coinCount += 100000;
        GameProfile.SharedInstance.Player.specialCurrencyCount += 1000;
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        GameProfile.SharedInstance.Serialize();
    }
}