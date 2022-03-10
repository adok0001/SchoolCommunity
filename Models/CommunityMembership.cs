using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace assignment2.Models
{
    public class CommunityMembership
    {
        //public uint Id { get; set; }   
        public int StudentId { get; set; }

        public Student Student { get; set; }

        public string CommunityId { get; set; }

        public Community Community { get; set; }
    }
}
