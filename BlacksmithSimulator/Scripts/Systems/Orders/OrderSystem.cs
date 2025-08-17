using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blacksmith.Systems.Blueprints;
using Blacksmith.Systems.Economy;

namespace Blacksmith.Systems.Orders
{
	public struct OrderResult
	{
		public OrderInstance Order;
		public bool Success;
		public float Quality;
		public float Payout;
	}

	public class OrderSystem : MonoBehaviour
	{
		[SerializeField] private List<OrderDefinition> orderTemplates = new List<OrderDefinition>();
		[SerializeField] private EconomySystem economy;

		private Coroutine timerRoutine;
		private float remainingTime;
		public OrderInstance ActiveOrder { get; private set; }

		public event Action<OrderInstance> OnOrderAccepted;
		public event Action<OrderResult> OnOrderCompleted;
		public event Action<float> OnOrderTimeUpdated;
		public event Action OnOrderExpired;

		public bool HasActiveOrder => ActiveOrder != null && ActiveOrder.blueprint != null;

		public void AcceptRandomOrder()
		{
			if (HasActiveOrder) return;
			if (orderTemplates.Count == 0) return;
			var def = orderTemplates[UnityEngine.Random.Range(0, orderTemplates.Count)];
			SetActiveOrder(def.CreateInstance());
		}

		public void SetActiveOrder(OrderInstance instance)
		{
			ActiveOrder = instance;
			StartTimer(instance.timeLimitSeconds);
			OnOrderAccepted?.Invoke(instance);
		}

		public void CompleteActiveOrder(float quality)
		{
			if (!HasActiveOrder) return;
			StopTimer();
			var result = new OrderResult
			{
				Order = ActiveOrder,
				Quality = Mathf.Clamp01(quality),
				Success = quality >= ActiveOrder.requiredQuality,
				Payout = CalculatePayout(ActiveOrder, quality)
			};
			if (result.Payout > 0f) economy?.AddMoney(result.Payout);
			ActiveOrder = null;
			OnOrderCompleted?.Invoke(result);
		}

		public void ExpireActiveOrder()
		{
			if (!HasActiveOrder) return;
			StopTimer();
			ActiveOrder = null;
			OnOrderExpired?.Invoke();
		}

		private float CalculatePayout(OrderInstance order, float quality)
		{
			var bp = order.blueprint;
			float basePayout = bp.basePrice * Mathf.Lerp(0.25f, 1f + bp.qualityPriceMultiplier, Mathf.Clamp01(quality));
			float successMultiplier = quality >= order.requiredQuality ? 1f : 0.5f;
			return Mathf.Max(0f, basePayout * order.payoutMultiplier * successMultiplier);
		}

		private void StartTimer(float seconds)
		{
			StopTimer();
			remainingTime = seconds;
			timerRoutine = StartCoroutine(Timer());
		}

		private void StopTimer()
		{
			if (timerRoutine != null)
			{
				StopCoroutine(timerRoutine);
				timerRoutine = null;
			}
		}

		private IEnumerator Timer()
		{
			var wait = new WaitForSeconds(0.2f);
			while (remainingTime > 0f)
			{
				remainingTime -= 0.2f;
				OnOrderTimeUpdated?.Invoke(Mathf.Max(0f, remainingTime));
				yield return wait;
			}
			ExpireActiveOrder();
		}
	}
}