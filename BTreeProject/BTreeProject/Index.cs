//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		Index.cs
//	Description:		Index node type for the B-tree class. Subclasses from Node, with added functionality
//                          to accomodate acting as an index to other nodes.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Tuesday, November 15, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace BTreeProject
{
    /// <summary>
    /// Index node for use in the B-tree class.
    /// Subclasses the Node class, and adds an Insert method that accepts both an integer
    /// and a node to be added, as well as a list of Nodes acting as the index and a method
    /// to sort both the values list and the node list.
    /// </summary>
    /// <seealso cref="BTreeProject.Node" />
    class Index : Node
    {
        #region Properties
        public List<Node> Indexes { get; set; } // Index of nodes.
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterized constructor.
        /// Sets the NodeSize and Indexes list to the given argument size, but 
        /// makes the Values list one less than the given argument.
        /// </summary>
        /// <param name="nodeSize">Size of the node.</param>
        public Index(int nodeSize) : base(nodeSize)
        {
            Indexes = new List<Node>(nodeSize);
        }
        #endregion

        #region Insert and Sort methods        
        /// <summary>
        /// Inserts the given value and node into this index, and sorts the
        /// list of values and nodes after insertion. Returns an operation status code enum.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="node">The node to add.</param>
        /// <returns>An INSERT enum representing the success level of the insert:
        /// NEEDSPLIT: Successful, but is now overflowing.
        /// SUCCESS: Successful.
        /// DUPLICATE: A duplicate value was found, insertion failed.</returns>
        public INSERT Insert(int value, Node node)
        {
            INSERT statusCode; // Operation status code.

            if (Values.Contains(value))
            {
                // Duplicate value found, insertion fails.
                statusCode = INSERT.DUPLICATE;
            }
            else if (Indexes.Count == NodeSize)
            {
                // Requires an index split.
                statusCode = INSERT.NEEDSPLIT;
                Values.Add(value);
                Indexes.Add(node);
                Sort();
            }
            else
            {
                // Insertion is successful.
                Values.Add(value);
                Indexes.Add(node);
                Sort();
                statusCode = INSERT.SUCCESS;
            }

            return statusCode;
        }

        /// <summary>
        /// Sorts the list of values and indices.
        /// </summary>
        public void Sort()
        {
            // List supports integer sorting, the Indexes list requires the Node class
            // to implement IComparable to be sorted correctly. Sorting these lists in
            // tandem should keep value-node association intact.
            Values.Sort();
            Indexes.Sort();
        }
        #endregion
    }
}