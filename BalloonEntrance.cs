using UnityEngine;
using System.Collections;

public class BalloonEntrance : TrackPieceAttatchment {

	
	
	public Renderer balloon;
	public Animation anim;
	
	private bool played = false;
	
	public override void OnPlayerEnteredPreviousTrackPiece(int num)
	{
		if(num<=5 && !played) {
			anim.Play();
			played = true;
		}
	}
	
	public override void OnPlayerEnteredNextTrackPiece()
	{
		if(balloon!=null)
			balloon.enabled = false;
	}
	
	public override void OnDisable()
	{
		if(balloon!=null)
			balloon.enabled = true;
		played = false;
	}
}
