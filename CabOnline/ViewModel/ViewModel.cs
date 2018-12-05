using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MvvmFoundation.Wpf;

namespace CabOnline.ViewModel
{
    internal class ViewModel : ObservableObject
    {
        protected void RaisePropertyChangedOnUiThread(string propertyName, bool async=true)
        {
            DoOnUiThread(() => RaisePropertyChanged(propertyName), DispatcherPriority.Normal, async);
        }

        protected void DoOnUiThread(Action action, DispatcherPriority priority = DispatcherPriority.Normal, bool async = true)
        {
            if (async)
                Application.Current.Dispatcher.BeginInvoke(
                    priority,
                    action);
            else
                Application.Current.Dispatcher.Invoke(
                    priority,
                    action);
        }
    }
}