using System.ComponentModel.DataAnnotations;

namespace Competition3.Enum
{
    public enum EnPaymentStatus
    {
        [Display(Name = "موفقیت آمیز")]
        Success=0,
        
        [Display(Name = "در انتظار تایید")]
        Pending,
        
        [Display(Name = "ناموفق")]
        Failed
    }
}