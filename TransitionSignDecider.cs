using UnityEngine;
using System.Collections;

/// <summary>
/// Decides if left or right goes to the new environment set
/// </summary>
public class TransitionSignDecider : MonoBehaviour {
	
	protected static Notify notify;
	
	public bool MainLeftGoesToTransitionTunnel { get; private set;}
	public int DestinationId { get; private set;}
	private static int lastChoice = -1;
	
	void printParent()
	{
		if (transform.parent != null)
		{
			notify.Debug ("       parent = " + transform.parent.name);	
		}		
	}
	
	void Awake()
	{
		if (notify == null)
		{
			notify = new Notify(this.GetType().Name);	
		}
		notify.Debug (this.GetInstanceID() + " " + Time.frameCount + " TransitionSignDecider Awake" );
		printParent();
	}
	
	// Use this for initialization
	void Start () {
		notify.Debug (this.GetInstanceID() + " " + Time.frameCount + " TransitionSignDecider Start" );	
		printParent();
	}
	
	void OnEnable()
	{
		notify.Debug (this.GetInstanceID() + " " + Time.frameCount + " TransitionSignDecider OnEnable" );		
		printParent();
	}
	
	void OnDisable()
	{
		notify.Debug (this.GetInstanceID() + " " + Time.frameCount + " TransitionSignDecider OnDisable" );
		printParent ();
	}
	
	void OnSpawned()
	{
		notify.Debug (this.GetInstanceID() + " " + Time.frameCount + " TransitionSignDecider OnSpawned" );	
		printParent();
		chooseDestination();
		chooseTunnelDirection();

	}
	
	void OnDespawned()
	{
		notify.Debug (this.GetInstanceID() + " " + Time.frameCount + " TransitionSignDecider OnDespawned" );	
		printParent();
	}	
	
	/// <summary>
	/// Determine if going left or going right brings us to the transition tunnel
	/// underlying assumption is that going left is the main path, and going right is the alternate path
	/// </summary>
	void chooseTunnelDirection()
	{
		if (Random.value < 0.5f)
		{	
			MainLeftGoesToTransitionTunnel = true;
		}
		else
		{
			MainLeftGoesToTransitionTunnel = false;	
		}
		notify.Debug (Time.frameCount + " MainLeftGoesToTransitionTunnel = " + MainLeftGoesToTransitionTunnel);	
		fixupSign();
	}
	
	/// <summary>
	/// Decide where the sign will take us
	/// </summary>

	void chooseDestination()
	{	
		DestinationId = EnvironmentSetManager.WhimsyWoodsId; // result when all else fails
		int preferredDest = Settings.GetInt("transition-sign-preferred-choice", -1);
		bool preferredDestFound = false;
		
		// if we have more than one, take out the one we are currently on and then choose from the remainder
		if (EnvironmentSetManager.SharedInstance.LocallyAvailableCount() > 1)
		{
			int availableMinusOne = EnvironmentSetManager.SharedInstance.LocallyAvailableCount() - 1;
			int [] choices = new int[availableMinusOne];
			int curIndex = 0;
			foreach( int envSetId in EnvironmentSetManager.SharedInstance.LocalDict.Keys)
			{
				if ( EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId != envSetId 
					&& lastChoice != envSetId)
				{
					choices[curIndex] = envSetId;
					curIndex ++;
					if (preferredDest == envSetId)
					{
						preferredDestFound = true;
					}
				}
			}
			if (preferredDestFound)
			{
				DestinationId = preferredDest;
			}
			else
			{
				int indexChoice = Random.Range(0, curIndex); //curIndex is the size of the array
				DestinationId = choices[indexChoice];
			}
			notify.Debug ("lastChoice old: " + lastChoice + " DestinationId = " + DestinationId);
			lastChoice = DestinationId;
			//notify.Debug("lastChoice new: " + lastChoice);
		}
	}
	
	/// <summary>
	/// Rotate the sign to make it match MainLeftGoesToTransitionTunnel
	/// </summary> 
	void fixupSign()
	{
		// we are crashing somewhere in fixupSign,  let's do some more checking
		if (EnvironmentSetManager.SharedInstance == null)
		{
			notify.Warning("fixupSign EnvironmentSetManager.SharedInstance is null, bailing");
			return;
		}
		
		if ( EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet == null)
		{
			notify.Warning("fixupSign EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet is null, bailing");
			return;			
		}
		
		if ( ! EnvironmentSetManager.SharedInstance.LocalDict.ContainsKey(DestinationId))
		{
			notify.Warning("fixupSign EnvironmentSetManager.SharedInstance.LocalDict.ContainsKey({0}) is false, bailing", DestinationId);
			return;					
		}
		
		string wordsToShow = EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode + "_to_" + 
				EnvironmentSetManager.SharedInstance.LocalDict[DestinationId].SetCode;
		GameObject lettering = HierarchyUtils.GetChildByName("lettering", transform.parent.gameObject);
		if (lettering)
		{

			GameObject wordsToShowGo = HierarchyUtils.GetChildByName(wordsToShow, lettering);
			MeshRenderer[] renderers = lettering.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer renderer in renderers)
			{
				renderer.enabled = false;	
			}
			if (wordsToShowGo)
			{
				wordsToShowGo.renderer.enabled = true;
			}
		}
		string sign = "oz_tt_sign_a";
		GameObject signGo = HierarchyUtils.GetChildByName(sign, transform.parent.gameObject);
		if (signGo)
		{
			if 	(MainLeftGoesToTransitionTunnel)
			{
				signGo.transform.localEulerAngles = Vector3.zero;
				// eyal edit, we acutally need to reverse it for the tutorial to make our lives easier
				// Since we don't want to switch environment. we are keeping the player in WW by telling him to go the wrong way if in Tutorial mode
				//if(GameController.SharedInstance.IsTutorialMode){
				//	signGo.transform.localEulerAngles = new Vector3 (0,180,0);	
				//}
			}
			else
			{
				signGo.transform.localEulerAngles = new Vector3 (0,180,0);	
				// eyal edit, we acutally need to reverse it for the tutorial to make our lives easier
				// Since we don't want to switch environment. we are keeping the player in WW by telling him to go the wrong way if in Tutorial mode
				//if(GameController.SharedInstance.IsTutorialMode){
				//	signGo.transform.localEulerAngles = Vector3.zero;	
				//}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
