using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using static MRRCmanagement.QualityOfLife;
using static System.Console;

namespace MRRCmanagement
{
    public class Rental
    {
        private string registration;
        public string Registration
        {
            get { return registration; }
            set { value = registration; }
        }
        private int id_number;
        public int ID_Number
        {
            get { return id_number; }
            set { value = id_number; }
        }
        private string cost; //String for cost so that blank spaces can be used for costs not given (in original rentals.csv file)

        public Rental(string Registration, int ID_Number, string Cost = "-")
        {
            registration = Registration;
            id_number = ID_Number;
            cost = string.Format("{0:0.00}", Cost);
        }

        public Rental(string csvLine)
        {
            string[] aspects = csvLine.Split(',');
            registration = aspects[0];
            id_number = int.Parse(aspects[1]);
            if (aspects.Length > 2)
            {
                cost = aspects[2];
                cost = string.Format("{0:0.00}", cost);
            } else
            {
                cost = "-";
            }
        }

        //Converts vehicle object to csv compatible string
        public static string RentalToCsvString(Rental rental)
        {
            string res;
            res = $"{rental.registration},{rental.id_number},{rental.cost}";
            return res;
        }

        //Display rentals in table format
        public static void Display_Rentals()
        {
            if (Menu.Rentals.Count > 0)
            {
                //initialise array of lengths of headings (if the longest name is shorter than the heading for first name, the length of the heading will instead be used)
                int[] highest = { 12, 11, 4 };

                //Looping through each customer, making each aspect to an element in a temp array, finding the highest length of each and assigning that number
                //to a variable
                foreach (Rental rental in Menu.Rentals)
                {
                    //Splitting the customer string into an array of it's aspects
                    string[] rentalAspects = { rental.registration, rental.id_number.ToString(), rental.cost };

                    for (int i = 0; i <= rentalAspects.Length - 1; i++)
                    {
                        if (rentalAspects[i] != null && rentalAspects[i].ToString().Length > highest[i])
                        {
                            highest[i] = rentalAspects[i].ToString().Length;
                        }
                    }
                }

                //Initialize border lengths (+1 used for readability and reducing clutter)
                int Regborder = highest[0] + 1;
                int IDBorder = highest[1] + 1;
                int CostBorder = highest[2] + 1;

                //Initialize Strings, holding a dynamic amount of dashes, depending on variable lengths
                string Regline = new string('-', Regborder);
                string IDline = new string('-', IDBorder);
                string CostLine = new string('-', CostBorder);

                //Top and bottom border of each cell
                string border = $"+{Regline}+{IDline}+{CostLine}+\n";

                //create and write heading row of table
                string heading = ($"\n{border}" +
                          $"|Registration{NumSpaces("Registration", Regborder)}|Customer ID{NumSpaces("Customer ID", IDBorder)}|Cost{NumSpaces("Cost", CostBorder)}|\n{border}");
                Write($"{heading}");

                //Loop through each customer and create a row for each
                foreach (Rental rental in Menu.Rentals)
                {
                    string body = ($"|{rental.registration}{NumSpaces(rental.registration, Regborder)}|{rental.id_number}{NumSpaces(rental.id_number.ToString(), IDBorder)}|" +
                    $"${rental.cost}{NumSpaces(rental.cost + 1, CostBorder)}|\n" +
                    $"{border}");

                    Write(body);
                }
            }
            else
            {
                WriteLine("\nERROR: No rentals exist to be displayed\n");
            }
        }

        public void Dispay_Rentals_Info(int format)
        {
            if (format == 1)
            {
                Console.WriteLine($"Registration: {this.registration}\nCustomer ID: {this.id_number}");
            }
            else if (format == 2)
            {
                Console.WriteLine($"{this.registration} - {this.id_number}");
            }
        }
    }
}
