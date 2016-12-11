//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project    :		Project 5 - B-Tree
//	File Name  :		INSERT.cs
//	Description:		Enumerated type, desribing the resultant status of an insertion operation on the B-tree.
//	Course     :		CSCI 2210-201 - Data Structures
//	Author     :		Casey Edwards, zcee10@etsu.edu
//	Created    :		Tuesday, November 15, 2016
//	Copyright  :		Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace BTreeProject
{
    /// <summary>
    /// Enumerated type representing the possible outcomes of an insertion operation on the B-tree.
    /// The operation may find a duplicate entry, in which case it will fail to insert.
    /// It may be successful, but the successful insertion may create a need for the 
    /// node to be split.
    /// </summary>
    enum INSERT
    {
        DUPLICATE, // Duplicate entry found.
        SUCCESS,   // Insertion was successful.
        NEEDSPLIT  // Insertion successful, but the node needs to be split.
    }
}