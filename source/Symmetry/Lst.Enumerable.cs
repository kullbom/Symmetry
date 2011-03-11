// Copyright 2011 Johan Kullbom (see the file license.txt)

namespace Symmetry
{
	using System;
	using System.Collections.Generic;
	
	public abstract partial class Lst<T> : IEnumerable<T> {

		IEnumerator<T> IEnumerable<T>.GetEnumerator () {
			return new LstEnumerator(this);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return new LstEnumerator(this);
		}
		
		private class LstEnumerator : IEnumerator<T>
        {
            private readonly Lst<T> source;
            private bool hasMoved = false;
			private Lst<T> current = Lst.Empty<T>();

            public LstEnumerator(Lst<T> source) {
                this.source = source;
            }

            public T Current {
                get {
                    if (!this.hasMoved)
                        throw new InvalidOperationException();
                    else
                        return this.current.Match<T>((hd, tl) => hd, () => { throw new InvalidOperationException(); });
                }
            }

            public void Dispose() { }

            object System.Collections.IEnumerator.Current {
                get { return this.Current; }
            }

            public bool MoveNext() {
                if(!this.hasMoved) { 
					this.hasMoved = true;
					this.current = this.source;
				    return !this.current.IsEmpty;
                } 
				else {
					this.current = this.current.Match((hd, tl) => tl, () => Lst.Empty<T>());
                    return !this.current.IsEmpty;
				}
            }

            public void Reset() {
                this.hasMoved = false;
            }
        }
	}
}