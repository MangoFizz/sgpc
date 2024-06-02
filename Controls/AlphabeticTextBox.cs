using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SGSC
{
    internal class AlphabeticTextBox : TextBox
    {
        private void previewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"[^a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$"))
            {
                e.Handled = true;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PreviewTextInput += previewTextInput;

        }
    }
}
