//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		BTree.cs
//	Description:		B-tree object that uses Index and Leaf Nodes to store integer values. Also keeps track of
//                          various statistical parameters such as depth, leaf count, etc.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Tuesday, November 15, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace BTreeProject
{
    /// <summary>
    /// Class representing a B-tree of any -arity.
    /// Provides methods for adding values, searching for values, traversing the
    /// entire tree, and retrieving statistical data and information on the contents.
    /// </summary>
    class BTree
    {
        #region Properties
        private int Count { get; set; }          // Total number of nodes.
        private int Depth { get; set; }          // Depth of the tree.
        private int IndexCount { get; set; }     // Number of index nodes.
        private int LeafCount { get; set; }      // Number of leaf nodes.
        private int NodeSize { get; set; }       // Size of the nodes.
        private Node Root { get; set; }          // The root node.
        private Stack<Node> stack { get; set; }  // Stack for tree traversal.
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// Initializes a new instance of the <see cref="BTree"/> class.
        /// Must be given a node size. Sets the root to a new empty leaf node,
        /// creates a new node stack, and sets the counts accordingly.
        /// </summary>
        /// <param name="nodeSize">Size of the nodes.</param>
        public BTree(int nodeSize)
        {
            NodeSize = nodeSize;
            Root = new Leaf(NodeSize);
            IndexCount = 0;
            LeafCount = 1;
            Count = 1;
            Depth = 0;
            stack = new Stack<Node>();
        }
        #endregion

        #region Add/Search Methods
        /// <summary>
        /// Adds a value to the tree.
        /// If adding a value causes a node overflow, it begins a node split
        /// operation that will propogate upward as far as needed to balance
        /// the tree. The stack will reflect the path taken to insert the node
        /// into the tree. In the case of split nodes, it will reflect the new
        /// path to the value.
        /// </summary>
        /// <param name="val">The value to be added.</param>
        /// <returns>True for successful entry, False if a duplicate is found.</returns>
        public bool AddValue(int val)
        {
            bool success = false; // Success or failure of entering the value.

            // First, check for a duplicate value.
            if (!FindValue(val))
            {
                // Found the leaf that should contain the value and it wasn't present,
                // so add the value to that leaf (top of the stack).
                INSERT insertionStatus = ((Leaf)stack.Peek()).Insert(val);
                if (insertionStatus == INSERT.NEEDSPLIT)
                {
                    // Leaf needs splitting, trace up the stack and split nodes accordingly.
                    SplitLeaf(val);
                }
                else if (insertionStatus == INSERT.DUPLICATE)
                {
                    // FindValue returned false when it should have returned true.
                    throw new ArgumentException("BTree.FindValue() failed to perform as expected.");
                }

                // Successfully added the value
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Finds a value in the tree.
        /// Beginning at the root, it probes the indices and takes the best path
        /// to search for the given value. Once a leaf node is reached, it will 
        /// return true or false depending on the existance of the sought after value.
        /// After searching, the stack will reflect the path taken to find the value.
        /// </summary>
        /// <param name="val">The value to search for.</param>
        /// <returns>True if found, otherwise False.</returns>
        public bool FindValue(int val)
        {
            bool success = false;    // Success of the operation.
            Node currentNode = Root; // The current node.
            stack.Clear();           // Clear out the stack for a new traversal.

            // Loop until a leaf is found.
            while (currentNode is Index)
            {
                // Track the traversal.
                stack.Push(currentNode);

                // Search for an index greater than the value, then descend into
                // the node one index behind it.
                for (int i = 0; i < currentNode.Values.Count; i++)
                {
                    if (currentNode.Values[i] > val)
                    {
                        // Found a higher value, take the corresponding index.
                        currentNode = ((Index)currentNode).Indexes[i];
                        break;
                    }

                    if (i == currentNode.Values.Count - 1)
                    {
                        // Made it to the end of the values list without finding a greater
                        // value; take the last index.
                        currentNode = ((Index)currentNode).Indexes[i + 1];
                        break;
                    }
                }
            }

            // A candidate leaf was found, push it onto the stack and determine if the value is present.
            stack.Push(currentNode);
            if (currentNode.Values.Contains(val))
                success = true;

            return success;
        }
        #endregion

        #region Node Splitting Methods
        /// <summary>
        /// Splits a leaf node.
        /// This method will create a new leaf to take the upper half of the
        /// values from an overflowing leaf node, then adds the new leaf to
        /// the parent index node. If this action creates a requirement for the
        /// parent index node to be split, it kickstarts the SplitIndex recursive
        /// function to balance the nodes.
        /// Care is taken to ensure the stack path is updated to reflect the true
        /// path to the value whose addition caused the overflow.
        /// </summary>
        /// <param name="value">The value added that caused the overflow.</param>
        private void SplitLeaf(int value)
        {
            Leaf newLeaf = new Leaf(NodeSize); // The new leaf node.
            Leaf oldLeaf = (Leaf)stack.Pop();  // The leaf that needs a split.

            // Copy half the values from the old leaf to the new.
            for (int i = oldLeaf.Values.Count / 2; i < oldLeaf.Values.Count; i++)
                newLeaf.Values.Add(oldLeaf.Values[i]);

            // Truncate the old leaf's values list.
            foreach (int v in newLeaf.Values)
                oldLeaf.Values.Remove(v);

            // Update the parent index, splitting it if necessary.
            // If there is no parent index, then the split leaf is Root; create an index.
            if (stack.Count == 0)
            {
                // Create a Root index, add the leaf nodes, and increment the counters.
                Root = new Index(NodeSize);
                ((Index)Root).Indexes.Add(oldLeaf); // The minimum index.
                ((Index)Root).Insert(newLeaf.Values[0], newLeaf);
                stack.Push(Root);
                Depth++;
                IndexCount++;
                Count++;
            }
            else
            {
                // Parent index has the oldLeaf, just add the new.
                // Check for a need to split the parent index.
                INSERT statusCode = ((Index)stack.Peek()).Insert(newLeaf.Values[0], newLeaf);

                if (statusCode == INSERT.NEEDSPLIT)
                    SplitIndex((Index)stack.Pop(), value);
                else if (statusCode == INSERT.DUPLICATE)
                {
                    // Something unexpected has gone wrong.
                    throw new ArgumentException("Something unexpected happened, consult the debugger.");
                }
            }

            // Put the correct leaf back on the stack.
            if (oldLeaf.Values.Contains(value))
                stack.Push(oldLeaf);
            else
                stack.Push(newLeaf);

            // Update the leaf count and total count.
            LeafCount++;
            Count++;
        }

        /// <summary>
        /// Splits an index node.
        /// This method recursively checks the given index node and its parents up to the
        /// root node, and if required, splits them and updates -their- parent index nodes.
        /// The newly created index takes the upper half of values and indices from the
        /// overflowing node.
        /// Care is taken to ensure the stack reflects the path to the leaf node holding
        /// the added value that caused the split requirements.
        /// </summary>
        /// <param name="oldIndex">The overflowing index.</param>
        /// <param name="newVal">The value whose addition created a need for splitting nodes.</param>
        private void SplitIndex(Index oldIndex, int newVal)
        {
            Index newIndex = new Index(NodeSize); // The new index.

            // Copy half the indices from the old index to the new.
            for (int i = oldIndex.Indexes.Count / 2; i < oldIndex.Indexes.Count; i++)
                newIndex.Indexes.Add(oldIndex.Indexes[i]);

            // Truncate the old index's index list.
            foreach (Node n in newIndex.Indexes)
                oldIndex.Indexes.Remove(n);

            // Update the Values list of each index to reflect their new indices.
            oldIndex.Values.Clear();
            newIndex.Values.Clear();
            for (int i = 1; i < oldIndex.Indexes.Count; i++)
                oldIndex.Values.Add(FindMinValue(oldIndex.Indexes[i]));
            for (int i = 1; i < newIndex.Indexes.Count; i++)
                newIndex.Values.Add(FindMinValue(newIndex.Indexes[i]));

            // Check for and add new index to the parent node.
            if (stack.Count == 0)
            {
                // Root has been split, create a new Root and add these indices to it.
                Root = new Index(NodeSize);
                ((Index)Root).Indexes.Add(oldIndex);
                ((Index)Root).Insert(FindMinValue(newIndex), newIndex);
                stack.Push(Root);
                Depth++;
                IndexCount++;
                Count++;
            }
            else
            {
                // Parent already has old index, just add the new one and check for insertion status.
                INSERT statusCode = ((Index)stack.Peek()).Insert(FindMinValue(newIndex), newIndex);
                if (statusCode == INSERT.NEEDSPLIT)
                    SplitIndex((Index)stack.Pop(), newVal); // Parent needs splitting, make a recursive call.
                else if (statusCode == INSERT.DUPLICATE)
                {
                    // This should never occur, something unexpected has gone wrong.
                    throw new ArgumentException("Duplicate index discovered, consult the debugger.");
                }
            }

            // Check to see which index is along the path to the new value that caused the splitting.
            if (newVal < FindMinValue(newIndex))
                stack.Push(oldIndex);
            else
                stack.Push(newIndex);

            // Update index count and total count.
            IndexCount++;
            Count++;
        }

        /// <summary>
        /// Finds the minimum value contained within the subtree starting at the given node.
        /// This method is important for ensuring correct index splitting, since the lowest
        /// value of an index's subtree is only contained in its leftmost leaf.
        /// </summary>
        /// <param name="node">The root of the subtree to search.</param>
        /// <returns>Minimum contained value.</returns>
        private int FindMinValue(Node node)
        {
            // Descend into the lowest index until a leaf is found.
            while (node is Index)
                node = ((Index)node).Indexes[0];

            // Return the leaf's lowest value.
            return node.Values[0];
        }
        #endregion

        #region Traversal methods
        /// <summary>
        /// Traverses the tree.
        /// This code is a public boilerplate code that clears the stack and
        /// kickstarts the recursive PreOrderTraverse method.
        /// </summary>
        public void Traverse()
        {
            // Clear the stack and start the recursive search.
            stack.Clear();
            PreOrderTraverse(Root);
        }

        /// <summary>
        /// Traverses the tree.
        /// Recursively probes every index of the index nodes, pushing them onto
        /// the stack in a pre-order traversal style.
        /// </summary>
        /// <param name="n">The node to probe.</param>
        private void PreOrderTraverse(Node n)
        {
            // Push the current node onto the stack.
            stack.Push(n);

            // If the node is an Index node, make a recursive call on each of its children.
            if (n is Index)
                foreach (Node node in ((Index)n).Indexes)
                    PreOrderTraverse(node);
        }
        #endregion

        #region Tree Information
        /// <summary>
        /// Retrieves a map of the most recent path taken by an operation 
        /// of the tree (added value, searching, traversal).
        /// </summary>
        /// <returns>A List of nodes representing the path from root to leaf of the
        /// travelled path (or the entire tree on a Traversal).</returns>
        public List<Node> GetPath()
        {
            List<Node> nodeList = new List<Node>(stack.Count); // The tree traversal path.

            // Pop each item off of the stack into the list.
            while (stack.Count != 0)
                nodeList.Add(stack.Pop());

            // The list is in reverse order, flip the list.
            nodeList.Reverse();

            return nodeList;
        }

        /// <summary>
        /// Generates and provides statistical information about this tree.
        /// Gives the number of index and leaf nodes, the average percentage full
        /// of the leaf nodes, the depth of the tree, number of index and leaf levels,
        /// and total number of values contained in the leaves.
        /// </summary>
        /// <returns>A string with the statistical representation of the tree.</returns>
        public string Stats()
        {
            String stats = String.Empty;                     // The statistics to be returned.
            int numValues = 0;                               // Total number of values stored.
            List<double> percentFilled = new List<double>(); // Tracks the average percentage filled.

            // Traverse the tree to get all nodes into the stack.
            Traverse();

            // Pop all nodes off the stack, taking the percentages and number of values from each leaf node.
            // Index nodes do not contain pertinent data and may be discarded.
            while (stack.Count != 0)
            {
                if (stack.Peek() is Index)
                    stack.Pop();
                else
                {
                    Leaf leaf = (Leaf)stack.Pop();
                    numValues += leaf.Values.Count;
                    percentFilled.Add(leaf.Values.Count / (double)NodeSize);
                }
            }

            // Build the stats string.
            stats += $"Number of total nodes is {Count}.\n";
            stats += $"Number of index nodes is {IndexCount}.\n";
            stats += $"Number of leaf nodes is {LeafCount} and they average ";
            stats += $"{(percentFilled.Sum() / LeafCount) * 100:#0.0#}% full.\n";
            stats += $"The depth of the tree is {Depth} with\n";
            stats += $"\t{Depth} levels of index nodes and 1 level of leaf nodes.\n\n";
            stats += $"The total number of values in the tree is {numValues}.";

            return stats;
        }
        #endregion
    }
}