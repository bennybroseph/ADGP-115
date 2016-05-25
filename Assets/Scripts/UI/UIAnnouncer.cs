using System.Collections;
using System.Collections.Generic;
using Library;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public enum FloatingTextType
    {
        Overhead,
        ExperienceGained,
        ExperienceLost,
        HealthGained,
        MagicDamage,
        PhysicalDamage,
        ManaGained,
        ManaLost,
    }

    public enum ChatType
    {
        Say,
        Yell,
        Whisper,
    }

    public class UIAnnouncer : MonoSingleton<UIAnnouncer>
    {
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
        [SerializeField]
        private AnimationSequence m_ChatSequence;

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
            m_ChatSequence.CacheAnimationTime();
            m_FloatingTextSequence.CacheAnimationTime();
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

        }
        #endregion

        public void Announce(string a_Announcement)
        {
            m_QueuedAnnouncements.Enqueue(a_Announcement);

            if (!m_CoroutineIsRunning)
                CreateNewAnnouncement();
        }
        public void DelayedAnnouncement(string a_Announcement, float a_TimeToWait)
        {
            StartCoroutine(
                WaitThenDoThis(
                    a_TimeToWait,
                    delegate { Announce(a_Announcement); }));
        }

        public void Chat(string a_Nickname, string a_Message, Unit a_Unit, ChatType a_ChatType = ChatType.Say)
        {
            Text newLogItem = Instantiate(m_LogTextPrefab);
            newLogItem.transform.SetParent(UIManager.self.backgroundUI.transform, false);

            switch (a_ChatType)
            {
                case ChatType.Say:
                    newLogItem.color = Color.black;
                    newLogItem.text = a_Nickname + " says: " + a_Message;
                    break;
                case ChatType.Yell:
                    newLogItem.color = Color.red;
                    newLogItem.text = a_Nickname + " yells: " + a_Message;
                    break;
            }

            FloatingText newObject = Instantiate(m_FloatingTextPrefab);
            newObject.transform.SetParent(UIManager.self.backgroundUI.transform, false);
            newObject.anchor = new Vector3(0, 0, 0.45f);

            newObject.parent = a_Unit;

            Text newText = newObject.GetComponent<Text>();
            newText.text = a_Message + "\n|";
            newText.color = newLogItem.color;

            StartCoroutine(Animations.Animate(m_ChatSequence, newText));
            StartCoroutine(WaitThenDoThis(m_ChatSequence.totalAnimationTime,
                delegate
                {
                    Destroy(newText);
                }));

            StartCoroutine(AnimateToLog(newLogItem));
        }

        public void FloatingText(string a_Message, Vector3 a_Anchor, FloatingTextType a_Type)
        {
            switch (a_Type)
            {
                case FloatingTextType.Overhead:
                    a_Anchor.z += 0.5f;
                    CreateFloatingText(a_Message, Color.black, a_Anchor);
                    break;
                case FloatingTextType.PhysicalDamage:
                    CreateFloatingText(a_Message, new Color(1, 0, 0), a_Anchor);
                    break;
                case FloatingTextType.MagicDamage:
                    CreateFloatingText(a_Message, new Color(0, 0, 1), a_Anchor);
                    break;
            }
        }
        public void FloatingText(float a_Message, Vector3 a_Anchor, FloatingTextType a_Type)
        {
            switch (a_Type)
            {
                case FloatingTextType.MagicDamage:
                case FloatingTextType.PhysicalDamage:
                    FloatingText(string.Format("-{0:0.0}", a_Message), a_Anchor, a_Type);
                    break;
            }
        }

        private void CreateFloatingText(string a_Message, Color a_Color, Vector3 a_Anchor)
        {
            FloatingText newObject = Instantiate(m_FloatingTextPrefab);
            newObject.transform.SetParent(UIManager.self.backgroundUI.transform, false);

            newObject.anchor = a_Anchor;

            Text newText = newObject.GetComponent<Text>();
            newText.text = a_Message;
            newText.color = a_Color;

            StartCoroutine(Animations.Animate(m_FloatingTextSequence, newText));
            StartCoroutine(WaitThenDoThis(m_FloatingTextSequence.totalAnimationTime,
                delegate
                {
                    Destroy(newText);
                }));
        }

        private void CreateNewAnnouncement()
        {
            m_CurrentAnnouncementObject = Instantiate(m_AnnouncementTextPrefab);
            m_CurrentAnnouncementObject.transform.SetParent(UIManager.self.backgroundUI.transform, false);

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
            newLogItem.transform.SetParent(UIManager.self.backgroundUI.transform, false);
            newLogItem.text = m_CurrentAnnouncementObject.text;

            Destroy(m_CurrentAnnouncementObject.gameObject);

            m_CoroutineIsRunning = false;

            if (m_QueuedAnnouncements.Count > 0)
                CreateNewAnnouncement();

            StartCoroutine(AnimateToLog(newLogItem));
        }

        private IEnumerator AnimateToLog(Text a_LogItem)
        {
            m_LogItems.Add(a_LogItem);

            if (m_LogItemLifetime != -1.0f)
            {
                StartCoroutine(WaitThenDoThis(m_LogItemLifetime - m_LogSequence.totalAnimationTime / 2,
                        delegate
                        {
                            StartCoroutine(
                                Animations.AnimateLayer(
                                    m_LogSequence.animationLayers[m_LogSequence.animationLayers.Count - 1],
                                    a_LogItem));
                        }));
                StartCoroutine(WaitThenDoThis(m_LogItemLifetime,
                        delegate
                        {
                            Destroy(a_LogItem.gameObject);
                            m_LogItems.Remove(a_LogItem);

                            SortAnnouncementLog();
                        }));
            }

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
