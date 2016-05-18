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
        public delegate void NoParameters();

        [SerializeField]
        private Text m_AnnouncementPrefab;
        [SerializeField]
        private Text m_CurrentAnnouncement;

        [SerializeField]
        private Queue<string> m_CurrentAnnouncements;
        [SerializeField]
        private Queue<string> m_OldAnnouncements;

        [SerializeField]
        private AnimationCurve m_ScaleCurve;
        [SerializeField]
        private AnimationCurve m_FadeCurve;

        [SerializeField, Range(0.0f, 5.0f)]
        private float m_TimeBetweenAnimations;

        [SerializeField]
        private bool m_CoroutineIsRunning;

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();

            if (m_AnnouncementPrefab == null)
                Debug.Log("'" + gameObject.name + "'needs a prefab of the Announcement Text");

            m_CurrentAnnouncements = new Queue<string>();
            m_OldAnnouncements = new Queue<string>();
        }

        private void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_CoroutineIsRunning && m_CurrentAnnouncements.Count > 0)
            {
                m_CurrentAnnouncement = Instantiate(m_AnnouncementPrefab);
                m_CurrentAnnouncement.transform.SetParent(UIManager.self.transform, false);
                //m_CurrentAnnouncement.transform.localPosition = Vector3.zero;
                //m_CurrentAnnouncement.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

                m_CurrentAnnouncement.text = m_CurrentAnnouncements.Dequeue();

                StartCoroutine(AnimateText());
            }
        }

        public void Announce(string a_Announcement)
        {
            m_CurrentAnnouncements.Enqueue(a_Announcement);
        }

        public void DelayedAnnouncement(string a_Announcement, float a_TimeToWait)
        {
            StartCoroutine(
                WaitForThis(
                    a_TimeToWait,
                    delegate { m_CurrentAnnouncements.Enqueue(a_Announcement); }));
        }

        private IEnumerator AnimateText()
        {
            m_CoroutineIsRunning = true;

            Keyframe lastFrame = m_ScaleCurve[m_ScaleCurve.length - 1];

            float deltaTime = 0.0f;
            while (deltaTime < lastFrame.time)
            {
                deltaTime += Time.deltaTime;

                m_CurrentAnnouncement.transform.localScale =
                    new Vector3(
                        m_ScaleCurve.Evaluate(deltaTime),
                        m_ScaleCurve.Evaluate(deltaTime));

                yield return false;
            }

            deltaTime = 0.0f;
            yield return new WaitForSeconds(m_TimeBetweenAnimations);

            lastFrame = m_FadeCurve[m_FadeCurve.length - 1];

            deltaTime = 0.0f;
            while (deltaTime < lastFrame.time)
            {
                deltaTime += Time.deltaTime;

                m_CurrentAnnouncement.color =
                    new Color(0, 0, 0, m_FadeCurve.Evaluate(deltaTime));

                yield return false;
            }

            m_OldAnnouncements.Enqueue(m_CurrentAnnouncement.text);

            Destroy(m_CurrentAnnouncement.gameObject);

            m_CoroutineIsRunning = false;
        }

        private IEnumerator WaitForThis(float a_TimeToWait, NoParameters a_Delegate)
        {
            yield return new WaitForSeconds(a_TimeToWait);

            a_Delegate();
        }
    }
}
