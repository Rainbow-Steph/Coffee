using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryManager", menuName = "Coffee/Inventory Manager")]
public class InventoryManager : ScriptableObject
{
    public enum InventoryField
    {
        Money,
        StoredLiquidAmount,
        StoredLiquidType,
        BlackCapsules,
        BlueCapsules,
        RedCapsules,
        Salt,
        Sugar,
        Pepper
    }

    [Header("Inventory")]
    public int Money = 0;

    [Header("Stored Liquid")]
    public string StoredLiquidType = "";
    public int StoredLiquidAmount = 0;

    [Header("Capsules")]
    public int BlackCapsules = 0;
    public int BlueCapsules = 0;
    public int RedCapsules = 0;

    [Header("Additives")]
    public int Salt = 0;
    public int Sugar = 0;
    public int Pepper = 0;

    /// <summary>
    /// Fired when any inventory value changes
    /// </summary>
    public Action onInventoryChanged;

    public string GetFieldDisplay(InventoryField field)
    {
        switch (field)
        {
            case InventoryField.Money:
                return Money.ToString();
            case InventoryField.StoredLiquidAmount:
                return StoredLiquidAmount.ToString();
            case InventoryField.StoredLiquidType:
                return string.IsNullOrEmpty(StoredLiquidType) ? "None" : StoredLiquidType;
            case InventoryField.BlackCapsules:
                return BlackCapsules.ToString();
            case InventoryField.BlueCapsules:
                return BlueCapsules.ToString();
            case InventoryField.RedCapsules:
                return RedCapsules.ToString();
            case InventoryField.Salt:
                return Salt.ToString();
            case InventoryField.Sugar:
                return Sugar.ToString();
            case InventoryField.Pepper:
                return Pepper.ToString();
            default:
                return string.Empty;
        }
    }

    #region Helper Setters

    public void SetMoney(int amount)
    {
        Money = amount;
        onInventoryChanged?.Invoke();
    }

    public void AddMoney(int delta)
    {
        Money += delta;
        onInventoryChanged?.Invoke();
    }

    public void SetStoredLiquid(string type, int amount)
    {
        StoredLiquidType = type;
        StoredLiquidAmount = amount;
        onInventoryChanged?.Invoke();
    }

    public void AddStoredLiquid(int amount)
    {
        StoredLiquidAmount += amount;
        onInventoryChanged?.Invoke();
    }

    public void SetCapsules(int black, int blue, int red)
    {
        BlackCapsules = black;
        BlueCapsules = blue;
        RedCapsules = red;
        onInventoryChanged?.Invoke();
    }

    public void AddBlackCapsules(int amount)
    {
        BlackCapsules += amount;
        onInventoryChanged?.Invoke();
    }

    public void AddBlueCapsules(int amount)
    {
        BlueCapsules += amount;
        onInventoryChanged?.Invoke();
    }

    public void AddRedCapsules(int amount)
    {
        RedCapsules += amount;
        onInventoryChanged?.Invoke();
    }

    public void AddSalt(int amount)
    {
        Salt += amount;
        onInventoryChanged?.Invoke();
    }

    public void AddSugar(int amount)
    {
        Sugar += amount;
        onInventoryChanged?.Invoke();
    }

    public void AddPepper(int amount)
    {
        Pepper += amount;
        onInventoryChanged?.Invoke();
    }

    #endregion
}
