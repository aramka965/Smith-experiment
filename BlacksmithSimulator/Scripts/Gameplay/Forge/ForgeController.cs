using System;
using UnityEngine;
using Blacksmith.Systems.Orders;

namespace Blacksmith.Gameplay.Forge
{
	public class ForgeController : MonoBehaviour
	{
		[SerializeField] private ForgingStateMachine stateMachine;

		public event Action<float> OnForgingCompleted; // quality 0..1
		public event Action<string> OnForgingFailed;
		public event Action<ForgeSequenceStage> OnStageChanged;
		public event Action<float> OnStageProgress;

		private void Awake()
		{
			if (stateMachine != null)
			{
				stateMachine.OnAllStagesCompleted += HandleComplete;
				stateMachine.OnFailed += HandleFailed;
				stateMachine.OnStageChanged += s => OnStageChanged?.Invoke(s);
				stateMachine.OnStageProgress += p => OnStageProgress?.Invoke(p);
			}
		}

		public void BeginForging(OrderInstance order)
		{
			stateMachine?.StartForging(order);
		}

		public void CancelForging()
		{
			stateMachine?.Cancel();
		}

		private void HandleComplete(float quality)
		{
			OnForgingCompleted?.Invoke(quality);
		}

		private void HandleFailed(string reason)
		{
			OnForgingFailed?.Invoke(reason);
		}
	}
}