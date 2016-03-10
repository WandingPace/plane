using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float ArrowSpeed = 500f;
    private EnemyTarget EnemyTarget;
    private TrailRenderer mo_TrailRenderer;
    public TrailRenderer mo_TrailRendererPrefab;
    private Vector3 Speed = Vector3.zero;
    private Vector3 StartPosition = Vector3.zero;
    private float TimeSinceShot;
    private float TimeToHitTarget;
    private float TimeToLive;

    private void Awake()
    {
        //DebugTools.Assert(mo_TrailRendererPrefab != null, string.Format("Arrow {0} has no mo_TrailRendererPrefab\n", gameObject.name));
    }

    public static Arrow Instanciate()
    {
        var pool = SimplePoolManager.Instance.GetPool("ArrowsPool");

        var go = pool.Spawn(Attack.SharedInstance.GetProjectilePrefab().gameObject);
        var arrow = go.GetComponent<Arrow>();

        return arrow;
    }

    public void DestroySelf()
    {
        var pool = SimplePoolManager.Instance.GetPool("ArrowsPool");
        pool.Unspawn(gameObject);
        Destroy(mo_TrailRenderer);
    }

    public void Setup(GamePlayer Player, EnemyTarget Target)
    {
        Setup(Player, Target, Target.transform.position + Target.KillCenterOffset, Vector3.zero);
    }

    public void Setup(GamePlayer Player, Vector3 TargetPosition, Vector3 TargetSpeed)
    {
        Setup(Player, null, TargetPosition, TargetSpeed);
    }

    protected void Setup(GamePlayer Player, EnemyTarget Target, Vector3 TargetPosition, Vector3 TargetSpeed)
    {
        EnemyTarget = Target;
        StartPosition = GamePlayer.SharedInstance.transform.position + new Vector3(0f, 4f, 0f);
        Vector3 impact;
        Speed =
            CalculateDirection(ref StartPosition, GamePlayer.SharedInstance.GetPlayerVelocity(), ArrowSpeed,
                ref TargetPosition, ref TargetSpeed, out impact)*ArrowSpeed;
        Speed += GamePlayer.SharedInstance.GetPlayerVelocity(); // Adding the player's speed as intertia
        transform.forward = Speed.normalized;
        transform.position = StartPosition + (20.0f*Speed.normalized);
        ;

        TimeToLive = 1f;
        TimeSinceShot = 0f;
        TimeToHitTarget = (impact - StartPosition).magnitude/ArrowSpeed;

        mo_TrailRenderer = Instantiate(mo_TrailRendererPrefab) as TrailRenderer;
        Tools.AttachObjectToTarget(mo_TrailRenderer.transform, transform);

        if (EnemyTarget != null)
            EnemyTarget.IsAimedByArrow = true;
    }

    private void Update()
    {
        transform.position = transform.position + Speed*Attack.TimeScale.GetDeltaTime();
        //this.transform.position = this.transform.position + Speed * Time.deltaTime;

        TimeSinceShot += Attack.TimeScale.GetDeltaTime();
        //TimeSinceShot += Time.deltaTime;

        if (TimeSinceShot >= TimeToHitTarget && EnemyTarget != null)
        {
            DestroySelf();
            EnemyTarget.Kill();
        }

        if (TimeSinceShot >= TimeToLive)
        {
            DestroySelf();

            if (EnemyTarget != null)
                EnemyTarget.IsAimedByArrow = false;
        }
    }

    private Vector3 CalculateDirection(ref Vector3 ShooterPosition, Vector3 ShooterSpeed, float ProjectileSpeed,
        ref Vector3 TargetPosition, ref Vector3 TargetSpeed, out Vector3 Impact)
    {
        var relativeTargetVelocity = TargetSpeed - ShooterSpeed;
        var VectorToTarget = TargetPosition - ShooterPosition;

        Impact = Vector3.zero;

        if (relativeTargetVelocity.sqrMagnitude == 0f)
            return VectorToTarget.normalized;

        var a = relativeTargetVelocity.sqrMagnitude - (ProjectileSpeed*ProjectileSpeed);
        var b = 2f*Vector3.Dot(relativeTargetVelocity, VectorToTarget);
        var c = VectorToTarget.sqrMagnitude;

        var det = b*b - 4f*a*c;

        if (det < 0) // There is no solution
            return Vector3.zero;
        var timeA = (-b + (float) Math.Sqrt(det))/(2f*a);
        var timeB = (-b - (float) Math.Sqrt(det))/(2f*a);

        var lowestPossibleTime = timeB;
        if (timeA > 0 && timeA < timeB)
            lowestPossibleTime = timeA;

        Impact = TargetPosition + relativeTargetVelocity*lowestPossibleTime;

        return (Impact - ShooterPosition).normalized;
    }
}