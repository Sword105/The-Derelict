using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public List<Node> nodeList = new List<Node>();

    //The sum of the scores of all the
    public int totalScore = 1;
    
    void Awake()
    {
        //Find all the Nodes
        getAllNodes();
        sumScores();
        calculateNodesProb();

        // Every 10 seconds, sums the scores
        InvokeRepeating("sumScores", 5f, 10f);
        InvokeRepeating("calculateNodesProb", 5f, 10f);
    }


    //The sum of the total amount of time that the player has been in all the Nodes
    private void sumScores(){
        totalScore = 0;
        for (int i = 0; i<nodeList.Count; i++){
            totalScore += nodeList[i].score;
        }

        Debug.Log("Sum of scores made");
    }


    //Finds all the Nodes, and adds them into NodeList
    private void getAllNodes(){
        // Find all game objects with the tag "Node"
        GameObject[] foundNodes = GameObject.FindGameObjectsWithTag("Node");

        //Adds the nodes to the NodeList
        foreach (GameObject obj in foundNodes)
        {
            Node node = obj.GetComponent<Node>();
            if (node != null)
            {
                nodeList.Add(node);
            }
        }
    }

    private void calculateNodesProb(){
        foreach(Node node in nodeList){
            node.calculateProbability();
        }
    }
}
