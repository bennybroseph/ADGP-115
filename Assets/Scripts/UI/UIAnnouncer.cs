using System;
using System.Collections;
using System.Collections.Generic;
using Library;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAnnouncer : MonoSingleton<UIAnnouncer>
    {
        private delegate void VoidFunction();

        private enum AnimationType
        {
            Fade,
            Scale,
            Rotate,
            Translate,
        }

        [Serializable]
        private class AnimationValue
        {
            public AnimationType animationType;
            [Tooltip("One Animation Curve means a Scale, Rotate, and Translate animation will use that curve on both values. Otherwise it's separate.")]
            public List<AnimationCurve> animationCurve;
        }

        [SerializeField]
        private Text m_AnnouncementTextPrefab;
        [SerializeField]
        private Text m_LogTextPrefab;

        private bool m_AnchorIsSet;
        private Vector3 m_Anchor;

        private Text m_CurrentAnnouncementObject;
        private List<Text> m_LogItems;

        private Queue<string> m_QueuedAnnouncements;

        [Header("Announcement Animation")]
        [SerializeField]
        private List<AnimationValue> m_AnimationValues;
        [SerializeField]
        private AnimationCurve m_ScaleCurve;
        [SerializeField]
        private AnimationCurve m_FadeCurve;
        [SerializeField, Range(0.0f, 5.0f)]
        private float m_TimeBetweenAnimations;

        [Header("Announcement Log Animation")]
        [SerializeField]
        private AnimationCurve m_LogFadeIn;

        [Header("Announcement Log Constraints")]
        [SerializeField]
        private float m_SpaceBetweenLogItems;
        [SerializeField, Range(0.0f, 10.0f)]
        private float m_MaxNumberOfLogItems;
        [SerializeField, Tooltip("The time until the log item is deleted. Set to -1 for an infinite lifetime")]
        private float m_LogItemLifetime;

        [SerializeField]
        private bool m_CoroutineIsRunning;

        private void OnValidate()
        {
            foreach (AnimationValue animationValue in m_AnimationValues)
            {
                if (animationValue.animationCurve.Count < 1)
                    animationValue.animationCurve.Add(new AnimationCurve());
                if (animationValue.animationCurve.Count > 2)
                    animationValue.animationCurve.RemoveAt(animationValue.animationCurve.Count - 1);
            }
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

        private void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
                Announce("Test Announcement Incoming! " + Time.time);
        }

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

        private IEnumerator AnimateText()
        {
            m_CoroutineIsRunning = true;

            int i = 0;
            foreach (AnimationValue animationValue in m_AnimationValues)
            {
                if (i != 0)
                    yield return new WaitForSeconds(m_TimeBetweenAnimations);

                switch (animationValue.animationType)
                {
                    case AnimationType.Fade:
                        yield return
                            StartCoroutine(
                                Animations.Fade2DGraphic(
                                    m_CurrentAnnouncementObject,
                                    animationValue.animationCurve.ToArray()));
                        break;
                    case AnimationType.Scale:
                        yield return
                            StartCoroutine(
                                Animations.Scale2DTransform(
                                    m_CurrentAnnouncementObject.transform,
                                    animationValue.animationCurve.ToArray()));
                        break;
                }
                i++;
            }

            Text newLogItem = Instantiate(m_LogTextPrefab);
            newLogItem.transform.SetParent(UIManager.self.transform, false);
            newLogItem.text = m_CurrentAnnouncementObject.text;

            m_LogItems.Add(newLogItem);

            Destroy(m_CurrentAnnouncementObject.gameObject);

            if (m_LogItemLifetime != -1.0f)
                StartCoroutine(
                    WaitThenDoThis(
                        m_LogItemLifetime,
                        delegate
                        {
                            Destroy(newLogItem.gameObject);
                            m_LogItems.Remove(newLogItem);

                            SortAnnouncementLog();
                        }));

            m_CoroutineIsRunning = false;

            if (m_QueuedAnnouncements.Count > 0)
                CreateNewAnnouncement();

            StartCoroutine(AnimateToLog());
        }
        private IEnumerator AnimateToLog()
        {
            while (m_LogItems.Count > m_MaxNumberOfLogItems)
            {
                Destroy(m_LogItems[m_LogItems.Count - 1].gameObject);
                m_LogItems.RemoveAt(m_LogItems.Count - 1);
            }

            SortAnnouncementLog();

            yield return StartCoroutine(Animations.Fade2DGraphic(m_LogItems[m_LogItems.Count - 1], m_LogFadeIn));

            yield return false;
        }

        private IEnumerator WaitThenDoThis(float a_TimeToWait, VoidFunction a_Delegate)
        {
            yield return new WaitForSeconds(a_TimeToWait);

            a_Delegate();
        }
    }
}
