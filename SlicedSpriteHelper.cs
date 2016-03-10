using UnityEngine;
using System.Collections;

public class SlicedSpriteHelper : MonoBehaviour {
	
	protected static Notify notify;
	private UISprite slicedSprite;
	private Vector3 origPos;
	private float ratioX;
	private float ratioY;
	private GameObject myParent;
	
	void Awake(){
		notify = new Notify(this.GetType().Name);	
		slicedSprite = GetComponent<UISprite>();
		notify.Debug ("Awake *************************");
		origPos = transform.localPosition;
		//DebugIt();
	}
	
	void Start(){
		notify.Debug ("Start ******************** " + name);
		DebugIt();

		ratioX = transform.localScale.x / slicedSprite.GetAtlasSprite().width;//.outer.width ;
		ratioY = transform.localScale.y / slicedSprite.GetAtlasSprite().height;//outer.height ;
		
		//Debug.Log("chosenUIResType " + UIManagerOz.SharedInstance.chosenUIResType);
		
		if(UIManagerOz.SharedInstance.chosenUIResType != UIManagerOz.UIResolutionType.kResolution480)
			return;
		
		//if(ratioX < 1 || ratioY < 1)
		//	return;
		
		myParent = new GameObject();
		myParent.transform.parent = transform.parent;
		myParent.transform.localPosition = origPos;
		myParent.transform.localScale = Vector3.one;
		
		transform.parent = myParent.transform;
		
		//myParent.transform.localScale = new Vector3(ratioX, ratioY, 1f);
		//transform.localScale = new Vector3(slicedSprite.sprite.outer.width, slicedSprite.sprite.outer.height);
				
		float ratio = 1;
		if(ratioY < ratioX){
			ratio = ratioX / ratioY;
			myParent.transform.localScale = new Vector3(ratioX / ratio, ratioY, 1f);
            transform.localScale = new Vector3(slicedSprite.GetAtlasSprite().width * ratio, slicedSprite.GetAtlasSprite().height);
		}
		else{
			ratio = ratioY / ratioX;
			myParent.transform.localScale = new Vector3(ratioX , ratioY / ratio, 1f);
            transform.localScale = new Vector3(slicedSprite.GetAtlasSprite().width, slicedSprite.GetAtlasSprite().height * ratio);
		}
        //slicedSprite.cornerMult = slicedSprite.cornerMult;
		
		notify.Debug("ratio x " + ratioX + " y " + ratioY  + " ratio " + ratio);
		
	}
	
	
	private void DebugIt(){
		//NOTE! It looks like we've been crashing here on devices... I'm going to UNITY_EDITOR this stuff...
#if UNITY_EDITOR
		notify.Debug ("scale is " + transform.localScale);
		//notify.Debug ("sprite inner " + slicedSprite.sprite.inner + " outer " + slicedSprite.sprite.outer);
		notify.Debug ("atlas pixelSize" + slicedSprite.atlas.pixelSize + " height " + slicedSprite.atlas.texture.height + " width " + slicedSprite.atlas.texture.width);
        notify.Debug("texture width " + slicedSprite.GetAtlasSprite().width + " height " + slicedSprite.GetAtlasSprite().height);
		notify.Debug( slicedSprite.mainTexture.name);
#endif
	}
	
#if UNITY_EDITOR
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			notify.Debug ("Debug on touch ");
//			slicedSprite.UpdateUVs(true);
			DebugIt();
		}
	}
#endif
}
