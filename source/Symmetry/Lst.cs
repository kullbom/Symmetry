// Copyright 2011 Johan Kullbom (see the file LICENSE)

namespace Symmetry
{
	using System;
	using System.Collections.Generic;
	
	/// <summary>
    /// Lst - A simple immutable singly linked list. Implementes IEnumerable&lt;T&gt;.
	/// </summary>
	public abstract partial class Lst<T> {
		public abstract R Match<R>(Func<T, Lst<T>, R> onSome, Func<R> onEmpty); 
		
		public abstract int Length();
		
		public bool IsEmpty() { return this.Match((hd, tl) => false, () => true); } 
		
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
			
			public override int Length() { return 0; }
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
			private readonly int length;
			
			internal LstValueCell(T head, Lst<T> tail) {
				this.head = head;
				this.tail = tail;
				this.length = tail.Length() + 1;
			}
			
			public override int Length() { return this.length; }
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

        public static Lst<T> Create<T>(params T[] elements) { return FromArray(elements); }

		public static Lst<T> Create<T>(IEnumerable<T> elements) {
            return FromEnumerator(elements.GetEnumerator());
        }
		
		private static Lst<T> FromEnumerator<T>(System.Collections.Generic.IEnumerator<T> e) {
			return (e.MoveNext())
				? Cons(e.Current, CreateFromEnumerator<T>(e))
				: Empty<T>();
		}
	
		
		// Abtractions ========================================================

		public static TAcc FoldL<T, TAcc>(this Lst<T> that, TAcc seed, Func<TAcc, T, TAcc> aggregator) {
			return OptimizedFoldL(aggregator, seed, that);
		}
		
		public static TAcc FoldR<T, TAcc>(this Lst<T> that, TAcc seed, Func<TAcc, T, TAcc> aggregator) {
			return OptimizedFoldR(aggregator, seed, that);
		}
		
		public static Lst<R> Map<T, R>(this Lst<T> that, Func<T, R> fn) {
            return OptimizedFoldR((acc, x) => Cons(fn(x), acc), Empty<R>(), that);
        }
		
		public static Lst<R> Bind<T, R>(this Lst<T> that, Func<T, Lst<R>> binder) {
			return OptimizedFoldR(
					(a0, x) => OptimizedFoldR((a1, r) => Cons(r, a1), a0, binder(x)),
					Empty<R>(),
				    that);
		}
		
        public static Lst<T> Filter<T>(this Lst<T> that, Func<T, bool> predicate) {
            return OptimizedFoldR(
				(acc, x) => predicate(x) ? Cons(x, acc) : acc,
				Empty<T>(), 
				that);
        }
		
		public static T[] ToArray<T>(this Lst<T> that) {
			T[] arr = new T[that.Length()];
			
			var index = 0;
			foreach(var element in that) {
				arr[index] = element;
				index++;
			}
			return arr;
		}
		
		public static Lst<T> FromArray<T>(T[] arr) {
			var result = Empty<T>();
            for(var i = (elements.Length - 1); i >= 0; i--)
                result = Cons(elements[i], result);
            return result;
		}
		
		public static bool IsEqual<T>(this Lst<T> that, Lst<T> other) where T : IEquatable<T>
		{
			return that.IsEqual(other, (e0, e1) => e0.Equals(e1));
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
			var arr = list.ToArray();
			for(int i = arr.Length - 1; i >= 0; i--)
				seed = aggregator(seed, arr[i]);
			return seed;
		}
	}
}
