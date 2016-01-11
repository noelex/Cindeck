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

namespace Cindeck.Views
{
    /// <summary>
    /// UnitViewModel.xaml の相互作用ロジック
    /// </summary>
    public partial class UnitView : UserControl
    {
        private bool m_scrollItem;

        public UnitView()
        {
            InitializeComponent();
            IdolGrid.SelectionChanged += IdolGrid_SelectionChanged;
        }

        private void IdolGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IdolGrid.SelectedItems.Count==1&&m_scrollItem)
            {
                IdolGrid.ScrollIntoView(IdolGrid.SelectedItem);
                m_scrollItem = false;
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IdolGrid.Focus();
            m_scrollItem = true;
        }
    }
}
