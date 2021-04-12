# Changelog
All notable changes to this package will be documented in this file.

## [Unreleased]
- Added: ConditionalFields supports Enum Flag as conditions! Thanks to Dietmar Puschmann for this addition
- Added: MyEditorEvents.OnEditorStarts event
- Extensions: MyCollections.FirstIndex - more generalized version 
- Extensions: MyCollections.SelectWithIndex and SelectManyWithIndex - like LINQ.Select, but with index passed along the item into selector
- Extensions: MyCollections.GetWeightedRandom and GetWeightedRandomIndex
- Changed: MyBox will only check for updates when editor opens

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
