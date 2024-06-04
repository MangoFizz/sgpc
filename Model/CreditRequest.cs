using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGSC
{
    public partial class CreditRequest
    {


        public enum RequestStatus
        {
            Captured,
            Pending,
            Rejected,
            WaitingForCorrection,
            Authorized,
            Paid,
            
        }

        public enum TimeIntervals
        {
            Fortnightly,
            Monthly
        }

        public static string TimeIntervalToString(TimeIntervals timeInterval)
        {
			switch (timeInterval)
            {
				case TimeIntervals.Fortnightly:
					return "Quincenal";
				case TimeIntervals.Monthly:
					return "Mensual";
				default:
					return "Intervalo de tiempo desconocido";
			}
		}

        public static string RequestStatusToString(RequestStatus requestStatus)
        {
            switch (requestStatus)
            {
                case RequestStatus.Paid:
                    return "Pagado";
                case RequestStatus.Authorized:
                    return "Aprobada";
                case RequestStatus.Rejected:
                    return "Rechazada";
                case RequestStatus.Captured:
                    return "Capturada";
                case RequestStatus.WaitingForCorrection:
                    return "Corrección";
                 case RequestStatus.Pending:
                    return "Pendiente";
                default:
                    return "Estado de solicitud desconocido";
            }
        }
    }
}
