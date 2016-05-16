using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Library;
using Units;
using Event = Define.Event;
using System;

public class AIController : MonoSingleton<AIController>, IController
{
    [SerializeField]
    private List<Vector3> m_SpawnPoints;
    [SerializeField]
    private GameObject m_EnemyPrefab;
    [SerializeField]
    private List<IStats> m_Enemies;
    [SerializeField]
    private int m_WaveCounter;
    private List<IControlable> m_Controlables;


    private void Awake()
    {
        m_Controlables = new List<IControlable>();
        m_Enemies = new List<IStats>();

        Publisher.self.Subscribe(Event.SpawnWaveClicked, SpawnWaves);
        Publisher.self.Subscribe(Event.UnitDied, OnUnitDied);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Search();

    }



    private void Search()
    {
        foreach (IControlable controlable in m_Controlables)
        {
            controlable.navMashAgent.SetDestination(controlable.following.transform.position);
        }
    }

    public void Register(IControlable a_Controlable)
    {
        switch (a_Controlable.controllerType)
        {
            case ControllerType.Enemy:
                a_Controlable.following = GameObject.FindGameObjectWithTag("Fortress");
                m_Controlables.Add(a_Controlable);
                break;
        }
    }

    public void UnRegister(IControlable a_Controlable)
    {
        m_Controlables.Remove(a_Controlable);
    }

    public void SpawnWaves(Event a_Event, params object[] a_Params)
    {
        if (m_Enemies.Count != 0)
            return;

        Vector3 spawnPoint1 = new Vector3(-14.25f, 0.5f, 0f);
        Vector3 spawnPoint2 = new Vector3(-0.50f, 0.5f, 7.50f);
        Vector3 spawnPoint3 = new Vector3(-0f, 0.5f, -8.5f);

        m_SpawnPoints.Add(spawnPoint1);
        m_SpawnPoints.Add(spawnPoint2);
        m_SpawnPoints.Add(spawnPoint3);

        foreach (Vector3 spawnPoint in m_SpawnPoints)
        {
            GameObject newObject = Instantiate(m_EnemyPrefab);
            newObject.transform.position = spawnPoint;

            m_Enemies.Add(newObject.GetComponent<IStats>());
        }
        m_WaveCounter++;
        Publisher.self.Broadcast(Event.SpawnWave,m_WaveCounter);
    }

    private void OnUnitDied(Event a_Event, params object[] a_Params)
    {
        IStats unit = a_Params[0] as IStats;

        if (unit == null)
            return;

        m_Enemies.Remove(unit);
    }
}
