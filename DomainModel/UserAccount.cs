using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class UserAccount
    {
        [Key]
        public int UserAccountId { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
