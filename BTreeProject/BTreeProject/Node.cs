//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		Node.cs
//	Description:		Node parent class for the Index and Leaf nodes of the B-tree.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Tuesday, November 15, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace BTreeProject
{
    /// <summary>
    /// Represents a node on the B-tree.
    /// Should only be used as a base class for the Index and Leaf node types.
    /// Describes the node size and list of node values.  Also contains a
    /// ToString representation of the node as well as constructors.
    /// </summary>
    class Node : IComparable
    {
        #region Properties
        public int NodeSize { get; set; }     // Size of the node.
        public List<int> Values { get; set; } // List of values.
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterized constructor.
        /// Accepts an int representing the node size as a parameter, and creates
        /// the list of Values according to that size.
        /// </summary>
        /// <param name="nodeSize">Size of the node.</param>
        public Node(int nodeSize)
        {
            NodeSize = nodeSize;
            Values = new List<int>(NodeSize);
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// Displays the node type, number of values, percent full, and a list of the values.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string representation = string.Empty; // Representation of the node.

            // Display the node type (would be either Leaf or Index)
            representation += $"Node Type: {this.GetType().Name}\n";

            // Display the node's fill percentage and number of values.
            // Use Values.Count + 1 if this is an index node.
            if (this.GetType().Name == typeof(Index).Name)
            {
                representation += $"Number of values: {Values.Count + 1}\n";
                representation += $"Node is {((Values.Count + 1) / (double)NodeSize) * 100:#0.0}% full.\n";
            }
            else
            {
                representation += $"Number of values: {Values.Count}\n";
                representation += $"Node is {(Values.Count / (double)NodeSize) * 100:#0.0}% full.\n";
            }
            // Display the contained values. If this node is an index, display ** as the first value.
            representation += "Values:\n";
            if (this.GetType().Name == typeof(Index).Name)
                representation += "** ";
            foreach (int val in Values)
                representation += $"{val} ";

            return representation;
        }

        /// <summary>
        /// Implementation of the IComparable interface.
        /// Allows sorting of Node type via the leading entry in the Values list.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>Comparison integer.</returns>
        public int CompareTo(object obj)
        {
            int compareValue; // Comparison integer.

            // If the object is not a node, throw an exception.
            if (!(obj is Node))
                throw new ArgumentException("Node must be compared to node.");
            else
            {
                // Cast the parameter as a Node.
                Node other = obj as Node;

                if (this.Values.Count == 0)
                {
                    // This node is empty. If the passed node is empty, return zero.
                    // Otherwise, return -1 since empty comes before anything.
                    if (other.Values.Count == 0)
                        compareValue = 0;
                    else
                        compareValue = -1;
                }
                else
                {
                    // This node is not empty. If the other node is empty, return 1,
                    // otherwise return the comparison of the first value in each node.
                    if (other.Values.Count == 0)
                        compareValue = 1;
                    else
                        compareValue = this.Values[0].CompareTo(other.Values[0]);
                }
            }

            return compareValue;
        }
        #endregion
    }
}