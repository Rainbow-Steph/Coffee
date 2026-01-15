using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that manages delivery targets, satisfaction ratings, and rewards
/// </summary>
[CreateAssetMenu(fileName = "DeliverableManager", menuName = "Coffee/Deliverable Manager")]
public class DeliverableManager : ScriptableObject
{
    [System.Serializable]
    public class RewardRange
    {
        [Tooltip("Minimum number of items to spawn")]
        public int min = 0;
        
        [Tooltip("Maximum number of items to spawn")]
        public int max = 3;

        public int GetRandomAmount()
        {
            return Random.Range(min, max + 1);
        }
    }

    [System.Serializable]
    public class SatisfactionRewardRanges
    {
        [Header("Reward Ranges")]
        [Tooltip("Number of rocks to spawn")]
        public RewardRange rocksRange = new RewardRange();
        
        [Tooltip("Number of coins to spawn")]
        public RewardRange coinsRange = new RewardRange();
        
        [Tooltip("Number of bills to spawn")]
        public RewardRange billsRange = new RewardRange();
    }

    [System.Serializable]
    public class DeliveryTarget
    {
        [Header("Target Info")]
        public string targetName;
        public Color targetColor = Color.white;

        [Header("High Satisfaction Items")]
        [Tooltip("Prefabs that give HIGH satisfaction when delivered")]
        public List<GameObject> highSatisfactionItems = new List<GameObject>();

        [Header("Medium Satisfaction Items")]
        [Tooltip("Prefabs that give MEDIUM satisfaction when delivered")]
        public List<GameObject> mediumSatisfactionItems = new List<GameObject>();

        [Header("Low Satisfaction Items")]
        [Tooltip("Prefabs that give LOW satisfaction when delivered")]
        public List<GameObject> lowSatisfactionItems = new List<GameObject>();

        /// <summary>
        /// Determine satisfaction level for a delivered item
        /// </summary>
        public SatisfactionLevel GetSatisfactionLevel(GameObject deliveredPrefab)
        {
            if (deliveredPrefab == null)
                return SatisfactionLevel.None;

            // Check high satisfaction
            foreach (var item in highSatisfactionItems)
            {
                if (item != null && IsSamePrefab(deliveredPrefab, item))
                    return SatisfactionLevel.High;
            }

            // Check medium satisfaction
            foreach (var item in mediumSatisfactionItems)
            {
                if (item != null && IsSamePrefab(deliveredPrefab, item))
                    return SatisfactionLevel.Medium;
            }

            // Check low satisfaction
            foreach (var item in lowSatisfactionItems)
            {
                if (item != null && IsSamePrefab(deliveredPrefab, item))
                    return SatisfactionLevel.Low;
            }

            return SatisfactionLevel.None;
        }

        /// <summary>
        /// Check if two GameObjects are the same prefab
        /// </summary>
        private bool IsSamePrefab(GameObject obj1, GameObject obj2)
        {
            // Compare by name (remove "(Clone)" suffix)
            string name1 = obj1.name.Replace("(Clone)", "").Trim();
            string name2 = obj2.name.Replace("(Clone)", "").Trim();
            return name1 == name2;
        }
    }

    [Header("Reward Prefabs - Shared Across All Satisfaction Levels")]
    [Tooltip("Rock prefab used for all satisfaction levels")]
    public GameObject rockPrefab;
    
    [Tooltip("Coin prefab used for all satisfaction levels")]
    public GameObject coinPrefab;
    
    [Tooltip("Bill prefab used for all satisfaction levels")]
    public GameObject billPrefab;

    [Header("High Satisfaction Rewards")]
    [Tooltip("Reward ranges for HIGH satisfaction delivery")]
    public SatisfactionRewardRanges highSatisfactionRewards = new SatisfactionRewardRanges();

    [Header("Medium Satisfaction Rewards")]
    [Tooltip("Reward ranges for MEDIUM satisfaction delivery")]
    public SatisfactionRewardRanges mediumSatisfactionRewards = new SatisfactionRewardRanges();

    [Header("Low Satisfaction Rewards")]
    [Tooltip("Reward ranges for LOW satisfaction delivery")]
    public SatisfactionRewardRanges lowSatisfactionRewards = new SatisfactionRewardRanges();

    [Header("Delivery Targets")]
    [Tooltip("List of all possible delivery targets")]
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();

    /// <summary>
    /// Find a delivery target by name
    /// </summary>
    public DeliveryTarget FindTargetByName(string targetName)
    {
        return deliveryTargets.Find(t => t.targetName == targetName);
    }

    /// <summary>
    /// Get reward ranges based on satisfaction level
    /// </summary>
    public SatisfactionRewardRanges GetRewardRanges(SatisfactionLevel level)
    {
        switch (level)
        {
            case SatisfactionLevel.High:
                return highSatisfactionRewards;
            case SatisfactionLevel.Medium:
                return mediumSatisfactionRewards;
            case SatisfactionLevel.Low:
                return lowSatisfactionRewards;
            default:
                return null;
        }
    }

    /// <summary>
    /// Validate configuration
    /// </summary>
    public bool ValidateConfiguration()
    {
        bool valid = true;

        // Check reward prefabs
        if (rockPrefab == null && coinPrefab == null && billPrefab == null)
        {
            Debug.LogWarning("[DeliverableManager] No reward prefabs assigned!");
            valid = false;
        }

        // Check delivery targets
        if (deliveryTargets.Count == 0)
        {
            Debug.LogWarning("[DeliverableManager] No delivery targets configured!");
            valid = false;
        }

        return valid;
    }
}

/// <summary>
/// Satisfaction levels for deliveries
/// </summary>
public enum SatisfactionLevel
{
    None,
    Low,
    Medium,
    High
}
