using System;
using System.Collections;
using UnityEngine;
using Blacksmith.Systems.Blueprints;
using Blacksmith.Player.Input;

namespace Blacksmith.Gameplay.Forge.Stages
{
	public class HammeringStage : MonoBehaviour, IForgeStage
	{
		[SerializeField] private PlayerInputHandler input;
		[SerializeField] private int requiredHits = 6;
		[SerializeField] private float timingWindow = 0.25f;
		[SerializeField] private float targetInterval = 0.8f;

		private int hits;
		private float lastHitTime;
		private float scoreAccum;
		private bool isRunning;
		private Action<float> onProgress;
		private Action<ForgeStageResult> onComplete;

		public string StageName => "Hammering";

		private void OnEnable()
		{
			if (input != null) input.OnPrimaryActionPerformed += OnHit;
		}
		private void OnDisable()
		{
			if (input != null) input.OnPrimaryActionPerformed -= OnHit;
		}

		public void BeginStage(ItemBlueprint blueprint, float difficultyModifier, Action<float> onProgress, Action<ForgeStageResult> onComplete)
		{
			this.onProgress = onProgress;
			this.onComplete = onComplete;
			hits = 0;
			scoreAccum = 0f;
			lastHitTime = Time.time;
			isRunning = true;
		}

		public void CancelStage()
		{
			isRunning = false;
		}

		private void OnHit(float strength)
		{
			if (!isRunning) return;
			float now = Time.time;
			float interval = now - lastHitTime;
			lastHitTime = now;

			float timingScore = 1f - Mathf.Clamp01(Mathf.Abs(interval - targetInterval) / timingWindow);
			float hitScore = Mathf.Clamp01(strength * 0.7f + timingScore * 0.3f);
			scoreAccum += hitScore;
			hits++;
			onProgress?.Invoke(Mathf.Clamp01((float)hits / requiredHits));
			if (hits >= requiredHits)
			{
				float finalScore = Mathf.Clamp01(scoreAccum / requiredHits);
				isRunning = false;
				onComplete?.Invoke(new ForgeStageResult { Success = finalScore > 0.3f, Score01 = finalScore, Message = "Hammered" });
			}
		}
	}
}