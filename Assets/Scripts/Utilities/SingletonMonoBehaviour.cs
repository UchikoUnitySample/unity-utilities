using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;
	public static T Instance {
		get{
			if(instance == null){
				instance = (T)FindObjectOfType(typeof(T));
				if(instance == null){
					Debug.LogError(typeof(T) + " is nothing !!"); 
				}
			}
			return instance;
		}
	}

	public virtual void OnDestroy(){
		if(instance == this) instance = null;
	}

	protected bool CheckInstance(){
		if(this != Instance){
			Destroy(this);
			return true;
		}else{
			return false;
		}
	}

	public static bool Exists{
		get{ return instance != null; }
	}
}