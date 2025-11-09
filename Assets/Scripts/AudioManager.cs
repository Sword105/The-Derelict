using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void PlaySoundFX(AudioClip audioClip, Vector3 audioLocation, float volume,  bool isSpatialized)
    {
        // These four lines of code create a new object that plays a sound in a certain location
        // This should be used when playing ANY sound
        // The reason we do this is so we can control which mixer track the sound plays in and prevent overlapping sounds from cutting each other out if they are from the same source
        AudioSource soundFXSource = Instantiate(soundFXObject, audioLocation, Quaternion.identity);
        soundFXSource.clip = audioClip;
        soundFXSource.volume = volume;
        soundFXSource.spatialize = isSpatialized;
        soundFXSource.Play();

        float clipLength = audioClip.length;
        Destroy(soundFXSource.gameObject, clipLength);
    }
}
