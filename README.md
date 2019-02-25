# MyBox
MyBox is a set of tools, features and extensions for Unity.<br />
It is MyBox. Now it's YourBox too.<br />

## [Tools](Tools/)
Tools such as Logger and TimeTest

## [Attributes](Attributes/)
### Button:
```c#
public Collider[] Colliders;

#if UNITY_EDITOR
[Button("CollectColliders"), SerializeField]
int _collectColliders;
private void CollectColliders()
{
  Colliders = FindObjectsOfType<Collider>();
}
#endif
```
![ButtonAttribute Example][ButtonAttribute]

[ButtonAttribute]: https://habrastorage.org/webt/bq/du/cg/bqducgp2jew2ecdryczayia5hbq.gif "ButtonAttribute Example"


### ConditionalField:
```c#
public bool WanderAround;
[ConditionalField("WanderAround")] public float WanderDistance = 5;

public AIState NextState = AIState.None;
[ConditionalField("NextState", AIState.Idle)] public float IdleTime = 5;
```
![ConditionalField Example][ConditionalField]

[ConditionalField]: http://deadcow.ru/MyBox/ConditionalField.gif "ConditionalField Example"


### DefinedValues:
```c#
[DefinedValues(1, 3, 5)]
public int AgentHeight;
```
![DefinedValues Example][DefinedValues]

[DefinedValues]: http://deadcow.ru/MyBox/DefinedValues.gif "DefinedValues Example"


### DisplayInspector:
Displays one inspector inside of another. 
It's handy if you'd like to store some settings in scriptable objects.

[DisplayInspector(displayScriptField:false)] to hide object field once assigned (useful for "single instance" settings)
```c#
[CreateAssetMenu]
public class AgentAIContextSettings : ScriptableObject
{
	public bool WanderAround;
	public float WanderDistance = 5;
}
```
```c#
[DisplayInspector]
public AgentAIContextSettings Settings;
```
Set displayScriptField to false (by default it's true) to hide property field once 
```c#
[DisplayInspector(displayScriptField:false)]
```
![DisplayInspector Example][DisplayInspector]
![DisplayInspector Example2][DisplayInspector2]

[DisplayInspector]: http://deadcow.ru/MyBox/DisplayInspector.gif "DisplayInspector Example"
[DisplayInspector2]: http://deadcow.ru/MyBox/DisplayInspector2.gif "DisplayInspector Example2"


### Tag, Layer, SpriteLayer:
```c#
[Tag]
public string Tag;
[Layer]
public int Layer;
[SpriteLayerAttribute]
public int SortingLayer;
```
![Layer Example][Layer]

[Layer]: https://habrastorage.org/webt/gj/is/yh/gjisyhaqi3hgzrghuvt3ppevfs8.gif "Layer Example"


### MinMaxRange and RangedFloat:
by [Richard Fine](https://bitbucket.org/richardfine/scriptableobjectdemo)

RangedFloat may be used without MinMaxRange. Default is 0-1 range
```c#
public class RandomisedHealth : MonoBehaviour
{
	[MinMaxRange(80, 120)] 
	public RangedFloat Health;
	public RangedFloat Raito;
}
```
![MinMaxRange Example][MinMaxRange]

[MinMaxRange]: http://deadcow.ru/MyBox/MinMaxRange.gif "MinMaxRange Example"


### MustBeAssigned:
Previously I used a lot of Debug.Assert() in Awake to ensure that all desired values are assigned through inspector.
Now I just use MustBeAssigned. 

It triggers on value types with default values, null refs, empty arrays and strings
```c#
[MustBeAssigned]
public MonoBehaviour MyScript;
[MustBeAssigned]
public float MyFloat;
```
![MustBeAssigned Example][MustBeAssigned]

[MustBeAssigned]: http://deadcow.ru/MyBox/MustBeAssigned.png "MustBeAssigned Example"


### PositiveValueOnly:
Assures that value is positive. Works with numbers and vectors
```c#
[PositiveValueOnly]
public Vector2 ScanRange;
[PositiveValueOnly]
public float ChaseSpeed;
```
![PositiveValueOnly Example][PositiveValueOnly]

[PositiveValueOnly]: https://habrastorage.org/webt/kj/g_/tx/kjg_tx0agin62_9grlpebcrpzi4.gif "PositiveValueOnly Example"


### ReadOnly:
I use it in rare cases to debug things through inspector
```c#
public float InitialHealth = 100;
[ReadOnly]
public float CurrentHealth;
```
![ReadOnly Example][ReadOnly]

[ReadOnly]: http://deadcow.ru/MyBox/ReadOnly.png "ReadOnly Example"


### SearchableEnum:
by incredible [Ryan Hipple](https://github.com/roboryantron/UnityEditorJunkie)
```c#
public float InitialHealth = 100;
[ReadOnly]
public float CurrentHealth;
```
![SearchableEnum Example][SearchableEnum]

[SearchableEnum]: https://user-images.githubusercontent.com/20144789/39614240-5e844c24-4f3c-11e8-998a-e0fbf969ddd4.gif "SearchableEnum Example"


### Separator:
Decorative separator. May be with or without title

![Separator Example][Separator]

[Separator]: http://deadcow.ru/MyBox/Separator.png "Separator Example"
