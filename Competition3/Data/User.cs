using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Competition3.Data
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        
        #region relations

        public ICollection<Payment> Payments { get; set; }

        #endregion
    }
}