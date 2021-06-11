# MyBox

[![openupm](https://img.shields.io/npm/v/com.domybest.mybox?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.domybest.mybox/)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/Deadcows/MyBox/blob/master/LICENSE.md)
<a href="https://www.buymeacoffee.com/andrewrumak" target="_blank"><img src="https://user-images.githubusercontent.com/969858/114297171-ca615080-9ab7-11eb-8010-0ae8762ff1f1.png" /></a>




MyBox is a set of tools, features and extensions for Unity.<br />
It is MyBox. Now it's yours too :raised_hands:<br />


## [Installation](https://github.com/Deadcows/MyBox/wiki/Installation)


Tons of images in docs below :point_down:

## [Attributes](https://github.com/Deadcows/MyBox/wiki/Attributes)
**[AutoProperty](https://github.com/Deadcows/MyBox/wiki/Attributes#autoproperty)** — Assign fields automatically<br />
**[ButtonMethod](https://github.com/Deadcows/MyBox/wiki/Attributes#buttonmethod)** — Display button in inspector<br />
**[CharactersRange](https://github.com/Deadcows/MyBox/wiki/Attributes#charactersrange)** — Filter string field by the set of characters<br/>
**[ConditionalField](https://github.com/Deadcows/MyBox/wiki/Attributes#conditionalfield)** — Conditionally display property in inspector, based on some other property value<br />
**[ConstantsSelection](https://github.com/Deadcows/MyBox/wiki/Attributes#constantsselection)** — Popup of const, readonly or static fields and properties<br />
**[DefinedValues](https://github.com/Deadcows/MyBox/wiki/Attributes#definedvalues)** — Display Dropdown with predefined values<br />
**[DisplayInspector](https://github.com/Deadcows/MyBox/wiki/Attributes#displayinspector)** — Display one inspector inside of another<br />
**[Foldout](https://github.com/Deadcows/MyBox/wiki/Attributes#foldout)** — Group your fields in inspector<br />
**[InitializationField](https://github.com/Deadcows/MyBox/wiki/Attributes#initializationfield)** — Field that is not editable in playmode<br />
**[Tag, Layer, SpriteLayer](https://github.com/Deadcows/MyBox/wiki/Attributes#tag-layer-spritelayer)** — Dropdown with Tags, Layers or SpriteLayers<br />
**[MinMaxRange, RangedFloat and RangedInt](https://github.com/Deadcows/MyBox/wiki/Attributes#minmaxrange-rangedfloat-and-rangedint)** — Ranged sliders<br />
**[MaxValue, MinValue and PositiveValueOnly](https://github.com/Deadcows/MyBox/wiki/Attributes#maxvalue-minvalue-and-positivevalueonly)** — Validation for numbers and vectors<br />
**[MustBeAssigned](https://github.com/Deadcows/MyBox/wiki/Attributes#mustbeassigned)** — Automatically checks if field is assigned<br />
**[OverrideLabel](https://github.com/Deadcows/MyBox/wiki/Attributes#overridelabel)** — Change visible in Inspector field name<br />
**[ReadOnly](https://github.com/Deadcows/MyBox/wiki/Attributes#readonly)** — Draw property with disabled GUI<br />
**[RegexString](https://github.com/Deadcows/MyBox/wiki/Attributes#regexstring)** — Filter string field by the Regular Expression<br />
**[RequireTag and RequireLayer](https://github.com/Deadcows/MyBox/wiki/Attributes#requiretag-and-requirelayer)** — Automatically set Tag and Layer<br />
**[Scene](https://github.com/Deadcows/MyBox/wiki/Attributes#scene)** — Friendly way to keep Scene name as a string. See also [SceneReference type](https://github.com/Deadcows/MyBox/wiki/Types#scenereference)<br />
**[SearchableEnum](https://github.com/Deadcows/MyBox/wiki/Attributes#searchableenum)** — Nice UI for enums with lots of elements<br />
**[Separator](https://github.com/Deadcows/MyBox/wiki/Attributes#separator)** — Draw separator with or without title<br />

--------

## [Tools and Features](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features)
**[AssetPressetPreprocessor](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#assetpressetpreprocessor)** — Conditionally apply Presets to your assets on import<br />
**[TimeTest](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#timetest)** — Measure performance with simple api<br />
**[IPrepare](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#iprepare)** — Easy way to replace caching, calculations and asserts from playmode<br />
**[Features](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#cleanup-empty-directories-and-autosave-features-and-some-hotkeys)** — Cleanup Empty Directories, AutoSave feature, Hotkeys<br />
**[UnityEvent Inspector](https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#reworked-unityevent-inspector)** — Allows you to fold and reorder event subscribers<br />

--------

## [Types](https://github.com/Deadcows/MyBox/wiki/Types)
**[ActivateOnStart Component](https://github.com/Deadcows/MyBox/wiki/Types#activestateonstart-component)** — Set state of specific GO on game start<br />
**[AnimationStateReference](https://github.com/Deadcows/MyBox/wiki/Types#animationstatereference)** — Specify AnimationClip on object with Animator<br />
**[AssetPath and AssetFolderPath](https://github.com/Deadcows/MyBox/wiki/Types#assetpath-and-assetfolderpath)** — Inspector button to browse for folder or asset under Assets folder<br />
**[Billboard Component](https://github.com/Deadcows/MyBox/wiki/Types#billboard-component)** — Force object to always face camera<br />
**[ColliderGizmo Component](https://github.com/Deadcows/MyBox/wiki/Types#collidergizmo-component)** — Highlight colliders and triggers in SceneView<br />
**[ColliderToMesh Component](https://github.com/Deadcows/MyBox/wiki/Types#collidertomesh)** — Generate Mesh from PolygonCollider2D data on the fly<br />
**[Commentary Component](https://github.com/Deadcows/MyBox/wiki/Types#commentary-component)** — Add text commentary to your GameObjects<br />
**[CoroutineGroup](https://github.com/Deadcows/MyBox/wiki/Types#coroutine-group)** — Wraps up bunch of coroutines to know when they all is completed<br />
**[FPSCounter Component](https://github.com/Deadcows/MyBox/wiki/Types#fpscounter)** — Display FPS counter on Playmode<br />
**[Guid Component](https://github.com/Deadcows/MyBox/wiki/Types#guidcomponent)** — Generate unique and persistent IDs<br />
**[MinMaxInt and MinMaxFloat](https://github.com/Deadcows/MyBox/wiki/Types#minmaxint-and-minmaxfloat)** — Asserts that Max => Min with handy inspector drawer<br />
**[MyCursor](https://github.com/Deadcows/MyBox/wiki/Types#mycursor)** — Nice way to set cursor with hotspot<br />
**[MyDictionary](https://github.com/Deadcows/MyBox/wiki/Types#mydictionary)** — Serializable Dictionary<br />
**[Optional and OptionalMinMax](https://github.com/Deadcows/MyBox/wiki/Types#optional-and-optionalminmax)** — Optionally assignable values<br />
**[Reorderable Collections](https://github.com/Deadcows/MyBox/wiki/Types#reorderable-collections)** — Reorder your collections in inspector<br />
**[SceneReference Component](https://github.com/Deadcows/MyBox/wiki/Types#scenereference)** — Reference scene with Scene asset in inspector<br />
**[Singleton](https://github.com/Deadcows/MyBox/wiki/Types#singleton)** — Cache and access instance of MonoBehaviour<br />
**[TransformData](https://github.com/Deadcows/MyBox/wiki/Types#transformdata)** — Type to store and apply position, rotation and scale <br />
**[UIFollow Component](https://github.com/Deadcows/MyBox/wiki/Types#uifollow-component)** — RectTransform will follow specified Transform<br />
**[UIImageBasedButton Component](https://github.com/Deadcows/MyBox/wiki/Types#uiimagebasedbutton-component)** — Used to create toggle button behaviour<br />
**[UIRelativePosition Component](https://github.com/Deadcows/MyBox/wiki/Types#uirelativeposition-component)** — Position one RectTransform relatively to another, regardless of hierarchy<br />
**[UISizeBy Component](https://github.com/Deadcows/MyBox/wiki/Types#uisizeby-component)** — Size one RectTransform relatively to another<br />


--------

//TODO: Extensions, Unfinished tools
