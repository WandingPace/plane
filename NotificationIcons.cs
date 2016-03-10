using System.Collections.Generic;
using UnityEngine;

public class NotificationIcons : MonoBehaviour
{
    private int curIconValue = -1;
    // these lists must all be the same size, as set up in the Inspector!
    public List<UISprite> iconBackgrounds = new List<UISprite>();
    public List<UILabel> iconLabels = new List<UILabel>();

    public int IconValue
    {
        get { return curIconValue; }
    }

    public void SetNotification(int buttonID, int iconValue)
    {
        curIconValue = iconValue;

        if (iconValue < 0) // if value < 0, turn off notification
        {
            if (iconBackgrounds.Count > buttonID)
                iconBackgrounds[buttonID].enabled = false;
//			iconLabels[buttonID].enabled = false;
        }
        else if (iconValue == 0 || iconValue > 9) // if value == 0 or value > 9, show exclamation point 
        {
            if (iconBackgrounds.Count > buttonID)
                iconBackgrounds[buttonID].enabled = true;
//			iconLabels[buttonID].enabled = false;
        }
        else if (iconValue > 0) // if value > 0, show actual number (capped at 9)
        {
            if (iconBackgrounds.Count > buttonID)
                iconBackgrounds[buttonID].enabled = true;
//			iconLabels[buttonID].enabled = true;
//			iconLabels[buttonID].text = (Math.Min(iconValue, 9)).ToString();
        }
    }
}