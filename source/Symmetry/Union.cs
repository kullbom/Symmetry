// Copyright 2011 Johan Kullbom (see the file LICENSE)

namespace Symmetry
{
	using System;
	
	public abstract class Union<TLeft, TRight> {
		public abstract R Match<R>(Func<TLeft, R> onLeft, Func<TRight, R> onRight); 
		
		public static implicit operator Union<TLeft, TRight>(TLeft value) { 
			return Union.CreateL<TLeft, TRight>(value); 
		}
        
		public static implicit operator Union<TLeft, TRight>(TRight value) { 
			return Union.CreateR<TLeft, TRight>(value); 
		}
		
		public override string ToString() {
            return this.Match(
                left  => string.Format("Left({0})", left.ToString()),
                right => string.Format("Right({0})", right.ToString()));
        }
	}
	
	public static class Union {
		private sealed class iLeft<TLeft, TRight> : Union<TLeft, TRight> {
			private readonly TLeft Value;
			
			internal iLeft(TLeft value) {
				this.Value = value;
			}
			
			public override R Match<R>(Func<TLeft, R> onLeft, Func<TRight, R> onRight) {
				return onLeft(this.Value);
			}
		}

		private sealed class iRight<TLeft, TRight> : Union<TLeft, TRight> {
			private readonly TRight Value;
			
			internal iRight(TRight value) {
				this.Value = value;
			}
			
			public override R Match<R>(Func<TLeft, R> onLeft, Func<TRight, R> onRight) {
				return onRight(this.Value);
			}
		}
		
		// Constructors =======================================================
		
		public static Union<TLeft, TRight> CreateL<TLeft, TRight>(TLeft value) { 
			return new iLeft<TLeft, TRight>(value); 
		}
		
		public static Union<TLeft, TRight> CreateR<TLeft, TRight>(TRight value) { 
			return new iRight<TLeft, TRight>(value); 
		}

		
		// Abstractions =======================================================
		
		public static Option<TLeft> Left<TLeft, TRight>(this Union<TLeft, TRight> that) {
			return that.Match(left => Option.Some(left), right => Option<TLeft>.None());  
		}
		
		public static Option<TRight> Right<TLeft, TRight>(this Union<TLeft, TRight> that) {
			return that.Match(left => Option<TRight>.None(), right => Option.Some(right));  
		}
	}
}