using UnityEngine;
using Blacksmith.Systems.Orders;

namespace Blacksmith.Gameplay.Forge
{
	public class ForgeGameCoordinator : MonoBehaviour
	{
		[SerializeField] private OrderSystem orders;
		[SerializeField] private ForgeController forge;

		private void OnEnable()
		{
			if (forge != null)
			{
				forge.OnForgingCompleted += HandleForgingCompleted;
			}
		}
		private void OnDisable()
		{
			if (forge != null)
			{
				forge.OnForgingCompleted -= HandleForgingCompleted;
			}
		}

		public void AcceptNextOrder()
		{
			orders?.AcceptRandomOrder();
		}

		public void BeginForgingActiveOrder()
		{
			if (orders != null && orders.HasActiveOrder)
			{
				forge?.BeginForging(orders.ActiveOrder);
			}
		}

		private void HandleForgingCompleted(float quality)
		{
			orders?.CompleteActiveOrder(quality);
		}
	}
}