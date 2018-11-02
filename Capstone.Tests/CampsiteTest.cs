using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Transactions;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class CampsiteDALTest
    {
        private TransactionScope tran;

        private string connectionString = @"Data Source =.\sqlexpress; Initial Catalog = NationalParkReservation; Integrated Security = True";

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd;
                conn.Open();

                cmd = new SqlCommand("INSERT INTO site (campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities) VALUES (1, 12, 6, 1, 20, 1);", conn);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date) VALUES (1, 'Test Fam', '2018-12-12', '2018-12-14');", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod]
        public void TestAvailableCampsites()
        {
            CampsiteDAL campsiteDAL = new CampsiteDAL(connectionString);
            List<Campsite> campsites = campsiteDAL.GetCampsites(1, "2019-10-10", "2019-10-12");
            bool result = false;

            for (int i = 0; i < campsites.Count; i++)
            {
                if (campsites[i].SiteId == 1)
                {
                    result = true;
                }
            }
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestAvailableCampsitesFail()
        {
            CampsiteDAL campsiteDAL = new CampsiteDAL(connectionString);
            List<Campsite> campsites = campsiteDAL.GetCampsites(1, "2018-12-12", "2018-12-14");
            bool result = true;

            for (int i = 0; i < campsites.Count; i++)
            {
                if (campsites[i].SiteId == 1)
                {
                    result = false;
                }
            }

            Assert.AreEqual(true, result);
        }
    }
}
