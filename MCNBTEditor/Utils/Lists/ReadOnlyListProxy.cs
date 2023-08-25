using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MCNBTEditor.Utils.Lists {
    public class ReadOnlyListProxy<T> : IReadOnlyList<T> {
        private readonly IList list;

        public ReadOnlyListProxy(IList list) {
            this.list = list;
        }

        public IEnumerator<T> GetEnumerator() {
            return this.list.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => this.list.Count;

        public T this[int index] => (T) this.list[index];
    }
}