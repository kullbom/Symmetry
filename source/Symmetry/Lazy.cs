// Copyright 2011 Johan Kullbom (see the file license.txt)

namespace Symmetry
{
	using System;
	
	/// <summary>
	/// A simple (thread safe) cached value cell. 
	/// </summary>
	public sealed partial class Lazy<T> {
		private readonly Func<T> constructor;
		private Option<T> value = Option.None;
		
		internal Lazy(Func<T> constructor) {
			this.constructor = constructor;
		}
		
		public T Force() {
			lock(this.value)
			{
				return this.value.Match(
					v => v,
					() =>
					{
						var v = constructor();
						this.value = v;
						return v;
					});
			}
		}		
		
		public override string ToString() {
            return this.value.Match(
                v => string.Format("Forced({0})", v.ToString()),
                () => "Unforced");
        }
	}
	
	public static partial class Lazy {
		// Constructor ========================================================
		public static Lazy<T> Create<T>(Func<T> constructor) { 
			return new Lazy<T>(constructor); 
		}
		
		// Abstractions ======================================================= 
		public static Lazy<R> Map<T, R>(this Lazy<T> that, Func<T, R> fn) {
			return new Lazy<R>(() => fn(that.Force()));
		}
	}
}