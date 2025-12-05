using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Node : MonoBehaviour
{
    // You can delete this if you don't need it in Inspector anymore
    // public Transform node_manager;
    public Transform player;
    public float range = 20f;

    private Renderer cachedRenderer;
    private NodeManager manager;

    public double nodeProbability;
    public float timeInside = 1f;
    public int score = 1;

    private bool reducingScore = false;
    private int lastScore = -1;


    void OnValidate()
    {
        CacheRenderer();
    }

    void Awake()
    {
        CacheRenderer();
        InvokeRepeating(nameof(checkLastTimeInside), 15f, 10f);
    }

    private void OnEnable()
    {
        TryRegisterWithManager();
    }

    void Start()
    {
        // Cache player
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Cache NodeManager via singleton
        manager = NodeManager.instance;
        if (manager == null)
        {
            Debug.LogError("[Node] NodeManager.instance is null. Make sure there is exactly one NodeManager in the scene.");
        }

        // Optional: if you still want the Transform reference
        // if (manager != null) node_manager = manager.transform;

        // In case OnEnable ran before instance was set
        TryRegisterWithManager();
    }

    private void OnDisable()
    {
        if (NodeManager.instance != null)
            NodeManager.instance.UnregisterNode(this);
    }

    void CacheRenderer()
    {
        if (cachedRenderer == null)
            cachedRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (cachedRenderer == null || player == null || manager == null)
            return;

        Vector3 center = cachedRenderer.bounds.center;
        float distance = Vector3.Distance(center, player.position);

        if (distance < range)
        {
            reducingScore = false;
            timeInside += Time.deltaTime * manager.incMultiplier;
        }
        else if (reducingScore && score > 1)
        {
            timeInside -= Time.deltaTime * manager.decMultiplier;
        }

        if (timeInside < 1f)
            timeInside = 1f;

        score = (int)timeInside;
    }

    public void calculateProbability()
    {
        if (manager == null)
            manager = NodeManager.instance;

        int total = (manager != null) ? manager.totalScore : 1;
        if (total <= 0) total = 1;

        nodeProbability = (1.0 * score) / total;
    }

    public void checkLastTimeInside()
    {
        if (lastScore == score)
            reducingScore = true;

        lastScore = score;
    }

    public void changeTag()
    {
        if (transform.tag != "Node")
            transform.tag = "Node";
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        CacheRenderer();
        if (cachedRenderer == null) return;

        Vector3 center = cachedRenderer.bounds.center;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, range);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;

        Handles.Label(center + Vector3.up * 2, "Score: " + score, style);
    }
#endif

    private void TryRegisterWithManager()
    {
        if (NodeManager.instance != null)
            NodeManager.instance.RegisterNode(this);
    }
}
