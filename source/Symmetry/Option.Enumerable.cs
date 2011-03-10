namespace Symmetry
{
	using System;
	using System.Collections.Generic;
	
	public abstract partial class Option<T> : IEnumerable<T> {

		IEnumerator<T> IEnumerable<T>.GetEnumerator () {
			return new OptionEnumerator(this);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return new OptionEnumerator(this);
		}
		
		private class OptionEnumerator : IEnumerator<T>
        {
            private readonly Option<T> source;
            private bool hasMoved = false;

            public OptionEnumerator(Option<T> source) {
                this.source = source;
            }

            public T Current {
                get {
                    if (!this.hasMoved)
                        throw new InvalidOperationException();
                    else
                        return this.source.Match<T>(v => v, () => { throw new InvalidOperationException(); });
                }
            }

            public void Dispose() { }

            object System.Collections.IEnumerator.Current {
                get { return this.Current; }
            }

            public bool MoveNext() {
                if(!this.hasMoved) {
                    this.hasMoved = true;
                    return this.source.IsSome();
                } else
                    return false;
            }

            public void Reset() {
                this.hasMoved = false;
            }
        }
	}
}