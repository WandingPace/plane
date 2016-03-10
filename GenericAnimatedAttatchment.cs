using UnityEngine;
using System.Collections;

public class GenericAnimatedAttatchment : TrackPieceAttatchment
{
	public string IdleAnimString = "idle";
	public string EventStartAnimString = "attack";
	public string EventIdleAnimString = "attack_idle";
	
	public ParticleSystem KillParticle;
	
	public string FallAnimString = "fall";
	
	//public float EventAnimLength = 1f;
	
	private float GemChance = 0.225f;
	
	public Collider colliderRef;
	
	public bool isLeft = true;
	
	private bool dead = false;
	
	public AudioClip sfx;
	
	public Renderer myRenderer; 
	
	public bool UseTriggers = false;
	
	public bool IsDead { get { return dead; } }
	
	public AnimatedTile OnlyDisableColliderIfThisHasntPlayed;
	
	//Used for translating the fall animation from the root to the current position of the enemy
	public Transform animRoot;	
	public Transform animParent;
	
	public override void Awake()
	{
		base.Awake();
		
		if(myRenderer!=null)	myRenderer.enabled = false;
	}

	public override void OnPlayerEnteredPreviousPreviousTrackPiece()
	{
		if(!UseTriggers)
			StartCoroutine(Animate());
	}
	
	public override void OnEnable()
	{
		base.OnEnable();
		StartCoroutine(Spawned()); 
		if(colliderRef!=null)	colliderRef.enabled = true;
		
		if(myRenderer!=null)	myRenderer.enabled = false;
		
		if(animParent)
			animParent.transform.localPosition = new Vector3(0,0,0);
	}
	
	public override void OnDisable()
	{
		dead = false;
		if(colliderRef!=null)	colliderRef.enabled = true;
	}
	
	/*public void OnTriggerEnter(Collider other)
	{
		if(!dead && other.gameObject == GameController.SharedInstance.Player.gameObject)
		{
			GameController.SharedInstance.Player.Stumble();
		}
	}*/
	
	public void OnTriggerEnter(Collider other)
	{
		if(UseTriggers)
		{
			if(!dead && other.gameObject == GameController.SharedInstance.Player.gameObject)
				StartCoroutine(Animate());
		}
	}
	
	IEnumerator Spawned()
	{
		Rewind();
		/*if (animation.GetClip(IdleAnimString) == null)
		{
			// note no notify instantiated at this point where we're getting this error
		//	Debug.LogError("no " + IdleAnimString + " animation for piece " + gameObject.name);
			Rewind();
			yield break;
		}
		animation.Play(IdleAnimString);*/
		yield return null;
		//yield return new; //WaitForSeconds(.75f);
	}
	
	//Spawns gem in front of player
//	IEnumerator SpawnGem()
//	{
//		TrackPiece trackPieceToSpawnOn = GamePlayer.SharedInstance.OnTrackPiece.NextTrackPiece.NextTrackPiece.NextTrackPiece;
//		
//		BonusItem gem = (BonusItem)BonusItem.Create(BonusItem.BonusItemType.Gem);
//		trackPieceToSpawnOn.BonusItems.Add(gem);
//		gem.transform.position = colliderRef.transform.position+Vector3.up;
//		
//		Vector3 targetpos = trackPieceToSpawnOn.transform.position + Vector3.up*2f;
//		
//		bool gemActivated = false;
//		
//		while(gem.transform.position!=targetpos && gem!=null && (gem.gameObject.active||!gemActivated))
//		{
//			if(!gemActivated && gem.gameObject.active)	gemActivated = true;
//			
//			float yval = Mathf.Max(0f, ((gem.transform.position - targetpos).magnitude-2f))/2f;
//			gem.transform.position = Vector3.MoveTowards(gem.transform.position,targetpos+Vector3.up*yval,Time.deltaTime*50f);
//			
//			yield return null;
//		}
//	}
	
	IEnumerator SpawnGem()
	{
		TrackPiece trackPieceToSpawnOn = GamePlayer.SharedInstance.OnTrackPiece.NextTrackPiece.NextTrackPiece;
		
		BonusItem gem = (BonusItem)BonusItem.Create(BonusItem.BonusItemType.Gem);
		trackPieceToSpawnOn.BonusItems.Add(gem);
		gem.transform.position = transform.position+Vector3.up;
		
		Vector3 targetpos = trackPieceToSpawnOn.transform.position + Vector3.up*2f;
		
		bool gemActivated = false;
		
		while(gem.transform.position!=GamePlayer.SharedInstance.transform.position/*targetpos*/ && gem!=null && (gem.gameObject.activeSelf||!gemActivated))
		{
			targetpos = GamePlayer.SharedInstance.transform.position;

            if (!gemActivated && gem.gameObject.activeSelf) gemActivated = true;
			
			//Instead of lerping it, we are going to just give it to the player. I'm leaving the while loop in case we decide to change it back,
			//gem.transform.position = GamePlayer.SharedInstance.transform.position;
		//	float yval = Mathf.Max(0f, ((gem.transform.position - targetpos).magnitude-2f))/2f;
			gem.transform.position = Vector3.MoveTowards(gem.transform.position,targetpos/*+Vector3.up*yval*/,Time.deltaTime*30f);
			
			yield return null;
		}
	}	

