using System;
using MyBox.EditorTools;
using MyBox.Internal;
using UnityEngine;
using Object = UnityEngine.Object;


namespace MyBox
{
[Serializable] public class ObservableBool : Observable<bool> { }

[Serializable] public class ObservableString : Observable<string> { }

[Serializable] public class ObservableInt : Observable<int> {}

[Serializable] public class ObservableFloat : Observable<float> { }

[Serializable] public class ObservableVector2 : Observable<Vector2> { }

[Serializable] public class ObservableVector3 : Observable<Vector3> { }

[Serializable] public class ObservableGameObjectReference : Observable<GameObject> { }

[Serializable] public class ObservableGameTransformReference : Observable<Transform> { }
}

namespace MyBox.Internal
{

[Serializable]
public class Observable<T> : ObservableBase
{
	public delegate void ValueChangedDelegate(T previous, T current);

	public event ValueChangedDelegate valueChanged;

	// Keep SerializeField & Name
	[SerializeField] T value = default;

	public T Value
	{
		get => value;

		set
		{
			if (this.value == null)
			{
				if (value == null)
					return;
			}
			else if (this.value.Equals(value))
				return;

			T previousState = this.value;
			this.value = value;

			valueChanged?.Invoke(previousState, this.value);
		}
	}

	public void OnValidateSubscribe(ValueChangedDelegate callback)
	{
		if (Application.isPlaying)
			return;

		valueChanged = null;
		valueChanged += callback;
	}

	public static implicit operator T(Observable<T> observable) => observable.value;

	public override string ToString() => value.ToString();

	internal sealed override void SetValue(object value) => Value = (T) value;
}

public abstract class ObservableBase
{
	internal abstract void SetValue(object value);
}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	
[CustomPropertyDrawer(typeof(ObservableBase), useForChildren: true)]
	public class ObservableTypePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			Undo.RecordObject((Object) property.GetObjectWithProperty(), "Value Changed"); 
			EditorGUI.indentLevel = 0; // PropertyDrawer Indent fix for nested inspectors

			SerializedProperty valueProperty = property.FindPropertyRelative("value");
 
			EditorGUI.PropertyField(position, valueProperty, label);
			object value = valueProperty.GetPropertyValue();
			((ObservableBase) property.GetValue()).SetValue(value);

			EditorGUI.EndProperty();
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
	}
}
#endif