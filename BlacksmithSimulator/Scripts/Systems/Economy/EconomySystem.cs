using System;
using UnityEngine;

namespace Blacksmith.Systems.Economy
{
	public class EconomySystem : MonoBehaviour
	{
		[SerializeField] private float startingMoney = 100f;
		public float Money { get; private set; }
		public event Action<float> OnMoneyChanged;

		private void Awake()
		{
			Money = startingMoney;
		}

		public bool CanAfford(float amount) => Money >= amount;

		public void AddMoney(float amount)
		{
			Money += Mathf.Max(0f, amount);
			OnMoneyChanged?.Invoke(Money);
		}

		public bool SpendMoney(float amount)
		{
			if (amount < 0f) return false;
			if (Money < amount) return false;
			Money -= amount;
			OnMoneyChanged?.Invoke(Money);
			return true;
		}

		public void SetMoney(float value)
		{
			Money = Mathf.Max(0f, value);
			OnMoneyChanged?.Invoke(Money);
		}
	}
}