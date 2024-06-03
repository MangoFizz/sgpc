using System;

namespace SGSC
{
    public class CreditPolicyWithStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string DateString => EffectiveDate.ToString("dd/MM/yyyy");
        public string Status { get; set; }

        public CreditPolicyWithStatus(int id, string name, string description, DateTime effectiveDate, string status)
        {
            Id = id;
            Name = name;
            Description = description;
            EffectiveDate = effectiveDate;
            Status = status;
        }
    }
}
