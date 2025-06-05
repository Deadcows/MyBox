# Changelog
All notable changes to this package will be documented in this file.

## [Unreleased]
- Ability to disable custom UnityObject inspector for better compatibility with other libraries, in MyBox Window settings.
- Now MyBox detects Odin Inspector and disables some of the features to prioritize Odin.
- Added: MyEditorEvents.AfterPlaymode/OnGUI/OnPlaymodeGUI/OnBehaviourUpdate events
- Added: ColliderGizmo now allows to assign custom presets from serializable ColliderGizmoPreset
- Added: MyEditor.SetEditorIcon with a better way to set color
- Added: SceneClickHandler support of Physics2d and Plane
- Added: MyGizmos.DrawBoxCollider2D
- Added: MyLayer now have several handy functions and extensions to operate with LayerMasks
- Added: A bunch of new Optional types (OptionalBool/Vector2(Int)/Vector3(Int)/Color) and now with optionalValue parameters
- Added: MyDebug.VisualizeNavMeshPath() to dynamically draw path segments with Debug.DrawLine
- Added: MustBeAssignedAttribute now allows disabling checks in Prefab mode
- Extensions: object.GetPrivateProperty/GetPrivateField and SetPrivateField methods to access private fields via reflection
- Extensions: SerializedProperty.ApplyModifiedProperties()
- Extensions: Array.InsertAt(index, item)
- Extensions: float.RemapTo01 and float.Remap to scale the value from one range to another
- Extensions: float.Clamp01() and int.Clamp01()
- Extensions: Quaternion.SetEulerX/Y/Z/XY/XZ/YZ methods
- Breaking Change: ActiveStateOnStart component is removed
- Fixed: camera.IsWorldPointInViewport() extension method bugfix
- Fixed: MyGUI.DrawColouredRect now properly works in dark mode (skin tint not mixed with the color)
- Fixed: DefinedValuesAttribute now properly works within serialized types
- Fixed: string.Colored() extension method now properly works in newer Unity versions
- Fixed: ButtonMethod failed condition will prevent the other ButtonMethods from drawing
- Fixed: ConstantsSelectionAttribute not working within serialized types
- [TODO DOCS] Added: MyDefinesUtility simplifies handling of PlayerSettings.Set(Get)ScriptingDefineSymbols
- [TODO DOCS] Added: MyEditorAudio tool to play AudioClip in Editor
- [TODO DOCS] Added: MyComponentUtility.MoveComponentInspectorToTop/ToBottom(Component component) utility methods
- [TODO DOCS] Added: Contextual Menu Items to "Move Component To Top/To Bottom" in inspector
- [TODO DOCS] Added: TopmostComponentAttribute to move on top in inspector automatically when it added
- [TODO DOCS] Added: DisplayPreviewAttribute to display the property image of the specified size in the inspector

