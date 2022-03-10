using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace assignment2.Models.ViewModels
{
    public class StudentMembershipViewModel
    {
        public Student Student { get; set; }
        public IEnumerable<CommunityMembershipViewModel> Memberships { get; set; }

    }
}
