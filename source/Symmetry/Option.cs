// Copyright 2011 Johan Kullbom (see the file license.txt)

namespace Symmetry
{
	using System;
	
	/// <summary>
	/// The option type is used when an actual value might not exist. 
	/// An option has an underlying type and can hold a value of that type (Some), or it might not have a value (None).
	/// </summary>
	public abstract partial class Option<T> {
		public abstract R Match<R>(Func<T, R> onSome, Func<R> onNone);
		
		/// <summary>
		/// Returns the None-value for the Option of type T.
		/// </summary>
		public static Option<T> None() { return Option.OptionNone<T>.none; }
		
		public static Option<T> Some(T value) { return Option.Some<T>(value); }

		
		public static implicit operator Option<T> (T value) { return Some(value); }
		
		public static implicit operator Option<T> (Option.UntypedNone value) { return None(); }
		
		public override string ToString() {
            return this.Match(
                v => string.Format("Some({0})", v.ToString()),
                () => "None");
        }
	}
	
	public static partial class Option {
		
		public sealed class UntypedNone {
			internal static UntypedNone untypedNone = new UntypedNone();
			
			private UntypedNone() {}
		}
		
		public static UntypedNone None { get { return UntypedNone.untypedNone; } } 
		
		
		internal sealed class OptionNone<T> : Option<T> {
			public static Option<T> none = new OptionNone<T>();
			
			private OptionNone() {}
			
			public override R Match<R>(Func<T, R> onSome, Func<R> onNone) {
				return onNone();
			}
		}
		
		internal abstract class OptionSome<T> : Option<T> {
			public abstract T Value();
			
			public override R Match<R>(Func<T, R> onSome, Func<R> onNone) {
				return onSome(this.Value());
			}
		}
		
		internal sealed class OptionValueSome<T> : OptionSome<T> {
			private readonly T value;
			
			internal OptionValueSome(T value) {
				if (value == null)
                	throw new NullReferenceException();
				this.value = value;
			}
			
			public override T Value() { return this.value; }
		}
		
		// Constructors =======================================================
		
		/// <summary>
		/// Create an Option from the given value (passing null will throw NullReferenceException).
		/// </summary>
		public static Option<T> Some<T>(T value) { return new OptionValueSome<T>(value); }
		
		/// <summary>
		/// Create an Option from the given value (null-case will become None).
		/// </summary>
		public static Option<T> Create<T>(T value) where T : class {
			return (value == null) ? Option<T>.None() : Some<T>(value);
		}
		
		/// <summary>
		/// Create an Option from the given value (null-case will become None).
		/// </summary>
		public static Option<T> Create<T>(Nullable<T> value) where T : struct {
			return (value.HasValue) ? Some<T>(value.Value) : Option<T>.None();
		}
		
		// Abstractions =======================================================
		
		public static Option<R> Bind<T, R> (this Option<T> that, Func<T, Option<R>> binder) {
			return that.Match(binder, () => Option<R>.None());
		}

		public static Option<R> Map<T, R> (this Option<T> that, Func<T, R> fn) {
			return that.Match(v => Option.Some<R>(fn(v)), () => Option<R>.None());
		}
		
		public static T Escape<T>(this Option<T> that, Func<T> onNone) {
            return that.Match(x => x, onNone);
        }
		
		public static bool IsSome<T>(this Option<T> that) {
			return that.Match(v => true, () => false);
		}
		
		// Lazy
		//public static R LazyMap<T, R>(this Option<T> that, Func<T, R> fn) {
		//	return that.Match(v => Option.Some<R>(fn(v)), () => Option<R>.None());
		//}
	}
}