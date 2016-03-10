using UnityEngine;
using System.Collections;

public class LoadingViewController : UIViewControllerOz
{
    public UISlider ProgressBar;
    public UILabel StepLabel;
    public UILabel VersionLabel;
    public UILabel tapToContinue;
    public UILabel loadingtipLabel;
    public Finished OnFinished;

    public delegate void Finished();
    private float nextValue;
    private bool justSwitch;
    private bool waitTap;
    private TweenAlpha ta;

    //加载提示信息
    IEnumerator ShowLoadingTip(float duration)
    {
        float waitTime = duration;/// (ObjectivesManager.LoadingtipData.Count - 1);
        float randomindex = Random.value * ObjectivesManager.LoadingtipData.Count;
        while (ObjectivesManager.LoadingtipData != null && ObjectivesManager.LoadingtipData.Count > 0)
        {
            int randomIndex = Random.Range(0, ObjectivesManager.LoadingtipData.Count - 1);
            loadingtipLabel.text = ObjectivesManager.LoadingtipData[randomIndex];
            yield return new WaitForSeconds(waitTime);
        }
    }
	// Use this for initialization
    protected override void Start ()
	{      
        waitTap = justSwitch = false;
        tapToContinue.gameObject.SetActive(false);
        StepLabel.gameObject.SetActive(true);
        VersionLabel.text = "版本号: " + CurrentBundleVersion.version;
        
        
	}
    public override void appear()
    {
        base.appear();
        StartCoroutine(ShowLoadingTip(3f));
        
    }
    public override void disappear()
    {
        base.disappear();
    }
	// Update is called once per frame
	void Update ()
	{

        if (isFinished() && justSwitch)
            disappear();

        if (isFinished() && !waitTap)
        {
            waitTap = true;

	        tapToContinue.text = "点击屏幕继续";
	        ta = TweenAlpha.Begin<TweenAlpha>(tapToContinue.gameObject, 1f);
            ta.style= UITweener.Style.PingPong;
	        ta.to = 0f;
	        ta.from = 1;

            tapToContinue.gameObject.SetActive(true);
            StepLabel.gameObject.SetActive(false);
	        UIEventListener.Get(gameObject).onClick = ClickToContinue;
	    }

	    if (ProgressBar)
        {
            float curValue = ProgressBar.value;
            if(justSwitch)
                ProgressBar.value = Mathf.Lerp(curValue, 1, Time.deltaTime * 5f);
            else
	            ProgressBar.value = Mathf.Lerp(curValue, nextValue, Time.deltaTime*2f);
        }
	}

    void ClickToContinue(GameObject g)
    {
        UIEventListener.Get(gameObject).onClick = null;
        Destroy(ta);
        if (OnFinished != null)
            OnFinished();
    }

    public void SetProgressValue(float value)
    {
        if (value > nextValue)
            nextValue = value;
    }

    public void NextStep(string action)
    {
        nextValue += 0.1f;
        if (nextValue >= 1)
            nextValue = 1;

        if (StepLabel)
            StepLabel.text = action;
    }

    public bool isFinished()
    {
        return (ProgressBar.value >0.999f);
    }

    public void SwitchToMainUI()
    {
        if (ProgressBar)
            ProgressBar.value = 0;
        if (StepLabel)
            StepLabel.text = "";
        if (tapToContinue)
            tapToContinue.text = "";
        if (VersionLabel)
            VersionLabel.text = "";
        appear();
        StartCoroutine(ShowLoadingTip(3f));
        justSwitch = true;
    }
}
