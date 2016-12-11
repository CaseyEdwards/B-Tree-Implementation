//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		Driver.cs
//	Description:		Driver file for the B-Tree project. Controls console input/output and manages a
//                          B-Tree object for user manipulation.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Tuesday, November 15, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UtilityNamespace;

namespace BTreeProject
{
    /// <summary>
    /// Driver class.
    /// Handles user I/O and manipulation of a B-tree object in a
    /// console-based menu-driven context.
    /// </summary>
    class Driver
    {
        #region Properties
        private static Menu menu = new Menu("B-Tree Program"); // The main menu.
        private static BTree tree;  // The B-Tree object
        #endregion

        #region Main and Menu Builder
        /// <summary>
        /// Application entry point.
        /// Displays a menu for user interaction and performs a switch operation
        /// to take the action specified by the user.
        /// </summary>
        public static void Main()
        {
            MenuChoice userChoice; // User's menu choice.

            // Build the menu and set the background color.
            BuildMenu();
            Console.BackgroundColor = ConsoleColor.White;

            // Loop until user wishes to exit.
            do
            {
                // Get the user's choice cast as a MenuChoice enum, then enter the switch.
                userChoice = (MenuChoice)menu.GetChoice();
                switch (userChoice)
                {
                    case MenuChoice.CREATE_TREE:
                        // Create a new B-Tree object.
                        CreateTree();
                        break;
                    case MenuChoice.POPULATE:
                        // Add values to the tree.
                        PopulateTree();
                        break;
                    case MenuChoice.DISPLAY:
                        // Display the tree.
                        DisplayTree();
                        break;
                    case MenuChoice.ADD_VALUE:
                        // Add a specific value to the tree.
                        AddValue();
                        break;
                    case MenuChoice.FIND_VALUE:
                        // Search for a value.
                        FindValue();
                        break;
                    case MenuChoice.EXIT:
                        // Display an exit message and, upon user keypress, exit the loop and program.
                        Console.Clear();
                        Console.SetCursorPosition((Console.WindowWidth - 50) / 2, Console.WindowHeight / 2);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Thank you for using the B-Tree Program!");
                        Console.SetCursorPosition((Console.WindowWidth - 50) / 2, Console.WindowHeight / 2 + 1);
                        Console.WriteLine("Press any key to close.");
                        Console.ReadKey();
                        break;
                }
            } while (userChoice != MenuChoice.EXIT);
        }
        
        /// <summary>
        /// Builds the main menu.
        /// </summary>
        private static void BuildMenu()
        {
            // Set an option corresponding to each MenuChoice enum.
            menu += "Set size of nodes and create a new B-Tree.";
            menu += "Add random integer values to the B-Tree.";
            menu += "Display the B-Tree.";
            menu += "Add a specific value to the B-Tree.";
            menu += "Find a value in the B-Tree.";
            menu += "End the program.";
        }
        #endregion