## [1.8.0] - 2023-10-09
- Now it is possible to toggle some features that may lead to performance issues in editor in MyBox Window
- AutoSave on Play, Clean Empty Folders, Prepare on Playmode and SO processing of [AutoProperty] and [MustBeAssigned] now disabled by default. Check MyBox Window for details
- Breaking Change: MyDelayedActions.DelayedAction methods now start themselves automatically
- Breaking Change: MinMaxFloat.RandomInRangeInclusive was redundant, removed
- Breaking Change: Extension IList.GetRandomCollection is removed, replaced with IList.ExclusiveSample
- Breaking Change: Removed bunch of methods from MyPhysics class
- Breaking Change: Billboard component now uses Camera.main instead of FindObjectOfType<Camera>(). Thanks to @adamgryu for this change!
- Breaking Change: MyGUI.EditorIcons.Cross icon removed as it is not included in Unity 2022+
- Added: Toggle Inspector Debug hotkey - Alt+D by default
- Added: SearchableEnumDrawer type for fast creation of Searchable Enums instead of SearchableEnumAttribute usage
- Added: PlayerPrefs and EditorPrefs Bool/Float/Int/String/Vector/VectorInt types
- Added: Ability to fold DisplayInspector
- Added: SceneReference now also have UnloadSceneAsync() and SetActive() methods
- Added: MyGUI.SearchablePopup to show popup list with ability to filter displayed content
- Added: AutoProperty now allows to specify predicate method to filter out the lookup. Thanks to @tonygiang for the addition!
- Added: AutoProperty and MustBeAssigned support of ScriptableObjects! Thanks to @tonygiang for the addition!
- Added: ConditionalField - multiple conditions per attribute
- Added: ConditionalField - ability to use method call to dynamically check condition
- Added: ButtonMethod now might be conditional, just like ConditionalField!
- Added: ReadOnlyAttribute now might be conditional, just like ConditionalField. Thanks to @CrizGames!
- Added: DefinedValuesAttribute now allows to show custom labels instead of values in a popup
- Added: DefinedValuesAttribute now allows to use method returning the collection of required values
- Added: RangeVectorAttribute, works with Vector2/3 or Vector2/3Int. Thanks to @WhaleTee!
- Added: AutoPropertyAttribute allowEmpty setting, to disable error logging. Thanks to @YogurtTheHorse! 
- Changed: SceneAttribute is now rendered as popup list of scenes from Editor Build Settings
- Extensions: Coroutine.OnComplete(Action);
- Extensions: collection.FillBy(index => { }) allows to use factory method to fill up the collection. Thanks to @tonygiang!
- Extensions: IList.SwapInPlace(a, b) swaps two elements in collection. Thanks to @tonygiang!
- Extensions: IList.Shuffle() shuffles elements in collection using the Knuth algorithm. Thanks to @tonygiang!
- Extensions: IList.ExclusiveSample() returns collection of random elements. Thanks to @tonygiang!
- Extensions: Rigidbody.ToggleConstraints() extension. Thanks to @tonygiang!
- Extensions: Transform.SetLossyScale(). Thanks to @tonygiang!
- Extensions: Transform.GetChildsWhere(PredicateFunc) allows to recursively get all the childs matching predicate
- Extensions: Camera.WorldPointOffsetByDepth() to keep point position on screen but with specified distance from camera. Thanks to @tonygiang!
- Extensions: Component/GameObject.SetLayerRecursively(). Thanks to @tonygiang!
- Extensions: RectTransform.SetWidth() and SetHeight()
- Extensions: RectTransform.ShiftAnchor() to offset anchor. Thanks to @tonygiang!
- Extensions: RectTransform.GetAnchorCenter() to get mid point between anchorMin and anchorMax. Thanks to @tonygiang!
- Extensions: RectTransform.GetAnchorDelta() to get parent-relative size of the RectTransform. Thanks to @tonygiang!
- Extensions: Vector.Pow() to raise each component of the source Vector to the specified power. Thanks to @tonygiang!
- Extensions: Vector.ScaleBy() immutably returns the result of the source vector multiplied with another vector. Thanks to @tonygiang!
- Extensions: Vector/Transform - ClampZ(), InvertX/Y/Z(), Offset etc
- Extensions: float.Clamp(), float.Round() and float.RoundToInt()
- Extensions: string.IsNullOrEmpty() and string.NotNullOrEmpty()
- Extensions: SerializedProperty.GetUniquePropertyId() to get unique Id per Object+Field combination
- Extensions: SerializedProperty.Repaint() to repaint inspector window where property is displayed
- Fix: AutoPropertyAttribute NullReferenceException fix. Thanks to @TheWalruzz for the fix
- Fix: Extensions Vector/Transform.ClampY() bugfix
- Fix: MyEditorEvents.OnEditorStarts event works properly now
- Fix: MyBox is not strictly dependant on Physics, Physics2D, ImageConversion and AI modules, thanks to @r1noff!
- Fix: AutoProperty will also be triggered before playmode. If scene is not saved before playmode, field will have actual values
- Fix: FPSCounter now works correctly if EditorOnly is toggled. Thanks to @TheWalruzz!
- Fix: IPrepareFeature now works with prefab instances
- Fix: AnimationStateReference now working with AnimatorOverrideControllers
- Fix: AssetPresetPreprocessor improvement to prevent unneeded reimports 
- Fix: ButtonMethods offsets in the inspector fixed
- Fix: OverrideLabelAttribute now might be used on fields with custom drawers and on custom types
- Fix: Unity 2021.2+ compatibility (UnityEditor.Experimental.SceneManagement namespace became UnityEditor.SceneManagement). Thanks to @I_Jemin!
- Fix: Occasional ReflectionTypeLoadException was fixed when ConditionalField is used
- Fix: Vector3Int.ToVector3 z field wasn't copied. Thanks to @Quriz for this fix!
- Fix: MyDebug.LogArray optimization. Thanks to @jcs090218 for this!
- Fix: Indent issue with Optional type in nested inspectors fixed. Thanks to @r3dskjn for the fix!
- Fix: DisplayInspector now will show warning if used on property of the wrong type
- Fix: FoldoutAttribute visual improvements
- Fix: AssetPresetPreprocessor were not working right after PreprocessorBase creation
- Fix: MySerializedProperty.GetFieldInfo now also searching the field in the base types

