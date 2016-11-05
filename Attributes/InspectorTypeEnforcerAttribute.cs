using System;
using UnityEngine;

public class InspectorTypeEnforcerAttribute : PropertyAttribute
{
	public Type EnforcedType;

	public InspectorTypeEnforcerAttribute(Type enforcedType)
	{
		EnforcedType = enforcedType;
	}
}