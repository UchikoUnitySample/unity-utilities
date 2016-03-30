using UnityEngine;
using System.Collections;

namespace Repository
{
	public class MonoBehaviourEntity : MonoBehaviour, Entity
	{
		public long ID {
			get { return gameObject.GetInstanceID(); }
		}
	}
}