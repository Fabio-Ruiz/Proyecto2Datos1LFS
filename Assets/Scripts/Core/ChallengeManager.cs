using UnityEngine;
using System.Collections.Generic;

public class ChallengeManager : MonoBehaviour
{
    private static ChallengeManager instance;
    public static ChallengeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ChallengeManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ChallengeManager");
                    instance = go.AddComponent<ChallengeManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private int numberOfChallenges = 50;
    [SerializeField] private int minDepth = 2;
    [SerializeField] private int maxDepth = 5;

    // Lista común de profundidades objetivo
    private List<int> depthChallenges;
    
    // Índice actual de cada jugador en la lista
    private Dictionary<string, int> playerProgress;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeChallenges();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeChallenges()
    {
        // Inicializar lista común
        depthChallenges = new List<int>();
        playerProgress = new Dictionary<string, int>();
        
        // Initialize player progress
        playerProgress["Player"] = 0;
        playerProgress["Player2"] = 0;
        playerProgress["Player3"] = 0;

        // Generar la secuencia de retos
        Random.InitState(System.DateTime.Now.Millisecond);
        for (int i = 0; i < numberOfChallenges; i++)
        {
            depthChallenges.Add(Random.Range(minDepth, maxDepth + 1));
        }

        Debug.Log($"ChallengeManager initialized with {numberOfChallenges} challenges");
    }

    public int GetCurrentChallengeDepth(string playerTag)
    {
        if (!playerProgress.ContainsKey(playerTag)) return -1;
        
        int currentIndex = playerProgress[playerTag];
        if (currentIndex >= depthChallenges.Count) return -1;
        
        return depthChallenges[currentIndex];
    }

    public int GetCurrentChallengeIndex(string playerTag)
    {
        return playerProgress.ContainsKey(playerTag) ? playerProgress[playerTag] : -1;
    }

    public bool CheckChallengeCompletion(string playerTag, int currentTreeDepth)
    {
        int targetDepth = GetCurrentChallengeDepth(playerTag);
        if (targetDepth == -1) return false;

        if (currentTreeDepth >= targetDepth)
        {
            // Avanzar al siguiente reto
            playerProgress[playerTag]++;
            
            // Buscar y limpiar el árbol del jugador
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                var playerBase = player.GetComponent<PlayerBase>();
                if (playerBase != null && playerBase.Tree != null)
                {
                    playerBase.Tree.ClearTree();
                    Debug.Log($"Cleared tree for {playerTag} after completing challenge");
                }
            }

            return true;
        }
        return false;
    }

    // Método para debug/UI
    public string GetPlayerProgress(string playerTag)
    {
        if (!playerProgress.ContainsKey(playerTag)) return "N/A";
        
        int current = playerProgress[playerTag];
        int target = GetCurrentChallengeDepth(playerTag);
        return $"Challenge {current + 1}/{numberOfChallenges} (Target Depth: {target})";
    }
}