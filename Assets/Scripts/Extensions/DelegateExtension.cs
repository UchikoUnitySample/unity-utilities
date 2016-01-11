using System;

public static class DelegateExtensions
{
	public static void SafeCall (this Action action)
	{
		if (action != null) {
			action ();
		}
	}

	public static void SafeCall<T> (this Action<T> action, T arg)
	{
		if (action != null) {
			action (arg);
		}
	}

	public static void SafeCall<T1, T2> (this Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		if (action != null) {
			action (arg1, arg2);
		}
	}

	public static void SafeCall<T1, T2, T3> (this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		if (action != null) {
			action (arg1, arg2, arg3);
		}
	}

	public static TResult SafeCall<TResult> (this Func<TResult> func, TResult result = default(TResult))
	{
		return func != null ? func () : result;
	}

	public static TResult SafeCall<T, TResult> (this Func<T, TResult> func, T arg, TResult result = default(TResult))
	{
		return func != null ? func (arg) : result;
	}

	public static TResult SafeCall<T1, T2, TResult> (this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, TResult result = default(TResult))
	{
		return func != null ? func (arg1, arg2) : result;
	}

	public static TResult SafeCall<T1, T2, T3, TResult> (this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, TResult result = default(TResult))
	{
		return func != null ? func (arg1, arg2, arg3) : result;
	}
}
