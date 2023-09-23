/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SecuredSpace.Settings;
using UnityEngine.Events;
using SecuredSpace.ClientControl.Model;
using UTanksClient.Extensions;

namespace SecuredSpace.UnityExtend.MarchingBytes {

	[System.Serializable]
	public class PoolInfo {
		public string poolName;
		public GameObject prefab;
		public int poolSize;
		public bool fixedSize;
		public UnityEvent<Object> afterSpawnAction;
		public IEntityManager poolOwner;
	}

	class Pool {
		private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

		private bool fixedSize;
		private GameObject poolObjectPrefab;
		private int poolSize;
		private string poolName;
		private UnityEvent<Object> afterSpawnAction;
		public IEntityManager poolOwner;

		public Pool(string poolName, GameObject poolObjectPrefab, int initialCount, bool fixedSize, UnityEvent<Object> afterSpawnAction = null, IEntityManager poolOwner = null, bool autoCreate = true) {
			this.poolName = poolName;
			this.poolObjectPrefab = poolObjectPrefab;
			this.poolSize = initialCount;
			this.fixedSize = fixedSize;
			this.afterSpawnAction = afterSpawnAction;
			this.poolOwner = poolOwner;
			//populate the pool
			if (autoCreate)
            {
				for (int index = 0; index < initialCount; index++)
				{
					AddObjectToPool(NewObjectInstance());
				}
			}
		}

		public IEnumerator AwaitAddToPool()
		{
			for (int index = 0; index < this.poolSize; index++)
			{
				AddObjectToPool(NewObjectInstance());
				yield return new WaitForEndOfFrame();
			}
		}

		public IEnumerator AwaitRemoveFromPool()
		{
			foreach(var poolObj in availableObjStack)
            {
				Object.Destroy(poolObj.gameObject);
				yield return new WaitForEndOfFrame();
			}
			availableObjStack.Clear();
		}
		//o(1)
		private void AddObjectToPool(PoolObject po) {
			//add to pool
			po.gameObject.SetActive(false);
			availableObjStack.Push(po);
			po.isPooled = true;
		}
		
		private PoolObject NewObjectInstance() {
			GameObject go = (GameObject)GameObject.Instantiate(poolObjectPrefab, EasyObjectPool.instance.header.transform);
			PoolObject po = go.GetComponent<PoolObject>();
			//UnityEngine.Object.DontDestroyOnLoad(go);
			if(po == null) {
				po = go.AddComponent<PoolObject>();
			}
			var ianchor = po.GetComponent<IAnchor>();
			if (ianchor != null)
			{
				ianchor.DirectRegisterObject();
			}
			//set name
			po.poolName = poolName;
			return po;
		}

		//o(1)
		public GameObject NextAvailableObject(Vector3 position, Quaternion rotation) {
			PoolObject po = null;
			if(availableObjStack.Count > 0) {
				po = availableObjStack.Pop();
			} else if(fixedSize == false) {
				//increment size var, this is for info purpose only
				poolSize++;
				Debug.Log(string.Format("Growing pool {0}. New size: {1}",poolName,poolSize));
				//create new object
				po = NewObjectInstance();
			} else {
				Debug.LogWarning("No object available & cannot grow pool: " + poolName);
			}
			
			GameObject result = null;
			if(po != null) {
				po.isPooled = false;
				result = po.gameObject;
				result.transform.position = position;
				result.transform.rotation = rotation;
				result.SetActive(true);
				if (this.afterSpawnAction != null)
				{
					this.afterSpawnAction.Invoke(result);
				}
				if(this.poolOwner != null)
                {
					ManagerSpace.InstantiatedProcess(result, this.poolOwner);
				}
			}
			
			return result;
		} 
		
		//o(1)
		public void ReturnObjectToPool(PoolObject po) {
			
			if(poolName.Equals(po.poolName)) {
				
				/* we could have used availableObjStack.Contains(po) to check if this object is in pool.
				 * While that would have been more robust, it would have made this method O(n) 
				 */
				if(po.isPooled) {
					Debug.LogWarning(po.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");	
				} else {
					AddObjectToPool(po);
				}
				
			} else {
				Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}",po.poolName,poolName));
			}
		}
	}

