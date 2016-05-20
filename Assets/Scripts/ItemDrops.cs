using UnityEngine;
using System.Collections;

public class ItemDrops : MonoBehaviour
{
    [SerializeField]
    private int m_HealthIncrease;
    [SerializeField]
    private int m_ManaIncrease;

    public int HealthIncrease
    {
        get { return m_HealthIncrease; }
        set { m_HealthIncrease = value; }
    }

    public int ManaIncrease
    {
        get { return m_ManaIncrease; }
        set { m_ManaIncrease = value; }
    }
    // Update is called once per frame
    void Update ()
    {
	   transform.Rotate(new Vector3(0,45,0) * Time.deltaTime * 5);
	   transform.Translate(new Vector3(0, Mathf.Sin(Time.time * 9)) * Time.deltaTime, 0);
    }

    //void OnTriggerStay(Collider a_Collision)
    //{
    //    if (a_Collision.transform.gameObject.tag == "Player")
    //    {
    //        transform.position = Vector3.Lerp(transform.position, a_Collision.transform.position, Time.deltaTime * 2);
    //    }
    //}

}
