using UnityEngine;
using Blacksmith.Systems.Blueprints;

namespace Blacksmith.Systems.Orders
{
	[CreateAssetMenu(menuName = "Blacksmith/Order Definition", fileName = "OrderDefinition")]
	public class OrderDefinition : ScriptableObject
	{
		public ItemBlueprint blueprint;
		[Range(0f, 1f)] public float requiredQuality = 0.6f;
		public float timeLimitSeconds = 180f;
		public float payoutMultiplier = 1f;
		public string customerName;
		[TextArea] public string notes;

		public OrderInstance CreateInstance()
		{
			return new OrderInstance
			{
				blueprint = blueprint,
				requiredQuality = requiredQuality,
				timeLimitSeconds = timeLimitSeconds,
				payoutMultiplier = payoutMultiplier,
				customerName = customerName,
				notes = notes
			};
		}
	}

	[System.Serializable]
	public class OrderInstance
	{
		public ItemBlueprint blueprint;
		public float requiredQuality;
		public float timeLimitSeconds;
		public float payoutMultiplier;
		public string customerName;
		public string notes;
	}
}