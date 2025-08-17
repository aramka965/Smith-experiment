using UnityEngine;

namespace Blacksmith.Systems.Blueprints
{
	[CreateAssetMenu(menuName = "Blacksmith/Blueprint", fileName = "ItemBlueprint")]
	public class ItemBlueprint : ScriptableObject
	{
		public string displayName;
		public ItemType itemType;
		[Header("Heating")]
		public float targetTemperatureC = 900f;
		public float temperatureTolerance = 50f;
		[Header("Difficulties (higher = harder)")]
		[Range(0.1f, 2f)] public float hammeringDifficulty = 1f;
		[Range(0.1f, 2f)] public float quenchingDifficulty = 1f;
		[Range(0.1f, 2f)] public float sharpeningDifficulty = 1f;
		[Header("Economy")]
		public float basePrice = 100f;
		public float qualityPriceMultiplier = 2.0f;
	}

	public enum ItemType { Sword, Dagger, Axe, Mace, Helmet, Shield }
}