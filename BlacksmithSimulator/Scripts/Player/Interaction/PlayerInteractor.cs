using System;
using UnityEngine;
using Blacksmith.Player.Input;

namespace Blacksmith.Player.Interaction
{
	public class PlayerInteractor : MonoBehaviour
	{
		[SerializeField] private Camera playerCamera;
		[SerializeField] private float interactDistance = 3f;
		[SerializeField] private LayerMask interactMask = ~0;
		[SerializeField] private PlayerInputHandler input;

		private void OnEnable()
		{
			if (input != null) input.OnInteractPressed += TryInteract;
		}

		private void OnDisable()
		{
			if (input != null) input.OnInteractPressed -= TryInteract;
		}

		private void TryInteract()
		{
			if (playerCamera == null) return;
			Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
			if (Physics.Raycast(ray, out var hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
			{
				var interactable = hit.collider.GetComponentInParent<IInteractable>();
				if (interactable != null)
				{
					interactable.OnInteract(this);
				}
			}
		}
	}
}