using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


public class AStar
{
    //Open List Variable
    private List<Node> m_OpenList;
    //Closed List Variable
    private List<Node> m_ClosedList;
    //Grid List Variable - Will contain ALL Nodes
    private List<Node> m_Grid;
    //Rows In Grid
    private int m_Rows;
    //Columns In Grid
    private int m_Columns;
    //Reference to Start Node
    private Node m_StartNode;
    //Reference to Goal Node
    private Node m_GoalNode;
    //Reference to the Current Node
    private Node m_CurrentNode;
    //Reference to the Returned path
    private List<Node> m_ReturnedPath;

    //Property for accessing the CurrentNode outside of class
    public Node CurrentNode
    {
        get{ return m_CurrentNode;}

        set { m_CurrentNode = value; }
    }

    //Property for accessing the OpenList outside of class
    public List<Node> OpenList
    {
        get { return m_OpenList; }
    }

    //Property for accessing the ClosedList outside of class
    public List<Node> ClosedList
    {
        get { return m_ClosedList; }
    }

    //Property for accessing the Path outside of class
    public List<Node> Path
    {
        get { return m_ReturnedPath; }
    }

    //Constructor to initialize the A* Algorithm
    public AStar(List<Node> a_SearchSpace, Node a_Start, Node a_Goal, int a_Rows, int a_Columns)
    {//Set up the Open list
        m_OpenList = new List<Node>();
        //Set up the Closed list
        m_ClosedList = new List<Node>();
        //Set the Grid to the SearchSpace List of nodes passed in 
        m_Grid = a_SearchSpace;
        //Set The Start Node
        m_StartNode = a_Start;
        //Set The Goal Node
        m_GoalNode = a_Goal;
        //Set The Current Node
        m_CurrentNode = a_Start;
        //Set The Rows
        m_Rows = a_Rows;
        //Set The Columns
        m_Columns = a_Columns;
        //Setup Adjacents and HScores
        SetUp();
    }

    //Function for returning the path that was taken from start to goal
    private List<Node> GetPath(Node a_Node)
    {//Local Variable used to store the path that was taken from start to goal
        List<Node> path = new List<Node>();
        //Set the Current Node to the passed in node
        m_CurrentNode = a_Node;
        //While the current node is not equal to the start node
        while (m_CurrentNode != m_StartNode)
        {//Add the current nodes parent to the path list
            path.Add(m_CurrentNode.parent);
            //Set the current node to the current nodes parent
            m_CurrentNode = m_CurrentNode.parent;
        }
        //Return the path
        return path;
    }
    
    //Set the adjacents of the passed in node
    private void SetAdjacents(Node a_Node)
    {//If the adjacents count is not equal to 0
        if (a_Node.adjacents.Count >= 0)
        {//Integer that will be used as reference to bottom node from current passed in node
            int bot = a_Node.id + m_Rows;
            //Integer that will be used as reference to top node from current passed in node
            int top = a_Node.id - m_Columns;
            //Integer that will be used as reference to right node from current passed in node
            int right = a_Node.id + 1;
            //Integer that will be used as reference to left node from current passed in node
            int left = a_Node.id - 1;
            //Integer that will be used as reference to top right node from current passed in node
            int topRight = right - m_Rows;
            //Integer that will be used as reference to top left node from current passed in node
            int topLeft = top - 1;
            //Integer that will be used as reference to bottom right node from current passed in node
            int botRight = bot + 1;
            //Integer that will be used as reference to bottom left node from current passed in node
            int botLeft = bot - 1;
            //Create a List of integers which will be used to reference the index of surrounding nodes
            List<int> surroundingNodes = new List<int> {bot, top ,right, left, topRight, topLeft, botRight, botLeft};
 
            //Loop through the adjacents
            foreach (int number in surroundingNodes)
            {//Check if the number is less than or equal to the length of the grid minus 1 and that the number is greater than or equal to 0
                if (number <= m_Grid.Count - 1 && number >= 0)
                {//If the current node is traversable
                    if (m_Grid[number].traversable)
                    {
                     //Add the node to the adjacents list for this specific node
                        a_Node.adjacents.Add(m_Grid[number]);
                    }
                    
                }
            }

        }
    }

