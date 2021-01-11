using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using static System.Console;
using myRPN;
using MRRCmanagement;
using static MRRCmanagement.QualityOfLife;

//RPN namespace, classes, and methds heavily influenced from code located under the week 9 learning recourses on BlackBoard.
namespace RPNSearch
{
    //Class for initiating the search
    public class Search
    {
        //First Search method, takes user input and runs the private search method
        public static void search()
        {
            setUpVehicles(out Fleet fleet); // set up a vehicles fleet
            while (true) // forever or until user quits
            {
                try
                {
                    getQuery(out ArrayList query); //Get user input, formatted in a way that complies with the search function
                    if (query[0].Equals("QUIT")) //If user pressed enter with no characters before it
                    {
                        break;
                    }
                    search(new RPN(query), fleet); // convert query to RPN and search
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }
            }

            //Set up vehicles from csv vehicles
            void setUpVehicles(out Fleet fleetVehicles)
            {
                fleetVehicles = new Fleet();
                for (int i = 0; i < QualityOfLife.csvVehicles.Count; i++)
                {
                    fleetVehicles.insertVehicle(QualityOfLife.csvVehicles[i]);
                }
            }

            //"Front-End" for the search method | display messages to user about the state of the search
            void search(RPN rpn, Fleet flt)
            {
                // create some vehicles and create sets of attributes.
                // a set is comprised of vehicle number plates

                WriteLine("\n------------ Searching ----------");

                List<VehicleSearchFormat> vehicles = new List<VehicleSearchFormat>();

                for (int i = 0; i < flt.Length; i++)
                {
                    vehicles.Add((VehicleSearchFormat)fleet.vehicles[i]);
                }
                Fleet.Display_Vehicles(vehicles); //Show list of vehicles that are being searched

                WriteLine("\n\n------------Results-------------");
                flt.search(out string[] result, rpn);

                if (result.Length == 0)
                    throw new Exception("No match found");
                flt.displayVehicle(result); //Display search result
            }

            //Prompts user for input and converts input to be search compatible
            string getQuery(out ArrayList query)
            {
                // accept user query and do some validation
                // note that some non-sensical queries can still pass validation
                // for instance,  AND RED (( OR ) BLUE)
                // but these will be caught later 

                query = new ArrayList();
                string queryText = "";

                Write("\nEnter your query, or just hit Enter to quit:  ");
                string temp = Console.ReadLine();
                if (temp.Length > 0)
                {
                    // separate parenthesis before splitting string
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Equals('(') || temp[i].Equals(')'))
                        {
                            queryText += $" {temp[i]} ";
                        }
                        else
                        {
                            queryText += temp[i];
                        }
                    }


                    if (queryText.Contains('"')) //If user is searching for a token with multiple words
                    {
                        int x = queryText.IndexOf('"');
                        int y = queryText.Substring(x + 1).ToString().IndexOf('"') + 1;
                        int len = queryText.Substring(x, y - x).Length; //Substring between the two quotation marks

                        string res = queryText.Substring(x, len).ToUpper();
                        queryText = queryText.Remove(x, len + 1); //Remove from queryText
                        query.AddRange(res.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)); //Add token to query
                    }

