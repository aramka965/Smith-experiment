using UnityEngine;
using UnityEngine.Events;
using Blacksmith.Gameplay.Forge;
using Blacksmith.Systems.Orders;
using Blacksmith.Systems.Economy;

namespace Blacksmith.UI.HUD
{
	public class HUDController : MonoBehaviour
	{
		[SerializeField] private ForgeController forge;
		[SerializeField] private OrderSystem orders;
		[SerializeField] private EconomySystem economy;

		[Header("UI Events")]
		public UnityEvent<string> onStageChangedText;
		public UnityEvent<float> onStageProgress;
		public UnityEvent<string> onOrderAcceptedText;
		public UnityEvent<string> onOrderCompletedText;
		public UnityEvent<float> onMoneyChanged;

		private void OnEnable()
		{
			if (forge != null)
			{
				forge.OnStageChanged += s => onStageChangedText?.Invoke($"Stage: {s}");
				forge.OnStageProgress += p => onStageProgress?.Invoke(p);
				forge.OnForgingCompleted += q => onOrderCompletedText?.Invoke($"Quality {Mathf.RoundToInt(q*100)}% done");
				forge.OnForgingFailed += r => onOrderCompletedText?.Invoke($"Failed: {r}");
			}
			if (orders != null)
			{
				orders.OnOrderAccepted += o => onOrderAcceptedText?.Invoke($"Order: {o.customerName} wants {o.blueprint.displayName}");
				orders.OnOrderCompleted += r => onOrderCompletedText?.Invoke($"Sold for {r.Payout:0} ({Mathf.RoundToInt(r.Quality*100)}%)");
			}
			if (economy != null)
			{
				economy.OnMoneyChanged += m => onMoneyChanged?.Invoke(m);
				onMoneyChanged?.Invoke(economy.Money);
			}
		}
	}
}