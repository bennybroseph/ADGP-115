using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Library;
using Units;
using Units.Controller;
using Event = Define.Event;

public class AIController : MonoSingleton<AIController>, IController
{
    [SerializeField]
    private List<Vector3> m_SpawnPoints;
    [SerializeField]
    private GameObject m_GoblinMagePrefab;
    [SerializeField]
    private GameObject m_GoblinPrefab;
    [SerializeField]
    private List<IStats> m_Enemies;
    [SerializeField]
    private int m_WaveCounter;
    private List<IControllable> m_Controlables; 
    [SerializeField]
    private Vector3 m_Variance;

    protected override void Awake()
    {
        base.Awake();

        m_Controlables = new List<IControllable>();
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

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Publisher.self.UnSubscribe(Event.SpawnWaveClicked, SpawnWaves);
        Publisher.self.UnSubscribe(Event.UnitDied, OnUnitDied);
    }

    private void Search()
    {
        foreach (IControllable controlable in m_Controlables)
        {
            if (controlable.controllerType == ControllerType.Goblin && controlable.following == null)           
                controlable.following = GameObject.FindGameObjectWithTag("Player");

            if (controlable.controllerType == ControllerType.GoblinMage && controlable.following == null)
                controlable.following = GameObject.FindGameObjectWithTag("Fortress");

            if (controlable.following != null)
            {
                IUsesSkills skillUser = controlable as IUsesSkills;

                controlable.navMashAgent.SetDestination(controlable.following.transform.position);

                float distanceFromEnemyToTarget = Vector3.Distance(controlable.following.transform.position, controlable.transform.position);

                if (distanceFromEnemyToTarget < 7)
                {
                    Publisher.self.Broadcast(Event.UseSkill, skillUser, 0);
                }

                
            }

        }
    }

    public void Register(IControllable a_Controllable)
    {
        switch (a_Controllable.controllerType)
        {
            case ControllerType.GoblinMage:
                a_Controllable.following = GameObject.FindGameObjectWithTag("Fortress");
                m_Controlables.Add(a_Controllable);
                break;

            case ControllerType.Goblin:
                a_Controllable.following = GameObject.FindGameObjectWithTag("Player");
                m_Controlables.Add(a_Controllable);
                break;
        }
    }

    public void UnRegister(IControllable a_Controllable)
    {
        m_Controlables.Remove(a_Controllable);
    }

    public void SpawnWaves(Event a_Event, params object[] a_Params)
    {
        if (m_Enemies.Count != 0)
            return;

        m_WaveCounter++;

        if (m_SpawnPoints.Count == 0)
        {
            Vector3 spawnPoint1 = new Vector3(-14.25f, 0.5f, 0f);
            Vector3 spawnPoint2 = new Vector3(-0.50f, 0.5f, 7.50f);
            Vector3 spawnPoint3 = new Vector3(0f, 0.5f, -8.5f);

            m_SpawnPoints.Add(spawnPoint1);
            m_SpawnPoints.Add(spawnPoint2);
            m_SpawnPoints.Add(spawnPoint3);
        }



        foreach (Vector3 spawnPoint in m_SpawnPoints)
        {
            for (int i = 0; i < m_WaveCounter; i++)
            {
                float x = Random.Range(-m_Variance.x, m_Variance.x);
                float z = Random.Range(-m_Variance.z, m_Variance.z);

                GameObject goblin = Instantiate(m_GoblinPrefab);
                goblin.transform.position = new Vector3(
                    spawnPoint.x + x,
                    spawnPoint.y,
                    spawnPoint.z + z);

                GameObject goblinMage = Instantiate(m_GoblinMagePrefab);
                goblinMage.transform.position = new Vector3(
                    spawnPoint.x + x,
                    spawnPoint.y,
                    spawnPoint.z + z);

                m_Enemies.Add(goblinMage.GetComponent<IStats>());
                m_Enemies.Add(goblin.GetComponent<IStats>());
            }


        }
        Publisher.self.Broadcast(Event.SpawnWave, m_WaveCounter);

    }

    private void OnUnitDied(Event a_Event, params object[] a_Params)
    {
        IStats unit = a_Params[0] as IStats;

        if (unit == null)
            return;

        m_Enemies.Remove(unit);
    }

}
