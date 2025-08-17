using System;
using UnityEngine;

namespace Blacksmith.Player.Input
{
	public class PlayerInputHandler : MonoBehaviour
	{
		[SerializeField] private InputBindings bindings;
		[SerializeField] private bool dispatchLookAndMove = false;
		[SerializeField] private float lookSensitivity = 1f;

		private bool isCharging;
		private float chargeStartTime;

		public event Action OnInteractPressed;
		public event Action OnPrimaryDown;
		public event Action<float> OnPrimaryActionPerformed; // strength 0..1 on release
		public event Action OnPrimaryUp;
		public event Action OnSecondaryActionPressed;
		public event Action OnConfirm;
		public event Action OnCancel;
		public event Action<Vector2> OnLookDelta; // optional
		public event Action<Vector2> OnMoveAxis; // optional

		private void Update()
		{
			if (bindings == null) return;
			// Interact
			if (UnityEngine.Input.GetKeyDown(bindings.interact)) OnInteractPressed?.Invoke();

			// Primary action with charge
			if (UnityEngine.Input.GetKeyDown(bindings.primaryAction))
			{
				OnPrimaryDown?.Invoke();
				isCharging = true;
				chargeStartTime = Time.time;
			}
			if (UnityEngine.Input.GetKeyUp(bindings.primaryAction))
			{
				OnPrimaryUp?.Invoke();
				if (isCharging)
				{
					float held = Mathf.Clamp(Time.time - chargeStartTime, bindings.minChargeSeconds, bindings.maxChargeSeconds);
					float strength = Mathf.InverseLerp(bindings.minChargeSeconds, bindings.maxChargeSeconds, held);
					isCharging = false;
					OnPrimaryActionPerformed?.Invoke(strength);
				}
			}

			// Secondary
			if (UnityEngine.Input.GetKeyDown(bindings.secondaryAction)) OnSecondaryActionPressed?.Invoke();

			// Confirm / Cancel
			if (UnityEngine.Input.GetKeyDown(bindings.confirm)) OnConfirm?.Invoke();
			if (UnityEngine.Input.GetKeyDown(bindings.cancel)) OnCancel?.Invoke();

			if (!dispatchLookAndMove) return;
			float mx = UnityEngine.Input.GetAxis("Mouse X") * lookSensitivity;
			float my = UnityEngine.Input.GetAxis("Mouse Y") * lookSensitivity;
			if (Mathf.Abs(mx) > 0.0001f || Mathf.Abs(my) > 0.0001f) OnLookDelta?.Invoke(new Vector2(mx, my));
			float h = UnityEngine.Input.GetAxisRaw("Horizontal");
			float v = UnityEngine.Input.GetAxisRaw("Vertical");
			if (Mathf.Abs(h) > 0.0001f || Mathf.Abs(v) > 0.0001f) OnMoveAxis?.Invoke(new Vector2(h, v));
		}
	}
}