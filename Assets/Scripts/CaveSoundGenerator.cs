using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CaveSoundGenerator : MonoBehaviour
{
    public List<AudioClip> caveSoundList = new List<AudioClip>();
    public double timeSinceLastCaveSoundCheck;
    public Transform player;

    public float deadzoneRange = 10f;
    public float maxDistance = 20f;

    // Start is called before the first frame update

    void Start()
    {
        timeSinceLastCaveSoundCheck = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastCaveSoundCheck += Time.deltaTime;
        if (timeSinceLastCaveSoundCheck > 1)
        {
            timeSinceLastCaveSoundCheck = 0;
            int randomSoundIndex = Random.Range(0, caveSoundList.Count);

            if (Random.Range(0f, 1f) > 0.9f)
            {
                Vector3 randomPoint = player.position + (Random.insideUnitSphere * maxDistance);
                while (Vector3.Distance(randomPoint, player.position) < deadzoneRange)
                {
                    randomPoint = player.transform.position + (Random.insideUnitSphere * maxDistance);
                }

                AudioManager.instance.PlaySoundFX(caveSoundList[randomSoundIndex], randomPoint, 1f, true);
            }
        }
    }
}
