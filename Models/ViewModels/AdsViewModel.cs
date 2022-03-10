using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace assignment2.Models.ViewModels
{
    public class AdsViewModel
    {
        public Community Community { get; set; }
        public IEnumerable<Advertisement> Advertisements { get; set; }
        public Advertisement Ad { get; set; }
    }
}
