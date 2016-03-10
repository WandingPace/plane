using UnityEngine;
using System.Collections;
using System;

public class Attack : MonoBehaviour 
{
	public static Attack SharedInstance = null;
	GamePlayer Player;
	
	//-- Input
	public GameInput InputController = null;
	public PCInput PCInputController = null;
	public TouchInput TouchInputController = null;
	
	public Transform projectile;
	public Transform GetProjectilePrefab()
	{
		return projectile;
	}
	
	private const float kf_MissedArrowDistance = 150.0f;
	private const float kf_MissedArrowDistanceY = 10.0f;
	private const float kf_MaxAngleForMissedArrows = 5.0f;
	
	void Awake()
	{
		Attack.SharedInstance = this;
		Player = GamePlayer.SharedInstance;
	}
	// Use this for initialization
	void Start () 
	{
#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)
		InputController = TouchInputController;
		PCInputController.enabled = false;
		PCInputController.gameObject.SetActive(false);
#else
		
		InputController = PCInputController;
		TouchInputController.gameObject.SetActive(false);
#endif
		InputController.gameObject.SetActive(true);
		InputController.init(GameController.SharedInstance);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (InputController.ShouldTurnLeft)
		{
//			Debug.Log("Should Turn Left");
		}
		
		if (InputController.ShouldTurnRight)
		{
//			Debug.Log("Should Turn Right");
		}
	}
	
	public void ShootArrow(Vector2 screenPosition)
	{
		//Camera camera = GetComponent<Camera>();
		Camera camera = Camera.main;

        float touchSide = Mathf.Floor((screenPosition.x / (Screen.width / 2f))) - 1f; // -1 is left, 1 is supposed to be right but it's actually 0
		
		Vector3 v3_CameraPosition = GameCamera.SharedInstance.transform.position;
		Vector3 v3_Worldpoint = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 1));
		
		Vector3 v3_Intersection;
		Vector3 v3_CharacterUp = GamePlayer.SharedInstance.transform.up;
		Vector3 v3_CharacterPosition = GamePlayer.SharedInstance.transform.position;
		
		//Monkey hack
		Monkey.KillOne();
		
		if (VectorTools.RayPlaneIntersect(v3_CameraPosition, v3_Worldpoint - v3_CameraPosition, v3_CharacterPosition, v3_CharacterUp, out v3_Intersection))
		{
			//Find the vector to the touch target and the angle to it
			Vector3 v3_CharacterForward = GamePlayer.SharedInstance.transform.forward;
			Vector3 v3_VectorToTarget = v3_Intersection - v3_CharacterPosition;
			float f_DotProduct = Vector3.Dot(v3_VectorToTarget, v3_CharacterForward);
			float f_AngleToTarget = Vector3.Angle(v3_VectorToTarget, v3_CharacterForward);
			if(Vector3.Cross(v3_CharacterForward, v3_VectorToTarget).y < 0.0f)
			{
				f_AngleToTarget = -f_AngleToTarget;
			}

			//Reflect the angle if it is behind
			Vector3 v3_ArrowDirection = v3_CharacterForward;
			if (f_DotProduct < 0.0f)
			{
				f_AngleToTarget = 180.0f - f_AngleToTarget;
			}


			//Express angle as value between -180.0f and 180.0f
			f_AngleToTarget = f_AngleToTarget > 180.0f ? f_AngleToTarget-360.0f : f_AngleToTarget;
			f_AngleToTarget = f_AngleToTarget < -180.0f ? f_AngleToTarget+360.0f : f_AngleToTarget;

			f_AngleToTarget = Mathf.Clamp(f_AngleToTarget, -kf_MaxAngleForMissedArrows, kf_MaxAngleForMissedArrows);
			v3_ArrowDirection = Quaternion.AngleAxis(f_AngleToTarget, Vector3.up) * v3_ArrowDirection;
			
			Arrow arrow = Arrow.Instanciate();
			//DebugTools.DrawMarkerAtPosition( v3_CharacterPosition + (kf_MissedArrowDistance * v3_ArrowDirection) + (kf_MissedArrowDistanceY * Vector3.up), Color.red, 3.0f, true);
			//Debug.Break();
			
			arrow.Setup(Player, v3_CharacterPosition + (kf_MissedArrowDistance * v3_ArrowDirection) + (kf_MissedArrowDistanceY * Vector3.up), Vector3.zero);
			
			Debug.Log("GamePlayer: " + Player.GetPlayerVelocity());
			
			//PlayAnimation("Shoot");
			
			//TODO Play sound when shooting arrow
			//AudioManager.Instance.PlayArrowShoot();
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.monkeyDie, 1);
		}
		
		Debug.Log ("Attack Launched: (" + touchSide + ") " + screenPosition.x + "," + screenPosition.y);

	}
	
	public void OnEnemyKilled(EnemyTarget Enemy)
	{
		/*if (!IsGameStarted)
			return;

		GamePlayer.AddCoins(GetArcheryTargetValue());
		//BEN NOTE - Now spawning coin sprite effect on touch rather than on target destroyed - GameInterface.SpawnSpriteParticleForCoin(2, new Vector2(guiPosition.x, guiPosition.y));

		Audio.PlayCoin();

        PlayVO(AudioManager.VOs.EN_VO_LVL_WG_MER_509, 0.1f);

		if(IsBuildingArcheryTrack() || IsArcheryMode())
		{
			if (Enemy.IsArcheryTarget())
			{
                AudioManager.Instance.PlayRandomFX(AudioManager.Effects.FX_ArrowHit_Impact1_IOS, AudioManager.Effects.FX_ArrowHit_Impact1_IOS);
				int i_ArcheryTargetIndex = ((Target)Enemy).GetArcheryTargetIndex();
				HitArcheryTarget(i_ArcheryTargetIndex);
				
				Vector3 vPos = GameCamera.SharedInstance.GetCamera().transform.InverseTransformPoint(Enemy.transform.position);
				
				GamePlayer.TargetShot++;
				
				AnalyticsEvent.SendGameActionLogEvent("target", "hit", vPos.x > 0 ? "Right" : "Left", "");
			}
		}*/
	}
	
	public void OnEnemyMissed(EnemyTarget Enemy)
	{
//		if (IsBuildingArcheryTrack() || IsArcheryMode())
//		{
//			if (Enemy.IsArcheryTarget())
//			{
//				int i_ArcheryTargetIndex = ((Target)Enemy).GetArcheryTargetIndex();
//				MissArcheryTarget(i_ArcheryTargetIndex);
//			}
//		}
	}
	
	public class TimeScale
	{
		static public float TimeRatio = 1f;
		static public float GetDeltaTime(bool Scale = true) { if (Scale) return Time.smoothDeltaTime * TimeRatio; else return Time.smoothDeltaTime; }
	}
}
