using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECSkyboxFireWorks : MonoBehaviour {
	
	public float MinTime;
	public float MaxTime;
	float UpMin;
	float UpMax;
	float RightMin;
	float RightMax;
	public float UpPick;
	public float RightPick;
	
	bool FireworkFlag;
	public Vector3 GetOutOfTheWay;
	//public GameObject Player;
	public List<GameObject> FireWorks= new List<GameObject>();
	public List<Transform> Fireworknodes= new List<Transform>();
	
	void Start () {
		FireworkFlag = true;
		UpMin = 5f;
		UpMax = 12f;
		RightMin = -10f;
		RightMax = 10f ;
		//if(Player == null)
		//{
		//	Player = GameObject.Find("Player");
		//}
	}
	// Use this for initialization
	void Update () {
		if(FireworkFlag == true && !GamePlayer.SharedInstance.IsInCatacombs)
		{
			StartCoroutine(LaunchFirework());
			FireworkFlag = false;
		}
		
		if(GamePlayer.SharedInstance.IsInCatacombs && !FireworkFlag)
		{
			StopCoroutine ("LaunchFirework");
			FireworkFlag = true;
		}
	}
	
	 IEnumerator LaunchFirework()
	{
		//Randomly pick what and where the firework is going off.
		int PickFireWork = Random.Range(0, FireWorks.Count);
		UpPick = Random.Range(UpMin,UpMax);
		RightPick = Random.Range(RightMin,RightMax);
		//int PickFireworknodes = Random.Range(0, Fireworknodes.Count - 1);
		
		if(FireWorks.Count==0)
			yield break;
		if(FireWorks[PickFireWork]==null)
			yield break;
		
		GamePlayer Player = GamePlayer.SharedInstance;
		
		if(gameObject.activeSelf)
		{
			//Move the firework
			FireWorks[PickFireWork].transform.position = Player.transform.position + (Player.transform.forward * 50)+ (Player.transform.up * UpPick)+ (Player.transform.right * RightPick);
			//PlayFireWork
			
			FireWorks[PickFireWork].transform.GetChild(0).particleSystem.Play();
		}
		
		//Wait for a random amount of time before launching the firework
		yield return new WaitForSeconds(Random.Range(MinTime, MaxTime));
		
		FireWorks[PickFireWork].transform.position = GetOutOfTheWay;
		
		FireworkFlag = true;
	}

}