                    queryText = queryText.ToUpper();
                    query.AddRange(queryText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)); // split to tokens (default delimiter is space)
                }
                else
                {
                    query.Add("QUIT");
                }
                return temp;
            } // end getQuery()
        }
    } // end search class

    //Vehicle class for vehicles in RPN search format
    public class VehicleSearchFormat
    {
        //Initialise vehicle aspects
        public string Rego { get; }
        public string Grade { get; }
        public string Make { get; }
        public string Model { get; }
        public string Year { get; }
        public string NumSeats { get; }
        public string Transmission { get; }
        public string Fuel { get; }
        public string GPS { get; }
        public string SunRoof { get; }
        public string DailyRate { get; }
        public string Colour { get; }

        //Get csv string of vehicle and set variables 
        public VehicleSearchFormat(string vehicleCSV)
        {
            // vehicle constructor from CSV string
            ArrayList values = new ArrayList();
            values.AddRange(vehicleCSV.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            Rego = (string)values[0].ToString().ToUpper();
            Grade = (string)values[1].ToString().ToUpper();
            Make = (string)values[2].ToString().ToUpper();
            Model = (string)values[3].ToString().ToUpper();
            Year = (string)values[4].ToString().ToUpper();
            NumSeats = (string)values[5].ToString().ToUpper();
            Transmission = (string)values[6].ToString().ToUpper();
            Fuel = (string)values[7].ToString().ToUpper();


            //For readability | So the user can search "GPS" and "~GPS" instead of "TRUE" and "FALSE" (which are the same for SunRoof)
            if ((string)values[8] == "False")
            {
                GPS = "~GPS";
            } else
            {
                GPS = "GPS";
            }

            if ((string)values[9] == "False")
            {
                SunRoof = "~SUNROOF";
            } else
            {
                SunRoof = "SUNROOF";
            }

            DailyRate = (string)values[10].ToString();
            Colour = (string)values[11].ToString().ToUpper();
        }
    } // end Vehicle class

    //Fleet class, for more general vehicle methods
    public class Fleet
    {   // Fleet class, maintaining a vehicles list, and attribute sets for lookup
        // as well as several methods to search and to display vehicles
        public ArrayList vehicles = new ArrayList(); // vehicles in the fleet
        public static SortedList attributeSets = new SortedList(); // sets of vehicles (regos) with a given attribute value
        public static List<string> attributeList = new List<string>();
        public int Length
        {
            get
            {
                return vehicles.Count;
            }
        }

        //Add vehicle aspects to relevant lists used in searching
        public Fleet()
        {

            foreach (Vehicle vehicle in Menu.Vehicles)
            {
                string GPSVal;
                if (vehicle.GPS == false)
                {
                    GPSVal = "~GPS";
                }
                else
                {
                    GPSVal = "GPS";
                }

                string SRVal;
                if (vehicle.sunRoof == false)
                {
                    SRVal = "~SUNROOF";
                }
                else
                {
                    SRVal = "SUNROOF";
                }

                //Add attributes to attributes lists
                if (!attributeList.Contains(vehicle.colour.ToString().ToUpper()))
                {
                    attributeSets.Add(vehicle.colour.ToString().ToUpper(), new HashSet<string>());
                    attributeList.Add(vehicle.colour.ToUpper());
                }
                if (!attributeList.Contains(vehicle.Make.ToString().ToUpper()))
                {
                    attributeSets.Add(vehicle.Make.ToString().ToUpper(), new HashSet<string>());
                    attributeList.Add(vehicle.Make.ToUpper());
                }
                if (!attributeList.Contains(vehicle.Model.ToString().ToUpper()))
                {
                    attributeSets.Add(vehicle.Model.ToString().ToUpper(), new HashSet<string>());
                    attributeList.Add(vehicle.Model.ToUpper());
                }
                if (!attributeList.Contains(vehicle.Year.ToString().ToUpper()))
                {
                    attributeSets.Add(vehicle.Year.ToString().ToUpper(), new HashSet<string>());
                    attributeList.Add(vehicle.Year.ToString().ToUpper());
                }
                if (!attributeList.Contains(vehicle.fuel.ToString().ToUpper()))
                {
                    attributeSets.Add(vehicle.fuel.ToString().ToUpper(), new HashSet<string>());
                    attributeList.Add(vehicle.fuel.ToString().ToUpper());
                }
                if (!attributeList.Contains(GPSVal))
                {
                    attributeSets.Add(GPSVal, new HashSet<string>());
                    attributeList.Add(GPSVal.ToUpper());
                }
                if (!attributeList.Contains(SRVal))
                {
                    attributeSets.Add(SRVal, new HashSet<string>());
                    attributeList.Add(SRVal.ToUpper());
                }
                if (!attributeList.Contains(vehicle.transmissionType.ToString().ToUpper()))
                {
                    attributeSets.Add(vehicle.transmissionType.ToString().ToUpper(), new HashSet<string>());
                    attributeList.Add(vehicle.transmissionType.ToString().ToUpper());
                }
            }

            // now initialise grades 
            if (!attributeList.Contains("ECONOMY"))
            {
                attributeSets.Add("ECONOMY", new HashSet<string>());
                attributeList.Add("ECONOMY");
            }
            if (!attributeList.Contains("FAMILY")){
                attributeSets.Add("FAMILY", new HashSet<string>());
                attributeList.Add("FAMILY");
            }
            if (!attributeList.Contains("LUXURY")) {
                attributeSets.Add("LUXURY", new HashSet<string>());
                attributeList.Add("LUXURY");
            }
            if (!attributeList.Contains("COMMERCIAL")) {
                attributeSets.Add("COMMERCIAL", new HashSet<string>());
                attributeList.Add("COMMERCIAL");
            }
        }

        // method to insert a vehicle
        public void insertVehicle(string vehicleCSV)
        {
            int idx;
            HashSet<string> hs;
            VehicleSearchFormat v = new VehicleSearchFormat(vehicleCSV);
            vehicles.Add(v);
            idx = attributeSets.IndexOfKey(v.Make);
            if (idx >= 0)
            {   // here if make set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego); // add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.Model);
            if (idx >= 0)
            {   // here if Make model found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.Year);
            if (idx >= 0)
            {   // here if year found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.Transmission);
            if (idx >= 0)
            {   // here if Turbo transmission found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.Fuel);
            if (idx >= 0)
            {   // here if Turbo fuel found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.GPS);
            if (idx >= 0)
            {   // here if GPS set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.NumSeats);
            if (idx >= 0)
            {   // here if NumSeats set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.SunRoof);
            if (idx >= 0)
            {   // here if SunRoof set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.DailyRate);
            if (idx >= 0)
            {   // here if DailyRate set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.Colour);
            if (idx >= 0)
            {   // here if Colour set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego);// add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
            idx = attributeSets.IndexOfKey(v.Grade);
            if (idx >= 0)
            {   // here if Grade set found
                hs = (HashSet<string>)attributeSets.GetByIndex(idx); // get set
                hs.Add(v.Rego); // add to set
                attributeSets.SetByIndex(idx, hs);  // save set (replaces old set)
            }
        }   

        //Get regos of vehicles and call Display_vehicles on the corresponding vehicles
        public void displayVehicle(string[] regos)
        {   // display vehicle by registration plate (rego)
            VehicleSearchFormat v;
            List<VehicleSearchFormat> vehicleList = new List<VehicleSearchFormat>();

            for (int i = 0; i < vehicles.Count; i++)
            {   // go through, find the vehicle by rego
                v = (VehicleSearchFormat)vehicles[i];
                if (regos.Contains<string>(v.Rego))
                {
                    vehicleList.Add(v);
                }
            }
            Display_Vehicles(vehicleList); //Desplay each vehicle in table
        }

        //Part of searching | controls the operator methods
        public void search(out string[] result, RPN rpn)
        {
            // Create and instantiate a new empty Stack for result sets.
            Stack setStack = new Stack();
            HashSet<string> hs1;
            HashSet<string> hs2;
            HashSet<string> hs;
            int idx;
            String[] temp = new string[] { };
            for (int i = 0; i < rpn.Postfix.Count; i++)
            {
                if (rpn.Postfix[i].Equals("AND"))
                {
                    // pop two sets off the stack and apply Intersect, push back result
                    hs1 = (HashSet<string>)setStack.Pop();
                    hs2 = (HashSet<string>)setStack.Pop();
                    temp = hs1.ToArray<string>(); // copy the elements of the set hs1
                    hs = new HashSet<string>(temp); // make a deep copy of hs1
                    hs.IntersectWith(hs2);// apply the Intersect to the new set
                    setStack.Push(hs); // push a reference to a new set
                }
                else if (rpn.Postfix[i].Equals("OR"))
                {
                    // pop two sets off the stack and apply Union
                    hs1 = (HashSet<string>)setStack.Pop();
                    hs2 = (HashSet<string>)setStack.Pop();
                    temp = hs1.ToArray<string>(); // copy the elements of the set hs1
                    hs = new HashSet<string>(temp); // make a deep copy of hs1
                    hs.UnionWith(hs2); // apply the Union to the new set
                    setStack.Push(hs); // push a reference to a new set
                }
                else
                {
                    // here if an operand
                    idx = attributeSets.IndexOfKey(rpn.Postfix[i]); // identify attribute set
                    if (idx >= 0)
                    {
                        hs1 = (HashSet<string>)attributeSets.GetByIndex(idx);
                        setStack.Push(hs1); // note: pushing a reference, not the actual set
                    }
                    else
                    {
                        throw new FormatException("Invalid attribute " + rpn.Postfix[i]);
                    }
                }
            }
            if (setStack.Count == 1)
            {
                hs1 = (HashSet<string>)setStack.Pop();
                result = hs1.ToList().ToArray();
            }
            else
            {
                throw new Exception("Invalid query");
            }
        }

        //Same method as in class FleetManagement | Displays vehicles to screen in table format
        public static void Display_Vehicles(List<VehicleSearchFormat> VehicleList)
        {
            int[] highest = { 12, 5, 4, 5, 4, 8, 12, 4, 3, 6, 10, 6 };

            foreach (VehicleSearchFormat vehicle in VehicleList)
            {
                string[] vehicleaspects = { vehicle.Rego, vehicle.Grade.ToString(), vehicle.Make, vehicle.Model, vehicle.Year.ToString(),
                    vehicle.NumSeats.ToString(), vehicle.Transmission.ToString(), vehicle.Fuel.ToString(), vehicle.GPS.ToString(), vehicle.SunRoof.ToString(),
                    vehicle.DailyRate.ToString(), vehicle.Colour};

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

            foreach (VehicleSearchFormat v in VehicleList)
            {
                string body = ($"|{v.Rego}{NumSpaces(v.Rego, regborder)}|{v.Grade}{NumSpaces(v.Grade.ToString(), gradeborder)}|" +
                $"{v.Make}{NumSpaces(v.Make, MakeBorder)}|{v.Model}{NumSpaces(v.Model, ModelBorder)}|{v.Year}{NumSpaces(v.Year.ToString(), YearBorder)}|" +
                $"{v.NumSeats}{NumSpaces(v.NumSeats.ToString(), numseatsBorder)}|{v.Transmission}{NumSpaces(v.Transmission, TransTypeBorder)}" +
                $"|{v.Fuel}{NumSpaces(v.Fuel.ToString(), FuelBorder)}|{v.GPS}{NumSpaces(v.GPS.ToString(), GPSborder)}|" +
                $"{v.SunRoof}{NumSpaces(v.SunRoof.ToString(), sunroofborder)}|{v.DailyRate}{NumSpaces(v.DailyRate.ToString(), dailyrateborder)}" +
                $"|{v.Colour}{NumSpaces(v.Colour, colourborder)}|\n{border}");

                Console.Write(body);
            }
        }

    } //end Fleet class
}
