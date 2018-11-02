using Capstone.DAL;
using Capstone.Models;
using System;
using System.Collections.Generic;

namespace Capstone
{
    public class CLI
    {
        public string connectionString = @"Data Source =.\sqlexpress; Initial Catalog = NationalParkReservation; Integrated Security = True";

        public void Run()
        {
            bool isDone = false;

            ParkDAL parkDAL = new ParkDAL(connectionString);
            List<Park> parks = parkDAL.GetParks();

            do
            {
                ParkInfoMenu(parks);
                try
                {
                    int userSelection = Int32.Parse(Console.ReadLine());

                    if (userSelection == 0)
                    {
                        isDone = true;
                        return;
                    }
                    else if (!IsValidParkId(userSelection))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Sorry, that is not a valid park ID.");
                    }
                    else
                    {
                        InformationForSelectedPark(userSelection);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Not a valid input- please try again");
                }
            }
            while (!isDone);
        }

        public void ParkInfoMenu(List<Park> parks)
        {

            Console.WriteLine("\nSelect a Park for Further Details");
            Console.WriteLine();

            DisplayAllParks(parks);

            Console.WriteLine("0) Quit");
            Console.WriteLine();
            return;
        }

        public void DisplayAllParks(List<Park> parks)
        {

            foreach (Park park in parks)
            {
                Console.WriteLine($"{park.Id}) {park.Name}");
            }
        }

        public void InformationForSelectedPark(int userInput)
        {
            ParkDAL parkDAL = new ParkDAL(connectionString);
            Park park = parkDAL.GetChosenPark(userInput);

            bool isDone = false;

            do
            {
                Console.WriteLine(park.ToString());

                isDone = ParkInformationMenu(park); //returns to previous menu if user presses zero in ParkInformationMenu
            }
            while (!isDone);
            return;
        }

        public bool ParkInformationMenu(Park selectedPark)
        {
            bool isDone = false;

            Console.WriteLine("");
            Console.WriteLine("Please Select a Command");
            Console.WriteLine("1) View Campgrounds");
            Console.WriteLine("0) Return to Previous Screen");
            Console.WriteLine();

            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    DisplayCampgroundInformation(selectedPark);
                    DisplayCampgroundMenu(selectedPark);
                    break;
                case "0":
                    isDone = true;
                    break;
                default:
                    Console.WriteLine("");
                    Console.WriteLine("Sorry, that is not a valid menu option.");
                    break;
            }
            return isDone;
        }

        public void DisplayCampgroundInformation(Park selectedPark)
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campground> campgrounds = campgroundDAL.GetCampground(selectedPark.Id);

