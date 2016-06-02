using Library;
using UnityEngine;
using UnityEngineInternal;

// Created enum to define different types of sounds
public enum SoundTypes
{
    Fireball,
    Melee,
    Lightning,
    TitleScreenMusic

}
[System.Serializable]
public class Sound
{
    // Type of the sound
    [SerializeField]
    private SoundTypes m_SoundType;
    // File Clip
    [SerializeField]
    private AudioClip m_Clip;
    // AudioSource Component
    private AudioSource m_Source;
    // Set volume to 0.5f by default
    private float m_Volume = 0.5f;

    public SoundTypes Type
    {
        get { return m_SoundType; }
        set { m_SoundType = value; }
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
        m_Source.playOnAwake = false;
        Mathf.Clamp(m_Volume, 0.0f, 1.0f);
        m_Source.volume = m_Volume;
    }
    // Function that plays the sound
    public void Play()
    {
        m_Source.Play();
    }
}

public class AudioManager : MonoSingleton<AudioManager>
{

    [SerializeField]
    private Sound[] m_Sounds;

	// Use this for initialization
	void Start ()
    {
	    for (int index = 0; index < m_Sounds.Length; index++)
	    {// Create game objects 
	        GameObject _go = new GameObject("Sound_" + index + "_" + m_Sounds[index].Type);
            // Set the objects parent to the game manager
            _go.transform.SetParent(transform);
            // Add an audio source component to the object
            m_Sounds[index].SetSource(_go.AddComponent<AudioSource>());
	    }
	}
	
    public void PlaySound(SoundTypes a_type)
    {
        for(int index = 0; index < m_Sounds.Length; index++)
	    {
	        if (m_Sounds[index].Type == a_type)
	        {// Play the sounds
	            m_Sounds[index].Play();
	            return;
	        }
        }

        Debug.Log("AudioManager: Sound Not found in list: " + a_type);
    }
}
