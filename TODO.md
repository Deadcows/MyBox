* #### MustBeAssigned may work with ScriptableObjects in project? 
  * Allow to disable this feature? Measure performance
  
* #### Multiscene asset
  * To save/load opened loaded/active scenes in editor
  
* #### MonoBehaviourPool

* #### Highlight empty fields with MustBeAssignedAttribute in inspector
	
* #### AssetPresetPreprocessor is very slow on matching assets, profile & optimize
  * At least skip import if nothing changed?
  
* #### Node editor tools (ง ͠° ͟ل͜ ͡°)ง

* #### IPrepare interface
  * To add custom logic to Behaviours
  * Execute those on Playmode and game build
  * To cache/calculate heavy stuff
	
* #### MyGizmosHandler && MyOnGUIHandler
  * MyDebug.DrawText is working only in OnDrawGizmos :( 
  * I want to access OnDrawGizmos in non-MonoBehaviour scripts
  * MyOnGUI may be useful, for instance, in FPSCounter feature
  * MB with static access, with lazy initialization and HideAndDontSave?
    * EveryFrame subscription is heavy...
    * Push struct with IDraw and logic to draw with gizmos, remove pusded structs from MyGizmosHandler.OnDrawGizmos? Measure performance
    * Some way to draw every-frame Gizmos with system, that run only once per x seconds?
		
* #### AnimationCreator is pretty cool with simple Idle-Play or looping Idle animators. 
  * Is it possible to play animation without animator? 
    * https://github.com/Unity-Technologies/SimpleAnimation
	
* #### EmbeddedAnimationCreator to EmbeddedAssetCreator? 
  * It's cool to have, to pack related assets into one parent asset
  * Yeah, I know how parent assets, but how to unparent?
	
* #### MyBundleUtility is a mess. Might be useful
  * Tools to build bundles out of scenes (with multiscene solutions)
    * And handle bundles loading/unloading on scene load/unload?
		
* #### SingleScriptableObject. No more messy Create/ with CreateAssetMenuAttribute for settings SO's
  * Base class (attribute?) to ensure that there is at least one SO of this type
  * SelectFolder/Filename prompt will automatically appear on recompilation if no instances of SO found? 
    * Warning if there is more than one?
    * Static accessor? Or probably better to add Load<T> method?

* #### TemplatesCreator
  * Add a way to add templates as separate assets
  * There is no way to have MenuItems with runtime naming :(?
    * MenuItems is separate feature/wrapper for TemplateCreator?
    * WHOA! Figured it out. I may generate separate script with MenuItems in any selected by used folder. Find this script and get its path to regenerate if needed!
   
* #### Conditionally remove some features like extension/hotkeys with Conditional Compilation?