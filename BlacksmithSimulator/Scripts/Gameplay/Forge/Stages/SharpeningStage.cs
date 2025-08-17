using System;
using System.Collections;
using UnityEngine;
using Blacksmith.Systems.Blueprints;
using Blacksmith.Player.Input;

namespace Blacksmith.Gameplay.Forge.Stages
{
	public class SharpeningStage : MonoBehaviour, IForgeStage
	{
		[SerializeField] private PlayerInputHandler input;
		[SerializeField] private float targetAngle = 20f;
		[SerializeField] private float angleTolerance = 8f;
		[SerializeField] private float targetCadence = 0.4f;
		[SerializeField] private float cadenceTolerance = 0.2f;
		[SerializeField] private Transform bladeTransform;

		private bool isRunning;
		private float lastGrindTime;
		private float accum;
		private int taps;
		private Action<float> onProgress;
		private Action<ForgeStageResult> onComplete;

		public string StageName => "Sharpening";

		private void OnEnable()
		{
			if (input != null) input.OnPrimaryActionPerformed += OnGrind;
		}
		private void OnDisable()
		{
			if (input != null) input.OnPrimaryActionPerformed -= OnGrind;
		}

		public void BeginStage(ItemBlueprint blueprint, float difficultyModifier, Action<float> onProgress, Action<ForgeStageResult> onComplete)
		{
			this.onProgress = onProgress;
			this.onComplete = onComplete;
			isRunning = true;
			lastGrindTime = Time.time;
			accum = 0f;
			taps = 0;
		}

		public void CancelStage()
		{
			isRunning = false;
		}

		private void OnGrind(float _)
		{
			if (!isRunning) return;
			float angle = bladeTransform != null ? Mathf.Abs(bladeTransform.localEulerAngles.z) : targetAngle;
			angle = angle > 180f ? 360f - angle : angle;
			float angleScore = 1f - Mathf.Clamp01(Mathf.Abs(angle - targetAngle) / angleTolerance);

			float now = Time.time;
			float cadence = now - lastGrindTime;
			lastGrindTime = now;
			float cadenceScore = 1f - Mathf.Clamp01(Mathf.Abs(cadence - targetCadence) / cadenceTolerance);

			float hitScore = Mathf.Clamp01(angleScore * 0.6f + cadenceScore * 0.4f);
			accum += hitScore;
			taps++;
			onProgress?.Invoke(Mathf.Clamp01(taps / 8f));
			if (taps >= 8)
			{
				float final = Mathf.Clamp01(accum / taps);
				isRunning = false;
				onComplete?.Invoke(new ForgeStageResult { Success = final > 0.2f, Score01 = final, Message = "Sharpened" });
			}
		}
	}
}