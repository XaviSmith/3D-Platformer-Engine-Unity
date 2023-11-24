3D Platformer made in Unity following SOLID principles and using C#, Probuilder and State Machines.

Extends from tutorial series here https://www.youtube.com/watch?v=--_CH5DYz0M 

Software Used: BFXR, Unity, Visual Studio

PACKAGES REQUIRED (All packages are free): Cartoon Remaster VFX (Free ver)
	  DOTween (Free ver)
	  Ultimate Platformer Pack - https://quaternius.itch.io/ultimate-platformer-pack
	  Probuilder
	  Text Mesh Pro


FLOW: 
SPAWNERS - See EntitySpawner.

All Input handled by InputReader using the new Unity Input System and Settings/PlayerInputActions.inputActions as the control scheme 
with things like the PlayerController and CameraManager using delegates to call function when certain inputs are pressed (E.g. input.Jump calls OnJump, input.EnableMouseControlCamera calls OnEnableMouseControlCamera)