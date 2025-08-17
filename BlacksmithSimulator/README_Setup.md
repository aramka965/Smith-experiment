# Blacksmith Simulator (Prototype)

## Folder Structure
- Scripts/Gameplay/Forge: state machine, controller, stages
- Scripts/Player: input, movement, interaction
- Scripts/Systems: economy, orders, upgrades, pooling, save
- Scripts/UI: HUD hooks

## Core Loop
1) Press a button in UI (or call from inspector) to AcceptNextOrder
2) BeginForgingActiveOrder to start mini-game sequence
3) Complete triggers payout via OrderSystem

## Scene Setup (Prototype)
- Create an empty `Systems` GameObject and add:
  - EconomySystem
  - UpgradeManager (link EconomySystem)
  - OrderSystem (add some OrderDefinition assets; link EconomySystem)
  - SaveManager (link EconomySystem, UpgradeManager)
- Create an empty `Forge` GameObject and add:
  - ForgingStateMachine
    - Assign HeatingStage, HammeringStage, QuenchingStage, SharpeningStage component refs
  - ForgeController (link the state machine)
  - ForgeGameCoordinator (link OrderSystem and ForgeController)
- Player
  - Character with CharacterController and FirstPersonController
  - Camera as child, link to `cameraPivot`
  - Add PlayerInputHandler
  - Add PlayerInteractor (link camera and PlayerInputHandler)
- UI
  - HUDController: link ForgeController, OrderSystem, EconomySystem, and bind UnityEvents to Text/Slider

## Data Assets
- Create `ItemBlueprint` assets for items (set basePrice, target temp, difficulties)
- Create `OrderDefinition` assets referencing blueprints (requiredQuality, time, payoutMultiplier)
- Create `UpgradeDefinition` assets with ids and costs
- Create `InputBindings` asset and assign to PlayerInputHandler

## Controls (Default)
- Interact: E
- Hold LMB to heat (Heating). Press Space to confirm stage.
- Hammering: press/hold LMB to hit (charge strength by hold time)
- Quenching: LMB to start/stop immersion. RMB to toggle Water/Oil
- Sharpening: LMB taps at cadence; optionally rotate blade child Z to simulate angle
- Cancel: Esc (not wired to abort yet in prototype)

## Extension Points
- Replace stage implementations with more realistic interactions
- Add particle effects via ObjectPool
- Add real UI instead of UnityEvents-to-Text
- Persist current order and more player state in SaveManager