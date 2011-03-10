namespace Symmetry
{
	using System;
	
	public abstract partial class Option<T> {
		public abstract R Match<R>(Func<T, R> onSome, Func<R> onNone);
		
		public static implicit operator Option<T> (T value) { return Option.Some<T>(value); }
		
		public override string ToString() {
            return this.Match(
                v => string.Format("Some({0})", v.ToString()),
                () => "None");
        }
	}
	
	public static partial class Option {
		private sealed class OptionNone<T> : Option<T> {
			public static Option<T> none = new OptionNone<T>();
			
			private OptionNone() {}
			
			public override R Match<R>(Func<T, R> onSome, Func<R> onNone) {
				return onNone();
			}
		}

		private sealed class OptionSome<T> : Option<T> {
			private readonly T Value;
			
			internal OptionSome(T value) {
				if (value == null)
                	throw new NullReferenceException();
				this.Value = value;
			}
			
			public override R Match<R>(Func<T, R> onSome, Func<R> onNone) {
				return onSome(this.Value);
			}
		}
		
		// Constructors =======================================================
		
		/// <summary>
		/// Returns the None-value for the Option of type T.
		/// </summary>
		public static Option<T> None<T>() { return OptionNone<T>.none; }
		
		/// <summary>
		/// Create an Option from the given value (passing null will throw NullReferenceException).
		/// </summary>
		public static Option<T> Some<T>(T value) { return new OptionSome<T>(value); }
		
		/// <summary>
		/// Create an Option from the given value (null-case will become None).
		/// </summary>
		public static Option<T> Create<T>(T value) where T : class {
			return (value == null) ? None<T>() : Some<T>(value);
		}
		
		/// <summary>
		/// Create an Option from the given value (null-case will become None).
		/// </summary>
		public static Option<T> Create<T>(Nullable<T> value) where T : struct {
			return (value.HasValue) ? Some<T>(value.Value) : None<T>();
		}
		
		// Abstractions =======================================================
		
		public static Option<R> Bind<T, R> (this Option<T> that, Func<T, Option<R>> binder) {
			return that.Match(binder, () => Option.None<R>());
		}

		public static Option<R> Map<T, R> (this Option<T> that, Func<T, R> fn) {
			return that.Match(v => Option.Some<R>(fn(v)), () => Option.None<R>());
		}
		
		public static T Exit<T>(this Option<T> that, Func<T> onNone) {
            return that.Match(x => x, onNone);
        }
		
		public static bool IsSome<T>(this Option<T> that) {
			return that.Match(v => true, () => false);
		}
	}
}