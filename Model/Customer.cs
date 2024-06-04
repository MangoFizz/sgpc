using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGSC
{
    public partial class Customer
    {
        // Define properties
        public string Name { get; set; }
        public string FirstSurname { get; set; }
        public string SecondSurname { get; set; }

        public enum CivilStatuses : Int32
        {
            Single = 0,
            Married,
            Divorced,
            Widowed,
            Concubinage
        }

        // Property to get full name
        public string FullName
        {
            get
            {
                return $"{Name} {FirstSurname} {SecondSurname}";
            }
        }

        // Method to get civil status as a string
        public static string GetCivilStatusString(CivilStatuses status)
        {
            switch (status)
            {
                case CivilStatuses.Single:
                    return "Soltero(a)";
                case CivilStatuses.Married:
                    return "Casado(a)";
                case CivilStatuses.Divorced:
                    return "Divorciado(a)";
                case CivilStatuses.Widowed:
                    return "Viudo(a)";
                case CivilStatuses.Concubinage:
                    return "Union libre";
                default:
                    return "Desconocido";
            }
        }
    }
}
