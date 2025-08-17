using System.Collections.Generic;
using UnityEngine;

namespace Blacksmith.Systems.Pooling
{
	public class ObjectPool : MonoBehaviour
	{
		[SerializeField] private GameObject prefab;
		[SerializeField] private int prewarm = 5;
		[SerializeField] private bool expandable = true;

		private readonly Queue<GameObject> pool = new Queue<GameObject>();

		private void Awake()
		{
			for (int i = 0; i < prewarm; i++)
			{
				Enqueue(CreateNew());
			}
		}

		private GameObject CreateNew()
		{
			var go = Instantiate(prefab, transform);
			go.SetActive(false);
			return go;
		}

		private void Enqueue(GameObject go)
		{
			go.SetActive(false);
			pool.Enqueue(go);
		}

		public GameObject Spawn(Vector3 position, Quaternion rotation)
		{
			GameObject go = pool.Count > 0 ? pool.Dequeue() : (expandable ? CreateNew() : null);
			if (go == null) return null;
			go.transform.SetPositionAndRotation(position, rotation);
			go.SetActive(true);
			return go;
		}

		public void Despawn(GameObject go)
		{
			if (go == null) return;
			Enqueue(go);
		}
	}
}