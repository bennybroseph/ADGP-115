using Library;
using UnityEngine;

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
    private float m_Volume = 0.1f;

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

    public AudioSource Source
    {
        get { return m_Source; }
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
}

public class AudioManager : MonoSingleton<AudioManager>
{

    [SerializeField]
    private Sound[] m_Sounds;

    public Sound[] Sounds
    {
        get { return m_Sounds; }
    }
    // Use this for initialization
    void Start()
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
        foreach (Sound sound in m_Sounds)
        {
            if (sound.Type == a_type)
            {// Play the sounds
                sound.Source.Play();
                return;
            }
        }

        Debug.Log("AudioManager: Sound Not found in list: " + a_type);
    }
}
