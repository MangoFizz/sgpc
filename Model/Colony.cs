using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGSC
{
	public partial class Colony
	{
		public override string ToString()
		{
			return $"{Name}";
		}

		public static List<Colony> GetColonies(string keyword, int amount)
		{
			using (sgscEntities db = new sgscEntities())
			{
				var colonies = new List<Colony>();
				
				// Get X colonies that match the keyword discarting duplicated zip codes
				colonies = db.Colonies
					.Where(c => c.Zipcode.ToString().StartsWith(keyword))
					.GroupBy(c => c.Zipcode)
					.Select(c => c.FirstOrDefault())
					.Take(amount)
					.ToList();

				return colonies;
			}
		}

		public static bool IsValidZipCode(string zipCode)
		{
			using (sgscEntities db = new sgscEntities())
			{
				return db.Colonies.Any(c => c.Zipcode.ToString() == zipCode);
			}
		}

		public static List<Colony> GetColoniesByZipcode(string zipcode)
		{
			using (sgscEntities db = new sgscEntities())
			{
				return db.Colonies
					.Where(c => c.Zipcode.ToString() == zipcode)
					.ToList();
			}
		}
	}
}
