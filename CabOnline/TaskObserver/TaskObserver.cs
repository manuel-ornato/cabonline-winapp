using System;
using System.Collections.Generic;
using CabOnline.TaskObserver.Messages;
using CabOnline.TaskObserver.Progress;

namespace CabOnline.TaskObserver
{

    internal class TaskObserver : ITaskObserver
    {
        private readonly MessagingManager _messagingManager;
        private readonly ProgressManager _progressManager;

        private TaskObserver(MessagingManager messagingManager, ProgressManager progressManager)
        {
            _messagingManager = messagingManager;
            _progressManager = progressManager;
        }

        public TaskObserver():this(new MessagingManager(), new ProgressManager())
        {
        }

        public event EventHandler<ProgressEventArgs> OnProgress
        {
            add => _progressManager.OnProgress += value;
            remove => _progressManager.OnProgress -= value;
        }
        public event EventHandler<MessageEventArgs> OnStatus
        {
            add => _messagingManager.OnStatus += value;
            remove => _messagingManager.OnStatus -= value;
        }
        public event EventHandler<MessageEventArgs> OnDebug
        {
            add => _messagingManager.OnDebug += value;
            remove => _messagingManager.OnDebug -= value;
        }
        public event EventHandler<MessageEventArgs> OnDetail
        {
            add => _messagingManager.OnDetail += value;
            remove => _messagingManager.OnDetail -= value;
        }
        public event EventHandler<MessageEventArgs> OnInfo
        {
            add => _messagingManager.OnInfo += value;
            remove => _messagingManager.OnInfo -= value;
        }
        public event EventHandler<MessageEventArgs> OnWarning
        {
            add => _messagingManager.OnWarning += value;
            remove => _messagingManager.OnWarning -= value;
        }
        public event EventHandler<MessageEventArgs> OnError
        {
            add => _messagingManager.OnError += value;
            remove => _messagingManager.OnError -= value;
        }
        public event EventHandler<ObserverEventArgs> OnStarted
        {
            add => _messagingManager.OnStarted += value;
            remove => _messagingManager.OnStarted -= value;
        }
        public event EventHandler<ObserverEventArgs> OnCompleted
        {
            add => _messagingManager.OnCompleted += value;
            remove => _messagingManager.OnCompleted -= value;
        }
        public event EventHandler<MessageEventArgs> OnAborted
        {
            add => _messagingManager.OnAborted += value;
            remove => _messagingManager.OnAborted -= value;
        }
        public event EventHandler<ObserverEventArgs> OnIsIndeterminate
        {
            add => _messagingManager.OnIsIndeterminate += value;
            remove => _messagingManager.OnIsIndeterminate -= value;
        }

        public void NotifyStatus(object sender, string status)
        {
            _messagingManager.NotifyStatus(sender, status);
        }

        public void NotifyProgress(object sender, int progress)
        {
            _progressManager.NotifyProgress(sender, progress);
        }

        public void NotifyDebug(object sender, string debug)
        {
            _messagingManager.NotifyDebug(sender, debug);
        }

        public void NotifyDetail(object sender, string detail)
        {
            _messagingManager.NotifyDetail(sender, detail);
        }

        public void NotifyInfo(object sender, string info)
        {
            _messagingManager.NotifyInfo(sender, info);
        }

        public void NotifyWarning(object sender, string warning)
        {
            _messagingManager.NotifyWarning(sender, warning);
        }

        public void NotifyError(object sender, string error)
        {
            _messagingManager.NotifyError(sender, error);
        }

        public void NotifyStarted(object sender)
        {
            _messagingManager.NotifyStarted(sender);
        }

        public void NotifyCompleted(object sender)
        {
            _messagingManager.NotifyCompleted(sender);
        }

        public void NotifyAborted(object sender, string message)
        {
            _messagingManager.NotifyAborted(sender, message);
        }

        public void NotifyIsIndeterminate(object sender)
        {
            _messagingManager.NotifyIsIndeterminate(sender);
        }

        public TaskObserver InSlice(int min, int max)
        {
            return InSlice(new Slice(min, max));
        }

        public TaskObserver InSlice(Slice slice)
        {
            return new TaskObserver(_messagingManager, _progressManager.GetSliceManager(slice));
        }

        public IEnumerable<Slice> GetSlices(int number)
        {
            double step = 100.0 / number;
            for (var i = 0; i < number; i++)
                yield return new Slice((int)Math.Round(i * step), (int)Math.Round((i + 1) * step));
            yield break;
        }

        public TaskObserver InPart(int relativeWeight = 1)
        {
            return new TaskObserver(_messagingManager, _progressManager.GetPartManager(relativeWeight));
        }

        public IEnumerable<int> GetSteps(int number)
        {
            double step = 100.0 / number;
            for (var i = 0; i < number; i++)
                yield return (int)Math.Round(i * step);
            yield break;
        }

        public IEnumerable<T> EnumerableProgressTracker<T>(object sender, IEnumerable<T> source)
        {
            return _progressManager.EnumerableProgressTracker(sender, source);
        }

        public IEnumerable<T> EnumerableProgressTracker<T>(object sender, IEnumerable<T> source, int count)
        {
            return _progressManager.EnumerableProgressTracker(sender, source, count);
        }

        public ITaskObserver GetNewObserver()
        {
            return new TaskObserver();
        }
    }
}