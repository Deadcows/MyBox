## Entity Component System Tools and Extentions
**ECS Extensions is compatible with "Unity.ECS 0.0.12p19" package.**

**I'll support them for the future but at the moment this extensions were commented out for better compatibility**


### [EventsSystem](https://forum.unity.com/threads/eventssystem-for-one-frame-components.548965/) 
ComponentSystem with a few extensions methods and helper structures to create end react on Events - one frame components
```c#
// Create Event. MyEventComponent is simple IComponentData
PostUpdateCommands.AddEvent(new MyEventComponent() { Data = Time.frameCount });

// Or create events outside of ComponentSystems
var manager = World.Active.GetExistingManager<EntityManager>();
manager.AddSharedEvent(new NewItemComponent() {Item = Item});

// Inject Event and use event.Fired and event.Data
[Inject] private Event<MyEventComponent> _event;
protected override void OnUpdate()
{
    if (_event.Fired) Debug.LogWarning("MyEventComponent fired: " + _event.Data[0].Data);
}
```
Follow link above for more examples


### UniqueComponent
Wrappers for injected structs for unique components. Those wrappers are:
```c#
UniqueComponent<T> where T : Component
UniqueTransformComponent<T> where T : Component
UniqueComponentData<T> where T : struct, IComponentData
UniqueTransformComponentData<T> where T : struct, IComponentData
```
This wrappers allows to inject components with ease and access it's data in a one-line way. Examples:
```c#
[Inject] private UniqueComponent<Camera> _camera;
[Inject] private UniqueTransformComponent<InputListenerComponent> _player;
[Inject] private UniqueComponentData<InputData> _input;
...
var anchoredPosition = _camera.Instance.WorldToScreenPoint(followPosition);
var playerPosition = _player.TransformInstance.position;
var toShoot = _input.Instance.LeftButtonClick;
```


### ECSExtensions
Bunch of helper methods. **I don't know how they'll work with multiple Worlds approaches since some of methods rely on cached EntityManager**

```c#
// Entity now has more to say:
entity.HasComponent<MyComponent>();
entity.GetComponent<MyComponent();
entity.GetComponentObject<MyComponentMB>();
entity.SetComponent(new MyComponent());
entity.AddComponent(new MyComponent());

PostUpdateCommands.AddComponent<MyTagComponent>();
// Add or Replace component
PostUpdateCommands.ReplaceComponent(entity, new MyComponent());
```
