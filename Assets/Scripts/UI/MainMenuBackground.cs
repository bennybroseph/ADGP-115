using UnityEngine;
using System.Collections;

public class MainMenuBackground : MonoBehaviour {

    private Vector2 textOffset;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newOffset = Vector2.zero;

        newOffset = new Vector2(textOffset.x, textOffset.y - Time.deltaTime / 3);

        textOffset = gameObject.GetComponent<Renderer>().material.mainTextureOffset = newOffset;
    }
}
