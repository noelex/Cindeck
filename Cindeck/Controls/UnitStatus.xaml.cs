using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Cindeck.Controls
{
    /// <summary>
    /// UnitStatus.xaml の相互作用ロジック
    /// </summary>
    public partial class UnitStatus : UserControl
    {
        public UnitStatus()
        {
            InitializeComponent();
        }

        public int Life
        {
            get { return (int)GetValue(LifeProperty);  }
            set { SetValue(LifeProperty, value); }
        }

        public int Dance
        {
            get { return (int)GetValue(DanceProperty); }
            set { SetValue(DanceProperty, value); }
        }

        public int Vocal
        {
            get { return (int)GetValue(VocalProperty); }
            set { SetValue(VocalProperty, value); }
        }

        public int Visual
        {
            get { return (int)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        public int TotalAppeal
        {
            get { return (int)GetValue(TotalAppealProperty); }
            set { SetValue(TotalAppealProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property.Name== nameof(Life))
            {
                LifeLabel.Content = e.NewValue;
            }
            else if (e.Property.Name == nameof(Vocal))
            {
                VocalLabel.Content = e.NewValue;
            }
            else if (e.Property.Name == nameof(Dance))
            {
                DanceLabel.Content = e.NewValue;
            }
            else if (e.Property.Name == nameof(Visual))
            {
                VisualLabel.Content = e.NewValue;
            }
            else if (e.Property.Name == nameof(TotalAppeal))
            {
                TotalAppealLabel.Content = e.NewValue;
            }
            base.OnPropertyChanged(e);
        }

        public static readonly DependencyProperty LifeProperty =
                DependencyProperty.Register(nameof(Life), typeof(int), typeof(UnitStatus), new PropertyMetadata(null));
        public static readonly DependencyProperty VocalProperty =
                DependencyProperty.Register(nameof(Vocal), typeof(int), typeof(UnitStatus), new PropertyMetadata(null));
        public static readonly DependencyProperty DanceProperty =
                DependencyProperty.Register(nameof(Dance), typeof(int), typeof(UnitStatus), new PropertyMetadata(null));
        public static readonly DependencyProperty VisualProperty =
                DependencyProperty.Register(nameof(Visual), typeof(int), typeof(UnitStatus), new PropertyMetadata(null));
        public static readonly DependencyProperty TotalAppealProperty =
        DependencyProperty.Register(nameof(TotalAppeal), typeof(int), typeof(UnitStatus), new PropertyMetadata(null));
    }
}
