using UnityEngine;
using System.Collections;

public class SelfDestructIfNotPlatform : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_IPHONE
	public iPhoneGeneration[] Platforms;
#endif
    public bool ForceStay = false;

    void Awake()
    {

#if UNITY_EDITOR
        if (ForceStay)
            return;
#endif

#if UNITY_EDITOR || UNITY_IPHONE
        for (int i = 0; i < Platforms.Length; ++i)
            if (iPhone.generation == Platforms[i])
                return;

        Destroy(gameObject); // getting here means non of the platforms were picked
#endif
    }
}
