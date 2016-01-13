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
    class SimulationViewModel : IViewModel, INotifyPropertyChanged
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
            Simulator = new Simulator(config);

            GrooveBursts = new List<Tuple<AppealType?, string>>
            {
               Tuple.Create(new AppealType?(),"なし" ),
               Tuple.Create((AppealType?)AppealType.Vocal,"Vo 150%"),
               Tuple.Create((AppealType?)AppealType.Dance,"Da 150%"),
               Tuple.Create((AppealType?)AppealType.Visual,"Vi 150%")
            };

            GrooveTypes = new List<Tuple<IdolCategory, string>>
            {
               Tuple.Create(IdolCategory.Cute, "Cu 30%"),
               Tuple.Create(IdolCategory.Cool, "Co 30%"),
               Tuple.Create(IdolCategory.Passion, "Pa 30%")
            };

            Simulator.Song = Songs.FirstOrDefault();
            Simulator.Unit = Units.FirstOrDefault();
        }

        public List<Tuple<AppealType?, string>> GrooveBursts
        {
            get;
            private set;
        }

        public List<Tuple<IdolCategory, string>> GrooveTypes
        {
            get;
            private set;
        }

        public bool EnableGuest
        {
            get;
            set;
        }

        public List<Song> Songs
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

        public Simulator Simulator
        {
            get;
            private set;
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

        public int? GuestIid
        {
            get;
            set;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if(propertyName== "GuestIid" || propertyName== "EnableGuest")
            {
                Simulator.Guest=  GuestIid == null || !EnableGuest ? null : m_config.ImplementedIdols.FirstOrDefault(x => x.Iid == GuestIid);
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            
        }

        public void OnActivate()
        {
            if (GuestIid != null && !m_config.ImplementedIdols.Any(x => x.Iid == GuestIid))
            {
                GuestIid = null;
            }
            if (Simulator.Unit != null && !m_config.Units.Contains(Simulator.Unit))
            {
                Simulator.Unit = m_config.Units.FirstOrDefault();
            }

            Simulator.Reload();
        }

        public void OnDeactivate()
        {
            
        }
    }
}
