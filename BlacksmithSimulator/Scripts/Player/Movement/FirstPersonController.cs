using UnityEngine;
using Blacksmith.Player.Input;

namespace Blacksmith.Player.Movement
{
	[RequireComponent(typeof(CharacterController))]
	public class FirstPersonController : MonoBehaviour
	{
		[SerializeField] private Transform cameraPivot;
		[SerializeField] private float moveSpeed = 4f;
		[SerializeField] private float lookSensitivity = 2f;
		[SerializeField] private bool lockCursor = true;
		[SerializeField] private PlayerInputHandler input;

		private CharacterController controller;
		private float pitch;
		private Vector2 pendingMove;
		private bool useInputEvents;

		private void Awake()
		{
			controller = GetComponent<CharacterController>();
		}

		private void OnEnable()
		{
			if (lockCursor)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (input != null)
			{
				useInputEvents = true;
				input.OnMoveAxis += HandleMoveAxis;
				input.OnLookDelta += HandleLookDelta;
			}
		}

		private void OnDisable()
		{
			if (input != null)
			{
				input.OnMoveAxis -= HandleMoveAxis;
				input.OnLookDelta -= HandleLookDelta;
			}
		}

		private void Update()
		{
			Vector2 move;
			if (useInputEvents)
			{
				move = pendingMove;
			}
			else
			{
				move = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
				float mx = UnityEngine.Input.GetAxis("Mouse X") * lookSensitivity;
				float my = UnityEngine.Input.GetAxis("Mouse Y") * lookSensitivity;
				Look(mx, my);
			}
			Vector3 wish = transform.right * move.x + transform.forward * move.y;
			controller.SimpleMove(wish.normalized * moveSpeed);
		}

		private void HandleLookDelta(Vector2 delta)
		{
			Look(delta.x, delta.y);
		}

		private void HandleMoveAxis(Vector2 v)
		{
			pendingMove = v;
		}

		private void Look(float mx, float my)
		{
			transform.Rotate(0f, mx, 0f);
			pitch = Mathf.Clamp(pitch - my, -85f, 85f);
			if (cameraPivot != null)
			{
				cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
			}
		}
	}
}