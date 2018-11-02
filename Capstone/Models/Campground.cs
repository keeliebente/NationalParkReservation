using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Campground
    {
        public int CampgroundId { get; set; }
        public int ParkId { get; set; }
        public string Name { get; set; }
        public int OpenMonth { get; set; }
        public int ClosingMonth { get; set; }
        public decimal DailyFee { get; set; }

        public override string ToString()
        {
            return CampgroundId.ToString().PadRight(5) + Name.PadRight(35) + OpenMonth.ToString().PadRight(10) + ClosingMonth.ToString().PadRight(10) + "$" + Math.Round(DailyFee * 1.00m, 2, MidpointRounding.AwayFromZero);
        }
    }
}
