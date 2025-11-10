using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Entity.Dto
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public bool MustChangePassword { get; set; }

        public string TemporaryPassword { get; set; }
    }
}
