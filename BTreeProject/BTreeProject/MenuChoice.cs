//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		MenuChoice.cs
//	Description:		Enumerated type representing the possible main menu choices.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Friday, November 18, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace BTreeProject
{
    /// <summary>
    /// Enumerates the possible menu choices for use in the driver program.
    /// </summary>
    enum MenuChoice
    {
        CREATE_TREE = 1, // Set node size and create the tree.
        POPULATE,        // Populate the tree with values.
        DISPLAY,         // Display the B-tree.
        ADD_VALUE,       // Add a value to the tree.
        FIND_VALUE,      // Search for a value in the tree.
        EXIT             // Exit the program.
    }
}