            Console.WriteLine($"\n{selectedPark.Name} National Park Grounds");
            Console.WriteLine("ID   Name                              Open Mo  Close Mo  Daily Fee");
            Console.WriteLine("-------------------------------------------------------------------");
            foreach (Campground campground in campgrounds)
            {
                Console.WriteLine(campground.ToString());
            }
            return;
        }

        public void DisplayCampgroundMenu(Park selectedPark)
        {
            bool isDone = false;
            do
            {
                Console.WriteLine("\nSelect a Command");
                Console.WriteLine("1) Search for Available Reservation");
                Console.WriteLine("0) Return to Previous Screen");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        AvailableResMenu(selectedPark);
                        isDone = true;
                        break;
                    case "0":
                        isDone = true;
                        break;
                    default:
                        Console.WriteLine("");
                        Console.WriteLine("Sorry, that is not a valid menu option.");
                        break;
                }
            }
            while (!isDone);
        }

        public void AvailableResMenu(Park selectedPark)
        {
            bool isDone = false;
            int campgroundId;
            do
            {
                DisplayCampgroundInformation(selectedPark);


                Console.WriteLine("\nPlease Enter the Campground ID or 0 to Return to Previous Screen");
                try
                {
                    campgroundId = Int32.Parse(Console.ReadLine());

                    if (campgroundId == 0)
                    {
                        isDone = true;
                    }
                    else if (!IsValidCampgroundId(selectedPark, campgroundId))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Sorry, that is not a valid campground ID for this park.");
                    }
                    else
                    {
                        DatesPrompt(campgroundId, selectedPark);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Not a valid input- please try again");
                }
            }
            while (!isDone);

            return;
        }

        public void DatesPrompt(int campgroundId, Park selectedPark)
        {
            string fromDate;
            string toDate;
            bool isDone = false;

            CampsiteDAL campsiteDAL = new CampsiteDAL(connectionString);
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campsite> campsites;
            try
            {
                do
                {
                    Console.WriteLine("\nWhat is the arrival date? (yyyy-mm-dd)");
                    fromDate = Console.ReadLine();

                    Console.WriteLine("\nWhat is the departure date? (yyyy-mm-dd)");
                    toDate = Console.ReadLine();

                    if (!AreDatesInFuture(fromDate, toDate) || !ArriveBeforeDepartCheck(fromDate, toDate))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Sorry, those are not valid dates.");
                    }
                    else if (!campgroundDAL.CampgroundCheck(campgroundId, selectedPark.Id, fromDate, toDate))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Sorry, the campground is not open during those dates.");
                    }
                    else
                    {
                        isDone = true;
                    }
                }
                while (!isDone);

                campsites = campsiteDAL.GetCampsites(campgroundId, fromDate, toDate);

                if (campsites.Count == 0)
                {
                    NoCampsitesAvailablePrompt(campsites);
                }
                else
                {
                    DisplayAvailableReservations(campsites, toDate, fromDate);
                    DisplayCreateReservationMenu(campsites, fromDate, toDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Not a valid value- please try again");
            }
        }

        public void NoCampsitesAvailablePrompt(List<Campsite> campsites)
        {
            Console.WriteLine("");
            Console.WriteLine("There are no available sites for those dates. Please try again.");
            return;
        }

        public void DisplayAvailableReservations(List<Campsite> availableSites, string todate, string fromdate)
        {
            Console.WriteLine("");
            Console.WriteLine("Results Matching Your Search Criteria");
            Console.WriteLine("");
            Console.WriteLine(String.Format("Site No.   Max Occup.   Accessible?    Max RV Length  Utility   Cost"));

            foreach (Campsite campsite in availableSites)
            {
                Console.WriteLine(campsite.ToString() + TotalCostCalculator(fromdate, todate, campsite.DailyFee));
            }
            return;
        }

        public void DisplayCreateReservationMenu(List<Campsite> availableSites, string fromDate, string toDate)
        {
            int reservationId;

            try
            {
                Console.WriteLine("Which site should be reserved? (Enter 0 to cancel)");
                int siteId = Int32.Parse(Console.ReadLine());

                if (siteId == 0)
                {
                    return;
                }
                else if (!IsValidCampsiteId(availableSites, siteId))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Sorry, that is not an available site ID");
                }

                Console.WriteLine("What name should the reservation be made under?");
                string reservationName = Console.ReadLine();

                ReservationDAL reservationDAL = new ReservationDAL(connectionString);
                reservationId = reservationDAL.CreateReservation(siteId, reservationName, fromDate, toDate);

                if (reservationId == 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Reservation was not made.");
                }
                else
                {
                    Console.WriteLine($"The reservation has been made and the confirmation id is {reservationId}");
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Not a valid value- please try again");
            }
        }

        public bool AreDatesInFuture(string fromDate, string toDate)
        {
            bool isFuture = true;

            DateTime StartDate = DateTime.Parse(fromDate);
            DateTime EndDate = DateTime.Parse(toDate);

            if (StartDate < DateTime.Now || EndDate < DateTime.Now)
            {
                isFuture = false;
            }

            return isFuture;
        }

        public bool ArriveBeforeDepartCheck(string fromDate, string toDate)
        {
            bool isBefore = true;

            DateTime StartDate = DateTime.Parse(fromDate);
            DateTime EndDate = DateTime.Parse(toDate);

            if (StartDate > EndDate)
            {
                isBefore = false;
            }

            return isBefore;
        }

        public bool IsValidParkId(int userSelectedParkId)
        {
            ParkDAL parkDAL = new ParkDAL(connectionString);
            List<Park> parks = parkDAL.GetParks();

            int invalidCount = 0;

            foreach (Park park in parks)
            {
                if (park.Id != userSelectedParkId)
                {
                    invalidCount++;
                }
            }
            if (invalidCount == parks.Count)
            {
                return false;
            }
            return true;
        }

        public bool IsValidCampgroundId(Park selectedPark, int userSelectedParkId)
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campground> campgrounds = campgroundDAL.GetCampground(selectedPark.Id);

            int invalidCount = 0;

            foreach (Campground campground in campgrounds)
            {
                if (campground.CampgroundId != userSelectedParkId)
                {
                    invalidCount++;
                }
            }
            if (invalidCount == campgrounds.Count)
            {
                return false;
            }
            return true;
        }

        public bool IsValidCampsiteId(List<Campsite> availableSites, int userSelectedSiteId)
        {
            int invalidCount = 0;

            foreach (Campsite campsite in availableSites)
            {
                if (campsite.SiteId != userSelectedSiteId)
                {
                    invalidCount++;
                }
            }
            if (invalidCount == availableSites.Count)
            {
                return false;
            }
            return true;
        }

        public double TotalCostCalculator(string from_date, string to_date, decimal dailyFee)
        {
            DateTime toDate = DateTime.Parse(to_date);
            DateTime fromDate = DateTime.Parse(from_date);

            return ((toDate - fromDate).TotalDays) * (double)dailyFee;
        }
    }
}
