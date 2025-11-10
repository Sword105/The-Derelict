using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CaveSoundGenerator : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public float deadzoneRange = 10f;
    public float maxDistance = 20f;
    public float minimumTimeForSoundToPlay = 30f;
    public float chanceForSoundToPlay = 0.1f;

    [Header("Sound List")]
    public List<AudioClip> caveSoundList = new List<AudioClip>();

    [Header("DEBUG")]
    [SerializeField] private double timeSinceLastCaveSoundCheck;
    

    void Start()
    {
        timeSinceLastCaveSoundCheck = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastCaveSoundCheck += Time.deltaTime;
        if (timeSinceLastCaveSoundCheck > minimumTimeForSoundToPlay)
        {
            timeSinceLastCaveSoundCheck = 0;
            int randomSoundIndex = Random.Range(0, caveSoundList.Count);

            if (Random.Range(0f, 1f) >= (1 - chanceForSoundToPlay))
            {
                Vector2 randomPoint = Random.insideUnitCircle;
                Vector3 soundPosition = player.position + (new Vector3(randomPoint.x, player.position.y, randomPoint.y) * maxDistance);
                while (Vector3.Distance(soundPosition, player.position) < deadzoneRange)
                {
                    randomPoint = Random.insideUnitCircle;
                    soundPosition = player.position + (new Vector3(randomPoint.x, player.position.y, randomPoint.y) * maxDistance);
                }

                AudioManager.instance.PlaySoundFX(caveSoundList[randomSoundIndex], soundPosition, 1f, true);
            }
        }
    }
}
