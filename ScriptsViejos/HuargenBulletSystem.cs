using UnityEngine;

public class HuargenBulletSystem : MonoBehaviour
{
    public GameObject BulletPrefab;
    public AudioClip disparoClip;
    private AudioSource audioSource;
    private float LastShoot;
    private HuargenPowerSystem powerSystem;

    void Start()
    {
        powerSystem = GetComponent<HuargenPowerSystem>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Sistema de disparo independiente
        if (Input.GetKeyDown(KeyCode.Alpha7) && Time.time > LastShoot + 0.25f)
        {
            if (powerSystem != null && powerSystem.HasPowerStock(PowerSystem.PowerType.ForceField))
            {
                Shoot();
                LastShoot = Time.time;
                powerSystem.UsePower(PowerSystem.PowerType.ForceField);

                if (disparoClip != null)
                    audioSource.PlayOneShot(disparoClip);
            }
        }
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) 
            direction = Vector3.left;
        else 
            direction = Vector3.right;

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.3f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }
}
