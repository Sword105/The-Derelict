using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class is abstract because it is NOT meant to be placed on an object
// Instead, you reference this class through another script
public abstract class AlienBrain : MonoBehaviour
{
    // Uses the "Roulette Wheel Selection" algorithm to calculate a node to go to
    // Don't ask me how it works unless you want gaps in your teeth
    public static Node MostLikelyNode(NodeManager nodeManager, float temperature)
    {
        List<Node> allNodes = nodeManager.nodeList;

        // Calculate the cumulative probabilities of all the nodes and place them in a new array that is parallel to the node list
        double[] cumulativeProbabilities = new double[allNodes.Count];
        cumulativeProbabilities[0] = Math.Pow(allNodes[0].nodeProbability, 1 / temperature);
        for (int i = 1; i < cumulativeProbabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + allNodes[i].nodeProbability;
        }

        // Generate random value
        float valueToFind = UnityEngine.Random.Range(0f, 1f);

        // If the random value is below a certain threshold, return that node index
        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (valueToFind <= cumulativeProbabilities[i])
                return nodeManager.nodeList[i];
        }

        return nodeManager.nodeList[0];
    }

    // Picks an adjacent node to explore based on the current node
    // Prioritizes adjacent nodes with higher likelihood of player presence
    // Ignores nodes that have already been explored
    public static Node PickAdjacentNodeToExplore(Node currentNode, LayerMask nodeLayer, List<GameObject> nodesToIgnore)
    {
        List<Collider> nearbyNodes = Physics.OverlapSphere(currentNode.transform.position, currentNode.range + 15f, nodeLayer).ToList();
        nearbyNodes.Remove(currentNode.GetComponent<Collider>());

        Node nextNodeToFollow = null;
        double maxNodeProbability = 0;
        foreach (Collider collider in nearbyNodes)
        {
            Node node = collider.GetComponent<Node>();

            RaycastHit hit;
            Physics.Raycast(currentNode.transform.position, node.transform.position - currentNode.transform.position, out hit, nodeLayer);

            if (!nodesToIgnore.Contains(node.gameObject))
            {
                if (node.nodeProbability >= maxNodeProbability && hit.collider == collider)
                {
                    nextNodeToFollow = node;
                    maxNodeProbability = node.nodeProbability;
                }
            }
        }

        nodesToIgnore.Add(currentNode.gameObject);
        return nextNodeToFollow;
    }
}
