using System;
using System.Collections.Generic;
using System.Linq;

namespace CabOnline.TaskObserver.Progress
{
    internal class ProgressManager
    {
        public event EventHandler<ProgressEventArgs> OnProgress;

        public void NotifyProgress(object sender, int progress)
        {
            RaiseProgressEvent(sender, progress);
        }

        public IEnumerable<T> EnumerableProgressTracker<T>(object sender, IEnumerable<T> source)
        {
            return EnumerableProgressTracker(sender, source, source.Count());
        }

        public IEnumerable<T> EnumerableProgressTracker<T>(object sender, IEnumerable<T> source, int count)
        {
            var progressTracker = new EnumerableProgressTracker<T>(source);
            var c = new EnumerableContext(count);
            progressTracker.OnYield += (s, e) => NotifyProgress(sender, c.Step());
            return progressTracker;
        }

        public ProgressManager GetPartManager(int relativeWeight = 1)
        {
            var pm = new ProgressManager();
            lock(_partProgresses)
                _partProgresses[pm] = new PartContext(relativeWeight);
            pm.OnProgress += (sender, evArgs) => HandlePartProgress(sender, pm, evArgs.Progress);
            return pm;
        }

        public ProgressManager GetSliceManager(Slice slice)
        {
            var pm = new ProgressManager();
            pm.OnProgress += (sender, evArgs) => RaiseProgressEvent(sender, (evArgs.Progress * (slice.Max - slice.Min)) / 100 + slice.Min);
            return pm;
        }

        public ProgressManager GetSliceManager(int min, int max)
        {
            return GetSliceManager(new Slice(min, max));
        }

        private readonly Dictionary<ProgressManager, PartContext> _partProgresses = new Dictionary<ProgressManager, PartContext>();

        private void RaiseProgressEvent(object sender, int progress)
        {
            var handler = OnProgress;
            if (handler != null)
                handler(this, new ProgressEventArgs(sender, progress));
        }

        private void HandlePartProgress(object sender, ProgressManager child, int progress)
        {
            lock(_partProgresses)
                _partProgresses[child].Progress = progress;
            RaiseProgressEvent(sender, SynthtizeChildrenProgress());
        }

        private int SynthtizeChildrenProgress()
        {
            lock(_partProgresses)
                return _partProgresses.Values.Select(p => p.Progress*p.RelativeWeight).Sum()/_partProgresses.Values.Select(p => p.RelativeWeight).Sum();
        }

        private class EnumerableContext
        {
            private readonly int _count;
            private int _current;

            public EnumerableContext(int count)
            {
                //if (count <= 0) throw new ArgumentException("count doit être strictement supérieur à 0", "count");
                _count = count;
                _current = 0;
            }

            public int Step()
            {
                if (_count == 0) return 100;
                _current++;
                if (_current > _count)
                    _current = _count;
                return (100*_current)/_count;
            }
        }

        private class PartContext
        {
            private readonly int _relativeWeight;
            public int RelativeWeight => _relativeWeight;
            public int Progress { get; set; }

            public PartContext(int relativeWeight)
            {
                _relativeWeight = relativeWeight;
                Progress = 0;
            }
        }
    }
}