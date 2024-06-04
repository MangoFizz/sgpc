using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGSC
{
    public partial class Employee
    {
        public enum EmployeeRoles : short
        {
            Admin = 0,
            CreditAdvisor,
            CreditAnalyst,
            CollectionExecutive
        }

        public static string GetRoleName(short role)
        {
            switch (role)
            {
                case (short)EmployeeRoles.Admin:
                    return "Administrador(a)";
                case (short)EmployeeRoles.CreditAdvisor:
                    return "Asesor(a) de crédito";
                case (short)EmployeeRoles.CreditAnalyst:
                    return "Analista de crédito";
                case (short)EmployeeRoles.CollectionExecutive:
                    return "Ejecutivo(a) de cobranza";
                default:
                    return "Rol no implementado";
            }
        }

        public string FullName
        {
            get
            {
                return $"{Name} {FirstSurname} {SecondSurname}";
            }
        }
    }
}
