using System;
using System.Collections;
using UnityEngine;
using Blacksmith.Systems.Blueprints;
using Blacksmith.Player.Input;

namespace Blacksmith.Gameplay.Forge.Stages
{
	public enum QuenchMedium { Water, Oil }

	public class QuenchingStage : MonoBehaviour, IForgeStage
	{
		[SerializeField] private PlayerInputHandler input;
		[SerializeField] private float idealWaterTime = 1.0f;
		[SerializeField] private float idealOilTime = 1.5f;
		[SerializeField] private float tolerance = 0.5f;

		private bool isRunning;
		private float startTime;
		private bool isImmersed;
		private QuenchMedium medium = QuenchMedium.Water;
		private Action<float> onProgress;
		private Action<ForgeStageResult> onComplete;

		public string StageName => "Quenching";

		private void OnEnable()
		{
			if (input != null)
			{
				input.OnPrimaryActionPerformed += HandlePrimaryPerformed;
				input.OnSecondaryActionPressed += SwitchMedium;
			}
		}
		private void OnDisable()
		{
			if (input != null)
			{
				input.OnPrimaryActionPerformed -= HandlePrimaryPerformed;
				input.OnSecondaryActionPressed -= SwitchMedium;
			}
		}

		public void BeginStage(ItemBlueprint blueprint, float difficultyModifier, Action<float> onProgress, Action<ForgeStageResult> onComplete)
		{
			this.onProgress = onProgress;
			this.onComplete = onComplete;
			isRunning = true;
			isImmersed = false;
			startTime = 0f;
			medium = QuenchMedium.Water;
		}

		public void CancelStage()
		{
			isRunning = false;
		}

		private void SwitchMedium()
		{
			if (!isRunning) return;
			medium = medium == QuenchMedium.Water ? QuenchMedium.Oil : QuenchMedium.Water;
		}

		private void HandlePrimaryPerformed(float _)
		{
			ToggleImmerse();
		}

		private void ToggleImmerse()
		{
			if (!isRunning) return;
			if (!isImmersed)
			{
				isImmersed = true;
				startTime = Time.time;
			}
			else
			{
				isImmersed = false;
				float duration = Time.time - startTime;
				float ideal = medium == QuenchMedium.Water ? idealWaterTime : idealOilTime;
				float score = 1f - Mathf.Clamp01(Mathf.Abs(duration - ideal) / tolerance);
				isRunning = false;
				onComplete?.Invoke(new ForgeStageResult { Success = score > 0.2f, Score01 = score, Message = "Quenched" });
			}
		}
	}
}