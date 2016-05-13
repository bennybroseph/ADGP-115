using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unit;

public class SpawnWaves : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> m_SpawnPoints;
    [SerializeField]
    private Enemy m_Enemy;

    // Use this for initialization
    void Start()
    {
        
        Vector3 spawnPoint1 = new Vector3(-14.25f,0f,0f);
        Vector3 spawnPoint2 = new Vector3(-0.50f, 0f, 7.50f);
        Vector3 spawnPoint3 = new Vector3(-0f, 0f, -8.5f);

        m_SpawnPoints.Add(spawnPoint1);
        m_SpawnPoints.Add(spawnPoint2);
        m_SpawnPoints.Add(spawnPoint3);

        foreach (Vector3 spawnPoint in m_SpawnPoints)
        {
           Instantiate( m_Enemy).transform.position = spawnPoint;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
