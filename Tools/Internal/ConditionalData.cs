using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MyBox.Internal
{
	public class ConditionalData
	{
		public readonly string FieldToCheck;
		public readonly bool Inverse;
		public readonly string[] CompareValues;

		public readonly string[] FieldsToCheckMultiple;
		public readonly bool[] InverseMultiple;
		public readonly string[] CompareValuesMultiple;

		public string PredicateMethod;

		public ConditionalData(string fieldToCheck, bool inverse = false, params object[] compareValues)
			=> (FieldToCheck, Inverse, CompareValues) =
				(fieldToCheck, inverse, compareValues.Select(c => c.ToString().ToUpper()).ToArray());

		public ConditionalData(string[] fieldToCheck, bool[] inverse = null, params object[] compare) =>
			(FieldsToCheckMultiple, InverseMultiple, CompareValuesMultiple) =
			(fieldToCheck, inverse, compare.Select(c => c.ToString().ToUpper()).ToArray());

		public ConditionalData(params string[] fieldToCheck) => FieldsToCheckMultiple = fieldToCheck;

		// ReSharper disable once UnusedParameter.Local
		public ConditionalData(bool useMethod, string methodName, bool inverse = false) 
			=> (PredicateMethod, Inverse) = (methodName, inverse);


#if UNITY_EDITOR
		public bool WithMethodCondition => PredicateMethod.NotNullOrEmpty();
		public MethodInfo GetMethodCondition(object owner)
		{
			if (PredicateMethod.IsNullOrEmpty()) return null;
			if (_initializedMethodInfo) return _cachedMethodInfo;
			_initializedMethodInfo = true;

			var ownerType = owner.GetType();
			var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			var method = ownerType.GetMethods(bindings).SingleOrDefault(m => m.Name == PredicateMethod);

			if (method == null || method.ReturnType != typeof(bool))
			{
				var unityObject = (Object)owner;
				var warning = "<color=brown>" + unityObject.name + "</color> caused: " +
				              $"[ConditionalField] is trying to invoke method {PredicateMethod} " +
				              "which is missing or with not with a bool return type";

				WarningsPool.LogWarning(warning, unityObject);
				_cachedMethodInfo = null;
			}
			else _cachedMethodInfo = method;

			return _cachedMethodInfo;
		}

		private MethodInfo _cachedMethodInfo;
		private bool _initializedMethodInfo;

		public IEnumerator<(string Field, bool Inverse, string[] CompareAgainst)> GetEnumerator()
		{
			if (FieldToCheck.NotNullOrEmpty()) yield return (FieldToCheck, Inverse, CompareValues);
			if (FieldsToCheckMultiple.NotNullOrEmpty())
			{
				for (var i = 0; i < FieldsToCheckMultiple.Length; i++)
				{
					var field = FieldsToCheckMultiple[i];
					bool withInverseValue = InverseMultiple != null && InverseMultiple.Length - 1 >= i;
					bool withCompareValue = CompareValuesMultiple != null && CompareValuesMultiple.Length - 1 >= i;
					var inverse = withInverseValue && InverseMultiple[i];
					var compare = withCompareValue ? new[] { CompareValuesMultiple[i] } : null;

					yield return (field, inverse, compare);
				}
			}
		}
#endif
	}
}