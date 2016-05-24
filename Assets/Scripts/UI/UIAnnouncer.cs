using System.Collections;
using System.Collections.Generic;
using Library;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UIAnnouncer : MonoSingleton<UIAnnouncer>
    {
        public enum AnnouncementType
        {
            Overhead,
            ExperienceGained,
            ExperienceLost,
            HealthGained,
            MagicDamage,
            PhysicalDamage,
            ManaGained,
            ManaLost,
            LevelUp
        }

        private delegate void VoidFunction();

        #region -- VARIABLES --
        [Header("Prefabs")]
        [SerializeField]
        private FloatingText m_FloatingTextPrefab;
        [SerializeField]
        private Text m_AnnouncementTextPrefab;
        [SerializeField]
        private Text m_LogTextPrefab;

        private bool m_AnchorIsSet;
        private Vector3 m_Anchor;

        private Text m_CurrentAnnouncementObject;
        private List<Text> m_LogItems;

        private Queue<string> m_QueuedAnnouncements;

        [Header("Floating Text")]
        [SerializeField]
        private AnimationSequence m_FloatingTextSequence;

        [Header("Announcement Animation")]
        [SerializeField]
        private AnimationSequence m_AnnouncementSequence;

        [Header("Announcement Log")]
        [SerializeField]
        private AnimationSequence m_LogSequence;
        [SerializeField]
        private float m_SpaceBetweenLogItems;
        [SerializeField, Range(0.0f, 10.0f)]
        private float m_MaxNumberOfLogItems;
        [SerializeField, Tooltip("The time until the log item is deleted. Set to -1 for an infinite lifetime")]
        private float m_LogItemLifetime;

        private bool m_CoroutineIsRunning;
        #endregion

        #region -- UNITY FUNCTIONS --
        private void OnValidate()
        {
            m_AnnouncementSequence.CacheAnimationTime();
            m_LogSequence.CacheAnimationTime();
        }

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();

            if (m_AnnouncementTextPrefab == null)
            {
                Debug.LogError("'" + gameObject.name + "'needs a prefab of the Announcement Text");
                gameObject.SetActive(false);
            }
            if (m_LogTextPrefab == null)
            {
                Debug.LogError("'" + gameObject.name + "'needs a prefab of the Chat Log Text");
                gameObject.SetActive(false);
            }

            m_QueuedAnnouncements = new Queue<string>();
            m_LogItems = new List<Text>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
                Announce("Test Announcement Incoming! " + Time.time);
            if (Input.GetKeyDown(KeyCode.F5))
                Announce(
                    10 * Random.value,
                    new Vector3(10 * Random.value, 0, 10 * Random.value),
                    AnnouncementType.MagicDamage);
        }
        #endregion
        public void Announce(string a_Announcement)
        {
            m_QueuedAnnouncements.Enqueue(a_Announcement);

            if (!m_CoroutineIsRunning)
                CreateNewAnnouncement();
        }

        public void Announce(string a_Announcemnt, Vector3 a_Anchor, AnnouncementType a_Type)
        {
            FloatingText newObject = Instantiate(m_FloatingTextPrefab);
            newObject.transform.SetParent(UIManager.self.transform, false);

            newObject.anchor = a_Anchor;

            Text newText = newObject.GetComponent<Text>();
            newText.text = a_Announcemnt;

            switch (a_Type)
            {
                case AnnouncementType.PhysicalDamage:
                    newText.color = new Color(1, 0, 0);
                    break;
                case AnnouncementType.MagicDamage:
                    newText.color = new Color(0, 0, 1);
                    break;
            }

            StartCoroutine(Animations.Animate(m_FloatingTextSequence, newText));
        }
        public void Announce(float a_Announcemnt, Vector3 a_Anchor, AnnouncementType a_Type)
        {
            switch (a_Type)
            {
                case AnnouncementType.MagicDamage:
                case AnnouncementType.PhysicalDamage:
                    Announce(string.Format("-{0:0.0}", a_Announcemnt), a_Anchor, a_Type);
                    break;
            }
        }


        public void DelayedAnnouncement(string a_Announcement, float a_TimeToWait)
        {
            StartCoroutine(
                WaitThenDoThis(
                    a_TimeToWait,
                    delegate { Announce(a_Announcement); }));
        }

        private void CreateNewAnnouncement()
        {
            m_CurrentAnnouncementObject = Instantiate(m_AnnouncementTextPrefab);
            m_CurrentAnnouncementObject.transform.SetParent(UIManager.self.transform, false);

            m_CurrentAnnouncementObject.text = m_QueuedAnnouncements.Dequeue();

            StartCoroutine(AnimateText());
        }

        private void SortAnnouncementLog()
        {
            if (!m_AnchorIsSet)
            {
                m_Anchor = m_LogItems[m_LogItems.Count - 1].transform.position;
                m_AnchorIsSet = true;
            }

            for (int i = 0; i < m_LogItems.Count; ++i)
            {
                m_LogItems[i].transform.position =
                    new Vector3(
                        m_Anchor.x,
                        m_Anchor.y + i * -m_SpaceBetweenLogItems);
            }
        }

        #region -- COROUTINES --
        private IEnumerator AnimateText()
        {
            m_CoroutineIsRunning = true;

            m_CurrentAnnouncementObject.transform.localScale = new Vector3(0, 0, 0);

            yield return StartCoroutine(Animations.Animate(m_AnnouncementSequence, m_CurrentAnnouncementObject));

            Text newLogItem = Instantiate(m_LogTextPrefab);
            newLogItem.transform.SetParent(UIManager.self.transform, false);
            newLogItem.text = m_CurrentAnnouncementObject.text;

            m_LogItems.Add(newLogItem);

            Destroy(m_CurrentAnnouncementObject.gameObject);

            if (m_LogItemLifetime != -1.0f)
            {
                StartCoroutine(WaitThenDoThis(m_LogItemLifetime - m_LogSequence.totalAnimationTime / 2,
                        delegate
                        {
                            StartCoroutine(
                                Animations.AnimateLayer(
                                    m_LogSequence.animationLayers[m_LogSequence.animationLayers.Count - 1],
                                    newLogItem));
                        }));
                StartCoroutine(WaitThenDoThis(m_LogItemLifetime,
                        delegate
                        {
                            Destroy(newLogItem.gameObject);
                            m_LogItems.Remove(newLogItem);

                            SortAnnouncementLog();
                        }));
            }

            m_CoroutineIsRunning = false;

            if (m_QueuedAnnouncements.Count > 0)
                CreateNewAnnouncement();

            StartCoroutine(AnimateToLog());
        }
        private IEnumerator AnimateToLog()
        {
            while (m_LogItems.Count > m_MaxNumberOfLogItems)
            {
                Destroy(m_LogItems[0].gameObject);
                m_LogItems.RemoveAt(0);
            }

            SortAnnouncementLog();

            yield return StartCoroutine(Animations.AnimateLayer(
                m_LogSequence.animationLayers[0],
                m_LogItems[m_LogItems.Count - 1]));
        }

        private IEnumerator WaitThenDoThis(float a_TimeToWait, VoidFunction a_Delegate)
        {
            yield return new WaitForSeconds(a_TimeToWait);

            a_Delegate();
        }
        #endregion
    }
}
