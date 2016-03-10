using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Tools
{
	public static void AttachObjectToTarget(Transform ao_Object, Transform ao_Target)
	{
		ao_Object.transform.parent = ao_Target.transform;
		ao_Object.transform.localPosition = Vector3.zero;
		ao_Object.transform.localRotation = Quaternion.identity;
	}

    public static ColliderType AddColliderObjectToGameObject<ColliderType>(GameObject ao_ParentObject, string asz_ColliderObjectName, PhysicMaterial ao_PhysicMaterial, int as_layer) where ColliderType : Collider
    {
        GameObject o_ColliderObject = new GameObject(asz_ColliderObjectName);
        o_ColliderObject.layer = as_layer;
        Tools.AttachObjectToTarget(o_ColliderObject.transform, ao_ParentObject.transform);
        ColliderType o_Collider = o_ColliderObject.AddComponent<ColliderType>();
        o_Collider.material = ao_PhysicMaterial;

        return o_Collider;
    }

	public static void DebugPrintFlag(int ai_Flag)
	{
		string sz_Flag = "";
		for(int i=(sizeof(int) * 8)-1; i>=0; i--)
		{
			if ((Convert.ToBoolean(ai_Flag & (1 << i))))
			{
				sz_Flag += "1";
			}
			else
			{
				sz_Flag += "0";
			}
		}
		if (Debug.isDebugBuild) Debug.Log(sz_Flag);
	}

	public static T GetRandomElementFromArray<T>(T[] to_array)
	{
		int i_RandomElementIndex = UnityEngine.Random.Range(0, to_array.Length);
		return to_array[i_RandomElementIndex];
	}
	public static T GetRandomElementFromArray<T>(T[] to_array, ref int ai_UnallowedIndex)
	{
		int i_RandomIndex;
		while (ai_UnallowedIndex == (i_RandomIndex = UnityEngine.Random.Range(0, to_array.Length)) && (to_array.Length > 1))
			; // loop
		ai_UnallowedIndex = i_RandomIndex;
		return to_array[i_RandomIndex];
	}

}
