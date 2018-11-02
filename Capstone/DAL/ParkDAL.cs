using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class ParkDAL
    {
        string connectionString;
        private string SQL_Getparks = "SELECT * FROM park order by name;";
        private string SQL_GetChosenPark = "SELECT * FROM park WHERE park_id = @parkid";

        public ParkDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //gets list of ALL parks for the user to choose from 
        public List<Park> GetParks()
        {
            List<Park> parks = new List<Park>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_Getparks, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Park park = new Park();

                        park.Id = Convert.ToInt32(reader["park_id"]);
                        park.Name = Convert.ToString(reader["name"]);

                        parks.Add(park);
                    }
                } 
            }
            catch (Exception ex)
            {

            }
            return parks;
        }
        //takes in parameters from user selection to return info on the specific park 
        public Park GetChosenPark(int parkId)
        {
            Park park = new Park();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GetChosenPark, conn);

                    cmd.Parameters.AddWithValue("@parkid", parkId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        park.Id = Convert.ToInt32(reader["park_id"]);
                        park.Name = Convert.ToString(reader["name"]);
                        park.Location = Convert.ToString(reader["location"]);
                        park.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
                        park.Area = Convert.ToInt32(reader["area"]);
                        park.AnnualVisitors = Convert.ToInt32(reader["visitors"]);
                        park.Description = Convert.ToString(reader["description"]);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return park;
        }
    }
}
