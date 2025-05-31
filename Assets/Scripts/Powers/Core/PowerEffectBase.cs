using UnityEngine;

public abstract class PowerEffectBase : MonoBehaviour
{
    protected PowerHandler powerHandler;
    protected PlayerBase player;

    [Header("Effect Settings")]
    [SerializeField] protected float effectDuration;
    [SerializeField] protected GameObject visualEffectPrefab;

    protected virtual void Start()
    {
        powerHandler = GetComponent<PowerHandler>();
        player = GetComponent<PlayerBase>();
    }

    protected abstract void ApplyEffect();
    protected abstract void EndEffect();
}