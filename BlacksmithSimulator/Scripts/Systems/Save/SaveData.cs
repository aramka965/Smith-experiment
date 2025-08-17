using System;

namespace Blacksmith.Systems.Save
{
	[Serializable]
	public class SaveData
	{
		public float money;
		public string[] purchasedUpgradeIds;
	}
}