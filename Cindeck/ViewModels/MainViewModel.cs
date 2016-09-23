using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class MainViewModel:IViewModel
    {
        private AppConfig m_config;

        public MainViewModel()
        {
            try
            {
                m_config = AppConfig.Load();
            }
            catch(Exception e)
            {
                MessageBox.Show($"app.configを読み込めませんでした：{e.Message}");
                Application.Current.Shutdown();
            }

            Units = new UnitViewModel(m_config, this);
            OwnedIdol = new OwnedIdolViewModel(m_config, this);
            ImplementedIdol = new ImplementedIdolViewModel(m_config, this);
            Simulation = new SimulationViewModel(m_config);
            Potential = new PotentialViewModel(m_config);
        }

        private async Task CheckUpdate()
        {
            using (var client = new WebClient())
            {
                var serializer = new DataContractJsonSerializer(typeof(ReleaseInfo));
                client.Headers["User-Agent"] = Title;
                using (var data = new MemoryStream(await client.DownloadDataTaskAsync(new Uri("https://api.github.com/repos/noelex/Cindeck/releases/latest"))))
                {
                    var releaseInfo = serializer.ReadObject(data) as ReleaseInfo;
                    if (Version.Parse(releaseInfo.tag_name.Substring(1)) > Version.Parse(m_config.Version.Substring(1)))
                    {
                        if (MessageBoxResult.Yes == MessageBox.Show("新しいバージョンがあります。今すぐダウンロードしますか？", "アップデート", MessageBoxButton.YesNo))
                        {
                            Process.Start(releaseInfo.html_url);
                        }
                    }
                }
            }
        }

        public OwnedIdolViewModel OwnedIdol
        {
            get;
        }

        public ImplementedIdolViewModel ImplementedIdol
        {
            get;
        }

        public UnitViewModel Units
        {
            get;
        }

        public SimulationViewModel Simulation
        {
            get;
        }

        public PotentialViewModel Potential
        {
            get;
        }

        public string Title => $"Cindeck {m_config.Version}";


        public void Dispose()
        {
            OwnedIdol.Dispose();
            ImplementedIdol.Dispose();
            Units.Dispose();
            Potential.Dispose();
            Simulation.Dispose();
            m_config.Save();
        }

        public async void OnActivate()
        {
            try
            {
                await CheckUpdate();
            }
            catch
            {

            }
        }

        public void OnDeactivate()
        {
            
        }
    }
}
