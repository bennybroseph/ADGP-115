using System.Collections;
using System.Collections.Generic;

public class Node
{   //List created for adjacent nodes
    private List<Node> m_AdjacentNodes;
    //Parent Node
    private Node m_Parent;
    //Determine whether node is traversable or not
    private bool m_Traversable;
    //Index of Node
    private int m_Id;
    //F Score For Node
    private int m_FScore;
    //G Score For Node
    private int m_GScore;
    //H Score For Node
    private int m_HScore;
    //X Pos
    private int m_XPos;
    //Y Pos
    private int m_YPos;

    //Property for accessing Adjacent Nodes List outside of class
    public List<Node> adjacents
    {
        get { return m_AdjacentNodes; }
        set { m_AdjacentNodes = value; }
    }

    //Property For accessing Parent Node outside of class
    public Node parent
    {
        get { return m_Parent; }
        set { m_Parent = value; }
    }

    //Property for accessing traversable variable outside of class
    public bool traversable
    {
        get { return m_Traversable;}
        set { m_Traversable = value; }
    }

    //Property for accessing the id outside of class
    public int id
    {
        get { return m_Id;}
    }

    //Property for accessing the GScore outside of class
    public int gScore
    {
        get { return m_GScore; }
        set
        {
            m_GScore = value;
            m_FScore = m_GScore + m_HScore;
        }
    }

    //Property for accessing the HScore outside of class
    public int hScore
    {
        get { return m_HScore; }
        set
        {
            m_HScore = value;
            m_FScore = m_GScore + m_HScore;
        }
    }

    //Property for accessing the FScore outside of class
    public int fScore
    {
        get { return m_FScore;}
    }

    //Property for accessing the XPos outside of class
    public int XPos
    {
        get { return m_XPos; }

    }

    //Property for accessing the YPos outside of class
    public int YPos
    {
        get { return m_YPos; }

    }

    //Constructor to construct Node object
    public Node(int a_X, int a_Y, int a_Id)
    {//Create an empty list for node to have
        m_AdjacentNodes = new List<Node>();
        //Set its parent node to null
        m_Parent = null;
        //Set each node to traversable
        m_Traversable = true;
        //Set X Pos
        m_XPos = a_X;
        //Set Y Pos
        m_YPos = a_Y;
        //Give node the Id that is passed in
        m_Id = a_Id;
        //Init FScore to 0
        m_FScore = 0;
        //Init GScore to 0
        m_GScore = 0;
        //Init HScore to 0
        m_HScore = 0;
    }
}
