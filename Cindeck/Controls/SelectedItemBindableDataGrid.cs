using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cindeck.Controls
{

    public class SelectedItemBindableDataGrid : DataGrid
    {

        public SelectedItemBindableDataGrid()
        {
            SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItemsList = SelectedItems;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if(newValue!=null&&newValue is ICollectionView&&Items!=null)
            {
                foreach (var x in Items.SortDescriptions)
                {
                    var col = Columns.FirstOrDefault(y => x.PropertyName == y.SortMemberPath);
                    if (col != null)
                    {
                        col.SortDirection = x.Direction;
                    }
                }
            }
        }

        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            string sortPropertyName = eventArgs.Column.SortMemberPath;
            if (!string.IsNullOrEmpty(sortPropertyName))
            {
                var index = Items.SortDescriptions.Select(x => (SortDescription?)x).FirstOrDefault(x => x.Value.PropertyName == sortPropertyName);

                if (index != null)
                {
                    Items.SortDescriptions.Remove(index.Value);
                }

                if (!eventArgs.Column.SortDirection.HasValue)
                {
                    index = new SortDescription { PropertyName = sortPropertyName, Direction = ListSortDirection.Descending };
                    Items.SortDescriptions.Add(index.Value);
                }
                else if (eventArgs.Column.SortDirection.Value == ListSortDirection.Descending)
                {
                    index = new SortDescription { PropertyName = sortPropertyName, Direction = ListSortDirection.Ascending };
                    Items.SortDescriptions.Add(index.Value);
                }


                foreach (var col in Columns)
                {
                    var item = Items.SortDescriptions.Select(x=>(SortDescription?)x).FirstOrDefault(y => col.SortMemberPath == y.Value.PropertyName);

                    if (item != null)
                    {
                        col.SortDirection = item.Value.Direction;
                    }
                    else
                    {
                        col.SortDirection = null;
                    }
                }

                Items.Refresh();
                eventArgs.Handled = true;

            }
        }

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(SelectedItemBindableDataGrid), new PropertyMetadata(null));
    }
}
