using System;
using System.Collections;
using UnityEngine;
using Blacksmith.Systems.Blueprints;
using Blacksmith.Player.Input;

namespace Blacksmith.Gameplay.Forge.Stages
{
	public class HeatingStage : MonoBehaviour, IForgeStage
	{
		[SerializeField] private PlayerInputHandler input;
		[SerializeField] private float heatRatePerSec = 150f;
		[SerializeField] private float coolRatePerSec = 50f;
		[SerializeField] private float uniformityDrift = 0.01f;

		private float currentTemp;
		private float uniformity01 = 1f;
		private bool isRunning;
		private bool heatingHeld;
		private Coroutine routine;
		private Action<float> onProgress;
		private Action<ForgeStageResult> onComplete;
		private ItemBlueprint blueprint;

		public string StageName => "Heating";

		private void OnEnable()
		{
			if (input != null)
			{
				input.OnSecondaryActionPressed += Vent;
				input.OnPrimaryDown += HandlePrimaryDown;
				input.OnPrimaryUp += HandlePrimaryUp;
				input.OnConfirm += HandleConfirm;
			}
		}
		private void OnDisable()
		{
			if (input != null)
			{
				input.OnSecondaryActionPressed -= Vent;
				input.OnPrimaryDown -= HandlePrimaryDown;
				input.OnPrimaryUp -= HandlePrimaryUp;
				input.OnConfirm -= HandleConfirm;
			}
		}

		public void BeginStage(ItemBlueprint blueprint, float difficultyModifier, Action<float> onProgress, Action<ForgeStageResult> onComplete)
		{
			this.blueprint = blueprint;
			this.onProgress = onProgress;
			this.onComplete = onComplete;
			currentTemp = Mathf.Max(20f, blueprint.targetTemperatureC * 0.25f);
			uniformity01 = 0.8f;
			heatingHeld = false;
			isRunning = true;
			if (routine != null) StopCoroutine(routine);
			routine = StartCoroutine(Run());
		}

		public void CancelStage()
		{
			isRunning = false;
			if (routine != null) StopCoroutine(routine);
		}

		private IEnumerator Run()
		{
			var wait = new WaitForSeconds(0.1f);
			while (isRunning)
			{
				float rate = heatingHeld ? heatRatePerSec : -coolRatePerSec;
				currentTemp = Mathf.Max(20f, currentTemp + rate * 0.1f);
				uniformity01 = Mathf.Clamp01(uniformity01 - uniformityDrift * 0.1f + (heatingHeld ? 0.005f : -0.005f));

				float target = blueprint.targetTemperatureC;
				float tol = blueprint.temperatureTolerance;
				float inRange = Mathf.InverseLerp(target + tol, target - tol, Mathf.Abs(currentTemp - target));
				float progress = Mathf.Clamp01(inRange * 0.5f + uniformity01 * 0.5f);
				onProgress?.Invoke(progress);
				yield return wait;
			}
		}

		private void Vent()
		{
			currentTemp = Mathf.Max(20f, currentTemp - 20f);
		}

		private void HandlePrimaryDown() { heatingHeld = true; }
		private void HandlePrimaryUp() { heatingHeld = false; }
		private void HandleConfirm() { Finish(); }

		private void Finish()
		{
			if (!isRunning) return;
			float target = blueprint.targetTemperatureC;
			float tol = blueprint.temperatureTolerance;
			float inRange = Mathf.InverseLerp(target + tol, target - tol, Mathf.Abs(currentTemp - target));
			float score = Mathf.Clamp01(inRange * 0.5f + uniformity01 * 0.5f);
			isRunning = false;
			onComplete?.Invoke(new ForgeStageResult { Success = score > 0.2f, Score01 = score, Message = score > 0.2f ? "Heated" : "Underheated" });
		}
	}
}