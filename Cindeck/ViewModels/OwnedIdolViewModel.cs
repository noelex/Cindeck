using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Cindeck
{
    [ImplementPropertyChanged]
    class OwnedIdolViewModel:IDisposable,INotifyPropertyChanged
    {
        private AppConfig m_config;
        private UnitViewModel m_uvm;

        public event PropertyChangedEventHandler PropertyChanged;

        public OwnedIdolViewModel(AppConfig config, UnitViewModel uvm)
        {
            m_config = config;
            m_uvm = uvm;
            Idols = new ListCollectionView(config.OwnedIdols);

            foreach (var option in config.OwnedIdolSortOptions)
            {
                Idols.SortDescriptions.Add(option.ToSortDescription());
            }

            DeleteCommand = new DelegateCommand(Delete, () => SelectedIdols.Count > 0);
        }

        public ICollectionView Idols
        {
            get;
            private set;
        }

        public IList SelectedIdols
        {
            get;
            set;
        }

        public DelegateCommand DeleteCommand
        {
            get;
            private set;
        }

        private void Delete()
        {
            if(SelectedIdols.Cast<OwnedIdol>().Any(x=>m_uvm.IsIdolInUse(x)))
            {
                var result = MessageBox.Show("既にユニットに編成されているアイドルを削除しようとしています。アイドルをユニットから外して削除しますか？", "確認", MessageBoxButton.YesNo);
               if (MessageBoxResult.No== result)
                {
                    return;
                }
            }
            foreach (var x in SelectedIdols.Cast<OwnedIdol>().ToArray())
            {
                m_config.OwnedIdols.Remove(x);
                m_uvm.RemoveIdolFromUnits(x);
            }
            m_config.Save();
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "SelectedIdols")
            {
                DeleteCommand.RaiseCanExecuteChanged();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            m_config.OwnedIdolSortOptions.Clear();
            foreach (var x in Idols.SortDescriptions)
            {
                m_config.OwnedIdolSortOptions.Add(x.ToSortOption());
            }
        }
    }
}
