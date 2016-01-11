using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimatorUtility : MonoBehaviourUtility
{
	Animator _animator;

	public Animator Animator {
		get {
			if (_animator == null)
				_animator = GetComponent<Animator> ();
			return _animator;
		}
	}

	public bool IsFinishAnimation {
		get;
		private set;
	}

	public int FinishAnimationCounter {
		get;
		private set;
	}

	public void SetAnimation(int state, float speed = -1.0f)
	{
		IsFinishAnimation = false;
		FinishAnimationCounter = 0;
		if (speed < 0f) {
			Animator.speed = speed;
		}
		Animator.SetInteger("state", state);
	}

	public int GetAnimation()
	{
		return Animator.GetInteger ("state");
	}

	public void OnFinishAnimation()
	{
		FinishAnimationCounter++;
		IsFinishAnimation = true;
	}
}
