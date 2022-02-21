using System;
using Competition3.Enum;

namespace Competition3.Data
{
    public class Payment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SystemCode { get; set; }

        public string? RefCode { get; set; }

        public int Amount { get; set; }
        
        public string Description { get; set; }

        public EnPaymentStatus Status { get; set; }

        public DateTime PaymentDate { get; set; }

        #region relations

        public User User { get; set; }

        #endregion
    }
}