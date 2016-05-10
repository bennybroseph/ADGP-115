using UnityEngine;
using System.Collections;

public class MyCamera : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_Following;
    [SerializeField]
    protected Vector3 m_Offset;

    //[SerializeField, Tooltip("How close the camera should get before it decides that it should stop trying to be more accurate")]
    //protected float m_CloseEnough;

    [System.Serializable]
    protected class Box
    {
        public Vector3 m_Min;
        public Vector3 m_Max;
    }

    [SerializeField]
    protected Box m_ScreenBorders;

    public GameObject following
    {
        get { return m_Following; }
        set { m_Following = value; }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(
            m_Following.transform.position.x - m_Offset.x,
            m_Following.transform.position.y - m_Offset.y,
            m_Following.transform.position.z - m_Offset.z);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_ScreenBorders.m_Min.x, m_ScreenBorders.m_Max.x), 
            Mathf.Clamp(transform.position.y, m_ScreenBorders.m_Min.y, m_ScreenBorders.m_Max.y),
            Mathf.Clamp(transform.position.z, m_ScreenBorders.m_Min.z, m_ScreenBorders.m_Max.z));
    }
}
