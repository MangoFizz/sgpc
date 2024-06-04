using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGSC
{
	public partial class Bank
	{
		public static Bank BankFromInterbankCodePrefix(string interbankCodePrefix)
		{
			using (sgscEntities db = new sgscEntities())
			{
				return db.Banks.Where(b => b.InterbankCodePrefix == interbankCodePrefix).FirstOrDefault();
			}
		}
	}
}