    //Function used to set the H Scores of the nodes
    private void SetHScores(Node a_Node)
    {//Create an XDifference variable to store the difference between the Goal Node XPos and Passed in Node XPos
        int XDifference = m_GoalNode.XPos - a_Node.XPos;
        //Create a YDifference variable to store the difference between the Goal Node YPos and Passed in Node YPos
        int YDifference = m_GoalNode.YPos - a_Node.YPos;
        //Create a Sum variable to store the sum of the XDifference and YDifference variables
        int Sum = XDifference + YDifference;
        //Set the H Score of the current node equal to the Sum variable multiplied by 10
        a_Node.hScore = (Sum * 10);

    }

    //Set up the Nodes with adjacents and H Scores
    private void SetUp()
    {//Loop through the grid
        foreach (Node node in m_Grid)
        {//Set the H Score for the node
            SetHScores(node);
            //Pass the node in to check for adjacents to that node
            SetAdjacents(node);
            
        }
    }

    //Function used to Sort the list by the F Score
    private List<Node> Sort(List<Node> a_NodeList)
    {//Return the new sorted list into a new variable
        List<Node> s = a_NodeList.OrderByDescending(n => n.fScore).ToList();
        return s;
    }

    //Function that will run the algorithm
    public bool Run()
    {//Add the start node to the open list
        m_OpenList.Add(m_StartNode);
        //While the open list is not empty
        while (m_OpenList.Count != 0)
        {//Sort the open list by f value and set the current node to the first index of the returned list
          List<Node> sort = Sort(m_OpenList);
            m_CurrentNode = sort[0];
            //If the goal node is in the open list
            if (m_OpenList.Contains(m_GoalNode))
            {//Set the path list with the returned list            
                m_ReturnedPath = GetPath(m_GoalNode);
                Console.Write(m_GoalNode.id);
                return false;
            }
            
            //Remove the current node from the open list
            m_OpenList.Remove(m_CurrentNode);
            //Add the current node to the closed list
            m_ClosedList.Add(m_CurrentNode);
            //Create an index variable that will be used to loop through adjacent nodes
            int index = 0;

           
            //Loop through the nodes in the current nodes adjacents list
            foreach (Node node in m_CurrentNode.adjacents)
            {//If the current adjacent node is traversable and not in the closed list
                if (node.traversable && !m_ClosedList.Contains(node))
                {//If the current adjacent node is not in the open list
                    if (!m_OpenList.Contains(node))
                    {//Add it to the open list
                       m_OpenList.Add(node);
                       //Set the parent to the current node
                        node.parent = m_CurrentNode;
                        //The first 4 nodes in the adjacents list will be either left,right,top or bottom. They will all have 10 as a g score
                        //The rest of the nodes are diagonals and there g score is 14
                        //Set the G Score of node
                        node.gScore = index < 4 ? 10 : 14;
                    }
                    else
                    {//Find out how much it will be to move to each of the nodes in the adjacent list
                     //If the current index is less than 4 then move will be set to 10 else it will be set to 14
                        int move = index < 4 ? 10 : 14;
                     //Add the move and current units g score to get the total move cost
                        int cost = move + m_CurrentNode.gScore;
                        //If the total move cost is less than the current adjacents g score
                        if (cost < node.gScore)
                        {//Set the adjacents parent to the current node
                            node.parent = m_CurrentNode;
                         //Set the adjacent nodes G Score to the total move cost
                            node.gScore = cost;
                        }
                    }
                }
                //Increment by one so we can loop through the adjacent nodes 
                index += 1;
            }
        }
        //Return true to continue looping
        return true;
    }
}
