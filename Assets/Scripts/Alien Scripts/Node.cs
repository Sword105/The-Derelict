using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; //To display something in scene
#endif

public class Node : MonoBehaviour
{
    public Transform node_manager;
    public Transform player;
    public float range = 5f;

    
    //Calculate the probability that the player is in that Node
    public double nodeProbability;
    
    /*
    The amount of time that the player has been in the Node. 
    We start with 1 second for the calculations of the probability 
    that the player is inside the range of that Node.
    */
    public float timeInside = 1f; 
    public int score = 1;
     
    
    //These variables are used to check the last time the player was inside a Node, and Also to reduce the score.
    private bool reducingScore = false;
    private int lastScore = -1; 
    
    
    void Awake(){
        InvokeRepeating("checkLastTimeInside", 15f, 10f);
    }

    
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        node_manager = GameObject.FindWithTag("NodeManager").transform;
    }
    

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if(distance<range){
            reducingScore = false;
            timeInside += Time.deltaTime * node_manager.GetComponent<NodeManager>().incMultiplier; //Increase the amount of time in the Node
        }
        //If the bool reduce score is true, and the score is > 1, reduce it
        else if(reducingScore && score>1){
            timeInside -= Time.deltaTime * node_manager.GetComponent<NodeManager>().decMultiplier;
        }

        score = (int) timeInside;
    }


    public void calculateProbability(){
        nodeProbability = (1.0* score)/node_manager.GetComponent<NodeManager>().totalScore;
    }


    //If the last score is equal to the current score, that means that the player has not been in this node, so reducingScore = true
    public void checkLastTimeInside(){
        if(lastScore == score){
            reducingScore = true;
        }
        lastScore = score;
    }


    //helper to add the nodes of the rooms that the player unlocks
    public void changeTag(){
        if(transform.tag != "Node"){
            transform.tag = "Node";
        }
    }


    #if UNITY_EDITOR
    //Displays the radious and timeInside
    void OnDrawGizmos()
    {
        //Display the range of the Node
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        //Display the timeInside variable
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black; // Text color
        style.fontSize = 14;                  // Font size
        style.fontStyle = FontStyle.Bold;     // Bold, Italic, etc.

        Handles.Label(transform.position + Vector3.up * 2, "Score: " + score, style);        
    }
    #endif
    
}
