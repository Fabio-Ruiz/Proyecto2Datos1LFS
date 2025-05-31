using UnityEngine;

public class AirJumpEffect : PowerEffectBase
{
    [Header("Air Jump Settings")]
    [SerializeField] private float jumpMultiplier = PowerConstants.AIR_JUMP_MULTIPLIER;
    [SerializeField] private GameObject jumpEffectPrefab;
    [SerializeField] private AudioClip jumpSound; // Añadido campo para el sonido

    protected override void ApplyEffect()
    {
        PlayerBase player = GetComponent<PlayerBase>();
        if (player == null) return;

        // Get the correct jump key based on player tag
        KeyCode jumpKey = player.gameObject.CompareTag("Player1") ? KeyCode.W : KeyCode.I;
        
        // Only apply if player is in the air and pressing jump
        if (!player.Grounded && Input.GetKeyDown(jumpKey))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Perform the air jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * player.JumpForce * jumpMultiplier, ForceMode2D.Impulse);

                // Visual effects
                if (jumpEffectPrefab != null)
                {
                    GameObject effect = Instantiate(jumpEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(effect, 1f);
                }

                // Audio effects
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null && jumpSound != null)
                {
                    audioSource.PlayOneShot(jumpSound);
                }
            }
        }
    }

    protected override void EndEffect()
    {
        // Air Jump es un efecto instantáneo, no necesita cleanup
        // pero debemos implementar el método abstracto
    }
}