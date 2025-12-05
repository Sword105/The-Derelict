using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Interactable
{
    public AudioClip[] radioSongs;
    public bool isPlaying;

    private void Start()
    {
        isPlaying = false;
    }

    public override void Interact(PlayerInteraction player, Item activeItem)
    {
        if (!isPlaying)
        {
            AlienStateMachine.instance.InvokeSuspiciousEvent(transform.position, 999f);
            StartCoroutine(PlayRadioSong());
        }
    }

    public IEnumerator PlayRadioSong()
    {
        int randomSongIndex = Random.Range(0, radioSongs.Length);
        AudioManager.instance.PlaySoundFX(radioSongs[randomSongIndex], transform.position, 0.5f, true);
        isPlaying = true;

        yield return new WaitForSeconds(radioSongs[randomSongIndex].length);
        isPlaying = false;
    }
}
