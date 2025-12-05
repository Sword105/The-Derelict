using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    //Looks for player and places camera above them
    public Transform player;
    public float height = 50f;
    [SerializeField] private Canvas enemyTarget;


    // Updates every frame
    void LateUpdate()
    {
        //If there is no player, do nothing
        if (player == null) return;

        //Gets the player's position and sets the minimap's position to be above the player at a set height
        Vector3 newPos = player.position;
        newPos.y += height;
        transform.position = newPos;

        //Moves the minimap to face where the player is facing
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);


    }
}