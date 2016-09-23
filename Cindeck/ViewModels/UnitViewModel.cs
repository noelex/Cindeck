using Cindeck.Core;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Cindeck.ViewModels
{
    [ImplementPropertyChanged]
    class UnitViewModel:IViewModel, INotifyPropertyChanged
    {
        private AppConfig m_config;
        private MainViewModel m_mvm;

        public event PropertyChangedEventHandler PropertyChanged;

        public UnitViewModel(AppConfig config, MainViewModel mvm)
        {
            m_config = config;
            m_mvm = mvm;

            SendToSlotCommand = new DelegateCommand<string>(SendToSlot, x => SelectedIdol != null);
            SaveCommand = new DelegateCommand(Save, () => !string.IsNullOrEmpty(UnitName));
            DeleteCommand = new DelegateCommand(Delete, () => Units.Contains(SelectedUnit));
            MoveToSlotCommand = new DelegateCommand<string>(MoveToSlot, CanMoveToSlot);
            ResetSlotCommand = new DelegateCommand<string>(ResetSlot, CanResetSlot);
            HighlightCommand = new DelegateCommand<string>(Highlight, CanHighlight);
            CopyIidCommand = new DelegateCommand(CopyIid, () => SelectedIdol != null);
            SetGuestCenterCommand = new DelegateCommand(SetGuestCenter, () => SelectedIdol != null);
            CopyIidFromSlotCommand = new DelegateCommand<string>(CopyIidFromSlot);
            SetGuestCenterFromSlotCommand = new DelegateCommand<string>(SetGuestCenterFromSlot);

            Idols = new ListCollectionView(m_config.OwnedIdols);
            Filter = new IdolFilter(config, Idols, false);
            Filter.SetConfig(config.UnitIdolFilterConfig);

            Units = m_config.Units;

            TemporalUnit = new Unit();
            SelectedUnit = Units.FirstOrDefault();

            foreach (var option in config.UnitIdolSortOptions)
            {
                Idols.SortDescriptions.Add(option.ToSortDescription());
            }
        }

        public ObservableCollection<Unit> Units
        {
            get;
        }

        public Unit TemporalUnit
        {
            get;
            set;
        }

        public Unit SelectedUnit
        {
            get;
            set;
        }

        public ICollectionView Idols
        {
            get;
        }

        public IdolFilter Filter
        {
            get;
        }

        public OwnedIdol SelectedIdol
        {
            get;
            set;
        }

        public string UnitName
        {
            get;
            set;
        }

        public DelegateCommand<string> SendToSlotCommand
        {
            get;
        }

        private void SendToSlot(string slot)
        {
            if(TemporalUnit.AlreadyInUnit(SelectedIdol))
            {
                MessageBox.Show("選択したアイドルはすでにこのユニットに編成されています");
            }
            else
            {
                TemporalUnit.GetType().GetProperty(slot).SetValue(TemporalUnit, SelectedIdol);
            }
        }

        public DelegateCommand SaveCommand
        {
            get;
        }

        private void Save()
        {
            var target = Units.FirstOrDefault(x => x.Name == UnitName);
            if (target==null)
            {
                var newUnit= TemporalUnit.Clone();
                newUnit.Name = UnitName;
                Units.Insert(0, newUnit);
                SelectedUnit = newUnit;
            }
            else
            {
                target.CopyFrom(TemporalUnit);
            }
            m_config.Save();
        }

        public DelegateCommand DeleteCommand
        {
            get;
        }

        private void Delete()
        {
            Units.Remove(SelectedUnit);
            SelectedUnit = Units.FirstOrDefault();
            if (SelectedUnit == null)
            {
                TemporalUnit = new Unit();
            }
            m_config.Save();
        }

        public DelegateCommand<string> MoveToSlotCommand
        {
            get;
        }

        private void MoveToSlot(string target)
        {
            var s = target.Split(',');
            var source = s[0];
            var dest = s[1];

            var srcIdol=TemporalUnit.GetType().GetProperty(source).GetValue(TemporalUnit);
            var dstIdol= TemporalUnit.GetType().GetProperty(dest).GetValue(TemporalUnit);

            TemporalUnit.GetType().GetProperty(source).SetValue(TemporalUnit, dstIdol);
            TemporalUnit.GetType().GetProperty(dest).SetValue(TemporalUnit, srcIdol);
        }

        private bool CanMoveToSlot(string target)
        {
            return TemporalUnit.GetType().GetProperty(target.Split(',')[0]).GetValue(TemporalUnit) != null;
        }

        public DelegateCommand<string> ResetSlotCommand
        {
            get;
        }

        private void ResetSlot(string target)
        {
            TemporalUnit.GetType().GetProperty(target).SetValue(TemporalUnit,null);
        }

        private bool CanResetSlot(string target)
        {
            return TemporalUnit.GetType().GetProperty(target).GetValue(TemporalUnit) != null;
        }

        public DelegateCommand<string> HighlightCommand
        {
            get;
        }

        private void Highlight(string target)
        {
            SelectedIdol = TemporalUnit.GetType().GetProperty(target).GetValue(TemporalUnit) as OwnedIdol;
        }

        private bool CanHighlight(string target)
        {
            return TemporalUnit.GetType().GetProperty(target).GetValue(TemporalUnit) != null;
        }

        public DelegateCommand CopyIidCommand
        {
            get;
        }

        private void CopyIid()
        {
            try
            {
                Clipboard.SetText(SelectedIdol.Iid.ToString("x8"));
            }
            catch
            {

            }
        }

        public DelegateCommand SetGuestCenterCommand
        {
            get;
            private set;
        }

        private void SetGuestCenter()
        {
            m_mvm.Simulation.GuestIid = SelectedIdol.Iid;
        }

        public DelegateCommand<string> CopyIidFromSlotCommand
        {
            get;
        }

        private void CopyIidFromSlot(string slot)
        {
            try
            {
                Clipboard.SetText(((IIdol)TemporalUnit.GetType().GetProperty(slot).GetValue(TemporalUnit)).Iid.ToString("x8"));
            }
            catch
            {

            }
        }

        public DelegateCommand<string> SetGuestCenterFromSlotCommand
        {
            get;
            private set;
        }

        private void SetGuestCenterFromSlot(string slot)
        {
            m_mvm.Simulation.GuestIid = ((IIdol)TemporalUnit.GetType().GetProperty(slot).GetValue(TemporalUnit)).Iid;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == nameof(SelectedIdol))
            {
                SendToSlotCommand.RaiseCanExecuteChanged();
                CopyIidCommand.RaiseCanExecuteChanged();
            }
            else if (propertyName == nameof(UnitName))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
            else if (propertyName == nameof(SelectedUnit))
            {
                if (SelectedUnit != null)
                {
                    TemporalUnit.CopyFrom(SelectedUnit);
                }
                DeleteCommand.RaiseCanExecuteChanged();
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsIdolInUse(OwnedIdol idol)
        {
            return TemporalUnit.OccupiedByUnit(idol) || m_config.Units.Any(x => x.OccupiedByUnit(idol));
        }

        public void RemoveIdolFromUnits(OwnedIdol idol)
        {
            TemporalUnit.RemoveIdol(idol);
            foreach(var x in Units)
            {
                x.RemoveIdol(idol);
            }
        }

        public void Dispose()
        {
            m_config.UnitIdolSortOptions.Clear();
            foreach (var x in Idols.SortDescriptions)
            {
                m_config.UnitIdolSortOptions.Add(x.ToSortOption());
            }
            m_config.UnitIdolFilterConfig = Filter.GetConfig();
        }

        public void OnActivate()
        {
            Idols.Refresh();
            TemporalUnit.Timestamp = DateTime.Now;
        }

        public void OnDeactivate()
        {
            
        }
    }
}
