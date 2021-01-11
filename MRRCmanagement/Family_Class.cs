using System;
using System.Collections.Generic;
using System.Text;
using MRRCmanagement;
using static MRRCmanagement.Vehicle;

namespace MRRCmanagement
{
    //Inherits from the abstract vehicle class | creates a vehicle object either with preset default attributes or user-inputted attributes
    public class Family_Class : Vehicle
    {
        //Method using defaults
        public Family_Class(string registration, string grade, string make, string model, int year, int numSeats = 4,
    transmission transmissionType = transmission.Automatic, fuelType fuel = fuelType.Petrol, bool GPS = true, bool sunRoof = true, double
    dailyRate = 80.00, string colour = "black") : base(registration, grade, make, model, year)
        {
            this.numSeats = numSeats;
            this.transmissionType = transmissionType;
            this.fuel = fuel;
            this.GPS = GPS;
            this.sunRoof = sunRoof;
            this.dailyRate = dailyRate;
            this.colour = colour;
        }

        //Method not using defaults
        public Family_Class(string registration, string grade, string make, string model, int year, int numseats, string transmission, string fuel, bool GPS,
            bool sunRoof, double dailyRate, string colour) : base(registration, grade, make, model, year, numseats, transmission, fuel, GPS,
            sunRoof, dailyRate, colour)
        {
            //Body taken from vehicle class
        }

        //Method using csv strings
        public Family_Class(string a) : base(a)
        {
            //body taken from vehicle class
        }
    }
}
