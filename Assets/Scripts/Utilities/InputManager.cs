using UnityEngine;
using System.Collections.Generic;

public class InputPositionInfo
{
	public Vector3 Screen {
		get; private set;
	}

	public Vector3 World {
		get; private set;
	}

	public InputPositionInfo(Vector3 screen, Vector3 world)
	{
		Screen = screen;
		World = world;
	}

	public Vector3 GetCurrentScreenToWorld()
	{
		return Camera.main.ScreenToWorldPoint(Screen);
	}
}

public interface IInputManagerListener
{
	void OnDown(InputPositionInfo pos);
	void OnFlick (InputPositionInfo startPos, InputPositionInfo endPos);
	void OnTap (InputPositionInfo pos);
	void OnDoubleTap (InputPositionInfo pos);
	void OnDragBegin (InputPositionInfo pos);
	void OnDrag(InputPositionInfo pos);
	void OnDragEnd(InputPositionInfo pos);
	void OnLongTapBegin(InputPositionInfo startPos);
	void OnLongTap(InputPositionInfo pos);
	void OnLongTapEnd(InputPositionInfo startPos,InputPositionInfo endPos,float duration);
}

public class NullInputManager : IInputManagerListener
{
	public void OnDown(InputPositionInfo pos){}
	public void OnFlick (InputPositionInfo startPos, InputPositionInfo endPos){}
	public void OnTap (InputPositionInfo pos){}
	public void OnDoubleTap (InputPositionInfo pos){}
	public void OnDragBegin (InputPositionInfo pos){}
	public void OnDrag(InputPositionInfo pos){}
	public void OnDragEnd(InputPositionInfo pos){}
	public void OnLongTapBegin(InputPositionInfo startPos){}
	public void OnLongTap(InputPositionInfo startPos){}
	public void OnLongTapEnd(InputPositionInfo startPos,InputPositionInfo endPos,float duration){}

	public static NullInputManager Instance {
		get { return new NullInputManager(); }
	}
}

public class InputManager : SingletonMonoBehaviour<InputManager>
{
	Vector3 startPosition;
	float startTime;

	IInputManagerListener listener = NullInputManager.Instance;

	[SerializeField]
	float flickThreshold = 0.1f;

	[SerializeField]
	float longTapThreshold = 0.5f;

	[SerializeField]
	float doubleTapThreshold = 0.2f;

	[SerializeField]
	float dragEventSpan = 0.2f;

	[SerializeField]
	float longTapDistanceThreshold = 10.0f;

	float lastUpdateTime;
	float lastTapTime;

	bool isDragging = false;
	bool isLongTapping = false;

	InputPositionInfo CurrentMousePositionInfo {
		get {
			return new InputPositionInfo(
				Input.mousePosition,
				Camera.main.ScreenToWorldPoint (Input.mousePosition)
			);
		}
	}

	InputPositionInfo StartMousePositionInfo {
		get {
			return new InputPositionInfo(
				startPosition,
				Camera.main.ScreenToWorldPoint (startPosition)
			);
		}
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			OnDown (CurrentMousePositionInfo);
			startPosition = Input.mousePosition;
			startTime = Time.time;
			lastUpdateTime = Time.time;
			isDragging = false;
			isLongTapping = false;
		}  else if (Input.GetMouseButtonUp (0)) {
			if(isDragging){
				OnDragEnd (CurrentMousePositionInfo);
			} else if (isLongTapping) {
				OnLongTapEnd(
					StartMousePositionInfo,
					CurrentMousePositionInfo,
					Time.time - startTime
				);
			} else if (Time.time - startTime < flickThreshold &&
				(startPosition - Input.mousePosition).magnitude * 10 > Screen.width)
			{
				OnFlick (StartMousePositionInfo, CurrentMousePositionInfo);
			} else {
				if(Time.time - lastTapTime < doubleTapThreshold) {
					lastTapTime = Time.time;
					OnDoubleTap (CurrentMousePositionInfo);
				} else {
					lastTapTime = Time.time;
					OnTap (CurrentMousePositionInfo);
				}
			}
		} else if (Input.GetMouseButton (0)) {
			if (!isDragging && !isLongTapping && Time.time - startTime > longTapThreshold &&
				(startPosition - Input.mousePosition).magnitude < longTapDistanceThreshold){
				isLongTapping = true;
				OnLongTapBegin(StartMousePositionInfo);
			} else if (isLongTapping) {
				OnLongTap(CurrentMousePositionInfo);
			} else if(
				(isDragging || (Time.time - lastUpdateTime) > dragEventSpan)
				&& (startPosition - Input.mousePosition).magnitude >= longTapDistanceThreshold
			) {
				lastUpdateTime = Time.time;

				if(isDragging) {
					OnDrag(CurrentMousePositionInfo);
				} else {
					OnDragBegin(CurrentMousePositionInfo);
				}

				isDragging = true;
			}
		}
	}

	public void AttachListener(IInputManagerListener l)
	{
		listener = l;
	}

	public void DetachListener()
	{
		listener = NullInputManager.Instance;
	}

	void OnDown (InputPositionInfo pos)
	{
		listener.OnDown(pos);
	}

	void OnFlick(InputPositionInfo start, InputPositionInfo end)
	{
		listener.OnFlick(start, end);
	}

	void OnTap(InputPositionInfo pos)
	{
		listener.OnTap(pos);
	}

	void OnDoubleTap(InputPositionInfo pos)
	{
		listener.OnDoubleTap(pos);
	}

	void OnDragBegin(InputPositionInfo pos)
	{
		listener.OnDragBegin(pos);
	}

	void OnDrag(InputPositionInfo pos)
	{
		listener.OnDrag(pos);
	}

	void OnDragEnd(InputPositionInfo pos)
	{
		listener.OnDragEnd(pos);
	}

	void OnLongTapBegin(InputPositionInfo startPos)
	{
		listener.OnLongTapBegin(startPos);
	}

	void OnLongTap(InputPositionInfo pos)
	{
		listener.OnLongTap(pos);
	}

	void OnLongTapEnd(InputPositionInfo startPos,InputPositionInfo endPos,float duration)
	{
		listener.OnLongTapEnd(startPos, endPos, duration);
	}
}
