using static System.Console;
using static MRRCmanagement.QualityOfLife;
using static MRRCmanagement.Vehicle;
using static MRRCmanagement.Menu;

namespace MRRCmanagement
{
    //Main Menus for the user interface
    public class UI
    {
        //Prompts the user to intiate one of the 4 vehicle based methods
        public static void VehicleManagementMenu()
        {
            while (true){
                WriteLine("\n---------Vehicle Management Menu---------\n");
                WriteLine("Please select from the options below:\nA. Display Vehicles\nB. New Vehicle\nC. Modify Vehicle\nD. Delete Vehicle");
                CharWithinRange('A', 'D');

                if (char.ToUpper(charqolchoice) == 'A')
                {
                    if (Vehicles.Count == 0)
                    {
                        WriteLine("ERROR: No vehicles to display. Please enter a vehcile and try again");
                    }
                    else
                    {
                        Display_Vehicles(Menu.Vehicles);
                    }
                }
                else if (char.ToUpper(charqolchoice) == 'B')
                {
                    Create_Vehicle();
                }
                else if (char.ToUpper(charqolchoice) == 'C')
                {
                    Modify_Vehicle();
                }
                else if (char.ToUpper(charqolchoice) == 'D')
                {
                    Delete_Vehicle();
                } else if (charqolchoice == '\0')
                {
                    break;
                }
            }
        }

        //Prompts user to intiate one the 4 customer based methods
        public static void CustomerManagementMenu()
        {
            WriteLine("\n---------Customer Management Menu---------");
            while (true)
            {
                WriteLine("\nPlease select from the options below:\nA. Display Customers\nB. New Customer\nC. Modify Customer\nD. Delete Customer");
                CharWithinRange('A', 'D');

                if (char.ToUpper(charqolchoice) == 'A')
                {
                    if (Customers.Count == 0)
                    {
                        WriteLine("\nERROR: No customers to display. Please add a customer and try again");
                    }
                    else
                    {
                        CRM.Display_Customers();
                    }
                }
                else if (char.ToUpper(charqolchoice) == 'B')
                {
                    Create_Customer();
                }
                else if (char.ToUpper(charqolchoice) == 'C')
                {
                    Modify_Customers();
                }
                else if (char.ToUpper(charqolchoice) == 'D')
                {
                    Delete_customer();
                }
                else if (charqolchoice == '\0')
                {
                    break;
                }
                
            }
        }

        //Prompts user to intiate one the 4 rental based methods
        public static void RentalManagementMenu()
        {
            WriteLine("\n---------Rental Management Menu---------");
            while (true)
            {
                WriteLine("\nPlease select from the options below:\nA. Display Rentals\nB. New Rental\nC. Search Vehicles\nD. Return Rental");
                CharWithinRange('A', 'D');

                if (char.ToUpper(charqolchoice) == 'A')
                {
                    if (Rentals.Count == 0)
                    {
                        WriteLine("\nERROR: No rentals to display. Please add a rental and try again");
                    }
                    else
                    {
                        Rental.Display_Rentals();
                    }
                }
                else if (char.ToUpper(charqolchoice) == 'B')
                {
                    Create_Rental();
                }
                else if (char.ToUpper(charqolchoice) == 'C')
                {
                    WriteLine("\n\nNote: cannot formally exit the program while searching. Return to main menu to do so.");
                    WriteLine("Search tokens of more than 1 word require double quotation marks around them | searching for lack of a GPS or SunRoof can be done " +
                        "with a tilda before the token (e.g. ~GPS or ~sunroof)");
                    RPNSearch.Search.search();
                }
                else if (char.ToUpper(charqolchoice) == 'D')
                {
                    Return_Rental();
                }
                else if (charqolchoice == '\0')
                {
                    break;
                }
            }
        }

        //Main menu | prompts the user to initiate one of the three sub-menus outlined in this class
        public static void MenuLayout()
        {
            while (true)
            {
                WriteLine("\n----- Mates Rates Rent-a-car Operation Menu -----\n");
                WriteLine("Press the BackSpace key at any time to go return to the previous menu\n");

                WriteLine("Please select from the choices below:\n\nA. Customer Management\nB. Fleet Management\nC. Rental Management\n");
                CharWithinRange('A', 'C');

                    if (char.ToUpper(charqolchoice) == 'A')
                    {
                        CustomerManagementMenu();
                    }
                    else if (char.ToUpper(charqolchoice) == 'B')
                    {
                        VehicleManagementMenu();
                    }
                    else if (char.ToUpper(charqolchoice) == 'C')
                    {
                        RentalManagementMenu();
                    } else if (charqolchoice == '\0')
                    {
                        WriteLine("\nERROR: No previous menu to enter\n");
                    }
            }
        }
    }
}
