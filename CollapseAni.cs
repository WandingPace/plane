using UnityEngine;
using System.Collections;

public class CollapseAni : TrackPieceAttatchment
{
    public ParticleSystem dirt01;
    public AudioManager.Effects sfx;
    public float sfxDelay = 0;
    public Animation anim;
    public string animationString = "Take01";
    public bool hideWhenDeactive = false;
    public Vector3 offSet;
    public float Triggerdiff = 1f;

    public override void Awake()
    {
        anim = GetComponentInChildren<Animation>();
        if (anim != null) 
            anim.playAutomatically = false;
    }

    void OnSpawned()
    {
        StartCoroutine(Rewind());
    }
    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Rewind());
    }

    public override void OnPlayerEnteredTrackPiece()
    {
        //StartCoroutine(DelayedAnimate());
    }

    public override void OnPlayerEnteredPreviousTrackPiece()
    {
        StartCoroutine(DelayedAnimate());
    }

    void Animate()
    {
        anim.gameObject.SetActive(true);

        //if (GameController.SharedInstance.Player.getModfiedMaxRunVelocity() > 0f)
        //    anim[animationString].speed = GameController.SharedInstance.Player.getRunVelocity() / 10f;

        anim.Play(animationString);

        //if (audio)
        //{
        //    if (AudioManager.SharedInstance)
        //        audio.volume = AudioManager.SharedInstance.SoundVolume;
        //    audio.Play();
        //}
        if (sfx > AudioManager.Effects.MAX)
            AudioManager.SharedInstance.PlayAnimatedSound(sfx, sfxDelay);
        if (dirt01 != null)
            dirt01.Play();
    }

    IEnumerator DelayedAnimate()
    {
        if (string.IsNullOrEmpty(animationString))
            yield break;

        while (true)
        {
            float distance = Mathf.Abs(Vector3.Distance(GamePlayer.SharedInstance.CurrentPosition, transform.position));
            float TriggeDistance = GameController.SharedInstance.Player.getRunVelocity()*Triggerdiff + 2f;
            if (distance <= TriggeDistance)
                break;
            
            yield return new WaitForFixedUpdate();
        }
        Animate();
    }

    IEnumerator Rewind()
    {
        transform.position += offSet;
        if (anim != null)
        {
            anim.Play(animationString);
            anim[animationString].enabled = true;
            anim[animationString].time = 0f;
            anim.Sample();
            anim[animationString].enabled = false;
            //anim.Play();
            //anim[animationString].speed = 0f;
            //yield return null;
            //anim.Rewind();
            //anim.Stop();
            anim.gameObject.SetActive(!hideWhenDeactive);
        }
        yield break;
    }
}
