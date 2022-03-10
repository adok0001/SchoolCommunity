using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace assignment2.Models
{
    public class Community
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "Registration Number")]
        public String Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public String Title { get; set; }

        [DataType(DataType.Currency)] 
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        public ICollection<CommunityMembership> Membership { get; set; }

    }
}
