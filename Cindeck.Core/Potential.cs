using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [ImplementPropertyChanged]
    public class Potential:IIdol, INotifyPropertyChanged
    {
        public string Name { get; set; }

        public IdolCategory Category { get; set; }

        public int Vocal { get; set; }

        public int Dance { get; set; }

        public int Visual { get; set; }

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
