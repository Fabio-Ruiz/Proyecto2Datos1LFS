using UnityEngine;
using System.Collections;

public class ShieldEffect : PowerEffectBase
{
    [Header("Shield Settings")]
    [SerializeField] private GameObject shieldVisual;
    private bool isActive = false;
    private float duration = PowerConstants.SHIELD_DURATION;

    protected override void Start()
    {
        base.Start();
        // Create and setup shield visual if not assigned
        if (shieldVisual == null)
        {
            shieldVisual = CreateShieldVisual();
        }
        shieldVisual.SetActive(false);
    }

    private GameObject CreateShieldVisual()
    {
        GameObject shield = new GameObject("Shield");
        shield.transform.parent = transform;
        shield.transform.localPosition = Vector3.zero;
        
        // Add sprite renderer
        SpriteRenderer renderer = shield.AddComponent<SpriteRenderer>();
        renderer.sprite = Resources.Load<Sprite>("Sprites/Shield");
        renderer.sortingOrder = 1;
        renderer.color = new Color(0.5f, 0.8f, 1f, 0.5f);
        
        return shield;
    }

    protected override void ApplyEffect()
    {
        if (isActive) return;
        
        isActive = true;
        shieldVisual.SetActive(true);
        
        // Make player invulnerable
        if (TryGetComponent<PlayerBase>(out var player))
        {
            player.SetInvulnerable(true);
        }
        
        StartCoroutine(DeactivateAfterDuration());
    }

    private IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        EndEffect();
    }

    protected override void EndEffect()
    {
        if (!isActive) return;
        
        isActive = false;
        shieldVisual.SetActive(false);
        
        // Remove invulnerability
        if (TryGetComponent<PlayerBase>(out var player))
        {
            player.SetInvulnerable(false);
        }
    }

    private void OnDisable()
    {
        EndEffect();
    }
}