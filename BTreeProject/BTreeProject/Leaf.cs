//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		Leaf.cs
//	Description:		Leaf node for use in the B-tree. Provides an additional Insert method that handles
//                          new value insertion in addition to the base Node functionality.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Tuesday, November 15, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace BTreeProject
{
    /// <summary>
    /// Leaf node type for the B-tree class.
    /// Contains an Insert method that allows for insertion into the node,
    /// as well as constructor boilerplate that calls the parent constructors.
    /// </summary>
    /// <seealso cref="BTreeProject.Node" />
    class Leaf : Node
    {
        #region Constructors
        /// <summary>
        /// Parameterized constructor.
        /// Calls the parent's parameterized constructor with the given argument,
        /// setting NodeSize to the given int.
        /// </summary>
        /// <param name="NodeSize">Size of the node.</param>
        public Leaf(int NodeSize) : base(NodeSize)
        {
            // Do nothing extra.
        }
        #endregion

        #region Insert method
        /// <summary>
        /// Insertion method.
        /// Allows for the insertion of values into the leaf node if the given value
        /// is not a duplicate.
        /// </summary>
        /// <param name="val">The value to be inserted.</param>
        /// <returns>An INSERT enum describing the operation success levels:
        /// SUCCESS: successful.
        /// NEEDSPLIT: successful, but overflowing.
        /// DUPLICATE: duplicate entry found, insertion failed.</returns>
        public INSERT Insert(int val)
        {
            INSERT statusCode; // The operation status code.

            if (Values.Contains(val)) // Duplicate found.
                statusCode = INSERT.DUPLICATE;
            else if (Values.Count == NodeSize)
            {
                // No duplicate, but node is full.
                statusCode = INSERT.NEEDSPLIT;
                Values.Add(val);
                Values.Sort();
            }
            else
            {
                // Node can accept the value.
                statusCode = INSERT.SUCCESS;
                Values.Add(val);
                Values.Sort();
            }

            return statusCode;
        }
        #endregion
    }
}