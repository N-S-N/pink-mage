using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider EfectSlider;

    private void Start()
    {
        if(PlayerPrefs.GetInt("mudouVolumeGeral") == 1)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("mesterVolume");
            SetGeralVolume();
        }
        else
        {
            SetGeralVolume();
        }
        if (PlayerPrefs.GetInt("mudouVolumeMusicl") == 1)
        {
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            SetMusicVolume();
        }
        else
        {
            SetMusicVolume();
        }
        if (PlayerPrefs.GetInt("mudouVolumeEfect") == 1)
        {
            EfectSlider.value = PlayerPrefs.GetFloat("efectVolume");
            SetEfectVolume();
        }
        else
        {
            SetEfectVolume();
        }
    }

    public void SetGeralVolume()
    {
        float volume = volumeSlider.value;
        audioMixer.SetFloat("Mestrer", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("mesterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume1 = MusicSlider.value;
        audioMixer.SetFloat("MUsic", Mathf.Log10(volume1) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume1);
    }

    public void SetEfectVolume()
    {
        float volume2 = EfectSlider.value;
        audioMixer.SetFloat("Efect", Mathf.Log10(volume2) * 20);
        PlayerPrefs.SetFloat("efectVolume", volume2);
    }


    public void mudencGeral()
    {
        PlayerPrefs.SetInt("mudouVolumeGeral", 1);
    }
    public void mudencMusic()
    {
        PlayerPrefs.SetInt("mudouVolumeMusicl", 1);
    }
    public void mudencEfect()
    {
        PlayerPrefs.SetInt("mudouVolumeEfect", 1);
    }

}
