using System.IO;
using UnityEngine;
using Blacksmith.Systems.Economy;
using Blacksmith.Systems.Upgrades;

namespace Blacksmith.Systems.Save
{
	public class SaveManager : MonoBehaviour
	{
		[SerializeField] private EconomySystem economy;
		[SerializeField] private UpgradeManager upgrades;
		[SerializeField] private string fileName = "save.json";

		private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

		public void Save()
		{
			var data = new SaveData
			{
				money = economy != null ? economy.Money : 0f,
				purchasedUpgradeIds = upgrades != null ? new System.Collections.Generic.List<string>(upgrades.GetPurchasedIds()).ToArray() : new string[0]
			};
			var json = JsonUtility.ToJson(data, true);
			Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
			File.WriteAllText(FilePath, json);
		}

		public void Load()
		{
			if (!File.Exists(FilePath)) return;
			var json = File.ReadAllText(FilePath);
			var data = JsonUtility.FromJson<SaveData>(json);
			if (economy != null) economy.SetMoney(data.money);
			if (upgrades != null) upgrades.RestorePurchased(data.purchasedUpgradeIds);
		}
	}
}