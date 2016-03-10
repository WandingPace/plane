using UnityEngine;
using System.Collections;

public class TTAnimatedTile : MonoBehaviour {
	
	public ParticleSystem[] effects;
	
	float lightningFlashDelayTime = 0.0f;
	bool lightningFlashDelay = false;
	bool lightningFlashFadeIn = false;
	bool lightningFlashFadeOut = false;	
	float lightningFlashDuration = 0.0f;
	float lightningFlashMinFogDensity = 0.06f; //Hardcoded value for fog in tunnel
	float lightningFlashMaxFogDensity = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(lightningFlashDelay)
		{
			lightningFlashDelayTime -= Time.deltaTime;		
			
			if(lightningFlashDelayTime <=0f)
			{
				lightningFlashDelayTime = 0f;
				lightningFlashFadeIn = true;
				lightningFlashDelay = false;
			}

		}		
		
		if(lightningFlashFadeIn && !lightningFlashFadeOut && 
			(GamePlayer.SharedInstance.fogTransitionSign == 0) && GamePlayer.SharedInstance.fogTransition)
		{
			RenderSettings.fogDensity += (Time.deltaTime*lightningFlashMaxFogDensity)/(lightningFlashDuration/4.0f);
			if(RenderSettings.fogDensity > lightningFlashMaxFogDensity)
			{
				RenderSettings.fogDensity = lightningFlashMaxFogDensity;
				lightningFlashFadeIn = false;				
				lightningFlashFadeOut = true;
			}
		}
		
		if(lightningFlashFadeOut && !lightningFlashFadeIn && 
			(GamePlayer.SharedInstance.fogTransitionSign == 0) && GamePlayer.SharedInstance.fogTransition)
		{
			RenderSettings.fogDensity -= (Time.deltaTime*lightningFlashMaxFogDensity)/(lightningFlashDuration/4.0f*3.0f);
			if(RenderSettings.fogDensity <= lightningFlashMinFogDensity)
			{
				RenderSettings.fogDensity = lightningFlashMinFogDensity;
				lightningFlashFadeOut = false;
			}
		}
		
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == GameController.SharedInstance.Player.gameObject)
		{
			Animate();
		}
	}
	
	void Animate()
	{
		if(!GamePlayer.SharedInstance.whiteoutTransition && (GamePlayer.SharedInstance.fogTransitionSign == 0) && 
			GamePlayer.SharedInstance.fogTransition)
		{
			float randomStart = Random.Range(0.01f, 0.4f);
			ParticleSystem randomEffect1 = effects[Random.Range(0,effects.Length)];
			ParticleSystem randomEffect2 = effects[Random.Range(0,effects.Length)];
			randomEffect1.startDelay = randomStart;		
			randomEffect1.Play ();
			randomEffect2.startDelay = randomStart + 0.1f;		
			randomEffect2.Play ();
			
			LightningFlash(randomStart, 0.4f, Random.Range(0.1f,0.15f));	
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.Lightning01, Random.Range(0.5f, 0.7f), Random.Range(0.7f, 1.0f));
		}
	}
	
	public void LightningFlash(float delay, float duration, float fogDensityMax)
	{
		lightningFlashDuration = duration;
		lightningFlashMaxFogDensity = fogDensityMax;
		
		if(delay > 0f)
		{
			lightningFlashDelayTime = delay;
			lightningFlashDelay = true;
		}
		else
			lightningFlashFadeIn = true;
		
		//lightningFlashFadeIn = true;		
	}
	
	public void PlayAllLightning()
	{
		foreach (ParticleSystem item in effects)
		{
			item.Play();
		}		
	}
}
