// Copyright 2011 Johan Kullbom (see the file LICENSE)

namespace Symmetry
{
	using System;
	
	public abstract class Union<T1, T2> {
		public abstract R Match<R>(Func<T1, R> on1, Func<T2, R> on2); 
		
		public static implicit operator Union<T1, T2>(T1 value) { return Union.Case1<T1, T2>(value); }
        
		public static implicit operator Union<T1, T2>(T2 value) { return Union.Case2<T1, T2>(value); }
		
		public override string ToString() {
            return this.Match(
                v1 => string.Format("Case1({0})", v1.ToString()),
				v2 => string.Format("Case2({0})", v2.ToString()));
        }
	}

	public abstract class Union<T1, T2, T3> {
		public abstract R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3); 
		
		public static implicit operator Union<T1, T2, T3>(T1 value) { return Union.Case1<T1, T2, T3>(value); }
        
		public static implicit operator Union<T1, T2, T3>(T2 value) { return Union.Case2<T1, T2, T3>(value); }
		
		public static implicit operator Union<T1, T2, T3>(T3 value) { return Union.Case3<T1, T2, T3>(value); }
		
		public override string ToString() {
            return this.Match(
                v1 => string.Format("Case1({0})", v1.ToString()),
                v2 => string.Format("Case2({0})", v2.ToString()),
				v3 => string.Format("Case3({0})", v3.ToString()));
        }
	}
	
	public abstract class Union<T1, T2, T3, T4> {
		public abstract R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3, Func<T4, R> on4); 
		
		public static implicit operator Union<T1, T2, T3, T4>(T1 value) { return Union.Case1<T1, T2, T3, T4>(value); }
		
		public static implicit operator Union<T1, T2, T3, T4>(T2 value) { return Union.Case2<T1, T2, T3, T4>(value); }
	
		public static implicit operator Union<T1, T2, T3, T4>(T3 value) { return Union.Case3<T1, T2, T3, T4>(value); }

		public static implicit operator Union<T1, T2, T3, T4>(T4 value) { return Union.Case4<T1, T2, T3, T4>(value); }

		public override string ToString() {
            return this.Match(
                v1 => string.Format("Case1({0})", v1.ToString()),
                v2 => string.Format("Case2({0})", v2.ToString()),
				v3 => string.Format("Case3({0})", v3.ToString()),
				v4 => string.Format("Case4({0})", v4.ToString()));
        }
	}
	
	public static class Union {
		// Union<T1, T2>
		private sealed class iCase1<T1, T2> : Union<T1, T2> {
			private readonly T1 Value;
			
			internal iCase1(T1 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2) {
				return on1(this.Value);
			}
		}

		private sealed class iCase2<T1, T2> : Union<T1, T2> {
			private readonly T2 Value;
			
			internal iCase2(T2 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2) { return on2(this.Value); }
		}
		
		// Union<T1, T2, T3>
		private sealed class iCase1<T1, T2, T3> : Union<T1, T2, T3> {
			private readonly T1 Value;
			
			internal iCase1(T1 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3) {
				return on1(this.Value);
			}
		}

		private sealed class iCase2<T1, T2, T3> : Union<T1, T2, T3> {
			private readonly T2 Value;
			
			internal iCase2(T2 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3) {
				return on2(this.Value);
			}
		}
		
		private sealed class iCase3<T1, T2, T3> : Union<T1, T2, T3> {
			private readonly T3 Value;
			
			internal iCase3(T3 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3) {
				return on3(this.Value);
			}
		}
		
		// Union<T1, T2, T3, T4>
		private sealed class iCase1<T1, T2, T3, T4> : Union<T1, T2, T3, T4> {
			private readonly T1 Value;
			
			internal iCase1(T1 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3, Func<T4, R> on4) {
				return on1(this.Value);
			}
		}

		private sealed class iCase2<T1, T2, T3, T4> : Union<T1, T2, T3, T4> {
			private readonly T2 Value;
			
			internal iCase2(T2 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3, Func<T4, R> on4) {
				return on2(this.Value);
			}
		}
		
		private sealed class iCase3<T1, T2, T3, T4> : Union<T1, T2, T3, T4> {
			private readonly T3 Value;
			
			internal iCase3(T3 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3, Func<T4, R> on4) {
				return on3(this.Value);
			}
		}

		private sealed class iCase4<T1, T2, T3, T4> : Union<T1, T2, T3, T4> {
			private readonly T4 Value;
			
			internal iCase4(T4 value) { this.Value = value; }
			
			public override R Match<R>(Func<T1, R> on1, Func<T2, R> on2, Func<T3, R> on3, Func<T4, R> on4) {
				return on4(this.Value);
			}
		}

		// Constructors =======================================================
		
		public static Union<T1, T2> Case1<T1, T2>(T1 value) { 
			return new iCase1<T1, T2>(value); 
		}
		
		public static Union<T1, T2> Case2<T1, T2>(T2 value) { 
			return new iCase2<T1, T2>(value); 
		}

				
		public static Union<T1, T2, T3> Case1<T1, T2, T3>(T1 value) { 
			return new iCase1<T1, T2, T3>(value); 
		}
		
		public static Union<T1, T2, T3> Case2<T1, T2, T3>(T2 value) { 
			return new iCase2<T1, T2, T3>(value); 
		}

		public static Union<T1, T2, T3> Case3<T1, T2, T3>(T3 value) { 
			return new iCase3<T1, T2, T3>(value); 
		}

						
		public static Union<T1, T2, T3, T4> Case1<T1, T2, T3, T4>(T1 value) { 
			return new iCase1<T1, T2, T3, T4>(value); 
		}
		
		public static Union<T1, T2, T3, T4> Case2<T1, T2, T3, T4>(T2 value) { 
			return new iCase2<T1, T2, T3, T4>(value); 
		}

		public static Union<T1, T2, T3, T4> Case3<T1, T2, T3, T4>(T3 value) { 
			return new iCase3<T1, T2, T3, T4>(value); 
		}

		public static Union<T1, T2, T3, T4> Case4<T1, T2, T3, T4>(T4 value) { 
			return new iCase4<T1, T2, T3, T4>(value); 
		}

		
		// Abstractions =======================================================
		
	}
}