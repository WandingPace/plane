using UnityEngine;
using System.Collections;

public class WWTreeCollapse : TrackPieceAttatchment {

	
	private Animation anim;
	public string animationString = "treefall";
	public AudioClip sfx;
	//public ParticleSystem treeBreak;
	public ParticleSystem[] effects;	
	
	public override void Awake()
	{
		anim = GetComponentInChildren<Animation>();
		if(anim!=null)	anim.playAutomatically = false;
	}
	
	void OnSpawned()
	{
		StartCoroutine(Rewind());
	}
	public override void OnEnable()
	{
		
		base.OnEnable();
		StartCoroutine(Rewind());
	}
	
	
	
//	public override void OnPlayerEnteredPreviousTrackPiece()
//	{
//		Animate();
//	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == GameController.SharedInstance.Player.gameObject)
		{
			Animate();
		}
	}
	
	void Animate()
	{
		if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			anim[animationString].speed = GameController.SharedInstance.Player.getRunVelocity()/10f;
		
		anim.Play(animationString);
		/*
		if (audio != null)
			{
			audio.volume = AudioManager.SharedInstance.SoundVolume;
			audio.Play();
			}
		*/
		
		if(sfx){
			AudioManager.SharedInstance.PlayAnimatedSound(sfx);
		}
		
		foreach(ParticleSystem item in effects)
		{
			item.Play();
		}
		
	}
	
	IEnumerator Rewind()
	{
		if(anim!=null)
		{
			anim.Play(animationString);
			anim[animationString].enabled = true;
			anim[animationString].time = 0f;
			anim.Sample();
			anim[animationString].enabled = false;
			//anim.Play();
			//anim[animationString].speed = 0f;
			//yield return null;
			//anim.Rewind();
			//anim.Stop();
		}
		yield break;
	}
}
