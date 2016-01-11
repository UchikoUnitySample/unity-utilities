using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using Util = UtilityFunctions;

public static class UtilityFunctions
{
	public static T[] Shuffle<T>(T[] list)
	{
	    System.Random rng = new System.Random();
	    int n = list.Length;
	    while (n > 1) {
	        n--;
	        int k = rng.Next(n + 1);
	        T tmp = list[k];
	        list[k] = list[n];
	        list[n] = tmp;
	    }
	    return list;
	}

	public static List<GameObject> GetTappedObjectsByTag(string tagName)
	{
		var list = new List<GameObject> ();
		Vector2 tapPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Collider2D collition2d = Physics2D.OverlapPoint (tapPoint);
		if (collition2d) {
			RaycastHit2D hitObject = Physics2D.Raycast (tapPoint, -Vector2.up);
			if (hitObject) {
				if (hitObject.collider.gameObject.tag == tagName) {
					list.Add (hitObject.collider.gameObject);
				}
			}
		}
		return list;
	}
}
