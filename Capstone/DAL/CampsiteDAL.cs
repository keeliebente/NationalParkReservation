using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampsiteDAL
    {
        string connectionString;
        private string SQL_GetSites = "Select TOP 5 s.*, c.daily_fee FROM site s JOIN campground c ON (s.campground_id = c.campground_id) WHERE s.campground_id = @campgroundid AND s.site_id NOT IN(SELECT site_id from reservation WHERE @requested_start < to_date AND @requested_end > from_date);";
        private string SQL_GetSitesAll = "Select * FROM site s WHERE s.campground_id = @campgroundid AND s.site_id NOT IN(SELECT site_id from reservation WHERE @requested_start < to_date AND @requested_end > from_date);";

        public CampsiteDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //returns the available campsites based on the campground and dates provided 
        public List<Campsite> GetCampsites(int campgroundId, string startDate, string endDate)
        {
            List<Campsite> campsites = new List<Campsite>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GetSites, conn);

                    cmd.Parameters.AddWithValue("@campgroundid", campgroundId);
                    cmd.Parameters.AddWithValue("@requested_start", startDate);
                    cmd.Parameters.AddWithValue("@requested_end", endDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Campsite campsite = new Campsite();

                        campsite.SiteId = Convert.ToInt32(reader["site_id"]);
                        campsite.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                        campsite.SiteNum = Convert.ToInt32(reader["site_number"]);
                        campsite.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                        campsite.Accessible = Convert.ToString(reader["accessible"]);
                        campsite.MaxRVLength = Convert.ToString(reader["max_rv_length"]);
                        campsite.Utilities = Convert.ToString(reader["utilities"]);
                        campsite.DailyFee = Convert.ToDecimal(reader["daily_fee"]);

                        campsites.Add(campsite);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return campsites;
        }

        //returns all available campgrounds (not just the top 5- for testing purposes)
        public List<Campsite> AllAvailableCampsites(int campgroundId, string startDate, string endDate)
        {
            List<Campsite> campsites = new List<Campsite>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GetSitesAll, conn);

                    cmd.Parameters.AddWithValue("@campgroundid", campgroundId);
                    cmd.Parameters.AddWithValue("@requested_start", startDate);
                    cmd.Parameters.AddWithValue("@requested_end", endDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Campsite campsite = new Campsite();

                        campsite.SiteId = Convert.ToInt32(reader["site_id"]);
                        campsite.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                        campsite.SiteNum = Convert.ToInt32(reader["site_number"]);
                        campsite.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                        campsite.Accessible = Convert.ToString(reader["accessible"]);
                        campsite.MaxRVLength = Convert.ToString(reader["max_rv_length"]);
                        campsite.Utilities = Convert.ToString(reader["utilities"]);
                        campsite.DailyFee = Convert.ToDecimal(reader["daily_fee"]);

                        campsites.Add(campsite);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return campsites;
        }
    }
}

