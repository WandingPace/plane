using UnityEngine;
using System.Collections;

public class PropMonkey : TrackPieceAttatchment {

	
	private Animation anim;
	public string animationString;
		

	public override void Awake()
	{
		base.Awake();
		anim = GetComponentInChildren<Animation>();
	}
	
	void OnSpawned()
	{
		StartCoroutine(Rewind());
	}
	
	public override void OnEnable()
	{
		//base.OnEnable();
		//StartCoroutine(Rewind());
		
		Animate();
		
	}
	
	
	
//	public override void OnPlayerEnteredPreviousTrackPiece()
//	{
//		Animate();
//	}
	/*
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == GameController.SharedInstance.Player.gameObject)
		{
			Animate();
		}
	}
	*/
	
	void Animate()
	{
		if(GameController.SharedInstance.Player.getModfiedMaxRunVelocity()>0f)
			anim[animationString].speed = GameController.SharedInstance.Player.getRunVelocity()/10f;
			anim[animationString].normalizedTime = Random.value;
			anim[animationString].wrapMode = WrapMode.Loop;
			//anim[animationString].Sample();
		
		anim.Play(animationString);
		if (audio != null)
			{
			audio.volume = AudioManager.SharedInstance.SoundVolume;
			audio.Play();
			}
	}
	
	IEnumerator Rewind()
	{
		if(anim!=null)
		{
			anim.Play();
			//anim[animationString].speed = 0f;
			yield return null;
			anim.Rewind();
			anim.Stop();
		}
	}
}
