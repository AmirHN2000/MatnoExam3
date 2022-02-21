using System;
using Competition3.Enum;

namespace Competition3.ViewModels
{
    public class PaymentVm
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }
        
        public string RefCode { get; set; }

        public string Amount { get; set; }
        
        public EnPaymentStatus Status { get; set; }
        public string NStatus { get; set; }
        
        public string Description { get; set; }

        public DateTime PaymentDate { get; set; }
        
        public string NPaymentDate { get; set; }
    }
}