using Library;
using UnityEngine;
using UnityEngineInternal;

[System.Serializable]
public class Sound
{
    // Name of the file
    [SerializeField]
    private string m_Name;
    // File Clip
    [SerializeField]
    private AudioClip m_Clip;
    // AudioSource Component
    private AudioSource m_Source;
    // Set volume to 0.5f by default
    private float m_Volume = 0.5f;

    public string Name
    {
        get { return m_Name; }
        set { m_Name = value; }
    }

    public float Volume
    {
        get { return m_Volume; }
        set { m_Volume = value; }
    }
    // Function to set the audisource
    public void SetSource(AudioSource a_Source)
    {
        m_Source = a_Source;
        m_Source.clip = m_Clip;
    }
    // Function that plays the sound
    public void Play()
    {
        Mathf.Clamp(m_Volume, 0.0f, 1.0f);
        m_Source.volume = m_Volume;
        m_Source.playOnAwake = false;
        m_Source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager s_Instance;
    [SerializeField]
    private Sound[] m_Sounds;

    public static AudioManager instance
    {
        get
        {
            return s_Instance;
        }
    }

    void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = new AudioManager();
        }

        s_Instance = this;
    }
	// Use this for initialization
	void Start ()
    {
	    for (int index = 0; index < m_Sounds.Length; index++)
	    {// Create game objects 
	        GameObject _go = new GameObject("Sound_" + index + "_" + m_Sounds[index].Name);
            // Set the objects parent to the game manager
            _go.transform.SetParent(transform);
            // Add an audio source component to the object
            m_Sounds[index].SetSource(_go.AddComponent<AudioSource>());
	    }
	}
	
    public void PlaySound(string a_name)
    {
        for(int index = 0; index < m_Sounds.Length; index++)
	    {
	        if (m_Sounds[index].Name == a_name)
	        {// Play the sounds
	            m_Sounds[index].Play();
	            return;
	        }
        }

        Debug.Log("AudioManager: Sound Not found in list: " + a_name);
    }
}
