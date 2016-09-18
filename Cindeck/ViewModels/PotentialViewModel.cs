using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cindeck.ViewModels
{

    [ImplementPropertyChanged]
    class PotentialViewModel : IViewModel
    {
        private AppConfig m_config;


        public PotentialViewModel(AppConfig config)
        {
            m_config = config;
            PotentialData = new ListCollectionView(config.PotentialData);
            Filter = new IdolFilter(config, PotentialData, enableCenterEffectFilter: false, enableOwnedFilter: false, enableRarityFilter: false, enableSkillFilter: false);
            Filter.SetConfig(config.PotentialFilterConfig);

            foreach (var option in config.PotentialSortOptions)
            {
                PotentialData.SortDescriptions.Add(option.ToSortDescription());
            }
        }

        public ListCollectionView PotentialData
        {
            get;
        }

        public IdolFilter Filter
        {
            get;
        }

        public void OnActivate()
        {

        }

        public void OnDeactivate()
        {

        }

        public void Dispose()
        {
            m_config.PotentialSortOptions.Clear();
            foreach (var x in PotentialData.SortDescriptions)
            {
                m_config.PotentialSortOptions.Add(x.ToSortOption());
            }
            m_config.PotentialFilterConfig = Filter.GetConfig();
        }
    }
}
