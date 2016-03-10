using UnityEngine;
using System.Collections;

public class CatacombJunAnim :TrackPieceAttatchment {

	
	public Animation anim;
	public string animationString;
	public string animationString02;
	public GameObject ColliderRight;
	public GameObject ColliderLeft;
	public string idleAnimString;
	public ParticleSystem particle01;	
	public ParticleSystem particle02;	
	public ParticleSystem particle03;	
	public ParticleSystem particle04;
	public ParticleSystem particle05;	
	public ParticleSystem particle06;
	public float SoundDelay;
	public AudioClip sfx;
	
	public GenericAnimatedAttatchment DontPlayIfThisIsDead;
	
	private bool didAnimate = false;
	
	public bool DidAnimate { get { return didAnimate; } }

	public override void Awake()
	{
		base.Awake();
		notify.Debug("AnimatedTile.Awake to get animation");
		if(anim==null)
			anim = GetComponentInChildren<Animation>();
	//	if(anim!=null)	anim.playAutomatically = false;
		notify.Debug("AnimatedTile to anim = " + anim);
		if(ColliderLeft)
		{
			ColliderLeft.SetActive(false);
		}
		if(ColliderRight)
		{
            ColliderRight.SetActive(false);
		}
		Rewind();
	}
	
	public void Start()
	{
	//	if(anim!=null)	anim.playAutomatically = false;
		notify.Debug ("AnimatedTile.Start");
		if(ColliderLeft)
		{
            ColliderLeft.SetActive(false);
		}
		if(ColliderRight)
		{
            ColliderRight.SetActive(false);
		}
		Rewind();
	}
	
	void OnSpawned()
	{
		notify.Debug ("AnimatedTile.OnSpawned");
		if(ColliderLeft)
		{
            ColliderLeft.SetActive(false);
		}
		if(ColliderRight)
		{
            ColliderRight.SetActive(false);
		}
		Rewind();
	}
	
	public override void OnEnable()
	{
		notify.Debug ("AnimatedTile.OnEnable");
		base.OnEnable();
		Rewind();
	}
	
	public override void OnDisable()
	{
		base.OnDisable();
		
		PlayParticle(particle01,false);
		PlayParticle(particle02,false);
		PlayParticle(particle03,false);
		PlayParticle(particle04,false);
		PlayParticle(particle05,false);
		PlayParticle(particle06,false);		
		
	}
	
	
	
//	public override void OnPlayerEnteredPreviousTrackPiece()
//	{
//		Animate();
//	}
	
	public void OnTriggerEnter(Collider other)
	{
		notify.Debug ("AnimatedTile.OnTriggerEnter");
		if(other.gameObject == GameController.SharedInstance.Player.gameObject && !GamePlayer.SharedInstance.HasBoost)
		{
			if(DontPlayIfThisIsDead!=null && DontPlayIfThisIsDead.IsDead)
				return;
			
			if(!didAnimate)
				Animate();
		}
	}
	
	public void PlayAllParticles(bool on)
	{
		PlayParticle(particle01,on);
		PlayParticle(particle02,on);
		PlayParticle(particle03,on);
		PlayParticle(particle04,on);
		PlayParticle(particle05,on);
		PlayParticle(particle06,on);	
	}
	
	public void PlayParticle(ParticleSystem prt,bool on)
	{
		if (prt != null)
		{
			notify.Debug("particle01.Play();");
			prt.playbackSpeed = GameController.SharedInstance.Player.getRunVelocity()/10f;
			if(on)
				prt.Play();
			else {
				prt.Stop();
				prt.Clear();
			}
		}
	}
	public float TimeTriggered = 0f;
	
