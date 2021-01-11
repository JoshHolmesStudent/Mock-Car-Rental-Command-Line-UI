using System;
using static System.Console;
using static MRRCmanagement.QualityOfLife;
using System.Collections.Generic;

namespace MRRCmanagement
{
    //Vehicle class | Contains constructors and methods for each class inheriting this class | This class inherits no class | This class is inherited by 4 classes
    public abstract class Vehicle
    {
        //Initialise enums (same purposes as the CRM.Gender enum)
        public enum vehicleGrade
        {
            Economy,
            Commercial,
            Luxury,
            Family,
            unknown
        }

        public enum fuelType
        {
            Petrol,
            Diesel,
            Unknown
        }

        public enum transmission
        {
            Manual,
            Automatic,
            Unknown
        }

        private string registration;
        //Registration getter
        public string Registration
        {
            get { return registration; }
        }
        private string make;
        public string Make { get { return make; } }
        private string model;
        public string Model { get { return model; } }
        private vehicleGrade grade;
        private int year;
        public int Year { get { return year; } }

        //Initialise public variables to be used in sub-classes
        public fuelType fuel;
        public bool GPS;
        public bool sunRoof;
        public int numSeats;
        public double dailyRate;
        public transmission transmissionType;   
        public string colour;

        //Constructor used by other classes when the user only enters the required fields - adequate default values are used
        public Vehicle(string registration, string grade, string make, string model, int year)
        {
            this.registration = registration.ToUpper();
            this.grade = QualityOfLife.ToGrade(grade);
            this.make = make;
            this.model = model;
            this.year = year;
        }

        //Constructor used when user inputs every aspect of the vehicle
        public Vehicle(string registration, string grade, string make, string model, int year, int numseats, string transmission, string fuel, bool GPS, 
            bool sunRoof, double dailyRate, string colour)
        {
            this.registration = registration.ToUpper();
            this.grade = QualityOfLife.ToGrade(grade);
            this.make = make;
            this.model = model;
            this.year = year;
            this.numSeats = numseats;
            this.fuel = QualityOfLife.ToFuelTypeEnum(fuel);
            this.GPS = GPS;
            this.sunRoof = sunRoof;
            this.dailyRate = dailyRate;
            this.transmissionType = QualityOfLife.ToTransmissionEnum(transmission);
            this.colour = colour;
        }

        //constructor used for creating vehicles from a csv file
        public Vehicle(string csv, bool searchType = false)
        {
                string[] aspects = csv.Split(',');
                this.registration = aspects[0];
                this.grade = QualityOfLife.ToGrade(aspects[1]);
                this.make = aspects[2];
                this.model = aspects[3];
                this.year = int.Parse(aspects[4]);
                this.numSeats = int.Parse(aspects[5]);
                this.transmissionType = QualityOfLife.ToTransmissionEnum(aspects[6]);
                this.fuel = QualityOfLife.ToFuelTypeEnum(aspects[7]);
                this.GPS = bool.Parse(aspects[8]);
                this.sunRoof = bool.Parse(aspects[9]);
                this.dailyRate = double.Parse(aspects[10]);
                this.colour = aspects[11];
        }
        
