using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager instance;

    public List<Node> nodeList = new List<Node>();

    public int totalScore = 1;

    public float incMultiplier = 2.0f;
    public float decMultiplier = 2.0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Multiple NodeManagers found, destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        // Now that nodes have registered, do initial sums
        sumScores();
        calculateNodesProb();

        InvokeRepeating(nameof(sumScores), 5f, 10f);
        InvokeRepeating(nameof(calculateNodesProb), 5f, 10f);
    }

    public void RegisterNode(Node node)
    {
        if (node == null) return;

        if (!nodeList.Contains(node))
        {
            nodeList.Add(node);
            Debug.Log($"[NodeManager] Registered node: {node.name}");
        }
    }

    public void UnregisterNode(Node node)
    {
        if (node == null) return;

        if (nodeList.Remove(node))
        {
            // Optional: recompute on removal
            sumScores();
            calculateNodesProb();
        }
    }

    //The sum of the total amount of time that the player has been in all the Nodes
    private void sumScores()
    {
        if (nodeList == null)
        {
            Debug.LogError("[NodeManager] nodeList is null in sumScores()");
            return;
        }

        totalScore = 0;

        // Go backwards so we can remove nulls safely
        for (int i = nodeList.Count - 1; i >= 0; i--)
        {
            Node node = nodeList[i];

            if (node == null)
            {
                Debug.LogWarning($"[NodeManager] Null node at index {i}, removing from list.");
                nodeList.RemoveAt(i);
                continue;
            }

            totalScore += node.score;
        }

        if (totalScore <= 0)
            totalScore = 1;

        Debug.Log($"[NodeManager] sumScores() count = {nodeList.Count}, totalScore = {totalScore}");
    }

    private void calculateNodesProb()
    {
        if (nodeList == null) return;

        for (int i = nodeList.Count - 1; i >= 0; i--)
        {
            Node node = nodeList[i];

            if (node == null)
            {
                nodeList.RemoveAt(i);
                continue;
            }

            node.calculateProbability();
        }
    }
}
