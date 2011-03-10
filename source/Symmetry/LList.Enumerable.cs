namespace Symmetry
{
	using System;
	using System.Collections.Generic;
	
	public abstract partial class LList<T> : IEnumerable<T> {

		IEnumerator<T> IEnumerable<T>.GetEnumerator () {
			return new LListEnumerator(this);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return new LListEnumerator(this);
		}
		
		private class LListEnumerator : IEnumerator<T>
        {
            private readonly LList<T> source;
            private bool hasMoved = false;
			private LList<T> current = LList.Empty<T>();

            public LListEnumerator(LList<T> source) {
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
					this.current = this.current.Match((hd, tl) => tl, () => LList.Empty<T>());
                    return !this.current.IsEmpty;
				}
            }

            public void Reset() {
                this.hasMoved = false;
            }
        }
	}
}