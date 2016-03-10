using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Concurrent;
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
        public event PropertyChangedEventHandler PropertyChanged;

        public SimulationViewModel(AppConfig config)
        {
            m_config = config;

            if(config.Songs==null)
            {
                config.Songs = new List<Song>();
            }

            Songs = config.Songs;
            LoadSongsCommand = new AwaitableDelegateCommand(LoadSongs);

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

            SkillControls = new List<Tuple<SkillTriggerControl, string>>
            {
                Tuple.Create(SkillTriggerControl.Auto, "制御しない"),
                Tuple.Create(SkillTriggerControl.AlwaysTrigger, "100%発動する"),
                Tuple.Create(SkillTriggerControl.NeverTrigger, "発動しない"),
            };

            StartSimulationCommand = new AwaitableDelegateCommand(StartSimulation);

            Simulator.Song = Songs.FirstOrDefault();
            Simulator.Unit = Units.FirstOrDefault();
        }

        public List<Tuple<AppealType?, string>> GrooveBursts
        {
            get;
        }

        public List<Tuple<IdolCategory, string>> GrooveTypes
        {
            get;
        }

        public List<Tuple<SkillTriggerControl, string>> SkillControls
        {
            get;
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

        public ObservableCollection<Unit> Units => m_config.Units;

        public Simulator Simulator
        {
            get;
        }

        public IAsyncCommand LoadSongsCommand
        {
            get;
        }

        public IAsyncCommand StartSimulationCommand
        {
            get;
        }

        public int MaxScore
        {
            get;
            set;
        }

        public int MinScore
        {
            get;
            set;
        }

        public int AverageScore
        {
            get;
            set;
        }

        public int MaxScorePerNote
        {
            get;
            set;
        }

        public int MinScorePerNote
        {
            get;
            set;
        }

        public int AverageScorePerNote
        {
            get;
            set;
        }

        public SimulationResult SelectedResult
        {
            get;
            set;
        }

        public List<SimulationResult> SimulationResults
        {
            get;
            private set;
        }

        private async Task LoadSongs()
        {
            try
            {
                LoadSongsCommand.RaiseCanExecuteChanged();
                var result= await new GamerChWikiSongSource(
                    new WebDocumentSource("http://imascg-slstage-wiki.gamerch.com/%E6%A5%BD%E6%9B%B2%E6%83%85%E5%A0%B1%E4%B8%80%E8%A6%A7")).GetSongs();
                Songs = m_config.Songs = result.Item1;
                m_config.Save();
                Simulator.Song = Songs.FirstOrDefault();
                MessageBox.Show($"取り込みが完了しました（{result.Item1.Count}成功・{result.Item2}失敗）");
            }
            catch (Exception ex)
            {
                MessageBox.Show("データを取り込めませんでした：" + ex.Message);
            }
            LoadSongsCommand.RaiseCanExecuteChanged();
        }

        private async Task StartSimulation()
        {
            if(Simulator.SongData==null)
            {
                MessageBox.Show("楽曲を選んでください");
                return;
            }
            if (Simulator.Unit == null)
            {
                MessageBox.Show("ユニットを選んでください");
                return;
            }

            var results = new ConcurrentBag<SimulationResult>();
            var rng = new Random();
            await Task.Run(() => Parallel.For(1, 101, i => results.Add(Simulator.StartSimulation(rng, i))));

            MaxScore = results.Max(x=>x.Score);
            MaxScorePerNote = results.Max(x => x.ScorePerNote);

            MinScore = results.Min(x => x.Score);
            MinScorePerNote = results.Min(x => x.ScorePerNote);

            AverageScore = (int)results.Average(x => x.Score);
            AverageScorePerNote = (int)results.Average(x => x.ScorePerNote);

            SimulationResults = results.ToList();
            SelectedResult = SimulationResults[0];
        }

        public int? GuestIid
        {
            get;
            set;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if(propertyName== nameof(GuestIid) || propertyName== nameof(EnableGuest))
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
            if (Simulator.Unit == null)
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
