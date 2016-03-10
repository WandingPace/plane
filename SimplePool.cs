using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

public class SimplePool : MonoBehaviour
{
	public delegate void ObjectSpawned(GameObject SpawnedObject);
	public delegate void ObjectDespawned(GameObject DespawnedObject);
	public event ObjectSpawned OnObjectSpawned;
	public event ObjectDespawned OnObjectDespawned;

	public Dictionary<Object, List<GameObject>> mPoolNonActive = new Dictionary<Object, List<GameObject>>();
	public Dictionary<GameObject, Object> mPoolActive = new Dictionary<GameObject, Object>();
	public Dictionary<string, Object> mPaths = new Dictionary<string, Object>();

	public int GetUsedCount(Object prefab)
	{
		int count = 0;
		foreach(Object obj in mPoolActive.Values)
		{
			if (obj.Equals(prefab))
			{
				++count;
			}
		}
		
		return count;
	}

	void Awake()
	{
		SimplePoolManager.Instance.AddPool(this);
	}

	public GameObject Spawn(string Path)
	{
		Object prefab = null;
		if (mPaths.ContainsKey(Path))
			prefab = mPaths[Path];
		else
		{
			prefab = Resources.Load(Path);
			mPaths.Add(Path, prefab);
		}

		return Spawn(prefab);
	}

	public GameObject Spawn(Object pPrefab)
	{
		if (pPrefab == null)
			return null;
		
		if (mPoolNonActive.ContainsKey(pPrefab))
		{
			List<GameObject> objects = mPoolNonActive[pPrefab];
			if (objects.Count > 0)
			{
				GameObject go = objects[objects.Count - 1];
				objects.RemoveAt(objects.Count - 1);
				go.SetActive(true);

				mPoolActive.Add(go, pPrefab);

				go.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);

				return go;
			}
		}

		// Not found, so create another
		{
			GameObject go = (GameObject)GameObject.Instantiate(pPrefab);
			mPoolActive.Add(go, pPrefab);

			go.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);

			if (OnObjectSpawned != null)
				OnObjectSpawned(go);

			return go;
		}
	}

	public void Unspawn(GameObject ObjectToDestroy)
	{
		if (ObjectToDestroy == null)
			return;
		
		//////Profiler.BeginSample("Unspawn");
		if (mPoolActive.ContainsKey(ObjectToDestroy))
		{
			
			Object prefab = mPoolActive[ObjectToDestroy];
			mPoolActive.Remove(ObjectToDestroy);
			
			if (!mPoolNonActive.ContainsKey(prefab))
			{
				mPoolNonActive.Add(prefab, new List<GameObject>());
			}

			mPoolNonActive[prefab].Add(ObjectToDestroy);
			
			//////Profiler.BeginSample("BroadcastMessage");
			ObjectToDestroy.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);

			if (OnObjectDespawned != null)
				OnObjectDespawned(ObjectToDestroy);

			//////Profiler.EndSample();
			
			ObjectToDestroy.SetActive(false);
			if (ObjectToDestroy.GetComponent<DynamicElement>())
				ObjectToDestroy.transform.parent = null;
			else
				ObjectToDestroy.transform.parent = transform;
		}
		else
		{
			// Not found in pools... Anyway destroy it
			Destroy(ObjectToDestroy);
		}
		
		//////Profiler.EndSample();
	}

	public void UnloadPrefab(Object prefab)
	{
		string s = null;
		foreach(KeyValuePair<string,Object> entry in mPaths)
		{
			if (entry.Value == prefab)
			{
				s = entry.Key;
				break;
			}
		}
		if (s != null)
		{
			mPaths.Remove(s);
		}
	}

	public void UnloadMemory ()
	{
		int prefabUnloaded = 0;
		int ObjectUnloaded = 0;
		
		foreach(KeyValuePair<Object,List<GameObject>> entry in mPoolNonActive)
		{
			Object prefab = entry.Key;
			List<GameObject> gos = entry.Value;
			
			int usedCount = GetUsedCount(prefab);

			// destroy GameObject
			foreach (GameObject go in gos)
			{
				go.transform.parent = null;
				DestroyImmediate(go);
			}
			
			if (usedCount == 0)
			{
				UnloadPrefab(prefab);
				++prefabUnloaded;
			}
			
			ObjectUnloaded += gos.Count;
		}
		
		mPoolNonActive.Clear();
		
		//Debug.Log(string.Format("name : {3} prefabUnloaded : {0} - ObjectUnloaded : {1} mPoolActive {2}", prefabUnloaded, ObjectUnloaded, mPoolActive.Count, this.name));
	}
	
	public void Preload(string path, int count)
	{
		// Get Prefab only one time.
		Object prefab = null;
		if (mPaths.ContainsKey(path))
		{
			prefab = mPaths[path];
		}
		else
		{
			prefab = (GameObject)Resources.Load(path);
			mPaths.Add(path, prefab);
		}
		
		Preload(prefab, count);
	}

	public void Preload(Object prefab, int count)
	{
		// how many item
		int alreadyCreated = GetUsedCount(prefab);
		
		//Debug.Log (string.Format ("Preload pool name : {0} Created : {1} new : {2}", this.name, alreadyCreated, count - alreadyCreated));
		// preload missing prefab
		for (int i = alreadyCreated; i < count; ++i)
		{
			Preload(prefab);
		}
	}
	
	public void Preload(Object Prefab)
	{
		if (Prefab == null)
			return;
		
		if (!mPoolNonActive.ContainsKey(Prefab))
			mPoolNonActive.Add(Prefab, new List<GameObject>());

		GameObject go = (GameObject)UnityEngine.Object.Instantiate(Prefab);
		go.SetActive(false);
		go.transform.parent = transform;
		mPoolNonActive[Prefab].Add(go);
		
		if (OnObjectSpawned != null)
			OnObjectSpawned(go);
	}
}