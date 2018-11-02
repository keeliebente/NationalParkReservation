using System;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime EstablishDate { get; set; }
        public int Area { get; set; }
        public int AnnualVisitors { get; set; }
        public string Description { get; set; }

        //method to split description into 80 char lines 
        private static string WriteLineWordWrap(String description)
        {
            String[] words = description.Split(' ');
            StringBuilder sb = new StringBuilder();

            foreach (String word in words)
            {
                sb.Append(word);

                if (sb.Length >= 80)
                {
                    String line = sb.ToString().Substring(0, sb.Length - word.Length);
                    Console.WriteLine(line);
                    sb.Clear();
                    sb.Append(word);
                }
                sb.Append(" ");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            Console.WriteLine($"\n{Name}\n" +
                $"Location: {Location}\n" +
                $"Established: {EstablishDate.ToShortDateString()}\n" +
                $"Area: {Area} sq.km.\n" +
                $"Annual Visitors: {AnnualVisitors}\n" +
                $"\n");
            return WriteLineWordWrap(Description);
        }
    }
}
