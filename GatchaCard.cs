using UnityEngine;
using System.Collections;

public class GatchaCard : MonoBehaviour {
	
	public Vector3 origLocalPos; //
	public Vector3 origScale;
	public bool isFlipped = false; //是否已开启
	public GatchaDataSet data; //奖品数据
	public UILabel awardDesc; //描述
    public UILabel amount;      //个数
    public UISprite awardIcon; //具体奖品图标
	public UISprite musicBox; //宝箱图标
    public GameObject BoxRoot;
    public Animation boxModel;//宝箱模型
	public ParticleSystem fx; 
	public ParticleSystem fxOpenBox; //
	public ParticleSystem fxEmpty;
	[HideInInspector]
	public Vector3 iconOrigScale;
	
	void Awake () 
    {
		origLocalPos = transform.localPosition;
		origScale = musicBox.transform.localScale;
		iconOrigScale = Vector3.one;
	}
	void Start(){
	}
	
	
	
}
