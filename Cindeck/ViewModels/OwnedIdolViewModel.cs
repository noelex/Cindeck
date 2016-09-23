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

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class OwnedIdolViewModel:IViewModel,INotifyPropertyChanged
    {
        private AppConfig m_config;
        private MainViewModel m_mvm;

        public event PropertyChangedEventHandler PropertyChanged;

        public OwnedIdolViewModel(AppConfig config, MainViewModel mvm)
        {
            m_config = config;
            m_mvm = mvm;
            Idols = new ListCollectionView(config.OwnedIdols);
            Filter = new IdolFilter(config, Idols, enableOwnedFilter: false);
            Filter.SetConfig(config.OwnedIdolFilterConfig);

            foreach (var option in config.OwnedIdolSortOptions)
            {
                Idols.SortDescriptions.Add(option.ToSortDescription());
            }

            DeleteCommand = new DelegateCommand(Delete, () => SelectedIdols.Count > 0);
            CopyIidCommand = new DelegateCommand(CopyIid, () => SelectedIdols != null && SelectedIdols.Count == 1);
            SetGuestCenterCommand = new DelegateCommand(SetGuestCenter, () => SelectedIdols != null && SelectedIdols.Count == 1);
        }

        public ICollectionView Idols
        {
            get;
        }

        public IdolFilter Filter
        {
            get;
        }

        public IList SelectedIdols
        {
            get;
            set;
        }

        public DelegateCommand DeleteCommand
        {
            get;
        }

        private void Delete()
        {
            if(SelectedIdols.Cast<OwnedIdol>().Any(x=>m_mvm.Units.IsIdolInUse(x)))
            {
                var result = MessageBox.Show("既にユニットに編成されているアイドルを削除しようとしています。アイドルをユニットから外して削除しますか？", "確認", MessageBoxButton.YesNo);
               if (MessageBoxResult.No== result)
                {
                    return;
                }
            }
            foreach (var x in SelectedIdols.Cast<OwnedIdol>().ToArray())
            {
                DeleteOwnedIdol(x);
            }
            m_config.Save();
        }

        public DelegateCommand CopyIidCommand
        {
            get;
        }

        private void CopyIid()
        {
            try
            {
                Clipboard.SetText(SelectedIdols.Cast<IIdol>().First().Iid.ToString("x8"));
            }
            catch
            {

            }
        }

        public DelegateCommand SetGuestCenterCommand
        {
            get;
            private set;
        }

        private void SetGuestCenter()
        {
            m_mvm.Simulation.GuestIid = SelectedIdols.Cast<IIdol>().First().Iid;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == nameof(SelectedIdols))
            {
                DeleteCommand.RaiseCanExecuteChanged();
                CopyIidCommand.RaiseCanExecuteChanged();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void DeleteOwnedIdol(OwnedIdol idol)
        {
            m_config.OwnedIdols.Remove(idol);
            m_mvm.Units.RemoveIdolFromUnits(idol);
        }

        public void Dispose()
        {
            m_config.OwnedIdolSortOptions.Clear();
            foreach (var x in Idols.SortDescriptions)
            {
                m_config.OwnedIdolSortOptions.Add(x.ToSortOption());
            }
            m_config.OwnedIdolFilterConfig = Filter.GetConfig();
        }

        public void OnActivate()
        {
            Idols.Refresh();
        }

        public void OnDeactivate()
        {
            
        }
    }
}
