using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cindeck.Core;
using System.ComponentModel;
using Cindeck.ViewModels;

namespace Cindeck.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainView : Window
    {
        private IViewModel vm;

        public MainView()
        {
            InitializeComponent();
            vm = DataContext as IViewModel;
            vm.OnActivate();
            tabControl.SelectionChanged += Items_CurrentChanged;
        }

        private void Items_CurrentChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source==sender)
            {
                foreach (var x in tabControl.Items.OfType<TabItem>().Select(x => (x.Content as FrameworkElement).DataContext).OfType<IViewModel>())
                {
                    if (x == ((tabControl.SelectedItem as TabItem).Content as FrameworkElement).DataContext)
                    {
                        x.OnActivate();
                    }
                    else
                    {
                        x.OnDeactivate();
                    }

                }
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            vm.OnDeactivate();
            vm.Dispose();
            base.OnClosed(e);
        }
    }
}
