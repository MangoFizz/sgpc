//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SGSC
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkCenter
    {
        public int WorkCenterId { get; set; }
        public string CenterName { get; set; }
        public string Street { get; set; }
        public string Colony { get; set; }
        public Nullable<int> InnerNumber { get; set; }
        public Nullable<int> OutsideNumber { get; set; }
        public Nullable<int> ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<int> CustomerId { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