        #region Menu Option Methods
        /// <summary>
        /// Creates the B-tree object, replacing the current one if it exists.
        /// Requests a node size from the user that must be > 2.
        /// </summary>
        private static void CreateTree()
        {
            int nodeSize = 0; // Node Size specified by user.

            // Loop until user enters a valid size.
            while (nodeSize <= 2)
            {
                // Prompt for a node size.
                Console.Clear();
                Console.Write("Enter a node size: ");
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    nodeSize = Int32.Parse(Console.ReadLine());
                    if (nodeSize <= 2)
                    {
                        // Size is too small.
                        Console.WriteLine("\n\nPlease enter a positive integer value > 2.");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }
                }
                catch (FormatException)
                {
                    // User entered a non-integer.
                    Console.WriteLine("\n\nPlease enter a valid number.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                }
                Console.ForegroundColor = ConsoleColor.Blue;
            }

            // Valid node size obtained, create a new BTree object with it.
            tree = new BTree(nodeSize);

            // Display success message and exit the method on keypress.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n\tTree successfully created with {nodeSize}-ary node size.");
            Console.WriteLine("\n\tPress any key to return to the main menu.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ReadKey();
        }

        /// <summary>
        /// Populates the tree with random integers. The user specifies how many integers to add.
        /// </summary>
        private static void PopulateTree()
        {
            int numAttempts = 0,     // Number of insertion attempts.
                numSuccesses = 0,    // Number of successful entries.
                numValues = 0;       // Number of values to be added.
            Random r = new Random(); // Random integer generator.

            // If the tree hasn't been initialized, display an error message and exit.
            if (tree == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tTree must be initialized before adding values.");
                Console.WriteLine("\n\tPress any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            // Prompt for the number of values to add. Since the random generator creates numbers between 0 and 9999,
            // do not allow the user to add 10,000 integers as this will cause an infinite loop due to every possibility
            // already being present in the tree.
            Console.Write("\n\tEnter the number of values to add: ");
            if (Int32.TryParse(Console.ReadLine(), out numValues) && numValues > 0 && numValues < 10000)
            {
                // Add unique integer values into the tree, while keeping track of
                // successes and attempts.
                while (numSuccesses < numValues)
                {
                    numAttempts++;
                    if (tree.AddValue(r.Next(0, 9999)))
                        numSuccesses++;
                }

                // Insertion complete, display a message showing the number of successes and attempts, then exit.
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tTree successfully populated.");
                Console.WriteLine($"\n\t{numSuccesses} values added over {numAttempts} attempts.");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nPress any key to continue.");
                Console.ReadKey();
            }
            else
            {
                // Invalid user entry, display an error message and exit.
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tError: invalid entry. Please enter an integer between 1 and 9999.");
                Console.WriteLine("\n\tPress any key to return to the main menu.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the entirety of the B-tree to the console.
        /// This method displays nodes one by one to the console in a Pre-Order style traversal,
        /// with index nodes in red and leaf nodes in green.
        /// </summary>
        private static void DisplayTree()
        {
            Console.Clear();

            // If the tree has not been initialized, show an error message.
            if (tree == null)
                Console.WriteLine("\n\tError: tree has not been initialized.");
            else
            {
                // Call the tree's Traverse method to search the whole tree, then
                // draw the path to the console and display a statistics summary.
                tree.Traverse();
                DrawPath();
                Console.WriteLine($"--------------------\n{tree.Stats()}");
            }
            Console.WriteLine("\n\tPress any key to return to the main menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// Attempts to add a user-specified value to the B-tree.
        /// Displays the path from root to leaf that is searched in order to find where the
        /// value should go.
        /// </summary>
        private static void AddValue()
        {
            int value; // The value to add

            Console.Clear();

            // If the tree is null, show an error message and exit.
            if (tree == null)
            {
                Console.WriteLine("\n\tError: tree has not been initialized.");
                Console.WriteLine("\n\tPress any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            // Prompt for a value.
            Console.Write("\nPlease enter a value to be added: ");
            Console.ForegroundColor = ConsoleColor.Red;

            if (Int32.TryParse(Console.ReadLine(), out value))
            {
                // Attempt to add the value and display either a success or error message.
                // Either way, show the path traveled while searching for the correct leaf.
                if (tree.AddValue(value))
                {
                    Console.WriteLine("Path to the value:\n");
                    DrawPath();
                    Console.WriteLine($"\n{value} has been successfully added.");
                }
                else
                {
                    Console.WriteLine("Path searched: ");
                    DrawPath();
                    Console.WriteLine($"{value} is already contained in the tree. Insertion failed.");
                }
            }
            else
            {
                // Integer parse failed, display an error and return to the menu.
                Console.WriteLine("\n\tError: value entered is not a valid integer.");
            }

            Console.WriteLine("\n\tPress any key to return to the main menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// Searches for a specific value in the tree.
        /// Displays the path to the value, and displays a message showing the success
        /// or failure of the search.
        /// </summary>
        private static void FindValue()
        {
            int value; // The value to search for.

            // If the tree is null, show an error message and exit the method.
            if (tree == null)
            {
                Console.WriteLine("\n\tError: tree must be initialized first.");
                Console.WriteLine("\n\tPress any key to continue.");
                Console.ReadKey();
                return;
            }

            // Prompt for the value.
            Console.Clear();
            Console.Write("\n\tEnter a value to find: ");
            if (Int32.TryParse(Console.ReadLine(), out value))
            {
                Console.Clear();
                if (tree.FindValue(value))
                {
                    // Value was found, show the path and success message.
                    Console.WriteLine("Path to the value starting at root: ");
                    DrawPath();
                    Console.WriteLine($"{value} was found in the tree.");
                }
                else
                {
                    // Value was not found, show the path that was searched.
                    Console.WriteLine("Search path starting at root: ");
                    DrawPath();
                    Console.WriteLine($"{value} was not found in the tree.");
                }
            }
            else
            {
                // Invalid integer value was entered, display an error message.
                Console.WriteLine("\n\tPlease enter a valid integer number.");
            }
            Console.WriteLine("\n\tPress any key to return to the main menu.");
            Console.ReadKey();
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Draws the path taken by the last action on the tree.
        /// Displays the index nodes in red and leaf nodes in green.
        /// </summary>
        private static void DrawPath()
        {
            // Iterate over the node list returned by GetPath
            foreach (Node node in tree.GetPath())
            {
                // Set the color accordingly and display the node.
                Console.ForegroundColor = 
                    (node is Index) ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
                Console.WriteLine(node + "\n");
            }
            // Reset the text color to blue before exiting.
            Console.ForegroundColor = ConsoleColor.Blue;
        }
        #endregion
    }
}