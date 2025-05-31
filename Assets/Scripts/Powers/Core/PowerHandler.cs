using UnityEngine;
using System.Collections.Generic;

public abstract class PowerHandler : MonoBehaviour
{
    protected PowerSystem powerSystem;
    protected PlayerBase player;
    protected AudioSource audioSource;
    protected Dictionary<PowerSystem.PowerType, PowerEffectBase> powerEffects;
    
    public PowerSystem PowerSystem => powerSystem;

    [Header("Power Audio")]
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected AudioClip shieldSound;
    [SerializeField] protected AudioClip airJumpSound;

    protected virtual void Start()
    {
        player = GetComponent<PlayerBase>();
        audioSource = GetComponent<AudioSource>();
        powerEffects = new Dictionary<PowerSystem.PowerType, PowerEffectBase>();
        InitializePowerSystem();
        InitializePowerEffects();
    }

    protected abstract void InitializePowerSystem();
    protected abstract void InitializePowerEffects();
    public abstract void HandlePowerInput();

    protected virtual void ActivatePower(PowerSystem.PowerType powerType)
    {
        if (powerSystem.HasPowerStock(powerType) && !powerSystem.IsPowerInCooldown(powerType))
        {
            powerSystem.UsePower(powerType);
            if (powerEffects.ContainsKey(powerType))
            {
                powerEffects[powerType].SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}