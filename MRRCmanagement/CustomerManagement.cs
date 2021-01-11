using System;
using static System.Console;
using static MRRCmanagement.QualityOfLife;

namespace MRRCmanagement
{
    //Customer class | Contains customer constructors, methods, and private and public variables | Does not inherit from any class | Is inherited by no class
    public class CRM
    {
        //Initialize private variables relating to customer information
        //Restrict gender inputs
        public enum Gender
        {
            Male,
            Female,
            Other,
            unknown   //Unknown used for invalid Gender inputs. This is never displayed unless the csv file has incorrect gender information
        }
        private int id_number;
        //General getter and setter for id_number
        public int Id_number
        {
            get { return id_number; }
            set { value = id_number; }
        }
        private string title;
        private string firstName;
        private string lastName;
        private Gender gender;
        private DateTime DoB;

        //Constructor 1 | every variable input needed
        public CRM(int id, string Title, string firstname, string lastname, string Gender, string dob)
        {
            id_number = id;
            title = Title;
            firstName = firstname;
            lastName = lastname;
            gender = ToGender(Gender);
            DoB = DateTime.Parse(dob);

        }

        //Constructor 2 | csv string inputted, split at the commas, and used to initialise the variables
        public CRM(string csv)
        {
            string[] aspects = csv.Split(',');
            id_number = int.Parse(aspects[0]);
            title = aspects[1];
            firstName = aspects[2];
            lastName = aspects[3];
            gender = ToGender(aspects[4]);
            DoB = DateTime.Parse(aspects[5]);
        }

        //Takes a CRM class customer, and converts the aspects to a csv compatible string
        public static string CRMToCsvString(CRM customer)
        {
            string res;
            res = $"{customer.id_number},{customer.title},{customer.firstName},{customer.lastName},{customer.gender},{customer.DoB}";
            return res;
        }

