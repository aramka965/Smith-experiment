using System;
using System.Collections.Generic;
using UnityEngine;
using Blacksmith.Systems.Orders;
using Blacksmith.Systems.Blueprints;
using Blacksmith.Gameplay.Forge.Stages;

namespace Blacksmith.Gameplay.Forge
{
	public enum ForgeSequenceStage { Idle, Heating, Hammering, Quenching, Sharpening, Completed, Failed }

	public class ForgingStateMachine : MonoBehaviour
	{
		[SerializeField] private MonoBehaviour heatingStageBehaviour;
		[SerializeField] private MonoBehaviour hammeringStageBehaviour;
		[SerializeField] private MonoBehaviour quenchingStageBehaviour;
		[SerializeField] private MonoBehaviour sharpeningStageBehaviour;

		private IForgeStage heating;
		private IForgeStage hammering;
		private IForgeStage quenching;
		private IForgeStage sharpening;

		private readonly List<float> stageScores = new List<float>(4);
		private ItemBlueprint currentBlueprint;
		private OrderInstance currentOrder;

		public ForgeSequenceStage CurrentStage { get; private set; } = ForgeSequenceStage.Idle;
		public event Action<ForgeSequenceStage> OnStageChanged;
		public event Action<float> OnStageProgress; // 0..1 per stage
		public event Action<float> OnAllStagesCompleted; // quality 0..1
		public event Action<string> OnFailed;

		private void Awake()
		{
			heating = heatingStageBehaviour as IForgeStage;
			hammering = hammeringStageBehaviour as IForgeStage;
			quenching = quenchingStageBehaviour as IForgeStage;
			sharpening = sharpeningStageBehaviour as IForgeStage;
		}

		public void StartForging(OrderInstance order)
		{
			currentOrder = order;
			currentBlueprint = order?.blueprint;
			stageScores.Clear();
			if (currentBlueprint == null)
			{
				Fail("No blueprint provided");
				return;
			}
			RunHeating();
		}

		public void Cancel()
		{
			heating?.CancelStage();
			hammering?.CancelStage();
			quenching?.CancelStage();
			sharpening?.CancelStage();
			CurrentStage = ForgeSequenceStage.Idle;
			OnStageChanged?.Invoke(CurrentStage);
		}

		private void RunHeating()
		{
			CurrentStage = ForgeSequenceStage.Heating;
			OnStageChanged?.Invoke(CurrentStage);
			heating?.BeginStage(currentBlueprint, 1f, p => OnStageProgress?.Invoke(p), r => HandleStageComplete(r, RunHammering));
		}

		private void RunHammering()
		{
			CurrentStage = ForgeSequenceStage.Hammering;
			OnStageChanged?.Invoke(CurrentStage);
			hammering?.BeginStage(currentBlueprint, currentBlueprint.hammeringDifficulty, p => OnStageProgress?.Invoke(p), r => HandleStageComplete(r, RunQuenching));
		}

		private void RunQuenching()
		{
			CurrentStage = ForgeSequenceStage.Quenching;
			OnStageChanged?.Invoke(CurrentStage);
			quenching?.BeginStage(currentBlueprint, currentBlueprint.quenchingDifficulty, p => OnStageProgress?.Invoke(p), r => HandleStageComplete(r, RunSharpening));
		}

		private void RunSharpening()
		{
			CurrentStage = ForgeSequenceStage.Sharpening;
			OnStageChanged?.Invoke(CurrentStage);
			sharpening?.BeginStage(currentBlueprint, currentBlueprint.sharpeningDifficulty, p => OnStageProgress?.Invoke(p), r => HandleStageComplete(r, CompleteAll));
		}

		private void HandleStageComplete(ForgeStageResult result, Action next)
		{
			if (!result.Success)
			{
				Fail(result.Message);
				return;
			}
			stageScores.Add(Mathf.Clamp01(result.Score01));
			next?.Invoke();
		}

		private void CompleteAll()
		{
			CurrentStage = ForgeSequenceStage.Completed;
			OnStageChanged?.Invoke(CurrentStage);
			float quality = ComputeQuality(stageScores, currentBlueprint);
			OnAllStagesCompleted?.Invoke(quality);
		}

		private void Fail(string reason)
		{
			CurrentStage = ForgeSequenceStage.Failed;
			OnStageChanged?.Invoke(CurrentStage);
			OnFailed?.Invoke(reason);
		}

		private float ComputeQuality(List<float> scores, ItemBlueprint bp)
		{
			if (scores == null || scores.Count == 0) return 0f;
			float heatingWeight = 1f;
			float hammerWeight = 1f + Mathf.Max(0f, bp.hammeringDifficulty - 1f);
			float quenchWeight = 1f + Mathf.Max(0f, bp.quenchingDifficulty - 1f);
			float sharpWeight = 1f + Mathf.Max(0f, bp.sharpeningDifficulty - 1f);
			float[] weights = new float[] { heatingWeight, hammerWeight, quenchWeight, sharpWeight };
			float sum = 0f, wsum = 0f;
			for (int i = 0; i < scores.Count && i < 4; i++)
			{
				float w = weights[i];
				sum += scores[i] * w;
				wsum += w;
			}
			if (wsum <= 0f) return 0f;
			return Mathf.Clamp01(sum / wsum);
		}
	}
}