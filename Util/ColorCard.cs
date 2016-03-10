using UnityEngine;
using System.Collections;

public class ColorCard : MonoBehaviour {

	private static ColorCard _main;
	public static ColorCard main
	{
		get
		{
			if(_main==null)
			{
				GameObject obj = new GameObject("ColorCard");
				
				obj.transform.localScale = new Vector3(2f,2f,1f);
				
				GUITexture guiTex = obj.AddComponent<GUITexture>();
				blackTex = new Texture2D(1,1,TextureFormat.RGBA32,false);
				blackTex.SetPixel(0,0,new Color(0,0,0,1));
				blackTex.Apply();
				guiTex.texture = blackTex;
				guiTex.color = new Color(0,0,0,0);
				
				_main = obj.AddComponent<ColorCard>();
				
				DontDestroyOnLoad(_main.gameObject);
			}
			return _main;
		}
	}
	
	private static Texture2D blackTex;
	
	private bool offscreen = false;
	
	private static int current_id = 0;
	
	
	void OnLevelWasLoaded(int level)
	{
		DontDestroyOnLoad(gameObject);
	}
	
	
	IEnumerator FadeTo(Color color, float duration)
	{
		current_id++;
		int my_id = current_id;
		
		float st = Time.realtimeSinceStartup;
		
		Color startcol = guiTexture.color;
		Color newcolor = startcol;
		
		while(my_id==current_id && color!=newcolor)
		{
			newcolor = Color.Lerp(startcol,color,(Time.realtimeSinceStartup-st)/duration);
			guiTexture.color = newcolor;
			
			//Move offscreen if the guitexture is transparent (ios rendering reasons)
			if(newcolor.a<0.001f && !offscreen)
			{
				offscreen = true;
				transform.position = new Vector3(0,5000,5000);
			}
			if(newcolor.a>0.001f && offscreen)
			{
				offscreen = false;
				transform.position = new Vector3(0,0,0);
			}
			
			yield return null;
		}
		
		yield break;
	}
	
	public static Coroutine FadeToBlack(float duration)
	{
		return main.StartCoroutine(main.FadeTo(Color.black,duration));
	}
	
	public static Coroutine FadeToPicture(float duration)
	{
		return main.StartCoroutine(main.FadeTo(new Color(0,0,0,0),duration));
	}
	
	public static Coroutine FadeToColor(Color color, float duration)
	{
		return main.StartCoroutine(main.FadeTo(color,duration));
	}
	
	public static void SetTexture(Texture t)
	{
		Vector3 scale = _main.guiTexture.transform.localScale;
		scale.x = ((float)Screen.height/(float)Screen.width) * (3f/4f);
		_main.guiTexture.transform.localScale = scale;
		_main.guiTexture.texture = t;
	}
	
	public static Color CurrentColor
	{
		get
		{
			return main.guiTexture.color;
		}
	}
	
}
