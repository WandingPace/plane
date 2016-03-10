using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereCastMono:MonoBehaviour
{
    public static SphereCastMono instance;
    void Awake()
    {
        if(instance==null)
            instance = this;
    }

    public void ClearRangeObstacle(int score = 0)
    {

        Vector3 p = GamePlayer.SharedInstance.CachedTransform.position;
        RaycastHit[] hits;
        Vector3 p1 =new Vector3(0f,-1,p.z);//中
        Vector3 p2 = new Vector3(1f,-1,p.z);//左
        Vector3 p3 = new Vector3(-1f,-1,p.z);//右

//        hits =Physics.SphereCastAll(p1,1.3f,GamePlayer.SharedInstance.CachedTransform.forward,30f);


        //        //清除前方
//        hits = Physics.RaycastAll(p1,GamePlayer.SharedInstance.CachedTransform.forward,30f);
//        Check(hits,score);
//        hits = Physics.RaycastAll(p2,GamePlayer.SharedInstance.CachedTransform.forward,30f);
//        Check(hits,score);
//        hits = Physics.RaycastAll(p3,GamePlayer.SharedInstance.CachedTransform.forward,30f);
//        //清除后方
//        Check(hits,score);
//        hits = Physics.RaycastAll(p1,-GamePlayer.SharedInstance.CachedTransform.forward,30f);
//        Check(hits,score);
//        hits = Physics.RaycastAll(p2,-GamePlayer.SharedInstance.CachedTransform.forward,30f);
//        Check(hits,score);
//        hits = Physics.RaycastAll(p3,-GamePlayer.SharedInstance.CachedTransform.forward,30f);
//        Check(hits,score);
//        //清除左右
//        hits = Physics.RaycastAll(p,GamePlayer.SharedInstance.CachedTransform.right,30f);
//        Check(hits,score);
//        hits = Physics.RaycastAll(p,-GamePlayer.SharedInstance.CachedTransform.right,30f);
//        Check(hits,score);
      

        int layer = 1<<LayerMask.NameToLayer("stumbleColliders");
            //清除前方
            Vector3 p12 =new Vector3(0f,2,p.z);
            Vector3 p22 = new Vector3(1f,2,p.z);
            Vector3 p32 = new Vector3(-1f,2,p.z);
        hits = Physics.CapsuleCastAll(p1,p12,0.5f,GamePlayer.SharedInstance.CachedTransform.forward,30f,layer);
            Check(hits,score);
        hits = Physics.CapsuleCastAll(p2,p22,0.5f,GamePlayer.SharedInstance.CachedTransform.forward,30f,layer);
            Check(hits,score);
        hits = Physics.CapsuleCastAll(p3,p32,0.5f,GamePlayer.SharedInstance.CachedTransform.forward,30f,layer);
            //清除后方
            Check(hits,score);
        hits = Physics.CapsuleCastAll(p1,p12,0.5f,-GamePlayer.SharedInstance.CachedTransform.forward,30f,layer);
            Check(hits,score);
        hits = Physics.CapsuleCastAll(p2,p22,0.5f,-GamePlayer.SharedInstance.CachedTransform.forward,30f,layer);
            Check(hits,score);
        hits = Physics.CapsuleCastAll(p3,p32,0.5f,-GamePlayer.SharedInstance.CachedTransform.forward,30f,layer);
            //清除左右
        hits = Physics.CapsuleCastAll(p,p12,0.5f,GamePlayer.SharedInstance.CachedTransform.right,30f,layer);
            Check(hits,score);
        hits = Physics.CapsuleCastAll(p,p12,0.5f,-GamePlayer.SharedInstance.CachedTransform.right,30f,layer);
            Check(hits,score);

    }

    private void Check(RaycastHit[] hits,int score)
    {

        if(hits.Length>0)
        {
           
            foreach(RaycastHit ht in hits)
            {  
                if(ht.transform.parent.gameObject.activeSelf)
                {   //Debug.LogError(ht.transform.name);
                    TrackPieceData tpData = null;
                    MovingObstacle mo = null;
                    Transform _transform = ht.transform;
                    while (true)
                    {
                        tpData =_transform.GetComponentInParent<TrackPieceData>();
                        mo =_transform.GetComponentInParent<MovingObstacle>();
                        _transform = _transform.parent;
                        
                        if (tpData != null)
                        {
                            StartCoroutine(PlayObstacleBreakEffect(tpData.transform.position));
                            PoolManager.Pools["Enemies"].Despawn(tpData.transform,null);
                            GamePlayer.SharedInstance.AddScore(score,true);
                            break;
                        }
                        
                        if (mo != null)
                        {   
                            if(mo.transform.name.Contains("ColinCowling_or_Leadbottom_prefab")||
                               mo.transform.name.Contains("Zed_and_Ned_prefab"))
                            {
                                if(mo.transform.childCount>0)
                                {
                                    Transform ts =  mo.transform.GetChild(0);
                                    StartCoroutine(PlayObstacleBreakEffect(ts.position));
                                    ts.parent = PoolManager.Pools["Enemies"].transform;
                                    ts.ResetTransformation();
                                    PoolManager.Pools["npc"].Despawn(ts,null);
                                }

                            }
                            else
                            {
                                StartCoroutine(PlayObstacleBreakEffect(mo.transform.position));
                                PoolManager.Pools["npc"].Despawn(mo.transform,null);
                            }
                            GamePlayer.SharedInstance.AddScore(score,true);
                            break;
                        }
                        
                        if (_transform == null)
                            break;
                    }
                }
            }
        }
    }


    public  IEnumerator PlayObstacleBreakEffect(Vector3 position)
    {
        ParticleSystem go =MonoBehaviour.Instantiate(GamePlayer.SharedInstance.playerFx.obstacleBreak) as ParticleSystem;
        if(go!=null)
        {
            go.transform.ResetTransformation();
            go.transform.position = position;
            go.Play();
            yield return new WaitForSeconds(go.duration);
            Destroy(go.gameObject);
        }
        yield break;
    }


}
