namespace Capstone.Models

{
    public class Campsite
    {
        public int SiteId { get; set; }
        public int CampgroundId { get; set; }
        public int SiteNum { get; set; }
        public int MaxOccupancy { get; set; }
        public string Accessible { get; set; }
        public string MaxRVLength { get; set; }
        public string Utilities { get; set; }
        public decimal DailyFee { get; set; }

        //updates values of accessible, rv length, and utilities to be more user-friendly
        public override string ToString()
        {
            if (Accessible == "False")
            {
                Accessible = "No";
            }
            else if (Accessible == "True")
            {
                Accessible = "Yes";
            }

            if (Utilities == "False")
            {
                Utilities = "No";
            }
            else if (Utilities == "True")
            {
                Utilities = "Yes";
            }

            if (MaxRVLength == "0")
            {
                MaxRVLength = "N/A";
            }

            return SiteId.ToString().PadRight(11) + MaxOccupancy.ToString().PadRight(13) + Accessible.PadRight(15) + MaxRVLength.PadRight(15) + Utilities.PadRight(11) + "$";
        }
    }
}

