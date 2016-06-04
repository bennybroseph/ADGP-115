using UnityEngine;
using System.Collections;
using UI;
using Library;
using Event = Define.Event;

public class MainMenuBackground : MonoBehaviour {

    private Vector2 m_TextOffset;
    private GameObject m_GameUIPrefab;

    // Use this for initialization
    void Start()
    {
        m_GameUIPrefab = GameObject.FindGameObjectWithTag("Background UI");
        AudioManager.self.PlaySound(SoundTypes.TitleScreenMusic);
        Destroy(m_GameUIPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector2 newOffset = Vector2.zero;

        newOffset = new Vector2(m_TextOffset.x, m_TextOffset.y - Time.deltaTime / 8);

        m_TextOffset = gameObject.GetComponent<Renderer>().material.mainTextureOffset = newOffset;
    }
}
