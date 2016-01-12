using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class MainViewModel:IDisposable
    {
        private AppConfig m_config;

        public MainViewModel()
        {
            m_config = AppConfig.Load();
            Units = new UnitViewModel(m_config);
            OwnedIdol = new OwnedIdolViewModel(m_config,Units);
            ImplementedIdol = new ImplementedIdolViewModel(m_config, OwnedIdol);
            Simulation = new SimulationViewModel(m_config);
        }

        public OwnedIdolViewModel OwnedIdol
        {
            get;
            private set;
        }

        public ImplementedIdolViewModel ImplementedIdol
        {
            get;
            private set;
        }

        public UnitViewModel Units
        {
            get;
            private set;
        }

        public SimulationViewModel Simulation
        {
            get;
            private set;
        }


        public void Dispose()
        {
            OwnedIdol.Dispose();
            ImplementedIdol.Dispose();
            Units.Dispose();
            m_config.Save();
        }
    }
}
