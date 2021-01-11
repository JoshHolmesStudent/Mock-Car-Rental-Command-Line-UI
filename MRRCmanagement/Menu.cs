using System;
using System.Collections.Generic;
using static System.Console;
using static MRRCmanagement.QualityOfLife;
using static MRRCmanagement.Vehicle;
using static MRRCmanagement.Rental;

namespace MRRCmanagement
{
    //Menu class | Contains sub-menu methods relating to both CRM and Vehicle classes | Inherited by and from no classes
    public class Menu
    {
        //Initialise two readonly customer and fleet csv filename strings
        public static string customerfilename;
        public static string fleetfilename;
        public static string rentalsfilename;

        //initialise customer and vehicle lists
        public static List<CRM> Customers = new List<CRM>();
        public static List<Vehicle> Vehicles = new List<Vehicle>();
        public static List<Rental> Rentals = new List<Rental>();
        
        //--------------------------------------------------------------------------------------------------------------------------------------------
        //Customer Management Menu
        
        //Delete Customer method | Deletes customer from list and updates csv file
        public static void Delete_customer()
        {
            if (Customers.Count > 0)
            {
                //Prompt user to delete customer and display customer info list
                WriteLine("\nPlease select the letter ID of the customer to remove\n");
                char count = 'A';
                for (int i = 0; i < Customers.Count; i++)
                {
                    Write(count + "-");
                    Customers[i].Display_Info(2);
                    count++;
                }
                count--;

                while (true)
                {
                    CharWithinRange('A', count);

                    if (charqolchoice == '\0')
                    {
                        return;
                    }

                    int choice = charqolchoice - 65;

                    if (IsRenting(Customers[choice].Id_number))
                    {
                        WriteLine("\nERROR: Customer already renting a vehicle cannot be deleted from the system");
                    } else
                    {
                        WriteLine($"\nSuccessfully deleted customer with ID {Customers[choice].Id_number}\n");

                        //Remove customer from list
                        Customers.RemoveAt(choice);

                        CustomersCsvUpdate(Customers);

                        break;
                    }

                }

                if (Customers.Count > 0)
                {
                    //Display customer info
                    CRM.Display_Customers();

                    WriteLine("Would you like to delete another customer?(Y/N)");
                    while (true)
                    {
                        //Read key Y or N. If N end the method, if Y call the method then end the method, else print error message
                        ConsoleKeyInfo YN = ReadKey();

                        char parser;

                        if (YN.Key == ConsoleKey.Escape)
                        {
                            Environment.Exit(0);
                        }
                        else if (YN.Key == ConsoleKey.Backspace)
                        {
                            return;
                        }
                        else if (char.TryParse(YN.Key.ToString(), out parser))
                        {
                            if (char.Parse(YN.Key.ToString().ToUpper()) == 'N')
                            {
                                return;
                            }
                            else if (char.Parse(YN.Key.ToString().ToUpper()) == 'Y')
                            {
                                Delete_customer();
                                return;
                            }
                        }
                        else
                        {
                            WriteLine("\nPlease enter a valid character (Y/N)\n");
                        }
                    }
                }
            }
            else
            {
                WriteLine("\nERROR: no customers exist to be deleted");
            }
        }

        //Create Customer | Prompts user for inputs relating to each customer variable and creates a new CRM object
        public static void Create_Customer()
        {
            //Prompt user for inputs for each variable
            Write("\nPlease enter the following fields (Note: you may not exit the program or go to the previous menu while creating a customer):\nTitle: ");
            string Title = ReadLine();
            Write("First Name: ");
            string firstname = ReadLine();
            Write("Last Name: ");
            string lastname = ReadLine();

            string Gender;
            while (true) // Check validity of input
            {
                Write("Gender: ");
                Gender = ReadLine();
                if (ToGender(Gender) == CRM.Gender.unknown)
                {
                    WriteLine("\nERROR: Please select from either Male, Female, or Other\n");
                }
                else break;
            }

            DateTime dob;
            while (true) // Check validity of input
            {
                Write("Date of Birth: ");
                if (!DateTime.TryParse(ReadLine(), out dob))
                {
                    WriteLine("\nERROR: Please enter a compatible date time format (xx/xx/xx)\n");
                }
                else break;

            }

            //Create ID number 1 higher than max
            int IDMax = 0;
            foreach (CRM customer in Customers)
            {
                if (IDMax < customer.Id_number)
                {
                    IDMax = customer.Id_number;
                }
            }

            //Create new CRM object
            CRM name = new CRM(IDMax + 1, Title, firstname, lastname, Gender, dob.ToString());
            WriteLine($"Successfully created Customer {name.Id_number} - {Title} {firstname} {lastname}\n");
            
            //Add customer to customers list
            Customers.Add(name);

            //Update csv customers file (from QualityOfLife class)
            CustomersCsvUpdate(Customers);
        }


