using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monkey : MonoBehaviour {
	
	public static Monkey main;
	
	[System.NonSerialized]
	public bool done = false;
	
	public Animation MonkeyModelRoot;
	public Transform ParentingTransform;
	
	private float CoinChance = 1f;
	private float GemChance = 0.3f;

	private static List<Monkey> allMonkeys = new List<Monkey>();
	
	private bool attacking = false;
	
	float AnimationSpeed
	{
		get
		{
			return 1f;
			//return GameController.SharedInstance.Player.getRunVelocity()/15f; 
		}
	}
	
	private static bool IsCarrying = false;
	
	public ParticleSystem wandStarEffect;
	public ParticleSystem wandSmokeEffect;	
	
	public AudioClip WingSFX;
	public AudioClip TurnSFX;
	public AudioClip AttackSFX;
	
	void Awake()
	{
	}
	
	void Start()
	{
		IsCarrying = false;
		
		if(audio!=null)
		{
			audio.clip = WingSFX;
			
			audio.volume = AudioManager.SharedInstance.SoundVolume;
			audio.Play();
			
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.BossBaboon);
		}
		
		if(AnimationSpeed < 0.5f)
			DisableNow();
	}
	
	void OnDestroy()
	{
		if(allMonkeys.Contains(this))
			allMonkeys.Remove(this);
	}
	
	
	
	
	public static void KillOne()
	{
		if(allMonkeys.Count>0)
		{
			Monkey m = allMonkeys[allMonkeys.Count-1];
			m.Kill();
		}
	}
	
	
	void OnPlayerEnteredTrackPiece()
	{
		BeginAttack();
	}
	
	
	public void BeginAttack()
	{
		main = this;
		StartCoroutine(Attack());
	}
	void OnDisable()
	{
		StopAllCoroutines();
		attacking = false;
	}
	
	
	IEnumerator Attack()
	{
		if(!GamePlayer.SharedInstance.IsInvicible())
		{
			//MonkeyModelRoot["idle"].speed = 0.6f;
			MonkeyModelRoot["idle"].speed = 0.7f * (GamePlayer.SharedInstance.GetPlayerVelocity().magnitude/GamePlayer.SharedInstance.getModfiedMaxRunVelocity());
			MonkeyModelRoot.Play ("idle");
			attacking = true;
			
			allMonkeys.Add(this);
			
			if(audio!=null)
			{
				audio.clip = WingSFX;
				
				audio.volume = AudioManager.SharedInstance.SoundVolume;
				audio.Play();
				
				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.BossBaboon);
			}
			
			while(MonkeyModelRoot.isPlaying)
				yield return null;
			done = true;
		}
	//	StartCoroutine(Disable());
		DisableNow();
	}
	
	public void Kill()
	{
		if(MonkeyModelRoot.isPlaying)
		{
			
			StartCoroutine(Disable());
			
		}
	}
	
	public IEnumerator Disable()
	{
		if(allMonkeys.Contains(this))
			allMonkeys.Remove(this);
		
		GetComponentInChildren<Collider>().enabled = false;
		
		yield return StartCoroutine(GamePlayer.SharedInstance.playerFx.ShootWandParticle(MonkeyModelRoot.transform));
		
		MonkeyModelRoot.CrossFade("hit");
		
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_WandImpact_01);
		
		if(wandSmokeEffect != null)	wandSmokeEffect.Play();
		if(wandStarEffect != null)	wandStarEffect.Play();
		
		yield return new WaitForSeconds(0.5f);
		
		if(!GameController.SharedInstance.IsTutorialMode)
		{
			float randNum = Random.value;
			if(randNum < GemChance)
				GamePlayer.SharedInstance.StartCoroutine(SpawnGem());
			else if(randNum < CoinChance)
				GamePlayer.SharedInstance.StartCoroutine(SpawnMegaCoin());
		}
		
		if(!IsCarrying)
			transform.parent.gameObject.SetActive(false);
		
		if(wandSmokeEffect != null) { wandSmokeEffect.gameObject.SetActive(true);	wandSmokeEffect.Play(); }
        if (wandStarEffect != null) { wandStarEffect.gameObject.SetActive(true); wandStarEffect.Play(); }
	}

	IEnumerator SpawnMegaCoin()
	{
		TrackPiece trackPieceToSpawnOn = GamePlayer.SharedInstance.OnTrackPiece.NextTrackPiece.NextTrackPiece;
		
		BonusItem megaCoin = (BonusItem)BonusItem.Create(BonusItem.BonusItemType.MegaCoin);
		trackPieceToSpawnOn.BonusItems.Add(megaCoin);
		megaCoin.transform.position = ParentingTransform.position+Vector3.up;
		
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
	
	IEnumerator SpawnGem()
	{
		TrackPiece trackPieceToSpawnOn = GamePlayer.SharedInstance.OnTrackPiece.NextTrackPiece.NextTrackPiece;
		
		BonusItem gem = (BonusItem)BonusItem.Create(BonusItem.BonusItemType.Gem);
		trackPieceToSpawnOn.BonusItems.Add(gem);
		gem.transform.position = ParentingTransform.position+Vector3.up;
		
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
	
	public void DisableNow()
	{
		if(allMonkeys.Contains(this))
			allMonkeys.Remove(this);
		GetComponentInChildren<Collider>().enabled = false;
		transform.parent.gameObject.SetActive(false);
	}
	
	IEnumerator OnTriggerEnter(Collider other)
	{
		if(other.gameObject == GameController.SharedInstance.Player.gameObject && !IsCarrying && attacking)
		{
			if(GamePlayer.SharedInstance.HasBoost == false && GamePlayer.SharedInstance.HasPoof==false)
			{
				AttatchPlayer();
				
				GameController.SharedInstance.Player.AnimateObject.CrossFade("CarriedOff01");
	
				MonkeyModelRoot.Play("catch");
				yield return new WaitForSeconds(1f); 
				
				DetatchPlayer();
				
				GameController.SharedInstance.Player.Kill(DeathTypes.Baboon);
			}
			else if(GamePlayer.SharedInstance.HasPoof)
				GamePlayer.SharedInstance.EndPoof();
		}
		yield break;
	}
	
	public void AttatchPlayer()
	{
		IsCarrying = true;
		
		Enemy.main.enabled = false;
		
		GamePlayer.SharedInstance.Dying = true;
		
		//transform.parent.GetComponent<FollowObject>().enabled = false;//.Stop();
		((OzGameCamera)OzGameCamera.SharedInstance).Stop();
	//	GamePlayer.SharedInstance.enabled = false;
		GamePlayer.SharedInstance.CharacterModel.transform.parent = ParentingTransform.transform;
		
		//GamePlayer.SharedInstance.CharacterModel.transform.localRotation = Quaternion.identity;
		GamePlayer.SharedInstance.CharacterModel.transform.localPosition = Vector3.down*0.1f + Vector3.forward*0.85f;
		
	}
	
	public void DetatchPlayer()
	{
		IsCarrying = false;
		
		//transform.parent.GetComponent<FollowObject>().enabled = true;
		((OzGameCamera)OzGameCamera.SharedInstance).Unstop();
		GamePlayer.SharedInstance.enabled = true;
		GamePlayer.SharedInstance.CharacterModel.transform.parent = null;
		GamePlayer.SharedInstance.CharacterModel.transform.localRotation = Quaternion.identity;
		GamePlayer.SharedInstance.CharacterModel.transform.localPosition = Vector3.zero;
		
		
	}
}
