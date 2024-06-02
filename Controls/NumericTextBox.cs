using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SGSC
{
	internal class NumericTextBox : TextBox
	{
		private void previewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"[^0-9]+"))
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