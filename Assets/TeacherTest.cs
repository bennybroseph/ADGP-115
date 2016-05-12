using UnityEngine;
using System.Collections;
using UnityEditor;

public class TeacherTest : MonoBehaviour
{
    public GameObject dest;
	// Use this for initialization
	void Start ()
	{
	    GetComponent<NavMeshAgent>().SetDestination(dest.transform.position);

	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<NavMeshAgent>().SetDestination(dest.transform.position);
    }
}
