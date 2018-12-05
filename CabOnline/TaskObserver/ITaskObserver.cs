using System;
using System.Collections.Generic;
using CabOnline.TaskObserver.Messages;
using CabOnline.TaskObserver.Progress;

namespace CabOnline.TaskObserver
{
    internal interface ITaskObserver
    {
        event EventHandler<ProgressEventArgs> OnProgress;
        event EventHandler<MessageEventArgs> OnStatus;
        event EventHandler<MessageEventArgs> OnDebug;
        event EventHandler<MessageEventArgs> OnDetail; 
        event EventHandler<MessageEventArgs> OnInfo;
        event EventHandler<MessageEventArgs> OnWarning;
        event EventHandler<MessageEventArgs> OnError;
        event EventHandler<ObserverEventArgs> OnStarted;
        event EventHandler<ObserverEventArgs> OnCompleted;
        event EventHandler<MessageEventArgs> OnAborted;
        event EventHandler<ObserverEventArgs> OnIsIndeterminate;

        void NotifyStatus(object sender, string status);
        void NotifyProgress(object sender, int progress);
        void NotifyDebug(object sender, string debug);
        void NotifyDetail(object sender, string detail);
        void NotifyInfo(object sender, string info);
        void NotifyWarning(object sender, string warning);
        void NotifyError(object sender, string error);
        void NotifyStarted(object sender);
        void NotifyCompleted(object sender);
        void NotifyAborted(object sender, string message);
        void NotifyIsIndeterminate(object sender);

        TaskObserver InSlice(int min, int max);
        TaskObserver InSlice(Slice slice);
        TaskObserver InPart(int relativeWeight = 1);

        IEnumerable<Slice> GetSlices(int number);
        IEnumerable<int> GetSteps(int number);

        IEnumerable<T> EnumerableProgressTracker<T>(object sender, IEnumerable<T> source);
        IEnumerable<T> EnumerableProgressTracker<T>(object sender, IEnumerable<T> source, int count);

    }
}