using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePoolManager
{
	public delegate void ObjectSpawned(GameObject SpawnedObject);
	public delegate void ObjectDespawned(GameObject DespawnedObject);
	public event ObjectSpawned OnObjectSpawned;
	public event ObjectDespawned OnObjectDespawned;

	static private SimplePoolManager sInstance = null;
	static public SimplePoolManager Instance { get { if (sInstance == null) sInstance = new SimplePoolManager(); return sInstance; } }

	private Dictionary<string, SimplePool> mPools = new Dictionary<string, SimplePool>();
	

	public void AddPool(SimplePool Pool)
	{
		if (mPools.ContainsKey(Pool.name))
		{
			// Already exists in the dictionnary, replace the existing one
			mPools[Pool.name].OnObjectSpawned -= OnPoolObjectSpawned;
			mPools[Pool.name].OnObjectDespawned -= OnPoolObjectDespawned;
			mPools[Pool.name] = Pool;
		}
		else
		{
			mPools.Add(Pool.name, Pool);
			Pool.OnObjectSpawned += OnPoolObjectSpawned;
			Pool.OnObjectDespawned += OnPoolObjectDespawned;
		}
	}

	public void RemovePool(SimplePool Pool)
	{
		RemovePool(Pool.name);
	}

	public void RemovePool(string Name)
	{
		if (mPools.ContainsKey(Name))
		{
			mPools[Name].OnObjectSpawned -= OnPoolObjectSpawned;
			mPools[Name].OnObjectDespawned -= OnPoolObjectDespawned;
		}

		mPools.Remove(Name);
		
	}

	public SimplePool GetPool(string Name)
	{
		if (mPools.ContainsKey(Name))
		{
			return mPools[Name];
		}
		else
			return null;
	}

	
	public void UnloadMemory()
	{
		foreach(SimplePool pool in mPools.Values)
		{
			pool.UnloadMemory();
		}
	}

	void OnPoolObjectSpawned(GameObject GameObjectSpawned)
	{
		if (OnObjectSpawned != null)
			OnObjectSpawned(GameObjectSpawned);
	}

	void OnPoolObjectDespawned(GameObject GameObjectDespawned)
	{
		if (OnObjectDespawned != null)
			OnObjectDespawned(GameObjectDespawned);
	}
}