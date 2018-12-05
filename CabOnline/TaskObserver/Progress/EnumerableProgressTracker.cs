using System;
using System.Collections;
using System.Collections.Generic;

namespace CabOnline.TaskObserver.Progress
{
    internal class EnumerableProgressTracker<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        public EnumerableProgressTracker(IEnumerable<T> source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _source)
            {
                // The yield holds execution until the next iteration,
                // so trigger the update event first.
                RaiseOnYield();
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event EventHandler OnYield;

        public void RaiseOnYield()
        {
            var handler = OnYield;
            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}