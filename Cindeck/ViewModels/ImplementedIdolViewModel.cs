using Cindeck.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections;

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class ImplementedIdolViewModel:IViewModel, INotifyPropertyChanged
    {
        private AppConfig m_config;
        private MainViewModel m_mvm;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImplementedIdolViewModel(AppConfig config, MainViewModel mvm)
        {
            m_config = config;
            m_mvm = mvm;

            Idols = new ListCollectionView(m_config.ImplementedIdols);
            Filter = new IdolFilter(config, Idols, true);
            Filter.SetConfig(config.ImplementedIdolFilterConfig);

            foreach(var option in m_config.ImplementedIdolSortOptions)
            {
                Idols.SortDescriptions.Add(option.ToSortDescription());
            }

            ReloadDataCommand = new AwaitableDelegateCommand(ReloadData);
            AddToOwnedCommand = new DelegateCommand(AddToOwned, () => SelectedIdols!=null && SelectedIdols.Count > 0);
            CopyIidCommand = new DelegateCommand(CopyIid, () => SelectedIdols != null && SelectedIdols.Count ==1);
            SetGuestCenterCommand = new DelegateCommand(SetGuestCenter, () => SelectedIdols != null && SelectedIdols.Count == 1);
        }

        public ICollectionView Idols
        {
            get;
            private set;
        }

        public IdolFilter Filter
        {
            get;
            private set;
        }

        public IList SelectedIdols
        {
            get;
            set;
        }

        public IAsyncCommand ReloadDataCommand
        {
            get;
            private set;
        }

        private async Task ReloadData()
        {
            try
            {
                ReloadDataCommand.RaiseCanExecuteChanged();
                var result = await new GamerChWikiIdolSource(new WebDocumentSource("https://imascg-slstage-wiki.gamerch.com/%E3%82%A8%E3%83%94%E3%82%BD%E3%83%BC%E3%83%89%E4%B8%80%E8%A6%A7%EF%BC%88%E5%85%A8%E3%82%AB%E3%83%A9%E3%83%A0%EF%BC%89")).GetIdols();
                var idolsNotFound = new List<OwnedIdol>();
                foreach(var item in m_config.OwnedIdols)
                {
                    if(!result.Item1.Any(x=>x.Iid==item.Iid))
                    {
                        idolsNotFound.Add(item);
                    }
                }
                if (idolsNotFound.Count>0)
                {
                    var res = MessageBox.Show("取り込んだデータから一部所属アイドルのデータが見つかりませんでした。見つからなかったアイドルを所属アイドルから削除して、取り込みを続行しますか？", "確認", MessageBoxButton.YesNo);
                    if (MessageBoxResult.No == res)
                    {
                        return;
                    }
                }
                m_config.ImplementedIdols.Clear();
                foreach (var item in result.Item1)
                {
                    m_config.ImplementedIdols.Add(item);
                }
                foreach(var item in idolsNotFound)
                {
                    m_mvm.OwnedIdol.DeleteOwnedIdol(item);
                }
                foreach (var item in m_config.OwnedIdols)
                {
                    item.UpdateReference(result.Item1.First(x=>x.Iid==item.Iid));
                }
                m_config.Save();
                MessageBox.Show($"取り込みが完了しました（{result.Item1.Count}成功・{result.Item2}失敗）");
            }
            catch (Exception ex)
            {
                MessageBox.Show("データを取り込めませんでした：" + ex.Message);
            }
            ReloadDataCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand AddToOwnedCommand
        {
            get;
            private set;
        }

        private void AddToOwned()
        {
            foreach(var x in SelectedIdols.Cast<Idol>())
            {
                m_config.OwnedIdols.Add(new OwnedIdol(m_config.GetNextLid(), x));
            }
            m_config.Save();
            if(Filter.FilterOwned)
            {
                Idols.Refresh();
            }
        }

        public DelegateCommand CopyIidCommand
        {
            get;
            private set;
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
            if(propertyName== nameof(SelectedIdols))
            {
                AddToOwnedCommand.RaiseCanExecuteChanged();
                CopyIidCommand.RaiseCanExecuteChanged();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            m_config.ImplementedIdolSortOptions.Clear();
            foreach(var x in Idols.SortDescriptions)
            {
                m_config.ImplementedIdolSortOptions.Add(x.ToSortOption());
            }
            m_config.ImplementedIdolFilterConfig = Filter.GetConfig();
        }

        public void OnActivate()
        {
            
        }

        public void OnDeactivate()
        {
            
        }
    }
}
