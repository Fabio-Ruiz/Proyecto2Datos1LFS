using UnityEngine;
using System.Collections;

public class ForcePushEffect : PowerEffectBase
{
    [Header("Force Push Settings")]
    [SerializeField] private GameObject forceBallPrefab;
    private bool isActive = false;

    private void Awake()  // Remove override keyword
    {
        // Try to load prefab if not assigned
        if (forceBallPrefab == null)
        {
            forceBallPrefab = Resources.Load<GameObject>("Prefabs/ForceBall");
            if (forceBallPrefab == null)
            {
                Debug.LogError($"[ForcePush] Create a ForceBall prefab in Resources/Prefabs folder");
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        // Usar el powerHandler de la clase base
        if (base.powerHandler != null)
        {
            // Eliminar el CheckForForcePushActivation ya que causa múltiples disparos
        }
        else
        {
            Debug.LogError($"[ForcePush] No se encontró PowerHandler en {gameObject.name}");
        }
    }

    protected override void ApplyEffect()
    {
        if (forceBallPrefab == null)
        {
            Debug.LogError($"[ForcePush] Assign ForceBall prefab in inspector for {gameObject.name}");
            return;
        }

        if (isActive) return;

        isActive = true;
        
        // Calcular posición de disparo automáticamente
        float offset = 1f; // Distancia desde el centro del personaje
        Vector2 shootDirection = transform.localScale.x < 0 ? Vector2.left : Vector2.right;
        Vector3 spawnPosition = transform.position + (Vector3)(shootDirection * offset);
        
        // Instanciar y inicializar la bola de fuerza
        GameObject ball = Instantiate(forceBallPrefab, spawnPosition, Quaternion.identity);
        ForceBallScript forceBall = ball.GetComponent<ForceBallScript>();
        
        if (forceBall != null)
        {
            forceBall.Initialize(shootDirection, gameObject.tag);
        }

        StartCoroutine(ResetEffect());
    }

    private IEnumerator ResetEffect()
    {
        yield return new WaitForSeconds(0.5f);
        isActive = false;
    }

    protected override void EndEffect()
    {
        // No necesario para Force Push ya que es instantáneo
    }
}