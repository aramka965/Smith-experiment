#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Blacksmith.Player.Input;
using Blacksmith.Player.Movement;
using Blacksmith.Player.Interaction;
using Blacksmith.Gameplay.Forge;
using Blacksmith.Gameplay.Forge.Stages;
using Blacksmith.Systems.Economy;
using Blacksmith.Systems.Orders;
using Blacksmith.Systems.Upgrades;
using Blacksmith.Systems.Save;
using Blacksmith.UI.HUD;

namespace Blacksmith.EditorTools
{
	public static class SceneAutoBuilder
	{
		[MenuItem("Blacksmith/Create Prototype Scene")] 
		public static void CreatePrototypeScene()
		{
			var sceneGO = new GameObject("_Scene");

			// Camera & Player
			var player = new GameObject("Player");
			var controller = player.AddComponent<CharacterController>();
			controller.height = 1.8f;
			controller.center = new Vector3(0, 0.9f, 0);
			player.transform.position = new Vector3(0, 0, -4);

			var camPivot = new GameObject("CameraPivot").transform;
			camPivot.SetParent(player.transform);
			camPivot.localPosition = new Vector3(0, 1.6f, 0);
			var cam = new GameObject("Main Camera").AddComponent<Camera>();
			cam.tag = "MainCamera";
			cam.transform.SetParent(camPivot);
			cam.transform.localPosition = Vector3.zero;
			cam.transform.localRotation = Quaternion.identity;

			var bindings = ScriptableObject.CreateInstance<InputBindings>();
			AssetDatabase.CreateAsset(bindings, "Assets/InputBindings.asset");

			var input = player.AddComponent<PlayerInputHandler>();
			typeof(PlayerInputHandler).GetField("bindings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(input, bindings);

			var fpc = player.AddComponent<FirstPersonController>();
			typeof(FirstPersonController).GetField("cameraPivot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(fpc, camPivot);
			typeof(FirstPersonController).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(fpc, input);

			var interactor = player.AddComponent<PlayerInteractor>();
			typeof(PlayerInteractor).GetField("playerCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(interactor, cam);
			typeof(PlayerInteractor).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(interactor, input);

			// Ground/forge area
			GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
			ground.name = "Ground";
			ground.transform.position = Vector3.zero;

			// Systems
			var systems = new GameObject("Systems");
			var econ = systems.AddComponent<EconomySystem>();
			var upgrades = systems.AddComponent<UpgradeManager>();
			typeof(UpgradeManager).GetField("economy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(upgrades, econ);
			var orders = systems.AddComponent<OrderSystem>();
			typeof(OrderSystem).GetField("economy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(orders, econ);
			systems.AddComponent<SaveManager>();
			var saver = systems.GetComponent<SaveManager>();
			typeof(SaveManager).GetField("economy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(saver, econ);
			typeof(SaveManager).GetField("upgrades", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(saver, upgrades);

			// Simple OrderDefinition asset
			var bp = ScriptableObject.CreateInstance<Systems.Blueprints.ItemBlueprint>();
			bp.displayName = "Practice Sword";
			bp.basePrice = 100;
			AssetDatabase.CreateAsset(bp, "Assets/PracticeSword.asset");
			var orderDef = ScriptableObject.CreateInstance<OrderDefinition>();
			orderDef.customerName = "Knight";
			orderDef.blueprint = bp;
			AssetDatabase.CreateAsset(orderDef, "Assets/PracticeOrder.asset");
			var listField = typeof(OrderSystem).GetField("orderTemplates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var list = (System.Collections.Generic.List<OrderDefinition>)listField.GetValue(orders);
			list.Add(orderDef);

			// Forge
			var forge = new GameObject("Forge");
			var fsm = forge.AddComponent<ForgingStateMachine>();
			var heating = forge.AddComponent<HeatingStage>();
			var hammering = forge.AddComponent<HammeringStage>();
			var quench = forge.AddComponent<QuenchingStage>();
			var sharpen = forge.AddComponent<SharpeningStage>();
			typeof(HeatingStage).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(heating, input);
			typeof(HammeringStage).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hammering, input);
			typeof(QuenchingStage).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(quench, input);
			typeof(SharpeningStage).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(sharpen, input);
			typeof(ForgingStateMachine).GetField("heatingStageBehaviour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(fsm, heating);
			typeof(ForgingStateMachine).GetField("hammeringStageBehaviour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(fsm, hammering);
			typeof(ForgingStateMachine).GetField("quenchingStageBehaviour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(fsm, quench);
			typeof(ForgingStateMachine).GetField("sharpeningStageBehaviour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(fsm, sharpen);
			var forgeController = forge.AddComponent<ForgeController>();
			typeof(ForgeController).GetField("stateMachine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(forgeController, fsm);
			var coordinator = forge.AddComponent<ForgeGameCoordinator>();
			typeof(ForgeGameCoordinator).GetField("orders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(coordinator, orders);
			typeof(ForgeGameCoordinator).GetField("forge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(coordinator, forgeController);

			// Simple UI Canvas
			var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			var canvas = canvasGO.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			var hud = canvasGO.AddComponent<HUDController>();
			typeof(HUDController).GetField("forge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, forgeController);
			typeof(HUDController).GetField("orders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, orders);
			typeof(HUDController).GetField("economy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, econ);
			var view = canvasGO.AddComponent<HUDSimpleView>();

			// Stage text
			var stageTextGO = new GameObject("StageText");
			stageTextGO.transform.SetParent(canvasGO.transform);
			var stageText = stageTextGO.AddComponent<Text>();
			stageText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			stageText.alignment = TextAnchor.UpperLeft;
			stageText.rectTransform.anchorMin = new Vector2(0, 1);
			stageText.rectTransform.anchorMax = new Vector2(0, 1);
			stageText.rectTransform.pivot = new Vector2(0, 1);
			stageText.rectTransform.anchoredPosition = new Vector2(10, -10);
			typeof(HUDSimpleView).GetField("stageText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(view, stageText);

			// Order text
			var orderTextGO = new GameObject("OrderText");
			orderTextGO.transform.SetParent(canvasGO.transform);
			var orderText = orderTextGO.AddComponent<Text>();
			orderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			orderText.alignment = TextAnchor.UpperLeft;
			orderText.rectTransform.anchorMin = new Vector2(0, 1);
			orderText.rectTransform.anchorMax = new Vector2(0, 1);
			orderText.rectTransform.pivot = new Vector2(0, 1);
			orderText.rectTransform.anchoredPosition = new Vector2(10, -40);
			typeof(HUDSimpleView).GetField("orderText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(view, orderText);

			// Money text
			var moneyTextGO = new GameObject("MoneyText");
			moneyTextGO.transform.SetParent(canvasGO.transform);
			var moneyText = moneyTextGO.AddComponent<Text>();
			moneyText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			moneyText.alignment = TextAnchor.UpperRight;
			moneyText.rectTransform.anchorMin = new Vector2(1, 1);
			moneyText.rectTransform.anchorMax = new Vector2(1, 1);
			moneyText.rectTransform.pivot = new Vector2(1, 1);
			moneyText.rectTransform.anchoredPosition = new Vector2(-10, -10);
			typeof(HUDSimpleView).GetField("moneyText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(view, moneyText);

			// Progress slider
			var sliderGO = new GameObject("StageProgress", typeof(Slider), typeof(Image));
			sliderGO.transform.SetParent(canvasGO.transform);
			sliderGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
			sliderGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
			sliderGO.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
			sliderGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
			var slider = sliderGO.GetComponent<Slider>();
			typeof(HUDSimpleView).GetField("stageProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(view, slider);

			// Wire HUDController UnityEvents to view
			var onStageChangedText = typeof(HUDController).GetField("onStageChangedText", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(hud) as UnityEngine.Events.UnityEvent<string>;
			onStageChangedText.AddListener(view.SetStageText);
			var onStageProgress = typeof(HUDController).GetField("onStageProgress", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(hud) as UnityEngine.Events.UnityEvent<float>;
			onStageProgress.AddListener(view.SetStageProgress);
			var onOrderAcceptedText = typeof(HUDController).GetField("onOrderAcceptedText", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(hud) as UnityEngine.Events.UnityEvent<string>;
			onOrderAcceptedText.AddListener(view.SetOrderText);
			var onOrderCompletedText = typeof(HUDController).GetField("onOrderCompletedText", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(hud) as UnityEngine.Events.UnityEvent<string>;
			onOrderCompletedText.AddListener(view.SetOrderText);
			var onMoneyChanged = typeof(HUDController).GetField("onMoneyChanged", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(hud) as UnityEngine.Events.UnityEvent<float>;
			onMoneyChanged.AddListener(view.SetMoney);

			// Quick interactables: OrderDesk and Anvil as simple cubes within raycast range
			var orderDesk = GameObject.CreatePrimitive(PrimitiveType.Cube);
			orderDesk.name = "OrderDesk";
			orderDesk.transform.position = new Vector3(-2, 0.5f, 0);
			orderDesk.AddComponent<OrderDeskInteractable>().Init(orders, coordinator);

			var anvil = GameObject.CreatePrimitive(PrimitiveType.Cube);
			anvil.name = "Anvil";
			anvil.transform.position = new Vector3(2, 0.5f, 0);
			anvil.AddComponent<AnvilInteractable>().Init(coordinator);

			EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
	}

	public class OrderDeskInteractable : MonoBehaviour, IBlacksmithInteractable
	{
		private OrderSystem orders;
		private ForgeGameCoordinator coordinator;
		public void Init(OrderSystem o, ForgeGameCoordinator c) { orders = o; coordinator = c; }
		private void OnDrawGizmos() { Gizmos.color = Color.green; Gizmos.DrawWireCube(transform.position, Vector3.one); }
		public void OnInteract(PlayerInteractor interactor)
		{
			if (orders.HasActiveOrder) coordinator.BeginForgingActiveOrder();
			else coordinator.AcceptNextOrder();
		}
	}

	public class AnvilInteractable : MonoBehaviour, IBlacksmithInteractable
	{
		private ForgeGameCoordinator coordinator;
		public void Init(ForgeGameCoordinator c) { coordinator = c; }
		private void OnDrawGizmos() { Gizmos.color = Color.cyan; Gizmos.DrawWireCube(transform.position, Vector3.one); }
		public void OnInteract(PlayerInteractor interactor)
		{
			coordinator.BeginForgingActiveOrder();
		}
	}

	public interface IBlacksmithInteractable : Blacksmith.Player.Interaction.IInteractable {}
}
#endif