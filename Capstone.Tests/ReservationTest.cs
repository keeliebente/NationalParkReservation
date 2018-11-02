using Capstone.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Transactions;
using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.Tests
{
    [TestClass]
    public class ReservationDALTest
    {
        private TransactionScope tran;

        private string connectionString = @"Data Source =.\sqlexpress; Initial Catalog = NationalParkReservation; Integrated Security = True";
        private int numberOfReservations;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd;
                conn.Open();

                cmd = new SqlCommand("SELECT COUNT(*) FROM reservation;", conn);
                numberOfReservations = (int)cmd.ExecuteScalar();

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
        public void Reservation_True()
        {
            ReservationDAL reservationDAL = new ReservationDAL(connectionString);
            CampsiteDAL campsteDAL = new CampsiteDAL(connectionString);

            List<Campsite> campsites = campsteDAL.AllAvailableCampsites(1, "2018-12-12", "2018-12-14");
            bool result = false;

            if (campsites.Count > 1)
            {
                if (reservationDAL.CreateReservation(1, "Hart Family", "2018-10-10", "2018-10-10") > 1)
                {
                    result = true;
                }
                Assert.AreEqual(true, result);
            }
        }
        [TestMethod]
        public void Reservation_Fail()
        {
            ReservationDAL reservationDAL = new ReservationDAL(connectionString);
            CampsiteDAL campsteDAL = new CampsiteDAL(connectionString);

            List<Campsite> campsites = campsteDAL.AllAvailableCampsites(1,"2018-12-12", "2018-12-14");
            bool result = false;

            if (campsites.Count > 1)
            {
                if (reservationDAL.CreateReservation(1, "Hart Family", "2018-12-12", "2018-12-14") > 1)
                {
                    result = true;
                }
            }
            Assert.AreEqual(false, result);
        }

    }
}
