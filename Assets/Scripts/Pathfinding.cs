using UnityEngine;
using System.Collections;
using UnityEditor;

public class Pathfinding : MonoBehaviour
{
    public Transform player1;
    private NavMeshAgent NavMesh;


	// Use this for initialization
	void Start ()
	{
	    player1 = GameObject.FindGameObjectWithTag("Player").transform;
	    NavMesh = GetComponent<NavMeshAgent>();

	}
	
	// Update is called once per frame
	void Update ()
    {
	   Search();
	}

    private void Search()
    {
       NavMesh.SetDestination(player1.position);

        

    }
}
