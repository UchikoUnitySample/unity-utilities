using System;
using System.Collections.Generic;

namespace Hatool
{
    public abstract class Option<T>
    {
        static Option<T> none = new None<T>();

        public static Option<T> Some(T v)
        {
            return new Some<T> (v);
        }

        public static Option<T> None()
        {
            return none;
        }

        public abstract bool IsDefined { get; }
        public bool IsEmpty { get { return !IsDefined; } }
        public T Value { get { return Get (); } }

        public abstract T Get();
        public abstract void Foreach(Action<T> func);
        public abstract Option<T2> Map<T2>(MapFunc<T,T2> func);
        public abstract Option<T2> FlatMap<T2>(MapFunc<T,Option<T2>> func);

        public abstract T GetOrElse(Func0<T> func);
        public abstract T GetOrElse(T dflt);

        public abstract Option<T> OrElse(Option<T> opt);
        public abstract Option<T> OrElse(Func0<Option<T>> opt);
        public abstract List<T> ToList();
    }

    public class Some<T> : Option<T>
    {
        T value;

        public Some(T v)
        {
            this.value = v;
        }

        public override bool IsDefined { get { return true; } }

        public override T Get()
        {
            return value;
        }

        public override void Foreach(Action<T> func)
        {
            func(value);
        }

        public override Option<T2> Map<T2> (MapFunc<T,T2> func)
        {
            return new Some<T2> (func(value));
        }

        public override Option<T2> FlatMap<T2> (MapFunc<T,Option<T2>> func)
        {
            var v = func (value);
            if (v.IsDefined)
                return v;
            else
                return None<T2>.None ();
        }

        public override T GetOrElse (Func0<T> func)
        {
            return value;
        }

        public override T GetOrElse (T dflt)
        {
            return value;
        }

        public override Option<T> OrElse (Option<T> opt)
        {
            return this;
        }

        public override Option<T> OrElse (Func0<Option<T>> opt)
        {
            return this;
        }

        public override List<T> ToList ()
        {
            return new List<T> (){ value };
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return this.value == null;
            if (this.value == null) 
                return false;

            var op = obj as Option<T>;
            if (op != null)
                return op.IsDefined && this.value.Equals (op.Get ());
            else
                return obj.Equals (value);
        }

        public override int GetHashCode()
        {
            if (value == null)
                return 1;
            else
                return (value.GetHashCode () + 1);
        }

        public override string ToString ()
        {
            return string.Format ("Some({0})", this.value);
        }
    }

    class None<T> : Option<T>
    {
        public override bool IsDefined { get { return false; } }

        public override T Get ()
        {
            throw new NoneToGetException ();
        }

        public override void Foreach(Action<T> func)
        {
        }

        public override Option<T2> Map<T2> (MapFunc<T,T2> func)
        {
            return None<T2>.None ();
        }

        public override Option<T2> FlatMap<T2> (MapFunc<T, Option<T2>> func)
        {
            return None<T2>.None ();
        }

        public override T GetOrElse (Func0<T> func)
        {
            return func ();
        }

        public override T GetOrElse (T dflt)
        {
            return dflt;
        }

        public override Option<T> OrElse (Option<T> opt)
        {
            return opt;
        }

        public override Option<T> OrElse (Func0<Option<T>> opt)
        {
            return opt ();
        }

        public override List<T> ToList ()
        {
            return new List<T> ();
        }

        public override bool Equals (object obj)
        {
            var op = obj as Option<T>;
            return op != null && op.IsEmpty;
        }

        public override string ToString ()
        {
            return "None";
        }

        public override int GetHashCode ()
        {
            return 0;
        }
    }

    public class NoneToGetException : Exception
    {
        public NoneToGetException() : base("Try to get None object")
        {
        }
    }

    public delegate V Func0<V>();
    public delegate To MapFunc<From, To>(From v); 
}