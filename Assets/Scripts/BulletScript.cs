using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float Speed = 3f;
    public float pushForce = 2.5f;

    private Rigidbody2D Rigidbody2D;
    private Vector3 Direction;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = Direction * Speed;
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player2"))
        {
            HandleCollision(collision);
        }
        DestroyBullet();
    }

    private void HandleCollision(Collision2D collision)
    {
        Rigidbody2D targetRB = collision.gameObject.GetComponent<Rigidbody2D>();
        if (targetRB != null)
        {
            // Reset completo del estado físico
            targetRB.linearVelocity = Vector2.zero;
            targetRB.angularVelocity = 0f;

            // Cálculo de dirección consistente
            Vector2 hitPoint = collision.contacts[0].point;
            Vector2 targetCenter = collision.transform.position;
            Vector2 pushDirection = (hitPoint.x < targetCenter.x) ? Vector2.right : Vector2.left;

            // Aplicar fuerza
            targetRB.AddForce(pushDirection * pushForce * 100f, ForceMode2D.Impulse);

            // Deshabilitar temporalmente el control del personaje
            var magoScript = collision.gameObject.GetComponent<MagoOscuroMovement>();
            var huargenScript = collision.gameObject.GetComponent<HuargenMovement>();

            if (magoScript != null)
                StartCoroutine(DisableControlsTemporarily(magoScript));
            if (huargenScript != null)
                StartCoroutine(DisableControlsTemporarily(huargenScript));
        }
    }

    private IEnumerator DisableControlsTemporarily(MonoBehaviour controller)
    {
        controller.enabled = false;
        yield return new WaitForSeconds(0.2f);
        controller.enabled = true;
    }
}