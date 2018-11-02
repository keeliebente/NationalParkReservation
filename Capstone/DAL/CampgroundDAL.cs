using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampgroundDAL
    {
        List<Campground> campgrounds = new List<Campground>();

        string connectionString;
        private string SQL_Getcampgrounds = "select campground.* from park join campground ON (park.park_id = campground.park_id) WHERE park.park_id = @parkid;";

        public CampgroundDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Campground> GetCampground(int parkId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_Getcampgrounds, conn);

                    cmd.Parameters.AddWithValue("@parkid", parkId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Campground campground = new Campground();

                        campground.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                        campground.ParkId = Convert.ToInt32(reader["park_id"]);
                        campground.Name = Convert.ToString(reader["name"]);
                        campground.OpenMonth = Convert.ToInt32(reader["open_from_mm"]);
                        campground.ClosingMonth = Convert.ToInt32(reader["open_to_mm"]);
                        campground.DailyFee = Convert.ToDecimal(reader["daily_fee"]);

                        campgrounds.Add(campground);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return campgrounds;
        }
        //checks that the campground is open on the dates the user selected 
        //start date must fall after the first day of opening month, end date must be before the last day of closing month
        public bool CampgroundCheck(int campgroundId, int parkId, string startDate, string endDate)
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campground> campgrounds = campgroundDAL.GetCampground(parkId);

            bool isAvailable = true;

            DateTime dtStartDate = Convert.ToDateTime(startDate);
            DateTime dtEndDate = Convert.ToDateTime(endDate);

            for (int i = 0; i < campgrounds.Count; i++)
            {
                if (campgrounds[i].CampgroundId == campgroundId)
                {   //converts closing month to a date/time, finds the last day by going up 1 month and down 1 day
                    DateTime getCloseMonth = Convert.ToDateTime($"{campgrounds[i].ClosingMonth}/01/{dtEndDate.Year}");
                    DateTime closeDate = getCloseMonth.AddMonths(1).AddDays(-1);

                    DateTime openDate = Convert.ToDateTime($"{campgrounds[i].OpenMonth}/01/{dtStartDate.Year}");

                    if (dtStartDate < openDate || dtEndDate > closeDate)
                    {
                        isAvailable = false;
                    }
                }
            }
            return isAvailable;
        }
    }
}
