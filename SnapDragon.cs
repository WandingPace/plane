using UnityEngine;
using System.Collections;


public class SnapDragon : TrackPieceAttatchment {
	
	
	private bool dead = false;
	public ParticleSystem wandStarEffect;
	public ParticleSystem wandSmokeEffect;	
	public Collider colliderRef;
	public AudioClip sfx;
	private float GemChance = 0.15f;
	private float CoinChance = 1f;
	

	public override void OnPlayerEnteredPreviousTrackPiece()
	{
		StartCoroutine(Animate());
	}
	
	public override void OnEnable()
	{
		base.OnEnable();
//		if(dead)	Debug.Log("Enabled",gameObject);
		StartCoroutine(Spawned()); 
		if(colliderRef!=null)	colliderRef.enabled = true;
	}
	
	public override void OnDisable()
	{
//		if(dead)	Debug.Log("Disabled",gameObject);
		dead = false;
		if(colliderRef!=null)	colliderRef.enabled = true;
		//colliderRef.enabled = true;
	}
	
	/*public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == GameController.SharedInstance.Player.gameObject)
		{
			GameController.SharedInstance.Player.Stumble();
		}
	}*/
	
	public void Kill()
	{
		StartCoroutine(Kill_internal());
	}
	
	public IEnumerator Kill_internal()
	{
		dead = true;
		if(colliderRef!=null)	{Debug.Log("Collider disabled.");colliderRef.enabled = false;}
		else Debug.Log("No collider!");
		//gameObject.SetActive(false);
		
		yield return StartCoroutine(GamePlayer.SharedInstance.playerFx.ShootWandParticle(transform));
	
		animation.Play("hit");
		
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_WandImpact_01);
		
		if(wandSmokeEffect != null)	wandSmokeEffect.Play();
		if(wandStarEffect != null)	wandStarEffect.Play();
		
		yield return new WaitForSeconds(0.4f);
		
		float randNum = Random.value;
		Debug.Log("Random Num: " + randNum);
		
		if(randNum<GemChance)
		{
			Debug.Log ("Spawned Gem");	
			GamePlayer.SharedInstance.StartCoroutine(SpawnGem());
		}
		
		else if(randNum<CoinChance)
		{
			Debug.Log ("Spawned Mega");	
			GamePlayer.SharedInstance.StartCoroutine(SpawnMegaCoin());	
		}
		
		
	}
	
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
			
			if(!gemActivated && gem.gameObject.activeSelf)	gemActivated = true;
			
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
		
		while(megaCoin.transform.position!=GamePlayer.SharedInstance.transform.position/*targetpos*/ && megaCoin!=null && (megaCoin.gameObject.activeSelf||!megaCoinActivated))
		{
			targetpos = GamePlayer.SharedInstance.transform.position;
			
			if(!megaCoinActivated && megaCoin.gameObject.activeSelf)	megaCoinActivated = true;
			
			//Instead of lerping it, we are going to just give it to the player. I'm leaving the while loop in case we decide to change it back,
			//gem.transform.position = GamePlayer.SharedInstance.transform.position;
		//	float yval = Mathf.Max(0f, ((gem.transform.position - targetpos).magnitude-2f))/2f;
			megaCoin.transform.position = Vector3.MoveTowards(megaCoin.transform.position,targetpos/*+Vector3.up*yval*/,Time.deltaTime*30f);
			
			yield return null;
		}
	}
	
	IEnumerator Spawned()
	{
		//TODO: clean this up
		//animation.Play("Emerge");
		
		
		animation.Play("Idle");
		yield return null;
		//yield return new; //WaitForSeconds(.75f);
	}
	
	IEnumerator Animate()
	{
		if(dead)	yield break;
		
		if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			animation["Attack"].speed = 2f*GameController.SharedInstance.Player.getRunVelocity()/GameController.SharedInstance.Player.getModfiedMaxRunVelocity();
		animation.CrossFade("Attack");
		//yield return new WaitForSeconds(0.2f);
		//if(AudioManager.SharedInstance!=null)	audio.volume = AudioManager.SharedInstance.SoundVolume;
		//audio.Play();
		
		
		if(sfx){
			AudioManager.SharedInstance.PlayAnimatedSound(sfx);
		}
		
		yield return new WaitForSeconds(0.75f);
		animation.CrossFade("Idle");
	}
}
