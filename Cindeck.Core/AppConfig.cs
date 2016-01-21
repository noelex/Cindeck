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

        private AppConfig()
        {
            ImplementedIdols = new ObservableCollection<Idol>();
            OwnedIdols = new ObservableCollection<OwnedIdol>();
            Units = new ObservableCollection<Unit>();

            ImplementedIdolSortOptions = new List<SortOption>() {
                new SortOption { Column=nameof(IIdol.Rarity), Direction=ListSortDirection.Descending },
                new SortOption { Column=nameof(IIdol.ImplementationDate), Direction=ListSortDirection.Descending }
            };
            OwnedIdolSortOptions = new List<SortOption>()
            {
                 new SortOption { Column=nameof(IIdol.Rarity), Direction=ListSortDirection.Descending },
                new SortOption { Column=nameof(IIdol.ImplementationDate), Direction=ListSortDirection.Descending }
            };
            UnitIdolSortOptions = new List<SortOption>()
            {
                 new SortOption { Column=nameof(IIdol.Rarity), Direction=ListSortDirection.Descending },
                new SortOption { Column=nameof(IIdol.ImplementationDate), Direction=ListSortDirection.Descending }
            };
            NextOid = 1;
        }

        public string Version => "v0.7";

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

        public static AppConfig Load()
        {
            if(!File.Exists("app.config"))
            {
                return new AppConfig();
            }
            using (var fs = File.OpenRead("app.config"))
            {
                return (AppConfig)m_serializer.ReadObject(fs);
            }
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