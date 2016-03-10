using UnityEngine;
using System;
using System.Collections;

public class ScoreUI : MonoBehaviour 
{
    public UILabel coinMulTxt;
    public UILabel scoreMulTxt;
	public UILabel ScoreLabel;
    public UILabel MedalScoreLabel;
	public UILabel CoinLabel;
	public UILabel GemLabel;	
    public UILabel BoxLabel;
    public Transform boxSprite;
    public Transform coinSprite;
//    public GameObject 
//    public Color doubleColor;

	private float lastAnimCoinVal;
	private float lastAnimCoinTime;
	
	private int _lastScore = 0;
	private int _score = 0;

    void Awake()
    {
        CoinLabel.text = "0";
        ScoreLabel.text = "0";
        BoxLabel.text = "0";
    }


	public void SetScore(int score) 
	{
		_score = score;
	}
	
    TweenAlpha ta;
	private bool UpdateScore()
	{
	    if (scoreMulTxt)
	    {
            scoreMulTxt.text = "x" + GamePlayer.SharedInstance.GetDisplayScoreMultiplier();
            //双倍得分效果
	        if (GamePlayer.SharedInstance.hasDoubleScore||GamePlayer.SharedInstance.addScoreMultiple>0)
            {
                scoreMulTxt.color = Color.yellow;
                if(ta==null)
                {
                    ta= UITweener.Begin<TweenAlpha>(scoreMulTxt.transform.parent.gameObject,0.5f);
                    ta.from = 0.2f;
                    ta.style = UITweener.Style.PingPong;
                }
                else
                {
                    ta.enabled = true;
                }
            }
	        else
            {
                if(ta && ta.enabled)
                {
                    ta.enabled = false;
                    scoreMulTxt.transform.parent.GetComponent<UISprite>().alpha = 1f;
                }
	            scoreMulTxt.color = Color.white;
            }
	    }

	    if(_score != _lastScore)
		{
			ScoreLabel.text = _score.ToString();
			if((_score == 0)||((int)Math.Floor(Math.Log10(_score)) != (int)Math.Floor(Math.Log10(_lastScore))))
			{
//				RepositionBar(scoreBar, null, _score, -30);
			}
			_lastScore = _score;
			return true;
		}
		return false;
		//ScoreLabel.transform.localPosition = new Vector3(ScoreLabel.transform.localPosition.x, ScoreLabel.transform.localPosition.y, -5.0f);
		
	}
	
	private int _lastCoins = 0;
	private int _coins = 0;
	public void SetCoinCount(int coins) 
    {  
		_coins = coins;
	}
	
	private bool UpdateCoins()
	{
	    int coinMultiplier = GamePlayer.SharedInstance.GetCoinMultiplier();
        if (coinMultiplier == 1)
            coinMulTxt.gameObject.SetActive(false);
        else
        {
            coinMulTxt.gameObject.SetActive(true);
            coinMulTxt.text = "x" + coinMultiplier;
        }

		if(_coins != _lastCoins)
		{
            CoinLabel.text = _coins.ToString();
//			if((_coins == 0)||((int)Math.Floor(Math.Log10(_coins)) != (int)Math.Floor(Math.Log10(_lastCoins))))
//			{
////				RepositionBar(coinBar, coinSprite, _coins, 10);
//			}
			_lastCoins = _coins;
			return true;
		}

		return false;
		//CoinLabel.transform.localPosition = new Vector3(CoinLabel.transform.localPosition.x, CoinLabel.transform.localPosition.y, -3.0f);
	}
	
	public void Update()
	{
		switch(Time.frameCount%4)
		{
		case 0:
			UpdateScore();
			break;
		case 2:
			UpdateCoins();
			break;
		default:
			break;
		}		
	}
	
	public void SetGemCount(int gems) 
	{
		GemLabel.text = gems.ToString();
        //int digits = 0;
        //if (gems == 0) { digits = 1; }
        //else { digits = (int)Math.Floor(Math.Log10(gems) + 1); }
//		Vector3 pos = new Vector3(-90f + digits * 30f, gemRoot.localPosition.y, -1.0f);
//		TweenPosition tp = TweenPosition.Begin(gemRoot.gameObject,0.5f, pos);
//		EventDelegate.Add(tp.onFinished,OnShowGem);
		
	}



    public void SetTreasureBoxCount(int boxs)
    {
        BoxLabel.text = boxs.ToString();
    }


	public void OnShowGem(){
//		tween.onFinished -= OnShowGem;
		EventDelegate.Remove(TweenPosition.current.onFinished,OnShowGem);
		
//		Vector3 pos = new Vector3(gemRoot.localPosition.x + 0.01f, gemRoot.localPosition.y, -1.0f);
//		TweenPosition tp = TweenPosition.Begin(gemRoot.gameObject,2f, pos);
//		EventDelegate.Add(tp.onFinished,OnShowGemFinished);
		
	}
	public void OnShowGemFinished(){
//		tween.onFinished -= OnShowGemFinished;
		EventDelegate.Remove(TweenPosition.current.onFinished,OnShowGemFinished);
		
//		Vector3 pos = new Vector3(-164, gemRoot.localPosition.y, -1.0f);
//		TweenPosition.Begin(gemRoot.gameObject,0.5f, pos);
	}

	private void RepositionBar(Transform bar, Transform sprite, int currencyVal, float offset = 0f)	
	{		
		int digits = 0;
		
		if (currencyVal == 0) { digits = 1; }
		else { digits = (int)Math.Floor(Math.Log10(currencyVal) + 1); }
		
		bar.localPosition = new Vector3(
			//-0.26f + (0.035f * (float)(digits - 1)),
			-620f + offset +  (30f * (float)(digits - 1)),
			bar.transform.localPosition.y,
			bar.transform.localPosition.z);
		
		if (sprite != null)
		{
			sprite.localPosition = new Vector3(
				//0.04f + (0.035f * (float)(digits - 1)),
				30f + (30f * (float)(digits - 1)),
				sprite.transform.localPosition.y,
				sprite.transform.localPosition.z);	
		}
	}

	public void ResetCurrencyBars()	
	{
		SetScore(0);
		SetCoinCount(0);
		//SetGemCount(0);
	}
	
	public void ScoreBonusEffects()
	{
//		if(scoreBonusElectric) scoreBonusElectric.Play(true);		
//		if(scoreBonusSpark) scoreBonusSpark.Play(true);		
//		if(scoreBonusSmoke) scoreBonusSmoke.Play(true);			
	}	
}
