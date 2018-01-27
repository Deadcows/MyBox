# MyBox
MyBox is a set of tools and extensions mostly made by me over the years of work with Unity
Almost all of the code was made during The Final Station development

### Attributes
#### ConditionalField
```c#
public bool WanderAround;
[ConditionalField("WanderAround")] public float WanderDistance = 5;
public AIState NextState = AIState.None;
[ConditionalField("NextState", AIState.Idle)] public float IdleTime = 5;
```
![ConditionalField Example](https://deadcow.ru/MyBox/ConditionalField.gif "Logo Title Text 1")
