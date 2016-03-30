using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Repository
{
	public class InMemoryRepository<T,S>
		where T : Entity
		where S : new()
	{
		private static S instance;
		public static S Instance {
			get {
				if (instance == null)
					instance = new S();
				return instance;
			}
		}

		protected Dictionary<long, T> repos;

		public InMemoryRepository()
		{
			repos = new Dictionary<long, T>();
		} 

		public long Add(T m)
		{
			repos[m.ID] = m;
			return m.ID;
		}

		public List<T> GetAll()
		{
			return repos.Values.ToList();
		}

		public T GetById(long id)
		{
			if (!repos.ContainsKey(id))
				return default(T);
			return repos[id];
		}

		public void RemoveById(long id)
		{
			if (!repos.ContainsKey(id))
				return;
			repos.Remove(id);
		}

		public void Remove(T t)
		{
			RemoveById(t.ID);
		}

		public void Clear()
		{
			repos.Clear();
		}

		public int Count {
			get { return repos.Count; }
		}
	}
}
