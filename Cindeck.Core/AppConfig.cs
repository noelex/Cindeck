using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract]
    public class AppConfig
    {
        private static DataContractSerializer m_serializer= new DataContractSerializer(typeof(AppConfig),new DataContractSerializerSettings{ PreserveObjectReferences=true  });
        private static AppConfig m_current = null;

        private AppConfig()
        {
            ImplementedIdols = new ObservableCollection<Idol>();
            OwnedIdols = new ObservableCollection<OwnedIdol>();
            Units = new ObservableCollection<Unit>();

            ImplementedIdolSortOptions = new List<SortOption>() {
                new SortOption { Column=nameof(IIdol.Rarity), Direction=ListSortDirection.Descending }
            };
            OwnedIdolSortOptions = new List<SortOption>()
            {
                 new SortOption { Column=nameof(IIdol.Rarity), Direction=ListSortDirection.Descending }
            };
            UnitIdolSortOptions = new List<SortOption>()
            {
                 new SortOption { Column=nameof(IIdol.Rarity), Direction=ListSortDirection.Descending }
            };

            NextOid = 1;
        }

        public string Version => "v1.9.9";

        [DataMember(Order = 1)]
        private int NextOid
        {
            get;
            set;
        }

        [DataMember(Order = 2)]
        public ObservableCollection<Idol> ImplementedIdols
        {
            get;
            set;
        }

        [DataMember(Order = 3)]
        public ObservableCollection<OwnedIdol> OwnedIdols
        {
            get;
            set;
        }

        [DataMember(Order = 4)]
        public ObservableCollection<Unit> Units
        {
            get;
            set;
        }

        [DataMember(Order = 5)]
        public List<SortOption> ImplementedIdolSortOptions
        {
            get;
            set;
        }

        [DataMember(Order = 6)]
        public List<SortOption> OwnedIdolSortOptions
        {
            get;
            set;
        }

        [DataMember(Order = 7)]
        public List<SortOption> UnitIdolSortOptions
        {
            get;
            set;
        }

        [DataMember(Order = 8)]
        public List<Song> Songs
        {
            get;
            set;
        }


        [DataMember(Order = 9)]
        public FilterConfig ImplementedIdolFilterConfig
        {
            get;
            set;
        }

        [DataMember(Order = 10)]
        public FilterConfig OwnedIdolFilterConfig
        {
            get;
            set;
        }

        [DataMember(Order = 11)]
        public FilterConfig UnitIdolFilterConfig
        {
            get;
            set;
        }

        [DataMember(Order = 12)]
        public ObservableCollection<Potential> PotentialData
        {
            get;
            set;
        }

        [DataMember(Order = 13)]
        public FilterConfig PotentialFilterConfig
        {
            get;
            set;
        }

        [DataMember(Order = 14)]
        public List<SortOption> PotentialSortOptions
        {
            get;
            set;
        }

        [DataMember(Order = 15)]
        public SimulatorConfig SimulatorConfig
        {
            get;
            set;
        }

        public int GetNextLid()
        {
            return NextOid++;
        }

        public void Save()
        {
            using (var fs = File.OpenWrite("app.config"))
            {
                fs.SetLength(0);
                m_serializer.WriteObject(fs, this);
            }  
        }

        public static AppConfig Current
        {
            get
            {
                return m_current;
            }
        }

        public static AppConfig Load()
        {
            AppConfig config = null;
            if(!File.Exists("app.config"))
            {
                config = new AppConfig();
            }
            else
            {
                using (var fs = File.OpenRead("app.config"))
                {
                    config = (AppConfig)m_serializer.ReadObject(fs);
                }
            }

            if (config.PotentialData == null)
            {
                config.PotentialData = new ObservableCollection<Potential>();
            }

            if (config.ImplementedIdols.Count > 0 && config.PotentialData.Count == 0)
            {
                var idolList = config.ImplementedIdols.GroupBy(x => x.Name)
                    .Select(x => new Potential { Category = x.First().Category, Name = x.Key });

                foreach (var x in idolList)
                {
                    config.PotentialData.Add(x);
                }
            }

            config.ImplementedIdols.CollectionChanged += (sender, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var idol in e.NewItems.OfType<Idol>())
                    {
                        if (!config.PotentialData.Any(x => x.Name == idol.Name))
                        {
                            config.PotentialData.Add(new Potential { Category = idol.Category, Name = idol.Name });
                        }
                    }
                }
            };

            if(config.PotentialSortOptions==null)
            {
                config.PotentialSortOptions = new List<SortOption>()
                {
                     new SortOption { Column=nameof(IIdol.Name), Direction=ListSortDirection.Ascending }
                };
            }

            if(config.SimulatorConfig==null)
            {
                config.SimulatorConfig = new Core.SimulatorConfig
                {
                    EnableGuest=true,
                    EnableRoomEffect=true,
                    EnableSupportMembers=true,
                    GrooveType=IdolCategory.Cute,
                    UtilizeActualPattern=true,
                    GuestPotential = new Potential { Category = IdolCategory.All },
                    Runs=1000
                };
            }

            if(config.SimulatorConfig.Runs<=0)
            {
                config.SimulatorConfig.Runs = 1000;
            }

            return m_current = config;
        }

        public static void Reset()
        {
            if(File.Exists("app.config"))
            {
                File.Delete("app.config");
            }
        }
    }
}