	/// <summary>
	/// Easy object pool.
	/// </summary>
	public class EasyObjectPool : MonoBehaviour {

		public static EasyObjectPool instance;
		[Header("Editing Pool Info value at runtime has no effect")]
		public PoolInfo[] poolInfo = new PoolInfo[0];
		public GameObject header;

		//mapping of pool name vs list
		private Dictionary<string, Pool> poolDictionary  = new Dictionary<string, Pool>();
		
		// Use this for initialization

		public void ExternInit()
        {
			instance = this;
			header = new GameObject("PoolsHeader");
			DontDestroyOnLoad(header);
		}

		void Start () {
			//set instance
			instance = this;
			//check for duplicate names
			CheckForDuplicatePoolNames();
			//create pools
			CreatePools();
		}
		
		private void CheckForDuplicatePoolNames() {
			for (int index = 0; index < poolInfo.Length; index++) {
				string poolName = poolInfo[index].poolName;
				if(poolName.Length == 0) {
					Debug.LogError(string.Format("Pool {0} does not have a name!",index));
				}
				for (int internalIndex = index + 1; internalIndex < poolInfo.Length; internalIndex++) {
					if(poolName.Equals(poolInfo[internalIndex].poolName)) {
						Debug.LogError(string.Format("Pool {0} & {1} have the same name. Assign different names.", index, internalIndex));
					}
				}
			}
		}

		private void CreatePools() {
			foreach (PoolInfo currentPoolInfo in poolInfo) {
				
				Pool pool = new Pool(currentPoolInfo.poolName, currentPoolInfo.prefab, 
				                     currentPoolInfo.poolSize, currentPoolInfo.fixedSize, currentPoolInfo.afterSpawnAction, currentPoolInfo.poolOwner);

				
				Debug.Log("Creating pool: " + currentPoolInfo.poolName);
				//add to mapping dict
				poolDictionary[currentPoolInfo.poolName] = pool;
			}
		}

		public void AddPool(PoolInfo poolInfo)
        {
			Pool pool = new Pool(poolInfo.poolName, poolInfo.prefab,
									 poolInfo.poolSize, poolInfo.fixedSize);

			StartCoroutine(pool.AwaitAddToPool());
			Debug.Log("Creating pool: " + poolInfo.poolName);
			//add to mapping dict
			poolDictionary[poolInfo.poolName] = pool;
		}

		public void RemovePool(string poolName)
		{
			StartCoroutine(poolDictionary[poolName].AwaitRemoveFromPool());
			Debug.Log("Removing pool: " + poolName);
            //add to mapping dict
            try
            {
				poolDictionary.Remove(poolName);
			}
            catch { }
		}

		/* Returns an available object from the pool 
		OR 
		null in case the pool does not have any object available & can grow size is false.
		*/
		public GameObject GetObjectFromPool(string poolName, Vector3 position, Quaternion rotation) {
			GameObject result = null;
			
			if(poolDictionary.ContainsKey(poolName)) {
				Pool pool = poolDictionary[poolName];
				result = pool.NextAvailableObject(position,rotation);
				//scenario when no available object is found in pool
				if(result == null) {
					Debug.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
				}
				
			} else {
				Debug.LogError("Invalid pool name specified: " + poolName);
			}
			
			return result;
		}

		public void ReturnObjectToPool(GameObject go) {
			PoolObject po = go.GetComponent<PoolObject>();
			if(po == null) {
				Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
			} else {
				if(poolDictionary.ContainsKey(po.poolName)) {
					Pool pool = poolDictionary[po.poolName];
					pool.ReturnObjectToPool(po);
				} else {
					Debug.LogWarning("No pool available with name: " + po.poolName);
				}
			}
		}
	}
}
