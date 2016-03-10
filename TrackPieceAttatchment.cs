using UnityEngine;
using System.Collections;

public class TrackPieceAttatchment : MonoBehaviour
{
	//private TrackPiece thisTrackPiece;
	protected static Notify notify;
	
	public int TrackPreviousAmt = -1;
	
	//private Vector3 pos = Vector3.zero;
	
	//private bool isOnNow = false;
	
	public virtual void Awake()
	{
		if (notify == null)
		{
			notify = new Notify(this.GetType().Name);	
		}
	}
	
	public virtual void OnEnable()
	{
		if (this != null)
		{
			if(GamePlayer.SharedInstance!=null)
				GamePlayer.SharedInstance.RegisterForOnTrackPieceChange(_OnPlayerEnteredTrackPiece);
		}
		else
		{
			notify.Warning("how did this happen, OnEnable called but this is null");
		}
	}
	
	public virtual void OnDespawned()
	{
	//	TR.LOG(string.Format("TrackPieceAttachment.OnDespawned {1} {0}", transform.parent.name, transform.name));
		GamePlayer.SharedInstance.UnregisterForOnTrackPieceChange(_OnPlayerEnteredTrackPiece);
	}
	
	public virtual void OnDisable()
	{
	//	TR.LOG(string.Format("TrackPieceAttachment.OnDisable {1}  {0}", transform.parent.name, transform.name));
		if(GamePlayer.SharedInstance!=null)
			GamePlayer.SharedInstance.UnregisterForOnTrackPieceChange(_OnPlayerEnteredTrackPiece);
	}
	
	public virtual void OnDestroy()
	{
	//	TR.LOG(string.Format("TrackPieceAttachment.OnDestroy {1}  {0}", transform.parent.name, transform.name));
		if(GamePlayer.SharedInstance!=null)
			GamePlayer.SharedInstance.UnregisterForOnTrackPieceChange(_OnPlayerEnteredTrackPiece);
	}
	
	private void _OnPlayerEnteredTrackPiece(TrackPiece previous, TrackPiece on)
	{
		if (this == null)
		{
			//TR.ERROR(string.Format("_OnPlayerEnteredTrackPiece called but gameObject is null, get REDMOND"));
			// this is a hack, ideally we unregister before this ever becomes null
			GamePlayer.SharedInstance.UnregisterForOnTrackPieceChange(_OnPlayerEnteredTrackPiece);
	//		TR.LOG(string.Format("_OnPlayerEnteredTrackPiece unregistered"));
			return;	
		}

		if(on==null || previous==null)
		{
			if(notify!=null)	notify.Warning("Entered a null track piece!");
			return;
		}
		
		if(transform.IsChildOf(on.transform))
		{
			OnPlayerEnteredTrackPiece(); 
		}
        if (on.NextTrackPiece != null && transform.IsChildOf(on.NextTrackPiece.transform))
        {
			OnPlayerEnteredPreviousTrackPiece();
		}
		if(on.NextTrackPiece!=null && on.NextTrackPiece.NextTrackPiece!=null && transform.IsChildOf(on.NextTrackPiece.NextTrackPiece.transform)) {
			OnPlayerEnteredPreviousPreviousTrackPiece();
		}
		if(previous!=null && transform.IsChildOf(previous.transform)) {
			OnPlayerEnteredNextTrackPiece();
		}
		if(previous!=null && previous.PreviousTrackPiece!=null && transform.IsChildOf(previous.PreviousTrackPiece.transform)) {
			OnPlayerEnteredNextNextTrackPiece();
		}
		
		int count = 0;
		TrackPiece cur = on;
		while(cur!=null && count<=TrackPreviousAmt)
		{
			if(transform.IsChildOf(cur.transform))
				OnPlayerEnteredPreviousTrackPiece(count);
			count++;
			cur = cur.NextTrackPiece;
		}
	}
	
	
	
	public virtual void OnPlayerEnteredTrackPiece()
	{
		
	}
	
	
	public virtual void OnPlayerEnteredPreviousTrackPiece()
	{
		
	}
	
	public virtual void OnPlayerEnteredPreviousPreviousTrackPiece()
	{
		
	}
	
	public virtual void OnPlayerEnteredNextTrackPiece()
	{
		
	}
	
	public virtual void OnPlayerEnteredNextNextTrackPiece()
	{
		
	}
	public virtual void OnPlayerEnteredPreviousTrackPiece(int num)
	{
		
	}
	
}
