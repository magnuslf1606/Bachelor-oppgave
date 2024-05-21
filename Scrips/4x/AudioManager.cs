 
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Aucio Score ----------")]
    [SerializeField] AudioSource m_Source;
    [SerializeField] AudioSource SFX_Source;
    public GameObject sfxObj;

    [Header("---------- Aucio Score ----------")]
    public AudioClip background;
    public AudioClip death;

    public AudioClip MoveSound, ClickSound, ArmySound, SettleSound, TownSound, ExitSound, TechSound, BackpackSound, BuildSound, FogClearingSound;
  

    public Slider volumSlider;
    public Toggle togle;

    private float savedMusic = 0f;
    private string volumStored = "Volume";
    private float position = 0f;

    private void Start()
    {

        savedMusic = PlayerPrefs.GetFloat(volumStored, 1.0f);

        if (volumSlider != null) 
        {
            volumSlider.value = savedMusic;
        }
        if(togle != null)
        {
            togle.onValueChanged.AddListener(onValueChanged);
            togle.isOn = (savedMusic > 0f);
        }
        
        m_Source.volume = savedMusic;
        SFX_Source.volume = savedMusic;
        m_Source.clip = background;
        m_Source.Play();
    }

    public void stopMusic()
    {
        if(volumSlider != null)
        {
            m_Source.volume = volumSlider.value;
            SFX_Source.volume = volumSlider.value;

            PlayerPrefs.SetFloat(volumStored, volumSlider.value);
            PlayerPrefs.Save();
        }
    }

    public void stopSound()
    {
        if(togle != null)
        {
            if(!togle.isOn)
            {
                m_Source.Stop();
            }
        }
    }

    private void onValueChanged(bool on)
    {
        if (on)
        {
            m_Source.Play();
        } else
        {
            m_Source.Stop();
        }
    }

    public void PlayFX(AudioClip sound){
        if (sound == FogClearingSound) {
            SFX_Source.clip = sound;
            SFX_Source.Play();
        }
        else {
            GameObject newSource = GameObject.Instantiate(sfxObj);
            newSource.GetComponent<AudioSource>().clip = sound;
            newSource.GetComponent<AudioSource>().Play();
        }
        
    }

}
