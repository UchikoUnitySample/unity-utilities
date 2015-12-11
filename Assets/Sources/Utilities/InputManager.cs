using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public interface InputManagerListener
{
	void OnDown(Vector3 pos);
	void OnFlick (Vector3 pos, Vector3 vec);
	void OnTap (Vector3 pos);
	void OnDoubleTap (Vector3 pos);
	void OnDragBegin (Vector3 pos);
	void OnDrag(Vector3 pos);
	void OnDragEnd(Vector3 pos);
	void OnLongTapBegin(Vector3 startPos);
	void OnLongTap(Vector3 startPos,Vector3 endPos,float duration);
}

public class InputManagerListenerNull : InputManagerListener
{
	public void OnDown(Vector3 pos)
	{
		Debug.Log ("OnDown");
	}
	public void OnFlick (Vector3 pos, Vector3 vec)
	{
		Debug.Log ("OnFlick");
	}
	public void OnTap (Vector3 pos)
	{
		Debug.Log ("OnTap");
	}
	public void OnDoubleTap (Vector3 pos)
	{
		Debug.Log ("OnDoubleTap");
	}
	public void OnDragBegin (Vector3 pos)
	{
		Debug.Log ("OnDragBegin");
	}
	public void OnDrag(Vector3 pos)
	{
		Debug.Log ("OnDrag");
	}
	public void OnDragEnd(Vector3 pos)
	{
		Debug.Log ("OnDragEnd");
	}
	public void OnLongTapBegin(Vector3 startPos)
	{
		Debug.Log ("OnLongTapBegin");
	}
	public void OnLongTap(Vector3 startPos,Vector3 endPos,float duration)
	{
		Debug.Log ("OnLongTap");
	}
}

public class InputManager : MonoBehaviour 
{
	Vector3 startPosition;
	float startTime;

	public InputManagerListener listener = new InputManagerListenerNull();
	public float flickThreshold = 0.1f;
	public float longTapThreshold = 0.5f;
	public float doubleTapThreshold = 0.2f;
	public float dragEventSpan = 0.2f;
	public float longTapDistanceThreshold = 10.0f;

	float lastUpdateTime;
	float lastTapTime;

	bool isDragging = false;
	bool isLongTapping = false;

	Vector3 CurrentMousePosInWorld {
		get {
			return Camera.main.ScreenToWorldPoint (Input.mousePosition);
		}
	}

	Vector3 StartMousePosInWorld {
		get {
			return Camera.main.ScreenToWorldPoint (startPosition);
		}
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			OnDown (CurrentMousePosInWorld);
			startPosition = Input.mousePosition;
			startTime = Time.time;
			lastUpdateTime = Time.time;
			isDragging = false;
			isLongTapping = false;
		}  else if (Input.GetMouseButtonUp (0)) {
			if(isDragging){
				OnDragEnd (CurrentMousePosInWorld);
			} else if (isLongTapping) {
				OnLongTap(
					StartMousePosInWorld,
					CurrentMousePosInWorld,
					Time.time - startTime
				);
			} else if (Time.time - startTime < flickThreshold &&
				(startPosition - Input.mousePosition).magnitude * 10 > Screen.width)
			{
				var end = CurrentMousePosInWorld;
				var start = StartMousePosInWorld;
				OnFlick (start, end - start);
			} else {
				if(Time.time - lastTapTime < doubleTapThreshold) {
					lastTapTime = Time.time;
					OnDoubleTap (CurrentMousePosInWorld);
				} else {
					lastTapTime = Time.time;
					OnTap (CurrentMousePosInWorld);
				}
			}
		} else if (Input.GetMouseButton (0)) {
			if (!isDragging && !isLongTapping && Time.time - startTime > longTapThreshold &&
				(startPosition - Input.mousePosition).magnitude < longTapDistanceThreshold){
				isLongTapping = true;
				OnLongTapBegin(StartMousePosInWorld);
			} else if(
				!isLongTapping 
				&& (isDragging || (Time.time - lastUpdateTime) > dragEventSpan)
				&& (startPosition - Input.mousePosition).magnitude >= longTapDistanceThreshold
			) {
				lastUpdateTime = Time.time;

				if(isDragging) {
					OnDrag(CurrentMousePosInWorld);
				} else {
					OnDragBegin(CurrentMousePosInWorld);
				}

				isDragging = true;
			} 
		}
	}

	void OnDown (Vector3 pos)
	{
		this.listener.OnDown (pos);
	}

	void OnFlick(Vector3 pos,Vector3 vec)
	{
		this.listener.OnFlick (pos, vec);
	}

	void OnTap(Vector3 pos)
	{
		this.listener.OnTap (pos);
	}

	void OnDoubleTap(Vector3 pos)
	{
		this.listener.OnDoubleTap (pos);
	}

	void OnDragBegin(Vector3 pos)
	{
		this.listener.OnDragBegin (pos);
	}

	void OnDrag(Vector3 pos)
	{
		this.listener.OnDrag (pos);
	}

	void OnDragEnd(Vector3 pos)
	{
		this.listener.OnDragEnd (pos);
	}

	void OnLongTapBegin(Vector3 startPos)
	{
		this.listener.OnLongTapBegin (startPos);
	}

	void OnLongTap(Vector3 startPos,Vector3 endPos,float duration)
	{
		this.listener.OnLongTap (startPos, endPos, duration);
	}
}