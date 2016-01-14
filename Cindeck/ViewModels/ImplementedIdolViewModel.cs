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
        private bool m_isLoading;
        private OwnedIdolViewModel m_ovm;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImplementedIdolViewModel(AppConfig config, OwnedIdolViewModel ovm)
        {
            m_config = config;
            m_ovm = ovm;

            Idols = new ListCollectionView(m_config.ImplementedIdols);
            
            Idols.Filter = FilterIdols;

            foreach(var option in m_config.ImplementedIdolSortOptions)
            {
                Idols.SortDescriptions.Add(option.ToSortDescription());
            }

            ReloadDataCommand = new AwaitableDelegateCommand(ReloadData, () => !m_isLoading);
            AddToOwnedCommand = new DelegateCommand(AddToOwned, () => SelectedIdols!=null && SelectedIdols.Count > 0);
            CopyIidCommand = new DelegateCommand(CopyIid, () => SelectedIdols != null && SelectedIdols.Count ==1);

            IdolTypes = new List<Tuple<IdolCategory, string>>
            {
                Tuple.Create(IdolCategory.All,"すべて"),
                Tuple.Create(IdolCategory.Cute,"キュート"),
                Tuple.Create(IdolCategory.Cool,"クール"),
                Tuple.Create(IdolCategory.Passion,"パッション")
            };
            TypeFilter = IdolCategory.All;

            Rarities = typeof(Rarity).GetEnumValues().Cast<Rarity?>().Reverse().Select(x => Tuple.Create(x, x.Value.ToLocalizedString())).ToList();
            Rarities.Insert(0, Tuple.Create(new Rarity?(), "すべて"));
        }

        public ICollectionView Idols
        {
            get;
            private set;
        }

        public bool HideOwned
        {
            get;
            set;
        }

        public List<Tuple<IdolCategory, string>> IdolTypes
        {
            get;
            private set;
        }

        public List<Tuple<Rarity?, string>> Rarities
        {
            get;
            private set;
        }

        public IdolCategory TypeFilter
        {
            get;
            set;
        }

        public Rarity? RarityFilter
        {
            get;
            set;
        }

        public string NameFilter
        {
            get;
            set;
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
                var idolsNotFound = new List<OwnedIdol>();
                foreach(var item in m_config.OwnedIdols)
                {
                    if(!result.Any(x=>x.Iid==item.Iid))
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
                foreach (var item in result)
                {
                    m_config.ImplementedIdols.Add(item);
                }
                foreach(var item in idolsNotFound)
                {
                    m_ovm.DeleteOwnedIdol(item);
                }
                foreach (var item in m_config.OwnedIdols)
                {
                    item.UpdateReference(result.First(x=>x.Iid==item.Iid));
                }
                m_config.Save();
                MessageBox.Show("取り込みが完了しました。");
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

        public DelegateCommand CopyIidCommand
        {
            get;
            private set;
        }

        private bool FilterIdols(object obj)
        {
            var idol = obj as Idol;
            var ok = TypeFilter.HasFlag(idol.Category);

            if(HideOwned)
            {
                ok &= !m_config.OwnedIdols.Any(x => x.Iid == idol.Iid);
            }

            if(!string.IsNullOrEmpty(NameFilter))
            {
                ok &= idol.Name.Contains(NameFilter) || (idol.Label != null && idol.Label.Contains(NameFilter));
            }

            if(RarityFilter!=null)
            {
                ok &= idol.Rarity == RarityFilter;
            }

            return ok;
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

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if(propertyName== "SelectedIdols")
            {
                AddToOwnedCommand.RaiseCanExecuteChanged();
                CopyIidCommand.RaiseCanExecuteChanged();
            }

            if(propertyName=="HideOwned"|| propertyName == "NameFilter" || propertyName == "RarityFilter" ||
                propertyName == "TypeFilter")
            {
                Idols.Refresh();
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

        public void OnActivate()
        {
            
        }

        public void OnDeactivate()
        {
            
        }
    }
}
