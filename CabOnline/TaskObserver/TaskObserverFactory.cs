using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CabOnline.TaskObserver
{
    internal class TaskObserverFactory : ITaskObserverFactory
    {
        private readonly Func<ITaskObserver> _factoryFunc;

        public TaskObserverFactory(Func<ITaskObserver> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }

        public ITaskObserver CreateTaskObserver()
        {
            return _factoryFunc();
        }
    }
}
