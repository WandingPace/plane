using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISlider))]
public class MyUIProgressBar : MonoBehaviour 
{
    private UISlider progress;
    public bool IsCellAnimating { get; private set; }
    private int count;           //充满进度条动画播放次数
    private float newProgress;    //当前进度值
    private float newMaxProgress; //总进度值

    void Awake()
    {
        progress = GetComponent<UISlider>();
    }

    public static MyUIProgressBar Get(GameObject go)
    {
        MyUIProgressBar bar = go.GetComponent<MyUIProgressBar>();
        if (bar == null) bar = go.AddComponent<MyUIProgressBar>();
        return bar;
    }

    public void StartAnimation(int count,float newPro,float newMaxPro)
    {
        this.count = count;
        newProgress = newPro;
        newMaxProgress = newMaxPro;

        StartCoroutine(coLogic());
    }

    private IEnumerator coLogic()
    {
        while(count>0)
        {
            AnimateProgressBar(1f);
            while(IsCellAnimating)
                yield return null;
            --count;
            progress.value=0f;

        }

        if(count ==0)
        {
            float val = newProgress*1.0f/newMaxProgress;
            AnimateProgressBar(val);
            while(IsCellAnimating)
                yield return null;
        }
        yield return 0;
    }

    public void AnimateProgressBar(float val)
    {
        IsCellAnimating = true;

        float tweenTime = (progress.value == val) ? 0.001f :0.5f;

        iTween.ValueTo( progress.gameObject, iTween.Hash(
            "time", tweenTime,
            "from", progress.value,
            "to", val,
            "onupdate", "OnProgressBarUpdate",
            "onupdatetarget", gameObject,
            "oncomplete", "OnProgressBarComplete",
            "oncompletetarget", gameObject
            ));
        
    }


    
    public void OnProgressBarUpdate(float val){
        progress.value = val;
    }
    
    public void OnProgressBarComplete(){
        IsCellAnimating = false;
    }

}
