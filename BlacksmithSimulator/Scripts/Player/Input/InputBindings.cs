using UnityEngine;

namespace Blacksmith.Player.Input
{
	[CreateAssetMenu(menuName = "Blacksmith/Input Bindings", fileName = "InputBindings")]
	public class InputBindings : ScriptableObject
	{
		[Header("Keys")]
		public KeyCode interact = KeyCode.E;
		public KeyCode primaryAction = KeyCode.Mouse0;
		public KeyCode secondaryAction = KeyCode.Mouse1;
		public KeyCode confirm = KeyCode.Space;
		public KeyCode cancel = KeyCode.Escape;
		[Header("Charge Timing (sec)")]
		public float minChargeSeconds = 0.1f;
		public float maxChargeSeconds = 1.5f;
	}
}