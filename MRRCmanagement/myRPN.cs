using System;
using System.Collections.Generic;
using System.Collections;
using MRRCmanagement;

//RPN Classes | Heavily influenced from the code examples given on blackboard under week 9 of learning resources
namespace myRPN
{
    public class RPN
    {
        //ArrayLists to contain the operands and operators
        ArrayList operators;
        ArrayList operands;

        //Arraylists to contain the infix and postfix of the stack for the search
        public ArrayList Infix { get; } = new ArrayList();
        public ArrayList Postfix { get; } = new ArrayList();

        //Constructor that completes the search
        public RPN(ArrayList query)
        {
            // define valid tokens
            operators = new ArrayList();
            operands = new ArrayList();
            string[] t1 = { "AND", "OR", "(", ")" }; // operators and parentheses
            operators.AddRange(t1);

            //Loop through csv vehicles list and add them to potential operands 
            List<string> temp = new List<string>();
            for (int i = 0; i<QualityOfLife.csvVehicles.Count; i++)
            {
                string [] x = QualityOfLife.csvVehicles[i].Split(',');

                //Change GPS value to be more readable for user (Instead of just displaying TRUE or FALSE for both GPS and SunRoof)
                if (x[8] == "True")
                {
                    x[8] = "GPS";
                } else {
                    x[8] = "~GPS";
                }

                if (x[9] == "False")
                {
                    x[9] = "~SUNROOF";
                } else
                {
                    x[9] = "SUNROOF";
                }


                for (int j = 0; j < x.Length; j++)
                {
                    if (!temp.Contains(x[j].ToUpper())) //If operand is not already in operand list
                    {
                        temp.Add(x[j].ToUpper());
                    }
                }
            }

            string[] t2 = temp.ToArray();

            //Add operands to operands page
            operands.AddRange(t2);

            // Create and instantiate a new empty Stack.
            Stack rpnStack = new Stack();

            // apply dijkstra algorithm using a stack to convert infix to postfix notation (=rpn)
            Infix.AddRange(query);
            foreach (string token in Infix)
            {
                if (operands.Contains(token))
                {   // move operands across to output
                    Postfix.Add(token);
                }
                else if (token.Equals("("))
                {   // push open parenthesis onto stack
                    rpnStack.Push(token);
                }
                else if (token.Equals(")"))
                {   // pop all operators off the stack until the mathcing open parenthesis is found
                    while ((rpnStack.Count > 0) && !((string)rpnStack.Peek()).Equals("("))
                    {
                        Postfix.Add(rpnStack.Pop());  // transfer operator to output
                        if (rpnStack.Count == 0)
                            throw new Exception("Ubalanced parenthesis");
                    }
                    if (rpnStack.Count == 0)
                        throw new Exception("Ubalanced parenthesis");
                    rpnStack.Pop(); // discard open parenthesis
                }
                else if (operators.Contains(token))
                {   // push operand to the rpn stack after moving to output all higher or equal priority operators
                    while (rpnStack.Count > 0 && ((string)rpnStack.Peek()).Equals("AND"))
                    {
                        Postfix.Add(rpnStack.Pop());  // pop and add to output
                    }
                    rpnStack.Push(token); // now pus the operator onto the stack
                }
                else
                    throw new Exception("Unrecognised token " + token);
            }
            // now copy what's left on the rpnStack
            while (rpnStack.Count > 0)
            {   // move to the output all remaining operators
                if (((string)rpnStack.Peek()).Equals("("))
                    throw new Exception("Ubalanced parenthesis");
                Postfix.Add(rpnStack.Pop());
            }
        } // end RPN() constructor

    } //end Class RPN
}