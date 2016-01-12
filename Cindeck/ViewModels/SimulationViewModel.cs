using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class SimulationViewModel : IDisposable, INotifyPropertyChanged
    {
        private AppConfig m_config;
        private bool m_isLoading;
        public event PropertyChangedEventHandler PropertyChanged;

        public SimulationViewModel(AppConfig config)
        {
            m_config = config;

            if(config.Songs==null)
            {
                config.Songs = new List<Song>();
            }

            Songs = config.Songs;
            LoadSongsCommand = new AwaitableDelegateCommand(LoadSongs, () => !m_isLoading);

            SelectedSong = Songs.FirstOrDefault();
            SelectedUnit = Units.FirstOrDefault();
        }

        public bool EnableGuest
        {
            get;
            set;
        }

        public Idol Guest
        {
            get;
            private set;
        }

        public bool EnableRoomEffect
        {
            get;
            set;
        }

        public List<Song> Songs
        {
            get;
            private set;
        }

        public IEnumerable<SongData> SongData
        {
            get
            {
                return SelectedSong != null ? SelectedSong.Data.Values : Enumerable.Empty<SongData>();
            }
        }

        public Song SelectedSong
        {
            get;
            set;
        }

        public SongData SelectedSongData
        {
            get;
            set;
        }

        public bool EnableSupportMembers
        {
            get;
            set;
        }

        public List<OwnedIdol> SupportMembers
        {
            get;
            private set;
        }

        public ObservableCollection<Unit> Units
        {
            get
            {
                return m_config.Units;
            }
        }

        public Unit SelectedUnit
        {
            get;
            set;
        }

        public AppealType? GrooveBurst
        {
            get;
            set;
        }

        public IAsyncCommand LoadSongsCommand
        {
            get;
            private set;
        }

        private async Task LoadSongs()
        {
            try
            {
                m_isLoading = true;
                LoadSongsCommand.RaiseCanExecuteChanged();
                Songs=m_config.Songs = await new GamerChWikiSongSource(
                    new WebDocumentSource("http://imascg-slstage-wiki.gamerch.com/%E6%A5%BD%E6%9B%B2%E6%83%85%E5%A0%B1%E4%B8%80%E8%A6%A7")).GetSongs();
                m_config.Save();
                MessageBox.Show("取り込みが完了しました。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("データを取り込めませんでした：" + ex.Message);
            }
            m_isLoading = false;
            LoadSongsCommand.RaiseCanExecuteChanged();
        }

        private void SelectSupportMembers()
        {
            if(SelectedSong==null)
            {
                return;
            }

            var lst = new List<OwnedIdol>();

            foreach (var item in m_config.OwnedIdols.Where(x=>SelectedSong.Type.HasFlag(x.Category)).OrderByDescending(x=>CalculateAppeal(x, true)))
            {
                if(lst.Count>=10)
                {
                    break;
                }
                lst.Add(item);
            }

            if(lst.Count<10)
            {
                foreach (var item in m_config.OwnedIdols.OrderByDescending(x => CalculateAppeal(x, true)))
                {
                    if (lst.Count >= 10)
                    {
                        break;
                    }
                    if(!lst.Contains(item))
                    {
                        lst.Add(item);
                    }
                }
            }

            SupportMembers = lst;
        }

        private int CalculateAppeal(AppealType targetAppeal, IIdol idol, bool isSupportMember)
        {
            if (idol == null)
            {
                return 0;
            }
            var rate = 1.0;

            if(!isSupportMember)
            {
                if (EnableRoomEffect)
                {
                    rate += 0.1;
                }

                if (SelectedUnit != null && SelectedUnit.Center != null &&
                    SelectedUnit.Center.CenterEffect != null && SelectedUnit.Center.CenterEffect is CenterEffect.AppealUp)
                {
                    var e = SelectedUnit.Center.CenterEffect as CenterEffect.AppealUp;
                    if (e.Targets.HasFlag(idol.Category) && e.TargetAppeal.HasFlag(targetAppeal))
                    {
                        rate += e.Rate;
                    }
                }

                if (Guest != null && Guest.CenterEffect != null && Guest.CenterEffect is CenterEffect.AppealUp)
                {
                    var e = Guest.CenterEffect as CenterEffect.AppealUp;
                    if (e.Targets.HasFlag(idol.Category) && e.TargetAppeal.HasFlag(targetAppeal))
                    {
                        rate += e.Rate;
                    }
                }
            }
            

            if (GrooveBurst != null && GrooveBurst.Value.HasFlag(targetAppeal))
            {
                rate += 1.5;
            }

            if (SelectedSong != null && SelectedSong.Type.HasFlag(idol.Category))
            {
                rate += 0.3;
            }


            switch (targetAppeal)
            {
                case AppealType.Vocal:
                    return (int)Math.Ceiling(idol.Vocal * rate * (isSupportMember ? 0.5 : 1));
                case AppealType.Dance:
                    return (int)Math.Ceiling(idol.Dance * rate * (isSupportMember ? 0.5 : 1));
                case AppealType.Visual:
                    return (int)Math.Ceiling(idol.Visual * rate * (isSupportMember ? 0.5 : 1));
                default:
                    throw new Exception();
            }
        }

        private int CalculateAppeal(IIdol idol, bool isSupportMember)
        {
            return CalculateAppeal(AppealType.Vocal, idol, isSupportMember) + 
                CalculateAppeal(AppealType.Dance, idol, isSupportMember) + 
                CalculateAppeal(AppealType.Visual, idol, isSupportMember);
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if(propertyName== "SelectedSong")
            {
                SelectedSongData = SongData.FirstOrDefault();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            
        }
    }
}
