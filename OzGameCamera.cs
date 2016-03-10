using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OzGameCamera : GameCamera	
{
	public static OzGameCamera OzSharedInstance;

    public Animation CameraAnimation;
    public Transform AnimationTarget;
    public bool LookAtPlayer = false;
    public bool StopMove { get; set; }
    public GameObject planes;
    public GameObject fade;
    private bool isMoveToPath;
    private float SpeedWhenAnim = 26.2f;
    //private bool isFristAnim = false;
    public float CameraAnimationLength {
        get { return 5f; }
    }

    public bool didPlayCinematicPullback = true;// we remove it false;
	Vector3 TempMoveNumber;

    public float RoundFollowHeight;
    public float RoundFollowDistance;
    public float RoundFocusHeight;
    public float RoundFocusDistance;

    public float ClimbFollowHeight = 7;
    public float ClimbFollowDistance;
    public float ClimbFocusHeight = 1;
    public float ClimbFocusDistance;

    public float RoundYaw;
    public float FollowYaw;
    public bool isCrash;
    private Vector3 curPos;
    //private Vector3 crashPos;
    private float crashDuration;

	public Transform pointObj;

	void Awake()
	{
		SharedInstance = this;
		OzSharedInstance = this;
		//cameraState = CameraState.cinematicPullback; // remove it
		cameraState = CameraState.cinematicOpening;
		
        //LateUpdate();
	}

	void Start()
	{
		
		SharedInstance = this;
		m_FocusHeight = 2.1f;
		m_FollowHeight = 2.0f;
		m_FollowDistance = 2.46f;
		
		m_FocusXOffset = 0f;
		
	}
	
	public void reset()
	{
		m_FocusHeight = RunFocusHeight;
		m_FollowHeight = RunFollowHeight;
		m_FollowDistance = RunFollowDistance;
		
		IsCameraShaking = false;
		ShakeAfterDelay = false;
		TimeSinceCameraShakeStart = 0.0f;
		CameraShakeDamperRate = 0.0f;
		CameraShakeMagnitude = 0.0f;
		CameraShakeDuration = 0.0f;
		CameraShakeFrequencyMultiplier = 1.0f;
		
        //holdT = 0f;
        holding = false;
        isMoveToPath = false;
        if (planes) 
            planes.SetActive(false);
        //transform.position = Vector3.zero;
        //transform.rotation = Quaternion.identity;

		stopped = false;
		
		setFocusTargetPosition();
		
		//-- Do this after setFocusTargetPosition but not inside.
		m_CurrentDistance = m_TargetDistance;
		
		m_CurrentPitch = m_TargetPitch;
		m_CurrentYaw = m_TargetYaw;
//		notify.Warning("Reset!");
		m_FocusXOffset = 0f;
	}


    private float LastXOffset;
	public void setFocusTargetPosition()
	{
	    if (PlayerTarget && FocusTarget && cameraState == CameraState.gamplay)
	    {
	        if (PlayerTarget.IsHangingFromWire == true || PlayerTarget.IsOnBalloon)
	        {
	            m_TargetCameraLocation = PlayerTarget.CurrentPosition;
	        }
	        else
	        {
	            m_TargetCameraLocation = PlayerTarget.CachedTransform.position;
	        }

            //if (PlayerTarget.OnTrackPiece != null)
            //{
            //    m_TargetCameraLocation.y = PlayerTarget.CurrentPosition.y;
            //}

	        //m_TargetCameraLocation.y = PlayerTarget.currentPosition.y;

	        if (PlayerTarget.IsClimb)
	        {
                m_TargetCameraLocation -= Vector3.Cross(Vector3.up, PlayerTarget.transform.forward) *
                                        PlayerTarget.PlayerXOffset;
	            m_TargetCameraLocation.x = 0f;
                FocusTarget.position = m_TargetCameraLocation;
	        }
	        else
	        {
	            FocusTarget.position = m_TargetCameraLocation;
	        }

	        FocusTarget.rotation = PlayerTarget.transform.rotation;

	        //-- Do Logic to set Local member variables according to Player state ( run, jump, slide, zipline, etc)
	        computeCameraOffsets();

            FocusTarget.Translate(Vector3.up * m_FocusHeight, Space.World);
            //FocusTarget.Translate(FocusTarget.right * m_FocusXOffset / 0.9f, Space.World);
	    }
	}

    private void computeCameraOffsets()
    {
        if (PlayerTarget.IsOnBalloon == true)
        {
            m_FocusHeight = BalloonFocusHeight;
            m_FocusDistance = BalloonFocusDistance;
            m_FollowDistance = BalloonFollowDistance;
            //m_FollowHeight = BalloonFollowHeight + tempTiltAmount;
            m_FollowHeight = BalloonFollowHeight;
            m_FocusXOffset = BalloonXOffset;
            SmoothZoomSpeed = BalloonSmoothZoomSpeed;

            //tempTiltAmount = Mathf.SmoothStep(currentTiltAmount, positiveTiltAmount * 0.2f, Time.deltaTime * 10f);
            //currentTiltAmount = tempTiltAmount;
        }
        else if (PlayerTarget.IsRound)
        {
            m_FocusHeight = RoundFocusHeight;
            m_FocusDistance = RoundFocusDistance;
            m_FollowDistance = RoundFollowDistance;
            m_FollowHeight = RoundFollowHeight;
            m_FocusXOffset = 0f;
            BalloonXOffset = 0f;
        }
        else if (PlayerTarget.IsClimb)
        {
            m_FocusHeight = ClimbFocusHeight;
            m_FocusDistance = ClimbFocusDistance;
            m_FollowDistance = ClimbFollowDistance;
            m_FollowHeight = ClimbFollowHeight;
            m_FocusXOffset = 0f;
            BalloonXOffset = 0f;
        }
        else
        {
            m_FocusHeight = RunFocusHeight;
            m_FocusDistance = RunFocusDistance;
            m_FollowDistance = RunFollowDistance;
            m_FollowHeight = RunFollowHeight;
            m_FocusXOffset = 0f;
            BalloonXOffset = 0f;
        }
        //UpdatePosDirOffsets();

        //m_FollowDistance += currentPositionOffset.z; //TileFollowDistanceOffset;
        //m_FollowHeight += currentPositionOffset.y; //TileFollowHeightOffset;	=

        //-- Calc a new target Pitch
        m_TargetPitch = Mathf.Atan2(m_FollowHeight, m_FocusDistance)*Mathf.Rad2Deg;
        m_TargetDistance = (new Vector2(m_FollowDistance, m_FollowHeight)).magnitude;
        //m_TargetDistance = Mathf.Sqrt((m_FollowDistance*m_FollowDistance)+(m_FollowHeight*m_FollowHeight));
    }

    private bool stopped = false;
	private float internal_vel = 0f;
	public void Stop()
	{
		stopped = true;
		internal_vel = PlayerTarget.GetPlayerVelocity().magnitude;
	}

	public void Unstop()
	{
		StartCoroutine(Unstop_internal());
	}
	IEnumerator Unstop_internal()
	{
		yield return new WaitForSeconds(0.1f);
		stopped = false;
	}
	
	private bool holding = false;
	public bool Holding { get { return holding; } }
	
    //private float holdT = 0f;
	//private Vector3 startoffset = Vector3.zero;
    //private Vector3 startpos = Vector3.zero;
    //private bool isset = false;

	//private Vector3 initCamLook = Vector3.zero;
	// do updating in LateUpdate to make sure all other objects have moved around.
	public new void  LateUpdate()
    {
        if(PlayerTarget == null || cameraState != CameraState.gamplay)
			return;

	    if (Time.timeScale == 0f)
	        return;

        if (StopMove)
            return;

		if(stopped)	//A hcak to make the camera slow down
		{
			internal_vel = Mathf.MoveTowards(internal_vel,0f,35f*Time.deltaTime);
			Vector3 frw = transform.forward;
			frw.y = 0f;
			frw.Normalize();
			transform.position += frw * internal_vel*Time.deltaTime;
		}
        else if (isCrash)
        {
            //等待iTween完成镜头缩放

            //if (transform.position != crashPos)
            //{
            //    iTween.MoveTo(gameObject, crashPos, 0.2f);
            //}
            //else
            //{
            //    iTween.MoveTo(gameObject, curPos, 0.2f);
            //    transform.position =  curPos;
            //    isCrash = false;
            //}
        }
        else if (PlayerTarget.Dying == false) 
		{
			if(PlayerTarget.OnTrackPiece==null || PlayerTarget.OnTrackPiece.CurrentTrackPieceData==null || PlayerTarget.OnTrackPiece.CurrentTrackPieceData.splineStart==null)
			{
				// Position camera holder correctly
			    if (FocusTarget.position.z > -56f && GameController.SharedInstance.TimeSinceGameStart < CameraAnimationLength)
			    {
			        if (GameController.SharedInstance.TimeSinceGameStart > Mathf.Epsilon && !isMoveToPath)
			        {
			            if (CameraAnimation)
			            {
			                transform.localPosition = Vector3.zero;
			                transform.localRotation = Quaternion.identity;
			                CameraAnimation.Play();

			                UIManagerOz.SharedInstance.inGameVC.HidePauseButton();

			                if (planes)
			                {
			                    planes.SetActive(true);

			                    Vector3[] nodes = new Vector3[] {Vector3.zero, new Vector3(0, 0, -120)};
			                    planes.transform.position = nodes[0];
			                    iTween.MoveTo(planes.gameObject,
			                        iTween.Hash("path", nodes,
			                            "time", 5f,
			                            "oncomplete", "OpeningAnimationEnd",
			                            "oncompletetarget", gameObject,
			                            "easetype", iTween.EaseType.linear));

			                    isMoveToPath = true;

			                    StartCoroutine(SetPlayerSpeedWhenAnim());
			                    StartCoroutine(DelayToPlayAudio(2f));
			                }
			            }
			            holding = true;
			        }
			        transform.LookAt(AnimationTarget.position);

			        setFocusTargetPosition();
			    }
			    else
			    {
			        if (holding)
                    {
                        if (CameraAnimation)
                            CameraAnimation.Stop();

			            holding = false;
			           // UIManagerOz.SharedInstance.inGameVC.appear();
                        StartCoroutine(GameController.SharedInstance.ShowSkillButtonWhenIntroSceneFinished()); 
			        }

			        // eyal since we timeScale to 0 on pause the camera doesn't update which results in camera pointing sideways on character after resurrect.			
			        float dt = Time.deltaTime;
			        if (dt == 0f)
			        {
			            dt = 0.02f;
			        }

			        //-- Move the FocusTarget to the player
			        setFocusTargetPosition();

			        m_CurrentDistance = Mathf.Lerp(m_CurrentDistance, m_TargetDistance, dt*SmoothZoomSpeed);

			        //-- Calculate offset vector and a target Yaw
			        m_TargetCameraLocation.x = m_TargetCameraLocation.y = 0.0f;
			        m_TargetCameraLocation.z = -m_CurrentDistance;

			        if (PlayerTarget.Hold == false)
			        {
			            m_TargetYaw = SignedAngle(m_TargetCameraLocation.normalized, -PlayerTarget.transform.forward, Vector3.up);
			            //-- Clamp targetYaw to -180, 180
			            m_TargetYaw = Mathf.Repeat(m_TargetYaw + 180f, 360f) - 180f;
			            //-- Clamp smooth currentYaw to targetYaw and clamp it to -180, 180
			            m_CurrentYaw = Mathf.LerpAngle(m_CurrentYaw, m_TargetYaw, dt*SmoothRotationSpeed);
			            m_CurrentYaw = Mathf.Repeat(m_CurrentYaw + 180f, 360f) - 180f;
			        }

			        //-- Smooth pitch
			        m_CurrentPitch = Mathf.LerpAngle(m_CurrentPitch, m_TargetPitch, dt*SmoothPitchSpeed);

			        // Rotate offset vector
			        m_TargetCameraLocation = Quaternion.Euler(m_CurrentPitch, m_CurrentYaw + FollowYaw + RoundYaw, 0f)*
			                                 m_TargetCameraLocation;

			        if (PlayerTarget.IsClimb)
			        {
			            transform.rotation = Quaternion.identity;
			            transform.Rotate(90, 180, 0);

			            Vector3 newPos = FocusTarget.position + m_TargetCameraLocation;
			            newPos.z = Mathf.Clamp(newPos.z, -210, -7);
			            transform.position = newPos;
			        }
			        else
			        {
                        transform.position = FocusTarget.position + m_TargetCameraLocation;
			            //transform.Translate(transform.right,Space.World);
			            // And then have the camera look at our target
			            if (LookAtPlayer)
			                transform.LookAt(FocusTarget.position);
			            else
			                transform.rotation = PlayerTarget.transform.rotation;
			        }
			    }

			    if(holding == true)
				{
				//	m_CurrentDistance = Vector3.Distance (FocusTarget.position, transform.position);
				//	m_CurrentYaw = transform.rotation.eulerAngles.y;
				}
                //else if (GameController.SharedInstance.TimeSinceGameStart > Mathf.Epsilon)
                //{
                //    holdT = 0f;
                //}
			}
			else
			{
				SplineNode start = PlayerTarget.OnTrackPiece.CurrentTrackPieceData.splineStart;
				List<Transform> pathlocations = PlayerTarget.OnTrackPiece.CurrentTrackPieceData.PathLocations;
				float distToLast = (pathlocations[0].position - PlayerTarget.transform.position).magnitude;
				float distToNext = (pathlocations[pathlocations.Count-1].position - PlayerTarget.transform.position).magnitude;
				float percent = distToLast/(distToLast+distToNext);
				
				int count = 0;
				SplineNode cur = start;
				while(cur.next!=null)
				{
					cur = cur.next;
					count++;
				}
				
				percent *= (float)count;
				
				SplineNode useNode = start;
				while(percent>=1f)
				{
					useNode = useNode.next;
					percent-=1f;
				}
				
				if(percent<1f)
				{
					transform.position = useNode.Bezier(percent);
				}

	        	// And then have the camera look at our target
	        	transform.LookAt(PlayerTarget.transform.position);
			}
			
			if (IsCameraShaking) {
				TimeSinceCameraShakeStart -= Time.smoothDeltaTime;
				CameraShakeMagnitude -= (Time.smoothDeltaTime * CameraShakeDamperRate);
				if (CameraShakeMagnitude <= 0.0f || TimeSinceCameraShakeStart > CameraShakeDuration)
					IsCameraShaking = false;
				else {
					float cameraShakeOffsetX = Mathf.Sin(TimeSinceCameraShakeStart * 35.0f * CameraShakeFrequencyMultiplier) * CameraShakeMagnitude;
					float cameraShakeOffsetY = Mathf.Sin(TimeSinceCameraShakeStart * 50.0f * CameraShakeFrequencyMultiplier) * CameraShakeMagnitude;
					transform.Translate(cameraShakeOffsetX, cameraShakeOffsetY, 0);
			//		Debug.Log(transform.position);
				}
			}
			else if(ShakeAfterDelay)
			{
			    ShakeDelay -= Time.deltaTime;
			    if (ShakeDelay <= 0)
			    {
                    ShakeAfterDelay = false;
			        IsCameraShaking = true;
			    }
			}
		}
		
		CachedPosition = transform.position;
	}
	
	
	//private Vector3 currentDirectionOverride = Vector3.zero;
	private Vector3 currentPositionOffset = Vector3.zero;
	
	private void UpdatePosDirOffsets()
	{
		//NOTE: We have a mysterious crash in this function, so there is  a lot of error checking... Hopefully some of the Debug.Logs catch something
		if(GamePlayer.SharedInstance==null || GamePlayer.SharedInstance.OnTrackPiece==null)
			return;
		GamePlayer player = GamePlayer.SharedInstance;
		TrackPieceData data = player.OnTrackPiece.CurrentTrackPieceData;
		
		//Find the two closest points on the track
		int splineIndex = 0;
		float minmag = 99999f;
		float nextmag = 99998f;
		int minind = 0;
		int nextind = 0;
		
		//NOTE: keep an eye on this, it was crashing on rare instances so I put in a null check...
		if(data==null || data.PathLocations==null || player.CachedTransform==null)	return;
		
		for(int i=0;i<data.PathLocations.Count;i++)
		{
            float newmag = (data.PathLocations[i].position-player.CachedTransform.position).sqrMagnitude;
		    if(newmag<minmag)
			{
				nextmag = minmag;
				minmag = newmag;
				nextind = minind;
				minind = i;
			}
			else if(newmag<nextmag)
			{
				nextmag = newmag;
				nextind = i;
			}
		}
		if(minind<nextind)	splineIndex = minind;
		else 				splineIndex = minind-1;
		
		float totaldist;
		float lerpval;
		if(splineIndex>=0 && splineIndex+1<data.PathLocations.Count 
			&& data.PathLocations[splineIndex]!=null && data.PathLocations[splineIndex+1]!=null)
		{
			totaldist = (data.PathLocations[splineIndex].position - data.PathLocations[splineIndex+1].position).magnitude;
			if(totaldist>Mathf.Epsilon)
			{
				lerpval = (player.CachedTransform.position-data.PathLocations[splineIndex].position).magnitude/totaldist;
			}
			else
			{
				lerpval = 1f;
				notify.Error("Problem in OzGameCamera...");
			}
		}
		else
		{
			//CrittercismIOS.LeaveBreadcrumb("PROBLEM IN OZGAMECAMERA!!!! Get Bryant or Redmond. UpdatePosDirOffsets had a potential crash.");
			totaldist = 1f;
			lerpval = 1f;
			notify.Error("PROBLEM IN OZGAMECAMERA!!!! Get Bryant or Redmond. UpdatePosDirOffsets had a potential crash.");
		}
		
		Vector3 pos1 = (data.CameraPositionOffsets!=null && data.CameraPositionOffsets.Count>splineIndex) ? 
			data.CameraPositionOffsets[splineIndex] : Vector3.zero;
		Vector3 pos2 = (data.CameraPositionOffsets!=null && data.CameraPositionOffsets.Count>splineIndex+1) ?
			data.CameraPositionOffsets[splineIndex+1] : Vector3.zero;
		currentPositionOffset = Vector3.Lerp(pos1,pos2,lerpval);
		
		//Vector3 dir1 = data.CameraDirectionVectors.Count>splineIndex ? data.CameraDirectionVectors[splineIndex] : new Vector3(0f,-0.2f,-1f);
		//Vector3 dir2 = data.CameraDirectionVectors.Count>splineIndex+1 ? data.CameraDirectionVectors[splineIndex+1] : new Vector3(0f,-0.2f,-1f);
		//currentDirectionOverride = Vector3.Lerp(dir1,dir2,lerpval);
		
	}
	
	
	//Used to move the camera based on the track piece.
	//bool testToggle = false;
	//bool adjustCameraOffsetOnTile = false;
	float TileFocusHeightOffset;
	float TileFocusDistanceOffset;
	float TileFollowDistanceOffset;
	float TileFollowHeightOffset;
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(FocusTarget.position, 0.5f);
		Gizmos.DrawLine(transform.position, FocusTarget.position);
	}
	
	public void OnCinematicPullBackComplete(){
		GameController.SharedInstance.isPlayingCinematicPullback = false;
		cameraState = CameraState.cinematicOpening;
		GameController.SharedInstance.SetMainMenuAnimation();
	}

    public void Crash(float magnitude, float duration, float delay = 0)
    {
        if (duration > 0 && magnitude > 0)
        {
            //StartCoroutine(delayCrash(magnitude, duration, delay));

            curPos = transform.position;
            transform.position += transform.forward * magnitude;
            //crashPos = transform.position + transform.forward * magnitude;
            crashDuration = duration;
            //iTween.MoveTo(gameObject, iTween.Hash("position", crashPos, "delay", delay, "time", crashDuration, "oncomplete", "cameraMoveOncomplete", "oncompletetarget", gameObject, "easyType", iTween.EaseType.easeInSine));
            iTween.ShakePosition(gameObject,
                iTween.Hash("amount", Vector3.one*0.2f, "delay", delay, "time", 0.2f,
                    "oncomplete", "cameraMoveOncomplete", "oncompletetarget", gameObject, "easyType",
                    iTween.EaseType.easeInSine));
        }
    }

    void cameraMoveOncomplete()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", curPos, "time", crashDuration, "easyType", iTween.EaseType.easeInSine));
        isCrash = false;
    }

    private IEnumerator delayCrash(float magnitude, float duration, float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        curPos = transform.position;
        //crashPos = transform.position + transform.forward*magnitude;
        crashDuration = duration;
        isCrash = true;
    }

    private void cameraMoveUpdate()
    {
        if (transform.position.y > 3.7f)
            transform.LookAt(FocusTarget);
    }

    private void OpeningAnimationEnd()
    {
        iTween.MoveTo(planes,
            iTween.Hash("position", new Vector3(0, 0, -140),
                "time", 1f,
                "oncomplete", "MoveToEnd",
                "oncompletetarget", gameObject,
                "easetype", iTween.EaseType.linear));
    }

    private void MoveToEnd()
    {
        if (planes)
            planes.SetActive(false);
    }

    public IEnumerator SetPlayerSpeedWhenAnim()
    {
        //if (!isFristAnim)
        //{
        //    GamePlayer.SharedInstance.SetPlayerVelocity(SpeedWhenAnim + 1.5f);
        //    isFristAnim = true;
        //}
        //else
            GamePlayer.SharedInstance.SetPlayerVelocity(SpeedWhenAnim);

        yield return new WaitForSeconds(CameraAnimationLength);

        //动画播放结束重置速度
        GamePlayer.SharedInstance.SetMaxRunVelocity(GameController.SharedInstance.CurrentMaxSpeed);
        GamePlayer.SharedInstance.SetPlayerVelocity(GameProfile.SharedInstance.DefaultMinSpeed);
    }

    public IEnumerator DelayToPlayAudio(float time)
    {
        yield return new WaitForSeconds(time);
        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.game_opening);
    }

    public void FollowPlayerTONextSafeSpot()
    {
        setFocusTargetPosition();

        m_TargetCameraLocation.x = m_TargetCameraLocation.y = 0.0f;
        m_TargetCameraLocation.z = -m_TargetDistance;

        transform.position = FocusTarget.position + m_TargetCameraLocation;

        transform.LookAt(FocusTarget.position);
    }

    public void Fade(float time)
    {
        var mesh = fade.GetComponent<MeshRenderer>();
        if (mesh)
            mesh.material.color = Color.white;

        iTween.ColorTo(fade, Color.black, time);
    }

    public void SwitchView(GameMode mode)
    {
        if (mode == GameMode.Climb)
        {
            StopMove = true;

            transform.rotation = Quaternion.identity;
            transform.Rotate(90, 180, 0);
            transform.position = GamePlayer.SharedInstance.CurrentPosition + Vector3.up*8f +
                                 GamePlayer.SharedInstance.transform.forward*7f;

            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 7;
        }
        else
        {
            Camera.main.orthographic = false;
            StopMove = false;
        }
    }
}