	void Animate()
	{
		int PickWhatToDo;
		PickWhatToDo = Random.Range(1,4);
		
		
		if(PickWhatToDo == 3)
		{
            ColliderLeft.SetActive(false);
            ColliderRight.SetActive(false);
			
			didAnimate = true;
		}
		
		if(PickWhatToDo == 1)
		{

            ColliderLeft.SetActive(true);
            ColliderRight.SetActive(false);
			
			notify.Debug ("AnimatedTile.Animate");
			
			TimeTriggered = Time.time;
			
			float animSpeed = 1.0f;
			if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			{
				animSpeed = GameController.SharedInstance.Player.getRunVelocity()/10f;
				if(anim != null && anim[animationString]!=null)
					anim[animationString].speed = animSpeed;
			}
			
			PlayParticle(particle01,true);
			//PlayParticle(particle02,true);
			//PlayParticle(particle03,true);
			//PlayParticle(particle04,true);
			//PlayParticle(particle05,true);
			//PlayParticle(particle06,true);
			
			if(anim != null)
			{
				if (anim.IsPlaying(animationString))
				{
					// solve a df_straight_over_anim_a crash when animate gets called twice	
					notify.Debug("AnimatedTile.animate returning as animation is playing");
					return;
				}
				
				if(anim.IsPlaying(idleAnimString))
					anim.CrossFade(animationString,0.2f);
				else
				{
					if(anim[animationString]!=null)
						anim.Play(animationString);
				}
			}
			
			if(sfx){
				notify.Debug("calling AudioManager.SharedInstance.PlayAnimatedSound");
				StartCoroutine(PlaySound());
				//AudioManager.SharedInstance.PlayAnimatedSound(sfx);
			}		
			
			didAnimate = true;
			
			notify.Debug ("Animate.Animate reached the end");
		}
		
		if(PickWhatToDo == 2)
		{
            ColliderRight.SetActive(true);
            ColliderLeft.SetActive(false);
			
			notify.Debug ("AnimatedTile.Animate");
			
			TimeTriggered = Time.time;
			
			float animSpeed = 1.0f;
			if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			{
				animSpeed = GameController.SharedInstance.Player.getRunVelocity()/10f;
				if(anim != null && anim[animationString02]!=null)
					anim[animationString02].speed = animSpeed;
			}
			
			//PlayParticle(particle01,true);
			PlayParticle(particle02,true);
			//PlayParticle(particle03,true);
			//PlayParticle(particle04,true);
			//PlayParticle(particle05,true);
			//PlayParticle(particle06,true);
			
			if(anim != null)
			{
				if (anim.IsPlaying(animationString02))
				{
					// solve a df_straight_over_anim_a crash when animate gets called twice	
					notify.Debug("AnimatedTile.animate returning as animation is playing");
					return;
				}
				
				if(anim.IsPlaying(idleAnimString))
					anim.CrossFade(animationString02,0.2f);
				else
				{
					if(anim[animationString02]!=null)
						anim.Play(animationString02);
				}
			}
			
			if(sfx){
				notify.Debug("calling AudioManager.SharedInstance.PlayAnimatedSound");
				StartCoroutine(PlaySound());
				//AudioManager.SharedInstance.PlayAnimatedSound(sfx);
			}		
			
			didAnimate = true;
			
			notify.Debug ("Animate.Animate reached the end");
		}
		
	}
	
	IEnumerator PlaySound() {	
        yield return new WaitForSeconds(SoundDelay / (GameController.SharedInstance.Player.getRunVelocity()/10));
		
        AudioManager.SharedInstance.PlayAnimatedSound(sfx);
    }
	
	void Rewind()
	{
		didAnimate = false;
		notify.Debug ("AnimatedTile.Rewind");
		if(anim!=null)
		{
			
			if(idleAnimString==null || idleAnimString=="")
			{
				if(anim[animationString]!=null)
				{
					anim.Play(animationString);
					anim[animationString].enabled = true;
					anim[animationString].time = 0f;
					anim.Sample();
					anim[animationString].enabled = false;
				}
			}
			if(idleAnimString==null || idleAnimString=="")
			{
				if(anim[animationString02]!=null)
				{
					anim.Play(animationString02);
					anim[animationString02].enabled = true;
					anim[animationString02].time = 0f;
					anim.Sample();
					anim[animationString02].enabled = false;
				}
			}
			else
			{
				if(anim[idleAnimString]!=null)
					anim.Play(idleAnimString);
			}
			//anim.Stop();
			//anim.Play();
			//anim[animationString].speed = 0f;
			//yield return null;
			//anim.Rewind();
			//anim.Stop();
		}
	}
}
