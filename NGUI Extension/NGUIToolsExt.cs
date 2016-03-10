using UnityEngine;
using System.Collections;

public class NGUIToolsExt : MonoBehaviour {

	static public void SetActive (GameObject go, bool state)
	{
		if (state)
		{
			Activate(go.transform);
		}
		else
		{
			Deactivate(go.transform);
		}
	}
	static void Activate (Transform t)
	{
		SetActiveSelf(t.gameObject, true);

        for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			Activate(child);
		}
	}
	static void Deactivate (Transform t)
	{
		for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			Deactivate(child);
		}
		SetActiveSelf(t.gameObject, false);
	}
	static public void SetActiveSelf(GameObject go, bool state)
	{
		#if UNITY_3_5
		go.active = state;
		#else
		go.SetActive(state);
		#endif
	}
}
