using UnityEngine;

public class TestCanvasScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float camHeight = 0f;
        if (Camera.main.orthographic)
            camHeight = Camera.main.orthographicSize * 2;
        //else
        //camHeight = 2.0f * distanceToMain * Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView * 0.5f));
        
        transform.localScale = new Vector3(1.4f * camHeight / Screen.height, 1.4f * camHeight / Screen.height);
    }
}
