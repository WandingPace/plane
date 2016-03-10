using UnityEngine;
using System.Collections;

public class TriggerBalloonAnimation : TrackPieceAttatchment {

	//public ParticleSystem dirt01;
	
	private Animation anim;
	public string animationString = "balloonanim";
	
	void Start()
	{
		anim = GetComponentInChildren<Animation>();
	}
	
	
	void OnSpawned()
	{
		if(anim!=null)
		{
			anim.Stop();
			anim.Play(animationString);
			anim[animationString].speed = 0f;
			anim[animationString].normalizedTime = 0f;
		}
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
		
		
		
	}
	
}
