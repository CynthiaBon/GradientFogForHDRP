# GradientFogForHDRP
A tool that creates fog based on a gradient. Only works for Unity HDRP.

Create the gradient fog object with AmbianceTools->GradientFog.
Then create materials from editor by filling the "Gradient Name" field and by creating a gradient with Unity's gradient editor.
(You can't edit a saved gradient so use Unity's save system for gradients).
You can then change the gradient used for the fog either within the editor or in a script by calling the ChangeGradient(gradientName) method.
You can change the time it takes to go from a gradient to another at runtime with the "Fog Transition Time" field.

/!\ Lit Shader Mode must be set to Forward Only
