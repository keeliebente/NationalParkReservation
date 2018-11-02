using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Transactions;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class CampgroundDALTest
    {
        private TransactionScope tran;

        private string connectionString = @"Data Source =.\sqlexpress; Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int testId;
        private int numberOfCampgroundsPark1;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd;
                conn.Open();

                cmd = new SqlCommand("SELECT COUNT(*) FROM campground WHERE park_id = 1", conn);
                numberOfCampgroundsPark1 = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("INSERT INTO campground (park_id, name, open_from_mm, open_to_mm, daily_fee) VALUES (1, 'Test Ground', 6, 12, 35);", conn);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT campground_id FROM campground WHERE name = 'Test Ground';", conn);
                testId = (int)cmd.ExecuteScalar();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod()]
        public void GetCampgroundsTest()
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campground> campgrounds = campgroundDAL.GetCampground(1);
            Assert.AreEqual(numberOfCampgroundsPark1 + 1, campgrounds.Count);
        }

        [TestMethod]
        public void TestAvailableCampgrounds()
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            campgroundDAL.GetCampground(1);
            bool result = campgroundDAL.CampgroundCheck(testId, 1, "2018-06-10", "2018-06-14");
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestAvailableCampgroundsFail()
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            campgroundDAL.GetCampground(1);
            bool result = campgroundDAL.CampgroundCheck(testId, 1, "2018-05-10", "2018-06-14");
            Assert.AreEqual(false, result);
        }
    }
}