## [1.7.0] - 2021-06-09
- Breaking Changes: MyCollections.AsEnumerable renamed to SingleToEnumerable
- Breaking Changes: MyCollections.GetOrDefault renamed to GetOrAdd
- Breaking Changes: MonoSingleton class is removed, its functionality combined with Singleton class
- Breaking Changes: WaitForUnscaledSeconds is removed since we have WaitForSecondsRealtime (wow, it's here since Unity 5.4! Nostalgic memories :D)
- Breaking Changes: UIImageBasedToggle removed. Unity's Toggle able to do same things this days
- Added: Donation button in MyBox window ;)
- Added: TransformData type to store and restore position, rotation and scale
- Added: CharactersRange Attribute to validate string by set of characters
- Added: RegexString Attribute to validate string by Regular Expression
- Added: OverrideLabelAttribute
- Added: Ability to use AutoPropertyAttribute to assign from parent, scene of asset folder, thanks to @tonygiang!
- Added: ConditionalFields supports Enum Flag as conditions! Thanks to Dietmar Puschmann for this addition
- Added: MyEditorEvents.OnEditorStarts event
- Added: UnityEvent inspector now is foldable, thanks to @karsion!
- Extensions: MyCollections.FirstIndex - more generalized version 
- Extensions: MyCollections.GetWeightedRandom and GetWeightedRandomIndex
- Changed: AutoPropertyAttribute will also check prefabs on prefab mode open
- Changed: MyBox will only check for updates when editor opens
- Changed: Singleton now might be used as parent class of MonoBehaviour to cache and remove duplicating instances
- Changed: UIImageBasedButton now uses Highlight sprites when Pointed
- Fix: DisplayInspectorAttribute works better with (and inside of) collections
- Fix: MyString.ToCamelCase() extension works better now, thanks to @derfium!
- Fix: MinValue/MaxValueAttribute build warnings fixed

## [1.6.2] - 2021-04-08
- Fix: MyBox Window settings were not applied correctly

## [1.6.1] - 2021-04-07
- Changed: MyBox Window now contains all settings and some useful links
- Fix: MyBox Updater now should properly update UPM version

## [1.6.0] - 2021-04-05
- Added: SceneAttribute to keep scene name in a string. Consider to use SceneReference type as it is more flexible
- Changed: MyCollection.NextIndexInCircle() extension now works with offsets
- Extension: MyString.Colored(UnityEngine.Color)
- Extension: MyString.SurroundedWith()
- Extension: GetObjectsOfLayerInChilds now also receives layer as a string
- Extension: MySerializedProperty.GetValue() now works with collections
- Fix: DisplayInspector now (finally) works right with Unity 2020.2 reorderable collections
- Fix: SeparatorAttribute works better with collections
- Fix: SeparatorAttribute looks not as ugly as before :D
- Fix: ConditionalFieldAttribute optimizations and fixes
- Fix: AnimationStateReference now works with collections

## [1.5.0] - 2020-07-15
- Added: WarningsPool, used to log repeated message only one time
- Changed: MustBeAssigned attribute now will check fields on prefab when it is saved in prefab mode
- Changed: AutoProperty attribute now will fill fields on prefab when it is saved in prefab mode
- Changed: IPrepareFeature now have three Prepare events: OnPrepareBefore, OnPrepare, OnPrepareAfter for execution order control
- Changed: IPrepareFeature now is in MyBox.EditorTools namespace
- Changed: ButtonMethodAttribute — new setting to draw button before or after inspector
- Changed: Clean Empty Directories Feature is now  disabled by default
- Extension: MyReflection extensions, with HasMethod, HasField and HasProperty object extensions
- Extension: Dictionary.GetOrDefault to return default() value if key is not found
- Extension: IEnumerable.ForEach, just like List.ForEach. Also takes Func<>!
- Fix: Build exception (again!) because of internal WarningsPool
- Fix: MustBeAssigned occasional NullReferenceException during build
- Fix: TransformShakeExtension — shake bounds wasn't worked


## [1.4.1] - 2020-06-06
- Added: SceneReference.LoadSceneAsync method
- Changed: Image.SetAlpha changed to more generic Graphic.SetAlpha
- Changed: WarningsPool now allows to write any log type, not just warnings
- Fix: Build exception because of CollectionWrapperBase


