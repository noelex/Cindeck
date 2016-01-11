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
    class ImplementedIdolViewModel:IDisposable, INotifyPropertyChanged
    {
        private AppConfig m_config;
        private bool m_isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImplementedIdolViewModel(AppConfig config)
        {
            m_config = config;
            Idols = new ListCollectionView(m_config.ImplementedIdols);
            
            foreach(var option in m_config.ImplementedIdolSortOptions)
            {
                Idols.SortDescriptions.Add(option.ToSortDescription());
            }

            ReloadDataCommand = new AwaitableDelegateCommand(ReloadData, () => !m_isLoading);
            AddToOwnedCommand = new DelegateCommand(AddToOwned, () => SelectedIdols!=null && SelectedIdols.Count > 0);
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

        public IAsyncCommand ReloadDataCommand
        {
            get;
            private set;
        }

        private async Task ReloadData()
        {
            try
            {
                m_isLoading = true;
                ReloadDataCommand.RaiseCanExecuteChanged();
                var result = await new GamerChWikiIdolSource(new WebDocumentSource("http://imascg-slstage-wiki.gamerch.com/%E3%82%AB%E3%83%BC%E3%83%89%E5%85%A8%E3%82%AB%E3%83%A9%E3%83%A0%E4%B8%80%E8%A6%A7")).GetIdols();
                m_config.ImplementedIdols.Clear();
                foreach (var item in result)
                {
                    m_config.ImplementedIdols.Add(item);
                }
                m_config.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("データを取り込めませんでした：" + ex.Message);
            }
            m_isLoading = false;
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
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if(propertyName== "SelectedIdols")
            {
                AddToOwnedCommand.RaiseCanExecuteChanged();
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
        }
    }
}
