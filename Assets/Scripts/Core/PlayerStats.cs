using UnityEngine;
using System.Collections.Generic;

public class PlayerStats
{
    private Dictionary<PowerSystem.PowerType, int> powerStocks;
    
    // Property para acceder a PowerStocks de forma segura
    public Dictionary<PowerSystem.PowerType, int> PowerStocks => powerStocks;

    public PlayerStats()
    {
        powerStocks = new Dictionary<PowerSystem.PowerType, int>
        {
            { PowerSystem.PowerType.ForceField, 0 },
            { PowerSystem.PowerType.Shield, 0 },
            { PowerSystem.PowerType.AirJump, 0 }
        };
    }

    public void AddPowerStock(PowerSystem.PowerType powerType, int amount = 1)
    {
        if (powerStocks.ContainsKey(powerType))
        {
            int previousAmount = powerStocks[powerType];
            powerStocks[powerType] = Mathf.Min(powerStocks[powerType] + amount, PowerConstants.MAX_POWER_STOCK);
            int newAmount = powerStocks[powerType];
            
            Debug.Log($"Power {powerType} increased: {previousAmount} -> {newAmount} " +
                     $"(Added {amount}, Max: {PowerConstants.MAX_POWER_STOCK})");
        }
    }

    public bool UsePowerStock(PowerSystem.PowerType powerType)
    {
        if (powerStocks.ContainsKey(powerType) && powerStocks[powerType] > 0)
        {
            powerStocks[powerType]--;
            return true;
        }
        return false;
    }

    public int GetPowerStock(PowerSystem.PowerType powerType)
    {
        return powerStocks.ContainsKey(powerType) ? powerStocks[powerType] : 0;
    }
}