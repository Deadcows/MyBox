# MyBox
MyBox is a set of tools, features and extensions for Unity.<br />
It is MyBox. Now it's yours too :raised_hands:<br />

--------

You may install MyBox by simply putting MyBox folder somewhere under your Assets folder.<br />
Or via <b>Unity Package Manager</b>:<br />
Open Packages/manifest.json file in your project folder and put this line along with other dependencies<br />
<pre>"com.mybox": "https://github.com/Deadcows/MyBox.git"</pre>
Note how the comma lies at the end of every line in dependencies, except of the last line!

To update, use Tools->MyBox->Update window<br />
MyBox will notify on new versions 🤗

--------

Tons of images below :point_down:

## [Attributes](https://github.com/Deadcows/MyBox/wiki/Attributes)
**[AutoProperty](https://github.com/Deadcows/MyBox/wiki/Attributes#autoproperty)** — Assign fields automatically<br />
**[ButtonMethod](https://github.com/Deadcows/MyBox/wiki/Attributes#buttonmethod)** — Display button in inspector<br />
**[ConditionalField](https://github.com/Deadcows/MyBox/wiki/Attributes#conditionalfield)** — Conditionally display property in inspector, based on some other property value<br />
**[DefinedValues](https://github.com/Deadcows/MyBox/wiki/Attributes#definedvalues)** — Display Dropdown with predefined values<br />
**[DisplayInspector](https://github.com/Deadcows/MyBox/wiki/Attributes#displayinspector)** — Display one inspector inside of another<br />
**[Tag, Layer, SpriteLayer](https://github.com/Deadcows/MyBox/wiki/Attributes#tag-layer-spritelayer)** — Dropdown with Tags, Layers or SpriteLayers<br />
**[MinMaxRange, RangedFloat and RangedInt](https://github.com/Deadcows/MyBox/wiki/Attributes#minmaxrange-rangedfloat-and-rangedint)** — Ranged sliders<br />
**[MustBeAssigned](https://github.com/Deadcows/MyBox/wiki/Attributes#mustbeassigned)** — Automatically checks if field is assigned (not null / not empty / not default value) on Playmode<br />
**[PositiveValueOnly](https://github.com/Deadcows/MyBox/wiki/Attributes#positivevalueonly)** — Prohibit values below zero<br />
**[ReadOnly](https://github.com/Deadcows/MyBox/wiki/Attributes#readonly)** — Draw property with disabled GUI<br />
**[RequireTag and RequireLayer](https://github.com/Deadcows/MyBox/wiki/Attributes#requiretag-and-requirelayer)** — Automatically set Tag and Layer<br />
**[SearchableEnum](https://github.com/Deadcows/MyBox/wiki/Attributes#searchableenum)** — Nice UI for enums with lots of elements<br />
**[Separator](https://github.com/Deadcows/MyBox/wiki/Attributes#separator)** — Draw separator with or without title<br />

--------

## [Tools and Features](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features)
**[AssetPressetPreprocessor](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#assetpressetpreprocessor)** — Conditionally apply Presets to your assets on import<br />
**[TimeTest](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#timetest)** — Measure performance with simple api<br />
**[IPrepare](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#iprepare)** — Easy way to replace caching, calculations and asserts from playmode<br />
**[Features](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#cleanup-empty-directories-and-autosave-features-and-some-hotkeys)** — Cleanup Empty Directories, AutoSave feature, Hotkeys<br />

--------

## [Types](https://github.com/Deadcows/MyBox/wiki/Types)
**[Guid Component](https://github.com/Deadcows/MyBox/wiki/Types#guidcomponent)** — Generate unique and persistent IDs<br />
**[SceneReference Component](https://github.com/Deadcows/MyBox/wiki/Types#scenereference)** — Reference scene with Scene asset in inspector<br />
**[ActivateOnStart Component](https://github.com/Deadcows/MyBox/wiki/Types#activestateonstart-component)** — Set state of specific GO on game start<br />
**[AnimationStateReference](https://github.com/Deadcows/MyBox/wiki/Types#animationstatereference)** — Specify AnimationClip on object with Animator<br />
**[AssetPath and AssetFolderPath](https://github.com/Deadcows/MyBox/wiki/Types#assetpath-and-assetfolderpath)** — Inspector button to browse for folder or asset under Assets folder<br />
**[Billboard Component](https://github.com/Deadcows/MyBox/wiki/Types#billboard-component)** — Force object to always face camera<br />
**[ColliderGizmo Component](https://github.com/Deadcows/MyBox/wiki/Types#collidergizmo-component)** — Highlight colliders and triggers in SceneView<br />
**[FPSCounter Component](https://github.com/Deadcows/MyBox/wiki/Types#fpscounter)** — Display FPS counter on Playmode<br />
**[MyDictionary](https://github.com/Deadcows/MyBox/wiki/Types#mydictionary)** — Serializable Dictionary<br />
**[MinMaxInt and MinMaxFloat](https://github.com/Deadcows/MyBox/wiki/Types#minmaxint-and-minmaxfloat)** — Asserts that Max => Min with handy inspector drawer<br />
**[Optional and OptionalMinMax](https://github.com/Deadcows/MyBox/wiki/Types#optional-and-optionalminmax)** — Optionally assignable values<br />

--------

//TODO: Extensions, Unfinished tools
