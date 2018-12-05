using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using CabOnline.ViewModel;

namespace CabOnline.Views
{
    /// <summary>
    ///   Logique d'interaction pour TâchesSubView.xaml
    /// </summary>
    public partial class TâcheView : UserControl
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DetailBackgroundProperty =
            DependencyProperty.Register("DetailBackground", typeof(Brush), typeof(TâcheView),
                                        new UIPropertyMetadata(null));


        public TâcheView()
        {
            InitializeComponent();
            
        }

        public Brush DetailBackground
        {
            get => (Brush)GetValue(DetailBackgroundProperty);
            set => SetValue(DetailBackgroundProperty, value);
        }

        private void CvsMessagesFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var vm = DataContext as TâcheViewModel;
            if (vm != null)
            {
                var messageTâcheViewModel = e.Item as MessageTâcheViewModel;
                if (messageTâcheViewModel != null)
                    e.Accepted = vm.FiltreMessages(messageTâcheViewModel);
            }
        }

        private void TâcheItemViewLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as TâcheViewModel;
            if (vm != null)
            {
                vm.PropertyChanged +=
                    (s, args) =>
                        {
                            if (args.PropertyName == "FiltreMessages")
                            {
                                var cvs = TryFindResource("_cvsMessages") as CollectionViewSource;
                                if (cvs != null)
                                    cvs.View.Refresh();
                            }
                        };
            }
        }
    }
}