        //Modify vehicle back-end (Function is the same as CRM.ModifyCustomers (modify customer back-end), comments are provided in different cases)
        public void ModifyVehicle()
        {
            string[] aspectNames = { "Registration", "Grade", "Make", "Model", "Year", "Number of seats", "Transmission type", "Fuel Type", "GPS Inclusion",
            "SunRoof Inclusion", "Daily Rate", "Colour"};
            string[] aspects = { registration, grade.ToString(), make, model, year.ToString(), numSeats.ToString(), transmissionType.ToString(),
            fuel.ToString(), GPS.ToString(), sunRoof.ToString(), dailyRate.ToString(), colour};

            WriteLine("\nSelect which aspect of the vehicle to modify");

            char count = 'A';
            foreach (string element in aspects)
            {
                int intcount = count - 65;
                WriteLine($"\n{count} - {aspectNames[intcount]}");
                count++;
            }
            count--;
            CharWithinRange('A', count);

            if (charqolchoice == '\0')
            {
                Menu.Modify_Vehicle();
                return;
            }

            int choice = charqolchoice - 65;

            WriteLine($"\nYou have entered {charqolchoice}, for {aspectNames[choice]}\nThe Current {aspectNames[choice]} for this customer is " +
                $"{aspects[choice]}. What would you like to change it to?\n");

            string change = ReadLine();

            if (charqolchoice == 'A')
            {
                if (!Check_registration(change, 1)) // Use check registration bool created in QualityOfLife class to verify validity (returns false if invalid)
                {
                    this.ModifyVehicle();
                    return;
                }
                else
                {
                    this.registration = change;
                }
            }
            else if (charqolchoice == 'B') // use ToGrade method in QualityOfLife class to convert user input to grade enum and validify the input
            {
                if (ToGrade(change) == vehicleGrade.unknown)
                {
                    WriteLine("ERROR: Please enter a compatible grade (Commercial, Luxury, Economy, or Family");
                    this.ModifyVehicle();
                    return;
                } else { this.grade = ToGrade(change); }
            }
            else if (charqolchoice == 'C') // Make
            {
                this.make = change;
            }
            else if (charqolchoice == 'D') // Model
            {
                this.model = change;
            }
            else if (charqolchoice == 'E') // Year
            {
                //Test if year is a valid integer
                int temp;
                if (!int.TryParse(change, out temp) || int.Parse(change) < 0)
                {
                    WriteLine("ERROR: Please enter a valid whole number above zero");
                    this.ModifyVehicle();
                    return;
                } else
                {
                    this.year = temp;
                }
            }
            else if (charqolchoice == 'F') // NumSeats
            {
                int temp;
                if (!int.TryParse(change, out temp) || int.Parse(change) < 0)
                {
                    WriteLine("ERROR: Please enter a valid whole number above zero");
                    this.ModifyVehicle();
                    return;
                } else
                {
                    this.numSeats = temp;
                }
            }
            else if (charqolchoice == 'G') // TransmissionType
            {
                //Change user input string to valid transmission enum using method found in class QualityOfLife
                if (ToTransmissionEnum(change) == transmission.Unknown)
                {
                    WriteLine("ERROR: Please enter a valid transmission type (Automatic, Manual)");
                    this.ModifyVehicle();
                    return;
                } else { this.transmissionType = ToTransmissionEnum(change); }
            }
            else if (charqolchoice == 'H') // FuelType
            {
                //Using ToFuelTypeEnum from class QualityOfLife (takes a string arg and turns it to a valid FuelType)
                if (ToFuelTypeEnum(change) == fuelType.Unknown)
                {
                    WriteLine("ERROR: Please enter a valid fuel type (Petrol, Diesel)");
                    this.ModifyVehicle();
                    return;
                } else { this.fuel = ToFuelTypeEnum(change); }
            }
            else if (charqolchoice == 'I') // GPS
            {
                //Test if input is valid bool
                bool temp;
                if (!bool.TryParse(change, out temp))
                {
                    WriteLine("ERROR: Please enter a valid response (true, false)");
                    this.ModifyVehicle();
                    return;
                } else
                {
                    this.GPS = temp;
                }
            }
            else if (charqolchoice == 'J') // SunRoof
            {
                bool temp;
                if (!bool.TryParse(change, out temp))
                {
                    WriteLine("ERROR: Please enter a valid response (true, false)");
                    this.ModifyVehicle();
                    return;
                } else
                {
                    this.sunRoof = temp;
                }
            }
            else if (charqolchoice == 'K') // DailyRate
            {
                double temp;
                if (!double.TryParse(change, out temp) || int.Parse(change) < 0)
                {
                    WriteLine("ERROR: Please enter a valid number");
                    this.ModifyVehicle();
                    return;
                } else
                {
                    this.dailyRate = temp;
                }
            }
            else if (charqolchoice == 'L') // Colour
            {
                this.colour = change;
            }

            Write($"\nYou have changed the {aspectNames[choice]} from {aspects[choice]} to ");
            //Update aspects list with change
            aspects[choice] = change;
            WriteLine($"{aspects[choice]}.\n");

            //Update csv file using QualityOfLife.VehiclesCsvUpdate
            VehiclesCsvUpdate(Menu.Vehicles);

            WriteLine("\nWould you like to modify another aspect of this vehicle? (Y/N)\n");

            //Read key Y or N. If N end the method, if Y call the method then end the method, else print error message
            while (true)
            {
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
                        Console.WriteLine();
                        return;
                    }
                    else if (char.Parse(YN.Key.ToString().ToUpper()) == 'Y')
                    {
                        ModifyVehicle();
                        return;
                    }
                }
                else
                {
                    WriteLine("\nPlease enter a valid character (Y/N)\n");
                }
            }
        }

        //Converts vehicle object to csv compatible string
        public static string VehicleToCsvString (Vehicle vehicle)
        {
            string res;
            res = $"{vehicle.registration},{vehicle.grade},{vehicle.make},{vehicle.model},{vehicle.year},{vehicle.numSeats}" +
                $",{vehicle.transmissionType},{vehicle.fuel},{vehicle.GPS},{vehicle.sunRoof},{vehicle.dailyRate},{vehicle.colour}";
            return res;
        }

        //Dispays one vehicle in different formats | same as CRM.Display_Info
        public void Dispay_Vehicle(int format)
        {
            if (format == 1)
            {
                Console.WriteLine($"Registration: {this.registration}\nGrade: {this.grade}\nMake: {this.make}\nModel: {this.model}\nYear: {this.year}\n" +
                    $"Number of seats: {this.numSeats} seater\nTransmission type: {this.transmissionType}\nFuel Type: {this.fuel}\nGPS Inclusion: {this.GPS}\n" +
                    $"SunRoof Inclusion: {this.sunRoof}\nDaily Rate: ${this.dailyRate}\nColour: {this.colour}");
            } else if (format == 2)
            {
                Console.WriteLine($"{this.registration} - {this.make} {this.model}");
            }
        }

        //Dispay each vehicle in table format | same method as CRM.Display_Customers
        public static void Display_Vehicles(List<Vehicle> VehicleList)
        {
            if (VehicleList.Count > 0)
            {

                int[] highest = { 12, 5, 4, 5, 4, 8, 12, 4, 3, 6, 10, 6 };

                foreach (Vehicle vehicle in VehicleList)
                {
                    string[] vehicleaspects = { vehicle.registration, vehicle.grade.ToString(), vehicle.make, vehicle.model, vehicle.year.ToString(),
                    vehicle.numSeats.ToString(), vehicle.transmissionType.ToString(), vehicle.fuel.ToString(), vehicle.GPS.ToString(), vehicle.sunRoof.ToString(),
                    vehicle.dailyRate.ToString(), vehicle.colour};

                    for (int i = 0; i <= vehicleaspects.Length - 1; i++)
                    {
                        if (vehicleaspects[i].ToString().Length > highest[i])
                        {
                            highest[i] = vehicleaspects[i].ToString().Length;
                        }
                    }
                }

                int regborder = highest[0] + 1;
                int gradeborder = highest[1] + 1;
                int MakeBorder = highest[2] + 1;
                int ModelBorder = highest[3] + 1;
                int YearBorder = highest[4] + 1;
                int numseatsBorder = highest[5] + 1;
                int TransTypeBorder = highest[6] + 1;
                int FuelBorder = highest[7] + 1;
                int GPSborder = highest[8] + 1;
                int sunroofborder = highest[9] + 1;
                int dailyrateborder = highest[10] + 1;
                int colourborder = highest[11] + 1;

                string regline = new String('-', regborder);
                string gradeline = new String('-', gradeborder);
                string makeline = new String('-', MakeBorder);
                string modelline = new String('-', ModelBorder);
                string yearline = new String('-', YearBorder);
                string numseatsline = new String('-', numseatsBorder);
                string transtypeline = new String('-', TransTypeBorder);
                string fuelline = new String('-', FuelBorder);
                string GPSline = new String('-', GPSborder);
                string sunroofline = new String('-', sunroofborder);
                string dailyrateline = new String('-', dailyrateborder);
                string colourline = new String('-', colourborder);

                string border = $"+{regline}+{gradeline}+{makeline}+{modelline}+{yearline}+{numseatsline}+{transtypeline}+{fuelline}+{GPSline}+{sunroofline}+{dailyrateline}" +
                    $"+{colourline}+\n";


                string heading = ($"\n{border}" +
                          $"|Registration{NumSpaces("Registration", regborder)}|Grade{NumSpaces("Grade", gradeborder)}|Make{NumSpaces("Make", MakeBorder)}|Model" +
                          $"{NumSpaces("Model", ModelBorder)}|Year{NumSpaces("Year", YearBorder)}|NumSeats{NumSpaces("NumSeats", numseatsBorder)}|Transmission" +
                          $"{NumSpaces("Transmission", TransTypeBorder)}|Fuel{NumSpaces("Fuel", FuelBorder)}|GPS{NumSpaces("GPS", GPSborder)}|SunRoof{NumSpaces("SunRoof", sunroofborder)}" +
                          $"|DailyRate{NumSpaces("DailyRate", dailyrateborder)}|Colour{NumSpaces("Colour", colourborder)}|\n{border}");

                Write($"{heading}");

                foreach (Vehicle vehicle in Menu.Vehicles)
                {
                    string body = ($"|{vehicle.registration}{NumSpaces(vehicle.registration, regborder)}|{vehicle.grade}{NumSpaces(vehicle.grade.ToString(), gradeborder)}|" +
                    $"{vehicle.make}{NumSpaces(vehicle.make, MakeBorder)}|{vehicle.model}{NumSpaces(vehicle.model, ModelBorder)}|{vehicle.year}{NumSpaces(vehicle.year.ToString(), YearBorder)}|" +
                    $"{vehicle.numSeats}{NumSpaces(vehicle.numSeats.ToString(), numseatsBorder)}|{vehicle.transmissionType}{NumSpaces(vehicle.transmissionType.ToString(), TransTypeBorder)}" +
                    $"|{vehicle.fuel}{NumSpaces(vehicle.fuel.ToString(), FuelBorder)}|{vehicle.GPS}{NumSpaces(vehicle.GPS.ToString(), GPSborder)}|" +
                    $"{vehicle.sunRoof}{NumSpaces(vehicle.sunRoof.ToString(), sunroofborder)}|{vehicle.dailyRate}{NumSpaces(vehicle.dailyRate.ToString(), dailyrateborder)}" +
                    $"|{vehicle.colour}{NumSpaces(vehicle.colour, colourborder)}|\n{border}");

                    Write(body);
                }
            }else
            {
                WriteLine("\nERROR: No vehicles exist to be displayed");
            }
        }
    }
}