        //Back-end of Menu.Modify_Customer | User selects aspect to modify and inputs what they will change it to
        public void ModifyCustomer()
        {
            //Two arrays used to help in de-cluttering the ModifyCustomer UI | aspectNames holds aspect names to refer to. Aspects holds the values of the
            //aspects to refer to
            string[] aspectNames = { "ID", "Title", "First Name", "Last Name", "Gender", "Date of Birth" };
            string[] aspects = { id_number.ToString(), title, firstName, lastName, gender.ToString(), DoB.ToString()};

            WriteLine("\nSelect which aspect of customer to modify\n");

            //Write the customer aspects and their letter ID
            char counter = 'A';
            foreach (string element in aspects) {
                //-65 used to convert the ascii character to equivalent number needed to access the list (-64 to convert to a number, -1 to access the choice in a list)
                int intcount = counter - 65;
                WriteLine($"\n{counter} - {aspectNames[intcount]}");
                counter++;
            }
            //Remove the last unnessesary increase of the counter value
            counter--;
            
            //Method from MRRCmanagement.QualityOfLife | Promts user for choice and tests if it is within range of two args
            //Also promts user for backspace or ESC input
            CharWithinRange('A', counter);

            //'\0' null value was used to signify that BackSpace was pressed, as the user cannot enter a null value
            if (charqolchoice == '\0')
            {
                //Return to previous menu
                Menu.Modify_Customers();
                return;
            }

            //Refer to loop above, used to convert char choice to number to access list
            int choice = charqolchoice - 65;

            //Prompt user for change
            WriteLine($"\nYou have entered {charqolchoice}, for {aspectNames[choice]}\nThe Current {aspectNames[choice]} for this customer is " +
                $"{aspects[choice]}. What would you like to change it to?\n");

            string change = ReadLine();
            
            //Set of if statements depending on user's input
            //Change ID number
            if (charqolchoice == 'A')
            {
                //Test if user's change was an integer
                if (!int.TryParse(change, out int new_ID))
                {
                    WriteLine("\nERROR: Please enter a valid whole number ID\n");
                    this.ModifyCustomer();
                    return;
                }
                //Test if ID input is unique
                bool modify = true; //if modification is unique this will be true
                foreach (CRM customer in Menu.Customers)
                {
                    if (customer.Id_number == new_ID)
                    {
                        //If ID is not unique, prompt user to change another aspect of the customer/return user to main menu
                        WriteLine($"\nERROR: Customer of id {new_ID} already exists. Please enter a unique ID number\n");
                        modify = false; // Using this.modifycustomer() here will modify the customer with the ID entered by the user, not the
                                        //customer that the user chose
                    }
                }
                if (modify == true)
                {
                    this.id_number = new_ID;
                } else
                {
                    this.ModifyCustomer();
                    return;
                }
            } else if (charqolchoice == 'B') // Change Title
            {
                this.title = change;
            } else if (charqolchoice == 'C') // Change First Name
            {
                this.firstName = change;
            } else if (charqolchoice == 'D') // Change Last Name
            {
                this.lastName = change;
            } else if (charqolchoice == 'E') // Change Gender
            {
                //Test if gender is valid (same method as ID number)
                if (ToGender(change) == Gender.unknown)
                {
                    WriteLine("\nERROR: Please enter a valid gender (Male, Female, Other)");
                    this.ModifyCustomer();
                    return;
                }
                else
                {
                    this.gender = ToGender(change);
                }
            } else if (charqolchoice == 'F') // Change Date of Birth
            {
                //Test if input is valid (Same method as ID number and gender)
                if (!DateTime.TryParse(change, out DateTime DOBtemp))
                {
                    WriteLine("\nERROR: Please enter a valid date time format (xx/xx/xx)");
                    this.ModifyCustomer();
                    return;
                }
                else
                {
                    DoB = DOBtemp;
                }
            }

            Write($"\nYou have changed the {aspectNames[choice]} from {aspects[choice]} to ");
            //Update aspects list with change
            aspects[choice] = change;
            WriteLine($"{aspects[choice]}.\n");

            //Display new customer table
            Display_Customers();
            
            WriteLine("\nWould you like to modify another aspect of this customer? (Y/N)\n");

            //Read key Y or N. If N end the method, if Y call the method then end the method, else print error message
            while (true)
            {
                ConsoleKeyInfo YN = ReadKey();
                char parser;

                if (YN.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
                else if (YN.Key == ConsoleKey.Backspace) // Return to previous menu
                {
                    return;
                }
                else if (char.TryParse(YN.Key.ToString(), out parser))
                {
                    if (char.Parse(YN.Key.ToString().ToUpper()) == 'N') // Return to previous menu, with a newline for better formatting
                    {
                        Console.WriteLine();
                        return;
                    }
                    else if (char.Parse(YN.Key.ToString().ToUpper()) == 'Y') // Call this method again, then end this instance of the method
                    {
                        ModifyCustomer();
                        return;
                    }
                }
                else
                {
                    WriteLine("\nPlease enter a valid character (Y/N)\n");
                }
            }
        }

        //Displays text format of customer | For use in other methods, when reasonable to show user customer info
        public void Display_Info(int format)
        {
            //Display information about private variables
            if (format == 1)
            {
                WriteLine($"\nID Number: {id_number}\nTitle: {title}\nFirst Name: {firstName}\nLast Name: {lastName}\nGender: {gender}\n" +
                    $"Date Of Birth: {DoB}");
            }
            //Only dispay most important variables, for quicker and easier reading
            //Only dispay most important variables, for quicker and easier reading
            else if (format == 2)
            {
                WriteLine($"{id_number} - {title}. {firstName} {lastName}");
            }
            //Format will never be higher than 2 or lower than 1. User has no control over this method's format
        }

        //Display all customers in table format
        public static void Display_Customers()
        {
            if (Menu.Customers.Count > 0)
            {
                //initialise array of lengths of headings (if the longest name is shorter than the heading for first name, the length of the heading will instead be used)
                int[] highest = { 2, 5, 10, 9, 6, 3 };

                //Looping through each customer, making each aspect to an element in a temp array, finding the highest length of each and assigning that number
                //to a variable
                foreach (CRM customer in Menu.Customers)
                {
                    //Splitting the customer string into an array of it's aspects
                    string[] CustAspects = { customer.id_number.ToString(), customer.title, customer.firstName, customer.lastName, customer.gender.ToString(), customer.DoB.ToString() };

                    for (int i = 0; i <= CustAspects.Length - 1; i++)
                    {
                        if (CustAspects[i].ToString().Length > highest[i])
                        {
                            highest[i] = CustAspects[i].ToString().Length;
                        }
                    }
                }

                //Initialize border lengths (+1 used for readability and reducing clutter)
                int IDBorder = highest[0] + 1;
                int TitleBorder = highest[1] + 1;
                int FirstNameBorder = highest[2] + 1;
                int LastNameBorder = highest[3] + 1;
                int GenderBorder = highest[4] + 1;
                int DoBBorder = highest[5] + 1;

                //Initialize Strings, holding a dynamic amount of dashes, depending on variable lengths
                string IDline = new String('-', IDBorder);
                string TitleLine = new String('-', TitleBorder);
                string FirstNameLine = new String('-', FirstNameBorder);
                string LastNameLine = new String('-', LastNameBorder);
                string GenderLine = new String('-', GenderBorder);
                string DoBline = new String('-', DoBBorder);

                //Top and bottom border of each cell
                string border = $"+{IDline}+{TitleLine}+{FirstNameLine}+{LastNameLine}+{GenderLine}+{DoBline}+\n";

                //create and write heading row of table
                string heading = ($"\n{border}" +
                          $"|ID{NumSpaces("ID", IDBorder)}|Title{NumSpaces("Title", TitleBorder)}|First Name{NumSpaces("First Name", FirstNameBorder)}|Last Name" +
                          $"{NumSpaces("Last Name", LastNameBorder)}|Gender{NumSpaces("Gender", GenderBorder)}|DOB{NumSpaces("DOB", DoBBorder)}|\n{border}");
                Write($"{heading}");

                //Loop through each customer and create a row for each
                foreach (CRM customer in Menu.Customers)
                {
                    string body = ($"|{customer.id_number}{NumSpaces(customer.id_number.ToString(), IDBorder)}|{customer.title}{NumSpaces(customer.title, TitleBorder)}|" +
                    $"{customer.firstName}{NumSpaces(customer.firstName, FirstNameBorder)}|{customer.lastName}{NumSpaces(customer.lastName, LastNameBorder)}|" +
                    $"{customer.gender}{NumSpaces(customer.gender.ToString(), GenderBorder)}|{customer.DoB}{NumSpaces(customer.DoB.ToString(), DoBBorder)}|\n" +
                    $"{border}");

                    Write(body);
                }
            }
            else
            {
                WriteLine("\nERROR: No customers exist to be displayed\n");
            }
        }
    }
}
