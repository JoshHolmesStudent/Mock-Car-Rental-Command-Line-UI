using System;
using System.Collections.Generic;
using static MRRCmanagement.CRM;
using static MRRCmanagement.Vehicle;
using static MRRCmanagement.Rental;
using System.IO;

namespace MRRCmanagement
{
    // General methods used for quality of life | class used to declutter code | Inherited by and from no class
    public class QualityOfLife
    {
        //Initialise choice variables from withinRange and CharWithinRange | To be referenced in other classes/methods
        public static int qolchoice;
        public static char charqolchoice;

        //Check if user input is between the two args, changing variable qolchoice to be the users input if it is valid
        public static void WithinRange(int start, int end)
        {
            //Format 1 for methods which cannot be exited out of, these functions sometimes take more than one keypress by the user, requiring a string var
            //instead of a key
                while (true)
                {
                    //User inputs choice
                    string x = Console.ReadLine();

                    //Test if choice is an integer
                    if (!int.TryParse(x, out qolchoice))
                    {
                        Console.WriteLine("ERROR: Please enter a valid integer\n");
                    }
                    else { break; }
                }

                //Test if choice is between the valid numbers
                if (qolchoice < start || qolchoice > end)
                {
                    //If not valid, write error and begin again
                    Console.WriteLine($"ERROR: Please enter a valid whole number between {start} and {end}\n");
                    WithinRange(start, end);
                }
        }

        //Format 2 for functions that only need one keypress. These can be exited out of with backspace and escape.
        public static void CharWithinRange(char start, char end)
        {
            //Initialise key variable
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape) // exit the program if user presses ESC
                {
                    Environment.Exit(0);
                }
                else if (key.Key == ConsoleKey.Backspace) // return null if backspace is pressed
                {
                    QualityOfLife.charqolchoice = '\0';
                    return;
                }

                if (!char.TryParse(key.Key.ToString(), out char charqolchoice) || key.Key.ToString() == string.Empty) // Parse keypress to charqolchoice
                {
                    Console.WriteLine("\nERROR: Please enter a valid non-number character\n");
                }
                else break;
            }

            QualityOfLife.charqolchoice = char.ToUpper(key.KeyChar);

