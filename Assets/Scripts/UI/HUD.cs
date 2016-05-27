using UnityEngine;
using System.Collections;
using Interfaces;
using Library;
using UnityEngine.UI;
using Event = Define.Event;

//public class HUD : MonoBehaviour, IChildable<IStats>
//{
//    //[SerializeField]
//    //private Text m_Name;
//    //[SerializeField]
//    //private Text m_Level;
//    //[SerializeField]
//    //private Image m_EXPBar;
//    //[SerializeField]
//    //private Image m_NegativeManaBar;
//    //[SerializeField]
//    //private Image m_ManaBar;
//    //[SerializeField]
//    //private Image m_NegativeHealthBar;
//    //[SerializeField]
//    //private Image m_HealthBar;

//    //private IStats m_Parent;

//    //public IStats parent
//    //{
//    //    get { return m_Parent; }
//    //    set { m_Parent = value; }
//    //}

//    //private void Awake()
//    //{
//    //    Publisher.self.Subscribe(Event.UnitHealthChanged, OnValueChanged);
//    //}
//    //// Use this for initialization
//    //private void Start()
//    //{

//    //}

//    //// Update is called once per frame
//    //private void Update()
//    //{

//    //}

//    //private void OnValueChanged(Event a_Event, params object[] a_Params)
//    //{
//    //    IStats unit = a_Params[0] as IStats;

//    //    if (unit == null || unit != m_Parent)
//    //        return;

//    //    switch (a_Event)
//    //    {
//    //        case Event.UnitLevelChanged:
//    //            {
//    //                SetText()
//    //            }
//    //            break;
//    //    }
//    //}
//}