        //Call modifyCustomer function if a valid customer ID is inputted
        public static void Modify_Customers()
        {
            //if there are 1 or more elements in Customers array
            if (Customers.Count > 0)
            {
                WriteLine("\nPlease enter the letter ID of the customer you wish to modify:");
                //Write details of each customer for ease of use
                char count = 'A';
                for (int i = 0; i < Customers.Count; i++)
                {
                    Write(count + "- ");
                    Customers[i].Display_Info(2);
                    count++;
                }
                count--;
                //Method from MRRCmanagement.QualityOfLife | Promts user for choice and tests if it is within range of two args
                CharWithinRange('A', count);

                if (charqolchoice == '\0')
                {
                    return;
                }

                int choice = charqolchoice - 65;

                // Display info of customer and call the CRM.ModifyCustomer method on that customer
                Customers[choice].Display_Info(1);
                WriteLine();
                Customers[choice].ModifyCustomer();

                //Update customer csv file 
                CustomersCsvUpdate(Customers);

            }
            else // if no customers exist to be modified
            {
                WriteLine("\nThere must be at least 1 customer to modify");
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        //Fleet management menu

        //Using the same method as Create_Customer | Creating a Vehicle object based on users inputs
        public static void Create_Vehicle()
        {
            WriteLine("\n\nPlease select the details settings:\nA: Use defaults (only fill in the required fields)\nB: Fully intensive (Customise every field)" +
                "\n(Note: You may not be able to move to the previous menu or officially exit the program while creating a vehicle)\n");
            CharWithinRange('A', 'B');

            if (charqolchoice == 'A')
            {
                string rego;
                Write("\nPlease enter the following fields:\nRegistration: ");
                while (true)
                {
                    rego = ReadLine();
                    if (Check_registration(rego))
                    {
                        break;
                    }
                }

                string grade;
                while (true) // test if grade is valid
                {
                    Write("Vehicle Grade: ");
                    grade = ReadLine();
                    if (grade.ToLower() == "luxury" || grade.ToLower() == "commercial" || grade.ToLower() == "family" || grade.ToLower() == "economy")
                    {
                        break;
                    }
                    else
                    {
                        WriteLine("\nPlease enter a compatible grade type (Luxury, Commercial, Family, or Economy)");
                    }
                }
                Write("Make: ");
                string make = ReadLine();
                Write("Model: ");
                string model = ReadLine();
                int year;
                while (true)
                {
                    Write("Year: ");
                    if (!int.TryParse(ReadLine(), out year) || year < 0)
                    {
                        WriteLine("\nERROR: please enter a positive whole number signifying the year of the vehicle");
                    }
                    else { break; }
                }

                //Create vehicle object according to the grade defaults
                if (ToGrade(grade) == vehicleGrade.Commercial)
                {
                    Commercial_class vehicle = new Commercial_class(rego, grade, make, model, year);
                    WriteLine();
                    vehicle.Dispay_Vehicle(1);
                    Vehicles.Add(vehicle);
                }
                else if (ToGrade(grade) == vehicleGrade.Economy)
                {
                    Economy_Class vehicle = new Economy_Class(rego, grade, make, model, year);
                    WriteLine();
                    vehicle.Dispay_Vehicle(1);
                    Vehicles.Add(vehicle);
                }
                else if (ToGrade(grade) == vehicleGrade.Family)
                {
                    Family_Class vehicle = new Family_Class(rego, grade, make, model, year);
                    WriteLine();
                    vehicle.Dispay_Vehicle(1);
                    Vehicles.Add(vehicle);
                }
                else if (ToGrade(grade) == vehicleGrade.Luxury)
                {
                    Luxury_Class vehicle = new Luxury_Class(rego, grade, make, model, year);
                    WriteLine();
                    vehicle.Dispay_Vehicle(1);
                    Vehicles.Add(vehicle);
                }

                WriteLine($"\n Successfully created Vehicle {year} - {make} {model}\n");

                VehiclesCsvUpdate(Vehicles);
            }

            //If user chooses to enter every field instead of using defaults
            else if (charqolchoice == 'B')
            {
                    string rego;
                    Write("\nPlease enter the following fields:\nRegistration: ");
                    while (true)
                    {
                        rego = ReadLine();
                        if (Check_registration(rego))
                        {
                            break;
                        }
                    }

                string grade;
                while (true)
                {
                    Write("Vehicle Grade (Luxury, Commercial, Family, Economy): ");
                    grade = ReadLine();
                    if (grade.ToLower() == "luxury" || grade.ToLower() == "commercial" || grade.ToLower() == "family" || grade.ToLower() == "economy")
                    {
                        break;
                    }
                    else
                    {
                        WriteLine("\nPlease enter a compatible grade type (Luxury, Commercial, Family, or Economy)\n");
                    }
                }

                Write("Make: ");
                string make = ReadLine();
                Write("Model: ");
                string model = ReadLine();
                int year;
                while (true)
                {
                    Write("Year: ");
                    if (!int.TryParse(ReadLine(), out year))
                    {
                        WriteLine("\nERROR: please enter a whole number signifying the year of the vehicle");
                    }
                    else { break; }
                }
                int numseats;
                while (true)
                {
                    Write("Number of seats: ");
                    if (!int.TryParse(ReadLine(), out numseats) || numseats < 0)
                    {
                        WriteLine("ERROR: Please enter a whole number above zero signifying the number of seats in the vehicle");
                    }
                    else { break; }
                }

                string transmissionstr;
                Vehicle.transmission transmission;
                while (true)
                {
                    Write("Transmission Type (Automatic, Manual): ");
                    transmissionstr = ReadLine();
                    if (ToTransmissionEnum(transmissionstr) == Vehicle.transmission.Unknown) // Method used from QualityOfLife class
                    {
                        WriteLine("Please enter a valid transmission type (Automatic, Manual)");
                    }
                    else
                    {
                        transmission = ToTransmissionEnum(transmissionstr);
                        break;
                    }
                }

                Vehicle.fuelType fuel;
                while (true)
                {
                    Write("Fuel Type (Petrol, Diesel): ");
                    string fuelstr = ReadLine();
                    if (ToFuelTypeEnum(fuelstr) == fuelType.Unknown)
                    {
                        WriteLine("Please enter a valid fuel type (Petrol, Diesel)");
                    } else
                    {
                        fuel = ToFuelTypeEnum(fuelstr);
                        break;
                    }
                }

                bool GPS;
                while (true)
                {
                    Write("Has GPS (Y/N): ");
                    string choice = ReadLine();
                    if (choice.ToLower() == "y")
                    {
                        GPS = true;
                        break;
                    } else if (choice.ToLower() == "n")
                    {
                        GPS = false;
                        break;
                    } else
                    {
                        WriteLine("Please enter either 'Y' or 'N'");
                    }
                }

                bool sunRoof;
                while (true)
                {
                    Write("Has SunRoof (Y/N): ");
                    string choice = ReadLine();
                    if (choice.ToLower() == "y")
                    {
                        sunRoof = true;
                        break;
                    }
                    else if (choice.ToLower() == "n")
                    {
                        sunRoof = false;
                        break;
                    }
                    else
                    {
                        WriteLine("Please enter either 'Y' or 'N'");
                    }
                }

                double dailyRate;
                while (true)
                {
                    Write("Daily Rate: ");
                    if (!double.TryParse(ReadLine(), out dailyRate) || dailyRate < 0)
                    {
                        WriteLine("ERROR: Please enter a whole number above zero signifying the daily rate for the vehicle");
                    }
                    else { break; }
                }

                Write("Colour: ");
                string colour = ReadLine();

                //Create new vehicle object with no default values
                if (ToGrade(grade) == vehicleGrade.Commercial)
                {
                    Commercial_class vehicle = new Commercial_class(rego, grade, make, model, year, numseats, transmission, fuel, GPS, sunRoof, dailyRate, colour);
                    Vehicles.Add(vehicle);
                }
                else if (ToGrade(grade) == vehicleGrade.Economy)
                {
                    Economy_Class vehicle = new Economy_Class(rego, grade, make, model, year, numseats, transmission, fuel, GPS, sunRoof, dailyRate, colour);
                    Vehicles.Add(vehicle);
                }
                else if (ToGrade(grade) == vehicleGrade.Family)
                {
                    Family_Class vehicle = new Family_Class(rego, grade, make, model, year, numseats, transmission, fuel, GPS, sunRoof, dailyRate, colour);
                    Vehicles.Add(vehicle);
                }
                else if (ToGrade(grade) == vehicleGrade.Luxury)
                {
                    Luxury_Class vehicle = new Luxury_Class(rego, grade, make, model, year, numseats, transmission, fuel, GPS, sunRoof, dailyRate, colour);
                    Vehicles.Add(vehicle);
                }
                WriteLine($"\nSuccessfully created Vehicle {year} - {make} {model}\n");
            }
        }

        //Modify vehicle | Using same method as modify_customer | Modifies one aspect of a vehicle object
        public static void Modify_Vehicle()
        {
            if (Vehicles.Count > 0)
            {
                WriteLine("\nPlease select the vehicle you wish to modify:\n");
                char count = 'A';
                foreach (Vehicle vehicle in Vehicles)
                {
                    Write($"{count} - ");
                    vehicle.Dispay_Vehicle(2);
                    count++;
                }

                count--;

                CharWithinRange('A', count);

                if (charqolchoice == '\0')
                {
                    return;
                }

                int choice = charqolchoice - 64;

                WriteLine();
                Vehicles[choice - 1].Dispay_Vehicle(1);
                Vehicles[choice - 1].ModifyVehicle();
            }
            //Writes error message if no vehicles exist to be modified
            else
            {
                WriteLine("\nERROR: There must be at least one vehcile to modify");
            }

        }

        //User chooses a vehicle and deletes it from csv list and internal vehicle list
        public static void Delete_Vehicle()
        {
            if (Vehicles.Count > 0)
            {
                WriteLine("\nPlease select the ID of the vehicle you wish to remove\n");
                //Initialise letter ID to increase
                char count = 'A';

                //Display vehicles, along with letter ID
                for (int i = 0; i < Vehicles.Count; i++)
                {
                    Write(count + "- ");
                    Vehicles[i].Dispay_Vehicle(2);
                    count++;
                }

                //Minus the extra 1 from count
                count--;

                while (true)
                {
                    //User choice from list of vehicles
                    CharWithinRange('A', count);

                    if (charqolchoice == '\0')
                    {
                        return;
                    }

                    //convert choice to int -- to be deleted from list (65 minussed converts ascii version of char to int and minus' 1 to correlate with its position in the list)
                    int choice = charqolchoice - 65;

                    if (IsBeingRented(Vehicles[choice].Registration))
                    {
                        WriteLine("\nERROR: Vehicle being rented cannot be deleted");
                    } else
                    {
                        //Remove vehicle from vehicles list
                        Vehicles.RemoveAt(choice);

                        WriteLine($"\nSuccessfully deleted vehicle with ID {charqolchoice}\n");

                        //update csv file
                        VehiclesCsvUpdate(Vehicles);
                        break;
                    }
                }

                //Display vehicle list after deletion
                if (Vehicles.Count > 0)
                {
                    Display_Vehicles(Menu.Vehicles);

                    WriteLine("\nWould you like to delete another vehicle?(Y/N)");

                    while (true)
                    {
                        //Read key Y or N. If N end the method, if Y call the method then end the method, else print error message
                        ConsoleKeyInfo YN = ReadKey();

                        char parser;

                        if (YN.Key == ConsoleKey.Escape)
                        {
                            Environment.Exit(0);
                        }
                        else if (YN.Key == ConsoleKey.Backspace)
                        {
                            return;
                        }
                        else if (char.TryParse(YN.Key.ToString(), out parser))
                        {
                            if (char.Parse(YN.Key.ToString().ToUpper()) == 'N')
                            {
                                return;
                            }
                            else if (char.Parse(YN.Key.ToString().ToUpper()) == 'Y')
                            {
                                Delete_Vehicle();
                                return;
                            }
                        }
                        else
                        {
                            WriteLine("\nPlease enter a valid character (Y/N)\n");
                        }
                    }
                }
            } else
            {
                WriteLine("\nERROR: no vehicles exist to be deleted\n");
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Rental Management Menu
        //Delete rental method | Deletes rental from list and updates csv file
        public static void Return_Rental ()
        {
            if (Rentals.Count > 0)
            {
                //Prompt user to remove a Rental and display rental info list
                WriteLine("\nPlease select the letter ID of the Rental vehicle to return\n");
                char count = 'A';
                for (int i = 0; i < Rentals.Count; i++)
                {
                    Write(count + "-");
                    Rentals[i].Dispay_Rentals_Info(2);
                    count++;
                }
                count--;

                CharWithinRange('A', count);

                if (charqolchoice == '\0')
                {
                    return;
                }

                int choice = charqolchoice - 65;

                WriteLine($"\nSuccessfully returned rental with registration {Rentals[choice].Registration}\n");

                //Remove rental from list
                Rentals.RemoveAt(choice);

                RentalsCsvUpdate(Rentals);

                if (Rentals.Count > 0)
                {
                    //Display customer info
                    Display_Rentals();

                    WriteLine("\nWould you like to return another rental?(Y/N)");
                    while (true)
                    {
                        //Read key Y or N. If N end the method, if Y call the method then end the method, else print error message
                        ConsoleKeyInfo YN = ReadKey();

                        char parser;

                        if (YN.Key == ConsoleKey.Escape)
                        {
                            Environment.Exit(0);
                        }
                        else if (YN.Key == ConsoleKey.Backspace)
                        {
                            return;
                        }
                        else if (char.TryParse(YN.Key.ToString(), out parser))
                        {
                            if (char.Parse(YN.Key.ToString().ToUpper()) == 'N')
                            {
                                return;
                            }
                            else if (char.Parse(YN.Key.ToString().ToUpper()) == 'Y')
                            {
                                Return_Rental();
                                return;
                            }
                        }
                        else
                        {
                            WriteLine("\nPlease enter a valid character (Y/N)\n");
                        }
                    }
                }
            }
            else
            {
                WriteLine("\nERROR: no rentals exist to be removed");
            }
        }

        public static void Create_Rental()
        {
            if (Vehicles.Count > 0 && Customers.Count > 0)
            {
                //Prompt user for inputs for each variable
                Write("\nPlease enter the following fields :\n");

                string Registration;
                double rate;
                WriteLine("\nVehicle to rent");
                char count = 'A';
                for (int i = 0; i < Vehicles.Count; i++)
                {
                    Write(count + "- ");
                    Vehicles[i].Dispay_Vehicle(2);
                    count++;
                }
                count--;
                //Method from MRRCmanagement.QualityOfLife | Promts user for choice and tests if it is within range of two args
                while (true)
                {
                    CharWithinRange('A', count);

                    if (charqolchoice == '\0') //Value returned from CharWithinRange
                    {
                        return;
                    }

                    int choice = charqolchoice - 65;

                    if (IsBeingRented(Vehicles[choice].Registration))
                    {
                        WriteLine("\nERROR: Vehicle is already being rented, please select another vehicle");
                    }
                    else
                    {
                        Registration = Vehicles[choice].Registration;
                        rate = Vehicles[choice].dailyRate;
                        break;
                    }
                }

                WriteLine("\nCustomer renting the vehicle");
                int ID;
                count = 'A';
                for (int i = 0; i < Customers.Count; i++)
                {
                    Write(count + "- ");
                    Customers[i].Display_Info(2);
                    count++;
                }
                count--;

                while (true)
                {
                    CharWithinRange('A', count);

                    if (charqolchoice == '\0') //Value returned from CharWithinRange
                    {
                        return;
                    }

                    int choice = charqolchoice - 65;

                    if (IsBeingRented(Customers[choice].Id_number))
                    {
                        WriteLine("\nERROR: Customer is already renting a vehicle, please select another customer");
                    }
                    else
                    {
                        ID = Customers[choice].Id_number;
                        break;
                    }
                }

                double cost;
                Write("\nLength of rental period (days): ");
                while (true)
                {
                    if (double.TryParse(ReadLine(), out double length) && length > 0)
                    {
                        cost = length * rate;
                        break;
                    } else
                    {
                        Console.WriteLine("ERROR: Please enter a valid whole integer greater than 0");
                    }
                }

                string displayCost = string.Format("{0:0.00}", cost);

                //Create new rental object
                Rental rental = new Rental(Registration, ID, displayCost);
                WriteLine($"\nSuccessfully created rental {Registration} with {ID} for ${displayCost}\n");

                //Add customer to customers list
                Rentals.Add(rental);

                Display_Rentals();

                //Update csv customers file (from QualityOfLife class)
                RentalsCsvUpdate(Rentals);
            } else
            {
                WriteLine("\nERROR: there must be at least one vehicle and one customer in order to add a rental");
            }
        }
    }
}