/*
using UnityEngine;

public interface IFadePanel
{
	void SetParameters(bool fadeInSetting, float fadeTime);
	void AddCallback(FadePanel.fadeDoneCallback callback);
}

public class FadePanel : MonoBehaviour, IFadePanel			
{
	private bool _fadeIn = true;					// true = fadeIn, false = fadeOut
    private float _duration = 0.3f;
    private float _mStart = 0f;
    private float _mFinish = 1f;	
	private float _alpha = 0f;	
    private UIWidget[] _mWidgets;
 
	public delegate FadePanel fadeDoneCallback(float duration = 0f, FadePanel.fadeDoneCallback callback = null);	//, GameObject go = null);	// functions called when fade is complete
	public event fadeDoneCallback onFadeDone;	
	
	public void SetParameters(bool isFadeIn, float fadeTime)
	{
		_fadeIn = isFadeIn;
		_duration = fadeTime;
	}		
	
	public void AddCallback(fadeDoneCallback callback)
	{
		onFadeDone += callback;
	}
	
    void Start()
    {
        _mStart = Time.realtimeSinceStartup;
		_mFinish = (_fadeIn) ? 1f : 0f;		
        _mWidgets = GetComponentsInChildren<UIWidget>();
		
		//Destroy(gameObject.GetComponent<FadePanel>());	// kill old lingering FadePanel component, if exists
		_alpha = (_fadeIn) ? 0f : 1f;						// set alpha to correct start value immediately, to eliminate pop-in 1st frame
		UpdateEachAlpha(_alpha);							// update each widget's alpha to correct starting value
    }
 
    void Update()
    {
		if (_fadeIn)
			_alpha = (_duration > 0f) ? Mathf.Clamp01((Time.realtimeSinceStartup - _mStart) / _duration) : 1f;
		else
        	_alpha = (_duration > 0f) ? 1f - Mathf.Clamp01((Time.realtimeSinceStartup - _mStart) / _duration) : 0f;
 
		UpdateEachAlpha(_alpha);
		
        if (_alpha == _mFinish)
		{
			if (onFadeDone != null)
				onFadeDone();			// trigger callbacks
			Destroy(this);				// end myself
		}
    }

	private void UpdateEachAlpha(float _alpha)		// actually update each alpha value
	{
        foreach (UIWidget w in _mWidgets)
        {
            Color c = w.color;
            c.a = _alpha;
            w.color = c;
        }		
	}
}
*/

//		tutorialCollectCoins.alpha = 0f;
//		tutorialCollectCoins.gameObject.SetActive(true);
//		TweenAlpha ta = TweenAlpha.Begin(tutorialCollectCoins.gameObject, 0.5f, 1f);
//		ta.onFinished += OnShowCollectCoinsTutorial;
