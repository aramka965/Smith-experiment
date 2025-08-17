using UnityEngine;
using UnityEngine.UI;

namespace Blacksmith.UI.HUD
{
	public class HUDSimpleView : MonoBehaviour
	{
		[SerializeField] private Text stageText;
		[SerializeField] private Text orderText;
		[SerializeField] private Text moneyText;
		[SerializeField] private Slider stageProgress;

		public void SetStageText(string value)
		{
			if (stageText != null) stageText.text = value;
		}

		public void SetOrderText(string value)
		{
			if (orderText != null) orderText.text = value;
		}

		public void SetMoney(float value)
		{
			if (moneyText != null) moneyText.text = "$" + Mathf.RoundToInt(value).ToString();
		}

		public void SetStageProgress(float value)
		{
			if (stageProgress != null) stageProgress.value = Mathf.Clamp01(value);
		}
	}
}