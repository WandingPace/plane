using UnityEngine;
using System.Collections;

public class WWCliffCollapse : TrackPieceAttatchment {

	public ParticleSystem dirt01;
	public AudioClip sfx;
	private Animation anim;
	
	public override void Awake()
	{
		anim = GetComponentInChildren<Animation>();
		if(anim!=null)	anim.playAutomatically = false;
	//	if(!dirt01.transform.IsChildOf(transform))
	//		dirt01 = (ParticleSystem)Instantiate(dirt01,transform.position,transform.rotation);
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
	
	
	public override void OnPlayerEnteredPreviousTrackPiece()
	{
		Animate();
	}
	
	void Animate()
	{
		if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			anim["collapse"].speed = GameController.SharedInstance.Player.getRunVelocity()/14f;
		
		anim.Play("collapse");
		/*
		if (audio != null)
		{
			if(AudioManager.SharedInstance!=null)	audio.volume = AudioManager.SharedInstance.SoundVolume;
			audio.Play();
		}
		*/
		
		if(sfx){
			AudioManager.SharedInstance.PlayAnimatedSound(sfx);
		}
		if (dirt01 != null)
		{
			dirt01.Play();
		}
		
	}
	
	IEnumerator Rewind()
	{
		if(anim!=null)
		{
			anim.Play("collapse");
			anim["collapse"].enabled = true;
			anim["collapse"].time = 0f;
			anim.Sample();
			anim["collapse"].enabled = false;
			//anim.Play();
			//anim[animationString].speed = 0f;
			//yield return null;
			//anim.Rewind();
			//anim.Stop();
		}
		yield break;
	}
	
}