            if (charqolchoice < start || charqolchoice > end)
            {
                //If not valid, write error and begin again
                Console.WriteLine($"\nERROR: Please enter a valid character between {start} and {end}\n");
                CharWithinRange(start, end);
                return;
            }
            
        }

        //Converts string to Gender enum option
        public static Gender ToGender(string gender)
        {
            if (gender.ToLower() == "male")
            {
                return Gender.Male;
            } else if (gender.ToLower() == "female")
            {
                return Gender.Female;
            } else if (gender.ToLower() == "other")
            {
                return Gender.Other;
            }
            else
            {
                return Gender.unknown;
                //The user will be forced to enter any of the three options, so there is no need to account for anything else being inputted
            }
        }

        //Initialize list of csv versions of customers and vehicles
        public static List<string> csvCustomers = new List<string>();
        public static List<string> csvVehicles = new List<string>();
        public static List<string> csvRentals = new List<string>();

        //Update the customer csv file
        public static void CustomersCsvUpdate(List<CRM> customers)
        {
            //clear the csvcustomers list, ready to be written into with current updated info
            csvCustomers.Clear();

            //Fill the list with new information
            foreach(CRM customer in customers)
            {
                string str = CRMToCsvString(customer);
                csvCustomers.Add(str);
            }

            //Empty the customer csv file, to be written into with updated info
            File.WriteAllText(Menu.customerfilename, string.Empty);

            //Write each customer in csv format to the file
            foreach (string csv in csvCustomers)
            {
                File.AppendAllText(Menu.customerfilename, "\n" + csv);
            }
        }

        //Updates the vehicle csv file | Using the same method as CustomersCsvUpdate above
        public static void VehiclesCsvUpdate(List<Vehicle> vehicles)
        {
            csvVehicles.Clear();

            foreach (Vehicle vehicle in vehicles)
            {
                string str = VehicleToCsvString(vehicle);
                csvVehicles.Add(str);
            }

            File.WriteAllText(Menu.fleetfilename, string.Empty);

            foreach (string csv in csvVehicles)
            {
                File.AppendAllText(Menu.fleetfilename, "\n" + csv);
            }
        }

        //Updates the vehicle csv file | Using the same method as CustomersCsvUpdate above
        public static void RentalsCsvUpdate(List<Rental> rentals)
        {
            //Clear the list of csv formatted rentals
            csvRentals.Clear();

            foreach (Rental rental in rentals)
            {
                //Add rental in csv format to csv list
                string str = RentalToCsvString(rental);
                csvRentals.Add(str);
            }

            //Clear the csv file
            File.WriteAllText(Menu.rentalsfilename, string.Empty);

            foreach (string csv in csvRentals)
            {
                File.AppendAllText(Menu.rentalsfilename, "\n" + csv);
            }
        }

        //Method used to convert string to grade enum
        public static Vehicle.vehicleGrade ToGrade(string grade)
        {
            if (grade.ToLower() == "economy")
            {
                return vehicleGrade.Economy;
            } else if (grade.ToLower() == "commercial")
            {
                return vehicleGrade.Commercial;
            } else if (grade.ToLower() == "family")
            {
                return vehicleGrade.Family;
            } else if (grade.ToLower() == "luxury")
            {
                return vehicleGrade.Luxury;
            } else
            {
                return vehicleGrade.unknown;
            }
        }

        //Method used to convert string to registration enum, only if the registration is unique (format 1 displays error message to the user, format 2
        //Is for use in methods which do not print error messages)
        public static bool Check_registration(string rego, int format = 1)
        {
            if (format == 1)
            {
                int count = 1;

                //If registration is not 6 characters long
                if (rego.Length < 6 || rego.Length > 6)
                {
                    Console.WriteLine("Incorrect length: Pease enter three numbers followed by three letters");
                    return false;
                }

                //Check if registration is 3 numbers followed by 3 letters
                foreach (char letter in rego)
                {
                    if (count <= 3)
                    {
                        if (!char.IsDigit(letter))
                        {
                            Console.WriteLine("Number in place of letter: Please enter three numbers followed by three letters");
                            return false;
                        } else if (letter < 0)
                        {
                            Console.WriteLine("ERROR: negative number inputs are not permitted for registration");
                            return false;
                        }

                    }
                    else if (count > 3)
                    {
                        if (!char.IsLetter(letter))
                        {
                            Console.WriteLine("Letter in place of number: Please enter three numbers followed by three letters");
                            return false;
                        }
                    }
                    count++;
                }
                //Check if registration is unique
                foreach (Vehicle vehicle in Menu.Vehicles)
                {
                    if (rego == vehicle.Registration)
                    {
                        Console.WriteLine("ERROR: Registration found in existing vehicle. Please enter a registration that does not already exist");
                        return false;
                    }
                }
                
            return true;

             //Behind the scenes format -- the same function with no messages displayed
            } else if (format == 2)
            {
                int count = 1;

                if (rego.Length < 6 || rego.Length > 6)
                {
                    return false;
                }

                foreach (char character in rego)
                {
                    if (count <= 3)
                    {
                        if (!char.IsDigit(character))
                        {
                            return false;
                        }

                    }
                    else if (count > 3)
                    {
                        if (!char.IsLetter(character))
                        {
                            return false;
                        }
                    }
                    count++;
                }
                foreach (Vehicle vehicle in Menu.Vehicles)
                {
                    if (rego == vehicle.Registration)
                    {
                        return false;
                    }
                }
                return true;
            } else { return true; } //Method will always be either format 1 or 2
        }

            //Turns string to transmission enum if the string is valid
            public static Vehicle.transmission ToTransmissionEnum(string transmission)
        {
            if (transmission.ToLower() == "manual")
            {
                return Vehicle.transmission.Manual;
            } else if (transmission.ToLower() == "automatic")
            {
                return Vehicle.transmission.Automatic;
            } else
            {
                return Vehicle.transmission.Unknown;
            }
        }

        //Turns string to fueltype enum if string valid
        public static Vehicle.fuelType ToFuelTypeEnum(string fuel)
        {
            if (fuel.ToLower() == "petrol")
            {
                return Vehicle.fuelType.Petrol;
            } else if (fuel.ToLower() == "diesel")
            {
                return Vehicle.fuelType.Diesel;
            } else { return Vehicle.fuelType.Unknown; }
        }

        //Used in the table formatting methods | Used to determine the number of spaces between a word and the end of the column the word is in
        public static string NumSpaces(string aspect, int highest)
        {
            //Aspect holds the word, highest holds the max width of the column
            int numspace = highest - aspect.Length;
            string res = new String(' ', numspace);
            return res;
            //returns a string of the required number of spaces (max column length - word length)
        }

        public static bool IsRenting(int ID)
        {
            bool res = false;
            foreach (Rental rental in Menu.Rentals)
            {
                if (ID != rental.ID_Number)
                {
                    res = false;
                } else
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        public static bool IsBeingRented(string registration)
        {
            bool res = false;

            foreach(Rental rental in Menu.Rentals)
            {
                if (registration != rental.Registration)
                {
                    res = false;
                } else
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        public static bool IsBeingRented(int ID)
        {
            bool res = false;

            foreach (Rental rental in Menu.Rentals)
            {
                if (ID != rental.ID_Number)
                {
                    res = false;
                }
                else
                {
                    res = true;
                    break;
                }
            }
            return res;
        }
    }
}
