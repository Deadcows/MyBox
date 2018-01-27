# MyBox
MyBox is a set of tools and extensions mostly made by me over the years of work with Unity
Almost all of the code was made during The Final Station development

## Attributes
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
Very handy if you'd like to store settings in scriptable objects, for example
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
![DisplayInspector Example][DisplayInspector]

[DisplayInspector]: http://deadcow.ru/MyBox/DisplayInspector.gif "DisplayInspector Example"
!TODO ScriptableObjectInspector

### MustBeAssigned:
Previously I used a lot of Debug.Assert() in Awake to ensure that all desired values are assigned through inspector.
Now I just use MustBeAssigned. This one also works with value types and show error message in Console if value is null or default.
```c#
[MustBeAssigned]
public MonoBehaviour MyScript;
[MustBeAssigned]
public float MyFloat;
```
![MustBeAssigned Example][MustBeAssigned]

[MustBeAssigned]: http://deadcow.ru/MyBox/MustBeAssigned.png "MustBeAssigned Example"

### ReadOnly:
I use it in rare cases to debug things through inspector
```c#
public float InitialHealth = 100;
[ReadOnly]
public float CurrentHealth;
```
![ReadOnly Example][ReadOnly]

[ReadOnly]: http://deadcow.ru/MyBox/ReadOnly.png "ReadOnly Example"

### Separator:
Decorative separator. May be with or without title

![Separator Example][Separator]

[Separator]: http://deadcow.ru/MyBox/Separator.png "Separator Example"
