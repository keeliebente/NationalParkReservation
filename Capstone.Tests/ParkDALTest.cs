using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Transactions;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class ParkDALTest
    {
        private TransactionScope tran;

        private string connectionString = @"Data Source =.\sqlexpress; Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int numberOfParks = 0;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd;
                conn.Open();

                cmd = new SqlCommand("SELECT COUNT(*) FROM park;", conn);
                numberOfParks = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("INSERT INTO park (name, location, establish_date, area, visitors, description) VALUES ('Test Park', 'Ohio', '2001-10-10', 11111, 11111, 'desc');", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod()]
        public void GetParks()
        {
            ParkDAL parkDAL = new ParkDAL(connectionString);
            List<Park> parks = parkDAL.GetParks();
            Assert.AreEqual(numberOfParks + 1, parks.Count);
        }
    }
}

