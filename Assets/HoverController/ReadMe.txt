1) Import your model in project.
2) Drag'n'drop your model on scene
3) If you need use custom pivot for this model (create empty GameObject and attach your model to it and setup position and rotation)
4) Add RigidBody component (3D) and script HoverController
5) Setup RigidBody mass and drag (example mass - 1000, drag - 0.5, angular drag - 5
6) Setup HoverController variables (forces, raycast helpers, height from ground, ground layer, etc)

Example, setup HoverController script like this:
GoUpForce - 12500
ForwardForce - 20000
RotationTorque - 20000
HeightFromGround - 2
GroundedThreshold - 4
BlockAirControl - false

To setup raycast helpers you need to create few empty GameObjects and attach to your model or custom pivot.
These helpers must be rotated so that their local up direction must be directed to model (or pivot) up direction.