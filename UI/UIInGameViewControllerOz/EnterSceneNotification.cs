using UnityEngine;
using System.Collections;

public class EnterSceneNotification : MonoBehaviour {

    public UILabel popupLabel;
    public Transform bgXform;
	// Use this for initialization
	void Start () {
        bgXform.gameObject.SetActive(false);
        EnvironmentSetSwitcher.SharedInstance.RegisterForOnEnvironmentStateChange(EnvironmentStateChanged);	
	}

    private void EnvironmentStateChanged(EnvironmentSetSwitcher.SwitchState newState, int destinationEnvironmentId)
    {
        if (newState == EnvironmentSetSwitcher.SwitchState.addingNewPools && !GameController.SharedInstance.IsTutorialMode)
        {
           // string loc = EnvironmentSetManager.SharedInstance.LocalDict[destinationEnvironmentId].GetLocalizedTitle();

            Show(GetEnvNameByID(destinationEnvironmentId));
        }
    }
    public void Show(string textToShow)
    {
        bgXform.gameObject.SetActive(true);
        popupLabel.text = textToShow;
        StartCoroutine(ShowQueued(textToShow));
    }
    private IEnumerator ShowQueued(string textToShow)
    {

        yield return new WaitForSeconds(3f);

        iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0f, -561f, 0f), "isLocal", true, "time", 2f));
        TweenAlpha.Begin(gameObject, 2f, 1f);
        yield return new WaitForSeconds(3f);
        iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(-80f, -561f, 0f),
            "isLocal", true,
            "time", 2f,
            "oncomplete", "HideComplete",
            "oncompletetarget", gameObject
            ));
        TweenAlpha.Begin(gameObject, 2f, 0f);
        yield return new WaitForSeconds(2f);
        bgXform.gameObject.SetActive(false);
    }
    private string GetEnvNameByID(int id)
    {
        string name = "";
        switch (id)
        {
            case 1: name = "冰岛"; break;
            case 2: name = "墨西哥"; break;
            case 3: name = "印度"; break;
        }
        return name;
    }
}
