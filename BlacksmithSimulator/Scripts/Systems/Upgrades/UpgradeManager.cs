using System;
using System.Collections.Generic;
using UnityEngine;
using Blacksmith.Systems.Economy;

namespace Blacksmith.Systems.Upgrades
{
	public class UpgradeManager : MonoBehaviour
	{
		[SerializeField] private EconomySystem economy;
		[SerializeField] private List<UpgradeDefinition> availableUpgrades = new List<UpgradeDefinition>();

		private readonly HashSet<string> purchased = new HashSet<string>();
		public event Action<UpgradeDefinition> OnUpgradePurchased;

		public bool IsPurchased(string id) => purchased.Contains(id);

		public bool TryPurchase(UpgradeDefinition def)
		{
			if (def == null || purchased.Contains(def.id)) return false;
			if (!economy.SpendMoney(def.cost)) return false;
			purchased.Add(def.id);
			OnUpgradePurchased?.Invoke(def);
			return true;
		}

		public float GetUpgradeValueOrDefault(UpgradeType type, float defaultValue = 0f)
		{
			float total = defaultValue;
			for (int i = 0; i < availableUpgrades.Count; i++)
			{
				var def = availableUpgrades[i];
				if (purchased.Contains(def.id) && def.type == type) total += def.value;
			}
			return total;
		}

		public IReadOnlyCollection<string> GetPurchasedIds() => purchased;

		public void RestorePurchased(IEnumerable<string> ids)
		{
			purchased.Clear();
			foreach (var id in ids)
			{
				if (!string.IsNullOrEmpty(id)) purchased.Add(id);
			}
		}
	}
}