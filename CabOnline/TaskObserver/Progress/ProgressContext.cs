using System.Collections.Generic;
using System.Linq;

namespace CabOnline.TaskObserver.Progress
{
    internal class ProgressContext
    {
        private class ProgressSlice
        {
            private readonly int _end;
            private readonly int _start;

            public ProgressSlice(int start, int end = 100)
            {
                _start = start;
                _end = end;
            }

            public int GetAbsoluteProgress(int progress)
            {
                return progress*(_end - _start)/100 + _start;
            }
        }

        private readonly List<ProgressSlice> _stack = new List<ProgressSlice>();

        private void EnterSlice(ProgressSlice slice)
        {
            _stack.Insert(0, slice);
        }

        public void EnterSlice(int start, int end)
        {
            EnterSlice(new ProgressSlice(start, end));
        }

        public void ExitSlice()
        {
            _stack.RemoveAt(0);
        }

        public void Clear()
        {
            _stack.Clear();
        }

        public int GetAbsoluteProgress(int progress)
        {
            return _stack.Aggregate(progress, (p, nextSlice) => nextSlice.GetAbsoluteProgress(p));
        }
    }
}