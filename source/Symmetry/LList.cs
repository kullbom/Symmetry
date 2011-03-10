namespace Symmetry
{
	using System;
	using System.Collections.Generic;
	
	/// <summary>
	/// LList - A simple immutable singly linked list. Implementes IEnumerable<T>.
	/// </summary>
	public abstract partial class LList<T> {
		public abstract R Match<R>(Func<T, LList<T>, R> onSome, Func<R> onEmpty); 
		
		public bool IsEmpty {
			get { return this.Match((hd, tl) => false, () => true); } 
		}
		
		public override string ToString() {
            return this.Match(
                (hd, tl) => string.Format("Cell({0})", hd),
                () => "Empty");
        }
	}
	
	public static class LList {
		private sealed class LListEmpty<T> : LList<T> {
			internal static LList<T> empty = new LListEmpty<T>();
			
			private LListEmpty() { }
			
			public override R Match<R>(Func<T, LList<T>, R> onSome, Func<R> onEmpty) {
				return onEmpty();
			}
		}

		private sealed class LListCell<T> : LList<T> {
			internal readonly T head; // internal to enable optimizations
			internal readonly LList<T> tail;
			
			internal LListCell(T head, LList<T> tail) {
				this.head = head;
				this.tail = tail;
			}
			
			public override R Match<R>(Func<T, LList<T>, R> onCell, Func<R> onEmpty) {
				return onCell(this.head, this.tail);
			}
		}
		
		// Constructors =======================================================

		public static LList<T> Empty<T>() { return LListEmpty<T>.empty; }
		
		public static LList<T> Cons<T>(T head, LList<T> tail) { return new LListCell<T>(head, tail); }

        public static LList<T> Create<T>(params T[] elements) {
            var result = Empty<T>();
            for(var i = (elements.Length - 1); i >= 0; i--)
                result = Cons(elements[i], result);
            return result;
        }

		
		public static LList<T> Create<T>(IEnumerable<T> elements) {
            return CreateFromEnumerator(elements.GetEnumerator());
        }
		
		public static LList<T> CreateFromEnumerator<T>(System.Collections.Generic.IEnumerator<T> e)
		{
			return (e.MoveNext())
				? Cons(e.Current, CreateFromEnumerator<T>(e))
				: Empty<T>();
		}
	
		
		// Abtractions ========================================================

		public static TAcc FoldL<T, TAcc>(this LList<T> that, TAcc seed, Func<TAcc, T, TAcc> aggregator) {
			return OptimizedFoldL(aggregator, seed, that);
		}
		
		public static LList<R> Map<T, R>(this LList<T> that, Func<T, R> fn) {
            return OptimizedFoldR((acc, x) => Cons(fn(x), acc), Empty<R>(), that);
        }

        public static LList<T> Filter<T>(this LList<T> that, Func<T, bool> predicate) {
            return OptimizedFoldR(
				(acc, x) => predicate(x) ? Cons(x, acc) : acc,
				Empty<T>(), 
				that);
        }
		
		public static bool IsEqual<T>(this LList<T> that, LList<T> other) where T : IEquatable<T>
		{
			return that.Match(
				(hd0, tl0) => other.Match(
						(hd1, tl1) => hd0.Equals(hd1) && IsEqual(tl0, tl1),
					    ()         => false),
				()         => other.Match(
						(hd1, tl1) => false,
					    ()         => true));
		}
		
		public static bool IsEqual<T>(this LList<T> that, LList<T> other, Func<T, T, bool> equality) 
		{
			return that.Match(
				(hd0, tl0) => other.Match(
						(hd1, tl1) => equality(hd0, hd1) && IsEqual(tl0, tl1, equality),
					    ()         => false),
				()         => other.Match(
						(hd1, tl1) => false,
					    ()         => true));
		}
		
		public static int SumBy<T>(this LList<T> that, Func<T, int> selector) {
			return OptimizedFoldL((sum, x) => sum + selector(x), 0, that);
		}
		
		public static long SumBy<T>(this LList<T> that, Func<T, long> selector) {
			return OptimizedFoldL((sum, x) => sum + selector(x), 0L, that);
		}

		public static int Sum(this LList<int> that) {
			return OptimizedFoldL((sum, x) => sum + x, 0, that);
		}
		
		public static long Sum(this LList<long> that) {
			return OptimizedFoldL((sum, x) => sum + x, 0L, that);
		}

		
		private static TAcc OptimizedFoldL<T, TAcc>(Func<TAcc, T, TAcc> aggregator, TAcc seed, LList<T> list) {
			var cell = list as LListCell<T>;
            while (cell != null) {
                seed = aggregator(seed, cell.head);
                cell = cell.tail as LListCell<T>;
            }
			return seed;
		}
		
		private static TAcc OptimizedFoldR<T, TAcc>(Func<TAcc, T, TAcc> aggregator, TAcc seed, LList<T> list) {
			// Not yet optimized in any way... 
			return list.Match<TAcc>(
				(hd, tl) => OptimizedFoldR(aggregator, aggregator(seed, hd), tl),
				() => seed);
		}
	}
}
