using UnityEngine;

public class Pathfinding
{

    private GameObject m_Following;

    private NavMeshAgent NavMesh;


    public Pathfinding(GameObject a_Following, NavMeshAgent a_Agent)
    {
        m_Following = a_Following;
        NavMesh = a_Agent;
    }

    public void Search()
    {
        NavMesh.SetDestination(m_Following.transform.position);
    }
}
