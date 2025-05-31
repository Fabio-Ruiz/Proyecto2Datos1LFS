using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PowerSystem : MonoBehaviour
{
    public enum PowerType
    {
        None,
        ForceField,   // Force Push
        Shield,
        AirJump
    }

    [System.Serializable]
    public class PowerData
    {
        public PowerType type;
        public bool isActive;
        public bool inCooldown;
        public const int MAX_STOCK = PowerConstants.MAX_POWER_STOCK;  // Cambiar aquí
    }

    protected Dictionary<PowerType, PowerData> powers;
    protected PlayerStats stats;
    protected string playerTag;

    protected virtual void Start()
    {
        InitializePowers();
    }

    private void InitializePowers()
    {
        powers = new Dictionary<PowerType, PowerData>();
        foreach (PowerType type in System.Enum.GetValues(typeof(PowerType)))
        {
            if (type != PowerType.None)
            {
                powers[type] = new PowerData
                {
                    type = type,
                    isActive = false,
                    inCooldown = false
                };
            }
        }
    }

    public virtual bool IsPowerAvailable(PowerType powerType)
    {
        if (!powers.ContainsKey(powerType))
            return false;

        PowerData powerData = powers[powerType];
        return !powerData.inCooldown && !powerData.isActive && HasPowerStock(powerType);
    }

    public virtual bool IsPowerActive(PowerType powerType)
    {
        if (!powers.ContainsKey(powerType))
            return false;

        return powers[powerType].isActive;
    }

    public virtual bool IsPowerInCooldown(PowerType type)
    {
        return powers.ContainsKey(type) && powers[type].inCooldown;
    }

    protected virtual void SetPowerCooldown(PowerType type, bool value)
    {
        if (powers.ContainsKey(type))
        {
            powers[type].inCooldown = value;
        }
    }

    public virtual void AddPowerCharges()
    {
        AddPowerStock(PowerType.ForceField);
        AddPowerStock(PowerType.Shield);
        AddPowerStock(PowerType.AirJump);
    }

    public abstract bool HasPowerStock(PowerType powerType);
    public abstract void AddPowerStock(PowerType powerType, int amount = 1);
    protected abstract IEnumerator StartPowerCooldown(PowerType powerType);

    protected virtual void Update()
    {
        CheckPowerInputs();
    }

    protected virtual void CheckPowerInputs()
    {
        KeyCode forcePushKey, shieldKey, airJumpKey;

        switch (playerTag)
        {
            case "Player":
                forcePushKey = GameInput.Player1.FORCE_PUSH;
                shieldKey = GameInput.Player1.SHIELD;
                airJumpKey = GameInput.Player1.AIR_JUMP;
                break;
            case "Player2":
                forcePushKey = GameInput.Player2.FORCE_PUSH;
                shieldKey = GameInput.Player2.SHIELD;
                airJumpKey = GameInput.Player2.AIR_JUMP;
                break;
            case "Player3":
                forcePushKey = GameInput.Player3.FORCE_PUSH;
                shieldKey = GameInput.Player3.SHIELD;
                airJumpKey = GameInput.Player3.AIR_JUMP;
                break;
            default:
                return;
        }

        if (Input.GetKeyDown(forcePushKey))
            UsePower(PowerType.ForceField);
        if (Input.GetKeyDown(shieldKey))
            UsePower(PowerType.Shield);
        if (Input.GetKeyDown(airJumpKey))
            UsePower(PowerType.AirJump);
    }

    public virtual void UsePower(PowerType powerType)
    {
        if (stats.UsePowerStock(powerType))
        {
            Debug.Log($"=== {playerTag} USÓ PODER ===");
            Debug.Log($"Poder: {powerType}");
            Debug.Log($"Stock restante: {stats.GetPowerStock(powerType)}");

            // Ejecutar el poder específico
            switch (powerType)
            {
                case PowerType.AirJump:
                    PlayerBase player = GetComponent<PlayerBase>();
                    if (player != null)
                    {
                        player.SendMessage("PerformAirJump", SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                // ... otros casos para otros poderes
            }

            Debug.Log("======================");
        }
        else
        {
            Debug.Log($"{playerTag}: No hay stock disponible para {powerType}");
        }
    }
}