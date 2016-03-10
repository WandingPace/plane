using UnityEngine;
using System.Collections;

public class Realtime : MonoBehaviour {

	private static Realtime _main;
	
	private static Realtime main
	{
		get {
			if(_main==null)
			{
				Init();
			}
			return _main;
		}
	}
	
	
	private static float lastFrameRealtime = 0f;
	
	private static void Init()
	{
		if(_main==null)
		{
			GameObject go = new GameObject("Realtime");
			_main = go.AddComponent<Realtime>();
			lastFrameRealtime = Time.realtimeSinceStartup - Time.deltaTime;
		}
	}
	
	
	
	
	public static float deltaTime
	{
		get {
			if(_main==null)
			{
				Init();
			//	return Time.timeScale==0 ? 0f : Time.deltaTime / Time.timeScale;
			}
			return Time.realtimeSinceStartup - lastFrameRealtime;
		}
	}
	
	
	public static IEnumerator WaitForSeconds(float seconds)
	{
		float st = Time.realtimeSinceStartup;
		
		while(Time.realtimeSinceStartup - st < seconds)
			yield return null;
	}
	
	
	
	
	
	void LateUpdate()
	{
		lastFrameRealtime = Time.realtimeSinceStartup;
	}
}