	IEnumerator SpawnMegaCoin()
	{
		TrackPiece trackPieceToSpawnOn = GamePlayer.SharedInstance.OnTrackPiece.NextTrackPiece.NextTrackPiece;
		
		BonusItem megaCoin = (BonusItem)BonusItem.Create(BonusItem.BonusItemType.MegaCoin);
		trackPieceToSpawnOn.BonusItems.Add(megaCoin);
		megaCoin.transform.position = transform.position+Vector3.up;
		
		Vector3 targetpos = trackPieceToSpawnOn.transform.position + Vector3.up*2f;
		
		bool megaCoinActivated = false;

        while (megaCoin.transform.position != GamePlayer.SharedInstance.transform.position/*targetpos*/ && megaCoin != null && (megaCoin.gameObject.activeSelf || !megaCoinActivated))
		{
			targetpos = GamePlayer.SharedInstance.transform.position;

            if (!megaCoinActivated && megaCoin.gameObject.activeSelf) megaCoinActivated = true;
			
			//Instead of lerping it, we are going to just give it to the player. I'm leaving the while loop in case we decide to change it back,
			//gem.transform.position = GamePlayer.SharedInstance.transform.position;
		//	float yval = Mathf.Max(0f, ((gem.transform.position - targetpos).magnitude-2f))/2f;
			megaCoin.transform.position = Vector3.MoveTowards(megaCoin.transform.position,targetpos/*+Vector3.up*yval*/,Time.deltaTime*30f);
			
			yield return null;
		}
	}	
	
	public void Kill()
	{
		StartCoroutine(Kill_internal());
	}
	
	IEnumerator Kill_internal()
	{
		dead = true;
		
		DisableCollider();
		
		if(FallAnimString!="")	{
			//Renderer[] renderers = GetComponentsInChildren <Renderer>(true);
			if(animRoot && animParent) {			
				animParent.transform.Translate (animRoot.transform.localPosition);			
			}		
			
			animation.Play(FallAnimString);
		}
		
		if(KillParticle!=null)	KillParticle.Play();
		
		yield return new WaitForSeconds(0.1f);
		
		if(KillParticle!=null)	KillParticle.Stop();
		
		if(isLeft)
		{
			float randVal = Random.value;
			
			if(randVal <= GemChance)		
				GamePlayer.SharedInstance.StartCoroutine(SpawnGem());
			
			else
				GamePlayer.SharedInstance.StartCoroutine(SpawnMegaCoin());
		}
		
		yield break;
	}
	
	
	void DisableCollider()
	{
		if (OnlyDisableColliderIfThisHasntPlayed == null)
		{
			if(colliderRef!=null)	
			{
				colliderRef.enabled = false;
				//Debug.Log (colliderRef.gameObject.name + " was disabled!");
			}
		}
		else
		{
			if(!OnlyDisableColliderIfThisHasntPlayed.DidAnimate)
			{
				if(colliderRef!=null)	colliderRef.enabled = false;
			}
			if(Time.time - OnlyDisableColliderIfThisHasntPlayed.TimeTriggered < 0.5f)
			{
				OnlyDisableColliderIfThisHasntPlayed.PlayAllParticles(false);
				if(colliderRef!=null)	colliderRef.enabled = false;
			}
		}
	}
	
	IEnumerator Animate()
	{
		if(dead)	yield break;
		
		if(myRenderer!=null)	myRenderer.enabled = true;
		
		if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			animation[EventStartAnimString].speed = 2f*GameController.SharedInstance.Player.getRunVelocity()/GameController.SharedInstance.Player.getModfiedMaxRunVelocity();
		
		if(animation.isPlaying)
			animation.CrossFade(EventStartAnimString);
		else
			animation.Play(EventStartAnimString);
		//yield return new WaitForSeconds(0.2f);
		//audio.volume = AudioManager.SharedInstance.SoundVolume;
		//audio.Play();
		
		if(sfx){
			AudioManager.SharedInstance.PlayAnimatedSound(sfx);
		}
		//yield return new WaitForSeconds(EventAnimLength/animation[EventStartAnimString].speed);
		if(EventIdleAnimString!="")	animation.CrossFadeQueued(EventIdleAnimString);
	}
	
	void Rewind()
	{
		notify.Debug ("AnimatedTile.Rewind");
		if(animation!=null)
		{
			
			if(IdleAnimString==null || IdleAnimString=="")
			{
				if(animation[EventStartAnimString]!=null)
				{
					animation.Play(EventStartAnimString);
					animation[EventStartAnimString].enabled = true;
					animation[EventStartAnimString].time = 0f;
					animation.Sample();
					animation[EventStartAnimString].enabled = false;
				}
			}
			else
			{
				if(animation[IdleAnimString]!=null)
				{
			//		Debug.Log("Play: "+gameObject.name + " " + IdleAnimString);
					animation.Play(IdleAnimString);
				}	
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
