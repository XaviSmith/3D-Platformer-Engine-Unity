3D Platformer with fluid movement made in Unity following SOLID principles using C#, Probuilder and State Machines.

Features
-Highly customizable with everything from jump height to attack hitboxes editable in inspector
-Full movement system including slope movement, jumping, longjumping, dashing, wall jumping (TODO) and more
-Easy to use custom Event System that supports enums and passing parameters to all subscribers 
-Gizmos for visualizing Spherecasts, OverlapSpheres etc
-Optimized to run smoothly in browser

Software Used: BFXR, Unity, Visual Studio

PACKAGES REQUIRED (All packages are free): Cartoon Remaster VFX (Free ver)
	  DOTween (Free ver)
	  Ultimate Platformer Pack - https://quaternius.itch.io/ultimate-platformer-pack
	  Probuilder
	  Text Mesh Pro

FLOW: 
SPAWNERS - See EntitySpawner.

Base code started from tutorial series here https://www.youtube.com/watch?v=--_CH5DYz0M 

All Input handled by InputReader using the new Unity Input System and Settings/PlayerInputActions.inputActions as the control scheme 
with things like the PlayerController and CameraManager using delegates to call function when certain inputs are pressed (E.g. input.Jump calls OnJump, input.EnableMouseControlCamera calls OnEnableMouseControlCamera)