using UnityEngine;

namespace Blacksmith.Systems.Upgrades
{
	public enum UpgradeType { FasterHeating, StrongerHammer, BetterQuenching, SharperWheel, OrderSlot, CooldownReduce }

	[CreateAssetMenu(menuName = "Blacksmith/Upgrade Definition", fileName = "UpgradeDefinition")]
	public class UpgradeDefinition : ScriptableObject
	{
		public string id;
		public string displayName;
		[TextArea] public string description;
		public UpgradeType type;
		public float value;
		public float cost;
	}
}