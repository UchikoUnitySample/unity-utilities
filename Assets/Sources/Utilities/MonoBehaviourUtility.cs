using UnityEngine;
using System.Collections;

public class MonoBehaviourUtility : MonoBehaviour
{
	public float X {
		set {
			Vector3 pos = transform.position;
			pos.x = value;
			transform.position = pos;
		}
		get {
			return transform.position.x;
		}
	}

	public float Y {
		set {
			Vector3 pos = transform.position;
			pos.y = value;
			transform.position = pos;
		}
		get {
			return transform.position.y;
		}
	}

	public float Z {
		set {
			Vector3 pos = transform.position;
			pos.z = value;
			transform.position = pos;
		}
		get {
			return transform.position.z;
		}
	}

	#region Position
	public Vector2 Position {
		get {
			return transform.position;
		}
	}

	public void AddPosition(float dx, float dy)
	{
		AddPosition (dx, dy, 0);
	}

	public void AddPosition(float dx, float dy, float dz)
	{
		X += dx;
		Y += dy;
		Z += dz;
	}

	public void SetPosition(float x, float y, float z)
	{
		Vector3 pos = transform.position;
		pos.Set (x, y, z);
		transform.position = pos;
	}

	public void SetPosition(float x, float y)
	{
		Vector3 pos = transform.position;
		SetPosition(x, y, pos.z);
	}

	public void SetPosition(Vector3 pos)
	{
		SetPosition (pos.x, pos.y, pos.z);
	}

	public void SetPosition(MonoBehaviour mono)
	{
		var pos = mono.transform.position;
		SetPosition (pos.x, pos.y, pos.z);
	}
	#endregion


	#region Scale
	public float ScaleX {
		set {
			Vector3 scale = transform.localScale;
			scale.x = value;
			transform.localScale = scale;
		}
		get {
			return transform.localScale.x;
		}
	}

	public float ScaleY {
		set {
			Vector3 scale = transform.localScale;
			scale.y = value;
			transform.localScale = scale;
		}
		get {
			return transform.localScale.y;
		}
	}

	public float ScaleZ {
		set {
			Vector3 scale = transform.localScale;
			scale.z = value;
			transform.localScale = scale;
		}
		get {
			return transform.localScale.z;
		}
	}

	public void SetScale(float x, float y, float z)
	{
		Vector3 scale = transform.localScale;
		scale.Set (x, y, z);
		transform.localScale = scale;
	}

	public void SetScale(float x, float y)
	{
		SetScale (x, y, (x + y) / 2);
	}

	public void AddScale(float d)
	{
		Vector3 scale = transform.localScale;
		scale.x += d;
		scale.y += d;
		scale.z += d;
		transform.localScale = scale;
	}

	public void MultiplyScale(float d)
	{
		transform.localScale *= d;
	}
	#endregion

	public void DestroyObject()
	{
		Destroy (gameObject);
	}
}
