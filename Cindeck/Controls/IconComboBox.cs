using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cindeck.Controls
{
    public class IconComboBox:ComboBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textBox = Template.FindName("PART_EditableTextBox", this) as TextBox;
            if (textBox != null)
            {
                textBox.Template = (ControlTemplate)FindResource("ComboBoxTextBoxTemplate");
            }
        }
    }
}
