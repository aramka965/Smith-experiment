using System;
using Blacksmith.Systems.Blueprints;

namespace Blacksmith.Gameplay.Forge.Stages
{
	public interface IForgeStage
	{
		string StageName { get; }
		void BeginStage(ItemBlueprint blueprint, float difficultyModifier, Action<float> onProgress, Action<ForgeStageResult> onComplete);
		void CancelStage();
	}

	public struct ForgeStageResult
	{
		public bool Success;
		public float Score01;
		public string Message;
	}
}