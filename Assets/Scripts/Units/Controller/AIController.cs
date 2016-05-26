using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Library;
using UnityEngine;
using Event = Define.Event;
using Random = UnityEngine.Random;

namespace Units.Controller
{
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
        [SerializeField]
        private GameObject m_ManaPickupPrefab;
        [SerializeField]
        private GameObject m_HealthPickupPrefab;

        private bool m_ApplicationIsQuitting;

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

        private void OnApplicationQuit()
        {
            m_ApplicationIsQuitting = true;
        }

        private void Search()
        {
            foreach (IControllable controlable in m_Controlables)
            {
                if (controlable.following == null)
                    SetFollowing(controlable);

                if (controlable.following != null)
                {
                    IUsesSkills skillUser = controlable as IUsesSkills;

                    controlable.navMashAgent.SetDestination(controlable.following.transform.position);

                    float distanceFromEnemyToTarget = Vector3.Distance(controlable.following.transform.position, controlable.transform.position);

                    if (distanceFromEnemyToTarget < 7 && controlable.controllerType == ControllerType.GoblinMage)
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
                    SetFollowing(a_Controllable);
                    m_Controlables.Add(a_Controllable);
                    break;

                case ControllerType.Goblin:
                    SetFollowing(a_Controllable);
                    m_Controlables.Add(a_Controllable);
                    break;
            }
        }

        public void UnRegister(IControllable a_Controllable)
        {
            m_Controlables.Remove(a_Controllable);
        }

        private void SetFollowing(IControllable a_Controllable)
        {
            if (a_Controllable.controllerType == ControllerType.Goblin)
            {
                List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
                if (players.Count == 0)
                    return;

                players.Sort(
                    delegate (GameObject a, GameObject b)
                    {
                        float distanceA = Vector3.Distance(a.transform.position, a_Controllable.transform.position);
                        float distanceB = Vector3.Distance(b.transform.position, a_Controllable.transform.position);
                        
                        if (distanceA > distanceB)
                            return 1;
                        if (distanceA < distanceB)
                            return -1;

                        return 0;
                    });
                a_Controllable.following = players[0];
            }

            if (a_Controllable.controllerType == ControllerType.GoblinMage)
            {
                List<GameObject> fortresses = GameObject.FindGameObjectsWithTag("Fortress").ToList();
                fortresses.Sort(
                    delegate (GameObject a, GameObject b)
                    {
                        float distanceA = Vector3.Distance(a.transform.position, a_Controllable.transform.position);
                        Debug.Log(distanceA);

                        float distanceB = Vector3.Distance(b.transform.position, a_Controllable.transform.position);
                        Debug.Log(distanceB);
                        if (distanceA > distanceB)
                            return 1;
                        if (distanceA < distanceB)
                            return -1;

                        return 0;
                    });
                a_Controllable.following = fortresses[0];
            }
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

            StartCoroutine(Spawn());

            Publisher.self.Broadcast(Event.SpawnWave, m_WaveCounter);

        }

        private void OnUnitDied(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null || m_ApplicationIsQuitting)
                return;

            if (unit.gameObject.tag == "Enemy")
            {
                Vector3 healthinstantposition = new Vector3(unit.gameObject.transform.position.x + 0.25f, unit.gameObject.transform.position.y, unit.gameObject.transform.position.z);
                Vector3 manainstantposition = new Vector3(unit.gameObject.transform.position.x - 0.25f, unit.gameObject.transform.position.y, unit.gameObject.transform.position.z);

                GameObject newHealthPickup = Instantiate(m_HealthPickupPrefab, healthinstantposition, Quaternion.identity) as GameObject;
                GameObject newManaPickup = Instantiate(m_ManaPickupPrefab, manainstantposition, Quaternion.identity) as GameObject;

                newHealthPickup.GetComponent<Rigidbody>().AddExplosionForce(250 + Random.value * 750, unit.gameObject.transform.position, 10);
                newManaPickup.GetComponent<Rigidbody>().AddExplosionForce(250 + Random.value * 750, unit.gameObject.transform.position, 10);
            }

            m_Enemies.Remove(unit);
        }

        private IEnumerator Spawn()
        {

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
                    yield return null;
                }


            }
          
        }
    }
}
