using UnityEngine;
using System.Collections;

public class TeacherTest : MonoBehaviour
{
    public GameObject prefab;
    public float bounds = 50;
    public int num = 50;
	// Use this for initialization
    [ContextMenu("spawn")]
	void Spawn ()
	{
        for (int i = 0; i < num; i++)
        {
            float rx = UnityEngine.Random.Range(-bounds, bounds);
            float ry = UnityEngine.Random.Range(1, 5);
            float rz = UnityEngine.Random.Range(-bounds, bounds);
            Vector3 rv = new Vector3(rx, ry, rz);
            GameObject go = Instantiate(prefab, rv, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
        }
	}

    void Start()
    {
        Spawn();
    }
	
 
}
