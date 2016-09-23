using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [ImplementPropertyChanged, DataContract]
    public class Potential:IIdol, INotifyPropertyChanged
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IdolCategory Category { get; set; }

        [DataMember]
        public int Vocal { get; set; }

        [DataMember]
        public int Dance { get; set; }

        [DataMember]
        public int Visual { get; set; }

        [DataMember]
        public int Life { get; set; }

        public ICenterEffect CenterEffect
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Iid
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ImplementationDate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Label
        {
            get
            {
                return Name;
            }
        }

        public string LabeledName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Rarity Rarity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISkill Skill
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int TotalAppeal
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if(PropertyChanged!=null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
