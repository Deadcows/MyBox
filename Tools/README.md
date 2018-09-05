## TimeTest 
TimeTest is a tool to benchmark code with simple api
```c#
// Use static methods
TimeTest.Start(string title, bool useMilliseconds = false)
...
TimeTest.End()
// Or as disposable with using statement
using (new TimeTest(string title, bool useMilliseconds = false))
{
  ...
}
```

Example:
```c#
TimeTest.Start("1k GO creation", true);
		
List<GameObject> objects = new List<GameObject>();
for (var i = 0; i < 1000; i++)
{
  objects.Add(new GameObject("Object No" + i));
}

TimeTest.End();

using (new TimeTest("Massive parenting"))
{
  Transform parent = null;
  foreach (var o in objects)
  {
    if (parent == null)
    {
      parent = o.transform;
      continue;
    }
    o.transform.SetParent(parent);
    parent = o.transform;
  }
}
```
![TimeTest Example][TimeTest]

[TimeTest]: http://deadcow.ru/MyBox/TimeTest.png "TimeTest Example"


## Logger
Logger is a tool to create custom log txt file. It will also automatically collect all Debug.Log messages and exceptions
```c#
Logger.Log(string text)
Logger.Log(Exception ex)
```
By default it will create customLog.txt file in Data folder for standalone builds. 