## [1.4.0] - 2020-05-18
- Added: ConstantsSelectionAttribute to popup all const values of a specific type
- Added: CollectionWrapper to use ConditionalField on collections
- Added: InitializationFieldAttribute to make field read-only in Playmode
- Added: MyCursor type to with handy hotspot assignment
- Added: MyCoroutines.CoroutineGroup with handy StartAll() and AnyProcessing
- Added: MyDebug.LogColor(Color) because why not
- Changed: ConditionalField now correctly uses CustomDrawer if drawer affects base type of target field 
- Changed: DisplayInspectorAttribute now supports ButtonMethodAttribute inside of displayed types
- Changed: RequiredLayerAttribute might accept layer index instead of the name
- Changed: AnimationStateReference now might reference any object on scene
- Changed: ColliderGizmo now works with MeshColliders
- Extension: MinMax.RandomInRange
- Extension: Transform.StartShake now have "fade" parameter
- Extension: Coroutine.StartNext(IEnumerator) to easily create sequence of coroutines 
- Extension: SerializedProperty.IsNumerical to detect vectors or float/int
- Extension: SerializedProperty.GetValue/SetValue to operate with object reference
- Fix: Reorderable Collections drawing issue
- Fix: ColliderGizmo compilation problem in Unity2020.1
- Fix: CleanEmptyDirectories didn't allow to create folders
- Fix: CleanEmptyDirectories NullReferenceException fix
- Fix: AutoProperty rare NullReferenceException fix
- Fix: ConditionalField multiple fixes
- Fix: PositiveValeOnlyAttribute label drawing properly
- Fix: GameObject.HasComponent extension redundant constraint removed
- Fix: UnityObjectEditor rare NullReferenceException fix
- Fix: Billboard component

## [1.3.0] - 2020-01-16
- Added: FoldoutAttribute. Thanks to PixeyeHQ!
- Added: UnityEvent inspector revamp! Now it's foldable and reorderable :). Thanks to Byron Mayne!
- Fix: TransformShakeExtension critical bug fixed

## [1.2.0] - 2019-11-13
- Added: Reorderable Collections!
- Added: Transform.StartShake and Transform.EndShake extension methods. Use on Camera transform for screen shake effect for instance
- Added: NavMeshPath.GetPointsOnPath extension to split path on evenly distributed points
- Added: MyEditor.CopyToClipboard method. Copy string via script like with Ctrl+C
- Changed: ConditionalFieldAttribute works on custom types inside of collections
- Changed: ConditionalFieldAttribute now works much faster!
- Changed: ColliderGizmo now also highlights NavMeshObstacles
- GUIDComponent updated
- Fix: Compilation error fixed
- Fix: MyBox Updater fixed. Exceptional cases logged with warnings


## [1.1.0] - 2019-09-25
- Added: Commentary component. Add commentaries in inspector;
- Fix: UIRelativePosition fixes;
- Few redundant warnings removed
- Versioning changed to release patches more often without extra warnings

## [1.0.4] - 2019-09-16
- Added: UIRelativePosition type allows to align UI element relative to some other RectTransform with offsets and stuff
- Added: AssetPath and AssetFolderPath types. String wrappers with "Browse" button in inspector. Thanks to Nate Wilson (wilsnat) for the idea
- Changed: ConditionalFieldAttribute now works on fields with custom inspectors! Thanks to Nate Wilson (wilsnat)
- Changed: RangedInt/Float and MinMaxInt/Float now have constructors for static instantiation
- Fix: ConditionalFieldAttribute always hide the field if "compare to" values were not assigned

## [1.0.3] - 2019-09-02
- RequireLayer and RequireTag attributes
- MonoSingleton Type
- Fixed indent issue with nested inspector for MinMaxInt/Float, Optional, OptionalMinMax type
- MySceneBundle is a Tool to transfer data from one scene to another. Thanks to Kaynn-Cahya for this addition!

## [1.0.2] - 2019-08-17
- Fix: breaking problem with MyCoroutines type
- Added: MinMaxInt/Float Clamp and Lerp extension methods
- Added: MinMaxInt/Float Length and MidPoint extension methods
- Now MyBox will automatically check for updates!

## [1.0.1] - 2019-08-15
- Compilation errors fixed
- Removed obsolete warning

## [1.0.0] - 2019-08-13
### First version with Unity Package Manager support.
Let's take it as the first release since now you are able to install MyBox with Package Manager and update it with "Tools/MyBox/Check for updates"
