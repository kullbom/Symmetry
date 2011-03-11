// Copyright 2011 Johan Kullbom (see the file license.txt)

namespace Symmetry
{
	using System;
	using System.Collections.Generic;
	
	/// <summary>
	/// Lst - A simple immutable singly linked list. Implementes IEnumerable<T>.
	/// </summary>
	public abstract partial class Lst<T> {
		public abstract R Match<R>(Func<T, Lst<T>, R> onSome, Func<R> onEmpty); 
		
		public bool IsEmpty {
			get { return this.Match((hd, tl) => false, () => true); } 
		}
		
		public override string ToString() {
            return this.Match(
                (hd, tl) => string.Format("Cell({0})", hd),
                () => "Empty");
        }
	}
	
	public static class Lst {
		private sealed class LstEmpty<T> : Lst<T> {
			internal static Lst<T> empty = new LstEmpty<T>();
			
			private LstEmpty() { }
			
			public override R Match<R>(Func<T, Lst<T>, R> onSome, Func<R> onEmpty) {
				return onEmpty();
			}
		}
		
		private abstract class LstCell<T> : Lst<T> { 
			// internal to enable optimizations
			internal abstract T Head();
			internal abstract Lst<T> Tail();
			
			public override R Match<R>(Func<T, Lst<T>, R> onCell, Func<R> onEmpty) {
				return onCell(this.Head(), this.Tail());
			}
		}
		
		private sealed class LstValueCell<T> : LstCell<T> {
			private readonly T head; 
			private readonly Lst<T> tail;
			
			internal LstValueCell(T head, Lst<T> tail) {
				this.head = head;
				this.tail = tail;
			}
			
			internal override T Head() { return this.head; }
			internal override Lst<T> Tail() { return this.tail; }
		}
		
//		private sealed class LstLazyCell<T> : LstCell<T> {
//			private readonly Lazy<Lst<T>> lazyLst;
//			
//			internal LstLazyCell(Func<Lst<T>> constructor) {
//				this.lazyList = Lazy.Create(constructor);
//			}
//			
//			internal override T Head() { return this.lazyList.Force().; }
//			internal override Lst<T> Tail() { return this.tailConstructor(); }
//		}
		
		// Constructors =======================================================

		public static Lst<T> Empty<T>() { return LstEmpty<T>.empty; }
		
		public static Lst<T> Cons<T>(T head, Lst<T> tail) { return new LstValueCell<T>(head, tail); }

        public static Lst<T> Create<T>(params T[] elements) {
            var result = Empty<T>();
            for(var i = (elements.Length - 1); i >= 0; i--)
                result = Cons(elements[i], result);
            return result;
        }

		
		public static Lst<T> Create<T>(IEnumerable<T> elements) {
            return CreateFromEnumerator(elements.GetEnumerator());
        }
		
		public static Lst<T> CreateFromEnumerator<T>(System.Collections.Generic.IEnumerator<T> e)
		{
			return (e.MoveNext())
				? Cons(e.Current, CreateFromEnumerator<T>(e))
				: Empty<T>();
		}
	
		
		// Abtractions ========================================================

		public static TAcc FoldL<T, TAcc>(this Lst<T> that, TAcc seed, Func<TAcc, T, TAcc> aggregator) {
			return OptimizedFoldL(aggregator, seed, that);
		}
		
		public static Lst<R> Map<T, R>(this Lst<T> that, Func<T, R> fn) {
            return OptimizedFoldR((acc, x) => Cons(fn(x), acc), Empty<R>(), that);
        }

        public static Lst<T> Filter<T>(this Lst<T> that, Func<T, bool> predicate) {
            return OptimizedFoldR(
				(acc, x) => predicate(x) ? Cons(x, acc) : acc,
				Empty<T>(), 
				that);
        }
		
		public static bool IsEqual<T>(this Lst<T> that, Lst<T> other) where T : IEquatable<T>
		{
			return that.Match(
				(hd0, tl0) => other.Match(
						(hd1, tl1) => hd0.Equals(hd1) && IsEqual(tl0, tl1),
					    ()         => false),
				()         => other.Match(
						(hd1, tl1) => false,
					    ()         => true));
		}
		
		public static bool IsEqual<T>(this Lst<T> that, Lst<T> other, Func<T, T, bool> equality) 
		{
			return that.Match(
				(hd0, tl0) => other.Match(
						(hd1, tl1) => equality(hd0, hd1) && IsEqual(tl0, tl1, equality),
					    ()         => false),
				()         => other.Match(
						(hd1, tl1) => false,
					    ()         => true));
		}
		
		public static int SumBy<T>(this Lst<T> that, Func<T, int> selector) {
			return OptimizedFoldL((sum, x) => sum + selector(x), 0, that);
		}
		
		public static long SumBy<T>(this Lst<T> that, Func<T, long> selector) {
			return OptimizedFoldL((sum, x) => sum + selector(x), 0L, that);
		}

		public static int Sum(this Lst<int> that) {
			return OptimizedFoldL((sum, x) => sum + x, 0, that);
		}
		
		public static long Sum(this Lst<long> that) {
			return OptimizedFoldL((sum, x) => sum + x, 0L, that);
		}
		
		// Lazy
		//public static Lst<R> LazyMap<T, R>(this Lst<T> that, Func<T, R> fn) {
        //    return that.Match(
		//			(hd, tl) => new LstLazyCell<R>(fn(hd), () => tl.LazyMap(fn)),
		//			() => Lst.Empty<R>());
        //}
		
		private static TAcc OptimizedFoldL<T, TAcc>(Func<TAcc, T, TAcc> aggregator, TAcc seed, Lst<T> list) {
			var cell = list as LstCell<T>;
            while (cell != null) {
                seed = aggregator(seed, cell.Head());
                cell = cell.Tail() as LstCell<T>;
            }
			return seed;
		}
		
		private static TAcc OptimizedFoldR<T, TAcc>(Func<TAcc, T, TAcc> aggregator, TAcc seed, Lst<T> list) {
			// Not yet optimized in any way... 
			return list.Match<TAcc>(
				(hd, tl) => OptimizedFoldR(aggregator, aggregator(seed, hd), tl),
				() => seed);
		}
	}
}
