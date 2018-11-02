using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class ReservationDAL
    {
        string connectionString;
        private string SQL_CreateRes = "Insert Into reservation (site_id, name, from_date, to_date) VALUES (@siteId, @name, @fromdate, @todate);";
        private string SQL_ReturnConfirmation = "SELECT top 1 reservation_id from reservation WHERE site_id = @siteId AND name = @name AND from_date = @fromdate AND to_date = @todate order by reservation_id DESC;";

        public ReservationDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int CreateReservation(int siteId, string name, string startDate, string endDate)
        {
            int confirmationNumber = 0;
            CampsiteDAL campsiteDAL = new CampsiteDAL(connectionString);
            List<Campsite> campsites = campsiteDAL.GetCampsites(siteId, startDate, endDate);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SQL_CreateRes, conn))
                    {
                        cmd.Parameters.AddWithValue("@siteId", siteId);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@fromdate", startDate);
                        cmd.Parameters.AddWithValue("@todate", endDate);

                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(SQL_ReturnConfirmation, conn))
                    {
                        cmd.Parameters.AddWithValue("@siteId", siteId);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@fromdate", startDate);
                        cmd.Parameters.AddWithValue("@todate", endDate);

                        confirmationNumber = (int)cmd.ExecuteScalar();
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return confirmationNumber;
        }
    }
}
