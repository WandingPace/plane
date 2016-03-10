///
/// Imangi Studios LLC ("COMPANY") CONFIDENTIAL
/// Copyright (c) 2011-2012 Imangi Studios LLC, All Rights Reserved.
///
/// NOTICE:  All information contained herein is, and remains the property of COMPANY. The intellectual and technical concepts contained
/// herein are proprietary to COMPANY and may be covered by U.S. and Foreign Patents, patents in process, and are protected by trade secret or copyright law.
/// Dissemination of this information or reproduction of this material is strictly forbidden unless prior written permission is obtained
/// from COMPANY.  Access to the source code contained herein is hereby forbidden to anyone except current COMPANY employees, managers or contractors who have executed 
/// Confidentiality and Non-disclosure agreements explicitly covering such access.
///
/// The copyright notice above does not evidence any actual or intended publication or disclosure of this source code, which includes  
/// information that is confidential and/or proprietary, and is a trade secret, of COMPANY. ANY REPRODUCTION, MODIFICATION, DISTRIBUTION, PUBLIC  PERFORMANCE, 
/// OR PUBLIC DISPLAY OF OR THROUGH USE OF THIS SOURCE CODE WITHOUT THE EXPRESS WRITTEN CONSENT OFCOMPANY IS STRICTLY PROHIBITED, AND IN VIOLATION OF APPLICABLE 
/// LAWS AND INTERNATIONAL TREATIES. THE RECEIPT OR POSSESSION OF THIS SOURCE CODE AND/OR RELATED INFORMATION DOES NOT CONVEY OR IMPLY ANY RIGHTS  
/// TO REPRODUCE, DISCLOSE OR DISTRIBUTE ITS CONTENTS, OR TO MANUFACTURE, USE, OR SELL ANYTHING THAT IT MAY DESCRIBE, IN WHOLE OR IN PART.                
///

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EnemyTarget : MovingObject
{
	public bool IsFalling;
	public bool IsJumping;
	public bool JumpAfterDelay;
	public float JumpDelay;
	public float JumpVelocity;
	public bool IsOverGround;
	public float GroundHeight;
    public bool IsFlying;
	public bool CanJump = true;
	public Vector3 SpawnPosition;
	public float LifeSpan;
	public bool HasLifeSpan;
	public bool IsAimedByArrow;
	protected bool mb_IsKilled;
	protected bool mb_IsMissed;
	protected bool mb_IsDying;
	public Vector3 KillCenterOffset = Vector3.zero;
	public float XLength = 1f;
	public float YLength = 1f;
	public float Height = 0;
	//public Region2D Region = null;
	public float TargetSize = 0.5f;
    public Animation Anim;
	
	static private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

	public static Enemy Instanciate(string PrefabName)
	{
		GameObject asset;
		if (_prefabs.ContainsKey(PrefabName))
			asset = _prefabs[PrefabName];
		else
		{
			asset = (GameObject)Resources.Load(PrefabName, typeof(GameObject));
			_prefabs.Add(PrefabName, asset);
		}


		SimplePool pool = SimplePoolManager.Instance.GetPool("EnemiesPool");

		GameObject go = pool.Spawn(asset.gameObject);
		Enemy enemy = go.GetComponent<Enemy>();

		return enemy;
	}

	protected static string GetPrefabName()
	{
		return "";
	}

	public virtual void DestroySelf()
	{
		SimplePool pool = SimplePoolManager.Instance.GetPool("EnemiesPool");
		transform.parent = pool.transform;
		pool.Unspawn(gameObject);
	}
	
	public virtual void Awake()
	{
		Anim = gameObject.GetComponentInChildren<Animation>();
	}
	
	// Use this for initialization
	public virtual void Start()
    {}

	void OnSpawned()
	{
		Reset();
	}

	public void Run()
	{
	}

	public void Jump(float delay=0)
	{
		if (!IsFlying && CanJump && !IsJumping && !IsFalling)
        {
            if (delay > 0.0f)
            {
                JumpAfterDelay = true;
                JumpDelay = delay;
            }
            else
            {
                JumpAfterDelay = false;
                IsJumping = true;
                Velocity.y = JumpVelocity;
            }
        }
	}
	
	// Update is called once per frame
	public override void Update()
	{

//		if (CharacterPlayer.Instance.Hold)
//			return;

		if (transform.position.y < -40.0f)
			return;

		if (JumpAfterDelay) {
			JumpDelay -= Time.deltaTime;
			if (JumpDelay <= 0)
				Jump();
		}

		// Gravity
        if (!IsFlying)
		    ApplyForce(new Vector3(0, -250.0f, 0));

		base.Update();

		if (IsOverGround) {
			// Stay above ground
			if (transform.position.y < GroundHeight && !IsFalling) {
				SetY(GroundHeight);
				Velocity.y = 0;
				
				if (IsJumping == true)
					OnJumpFinished();
				
				IsJumping = false;
			}
		}

		if (transform.position.y < GroundHeight - 2.0f) {
			IsFalling = true;
		}
		
		if (HasLifeSpan)
			LifeSpan -= Time.deltaTime;
	}

	public virtual void Reset()
	{
		SetY(0);
		IsFalling = false;
		IsJumping = false;
		JumpAfterDelay = false;
		JumpVelocity = 80.0f;
		IsOverGround = true;
		GroundHeight = 0.0f;
        IsFlying = false;
		LifeSpan = 0f;
		HasLifeSpan = false;
		IsAimedByArrow = false;
		mb_IsKilled = false;
		mb_IsMissed = false;
		mb_IsDying = false;
	}

	public virtual void Kill()
	{
		mb_IsKilled = true;
		//GameController.SharedInstance.OnEnemyKilled(this);
		Attack.SharedInstance.OnEnemyKilled(this);
	}

	public virtual void Miss()
	{
		mb_IsMissed = true;
		//GameController.SharedInstance.OnEnemyMissed(this);
		Attack.SharedInstance.OnEnemyMissed(this);
	}

	protected virtual void OnJumpFinished()
	{ }

	public bool _bIsSelected = true;

	void OnDrawGizmos()
	{
		if (_bIsSelected)
			OnDrawGizmosSelected();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.1f);  //center sphere
		if (transform.renderer != null)
			Gizmos.DrawWireCube(transform.position, transform.renderer.bounds.size);
	}

	public bool IsMissed()
	{
		return mb_IsMissed;
	}
	public bool IsKilled()
	{
		return mb_IsKilled;
	}
	public bool IsDying()
	{
		return mb_IsDying;
	}
	

		

	public virtual bool IsArcheryTarget()
	{
		return false;
	}


}
