using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGSC.Utils
{
	public interface IBankAccountSearchCallback
	{
		void OnBankAccountSelected(int bankAccountId);
	}
}
