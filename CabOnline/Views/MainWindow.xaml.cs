using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CabOnline.ViewModel;

namespace CabOnline.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.ChargerPlanningCommand.Execute(null);
        }


    }
}
