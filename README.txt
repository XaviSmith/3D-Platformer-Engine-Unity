Customizable Engine for a 3D Platformer with fluid movement made in Unity following SOLID principles using C#, Probuilder and State Machines.

Features
* Highly customizable with everything from jump height to attack hitboxes editable in inspector
* Full movement system including slope movement, jumping, longjumping, dashing, wall jumping, jump buffers, coyote time and more
* Easy to use custom Event System that supports enums and passing parameters to all subscribers 
* Gizmos for visualizing Spherecasts, OverlapSpheres etc
* Optimized to run smoothly in browser

Software Used: BFXR, Unity, Visual Studio

NOTE: This repository is mainly to share the code, and as such will not compile without first installing the Unity Packages below.

PACKAGES REQUIRED (Every packages is free): 
* Cartoon Remaster VFX (Free ver)
* DOTween (Free ver)	  
* Probuilder
* Text Mesh Pro
* Flexible Cel Shader

REFERENCES
Camera Movement - https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
Baseline Movement/AI/State Machine from tutorial series https://www.youtube.com/watch?v=--_CH5DYz0M 

All Input handled by InputReader using the new Unity Input System and Settings/PlayerInputActions.inputActions as the control scheme 
with things like the PlayerController and CameraManager using delegates to call function when certain inputs are pressed (E.g. input.Jump calls OnJump, input.EnableMouseControlCamera calls OnEnableMouseControlCamera)
