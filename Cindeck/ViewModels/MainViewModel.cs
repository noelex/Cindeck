using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            Units = new UnitViewModel(m_config);
            OwnedIdol = new OwnedIdolViewModel(m_config,Units);
            ImplementedIdol = new ImplementedIdolViewModel(m_config, OwnedIdol);
            Simulation = new SimulationViewModel(m_config);
            Potential = new PotentialViewModel(m_config);
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
            m_config.Save();
        }

        public void OnActivate()
        {
            
        }

        public void OnDeactivate()
        {
            
        }
    }
}
