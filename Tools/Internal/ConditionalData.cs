using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MyBox.Internal
{
	public class ConditionalData
	{
		public bool IsSet => _fieldToCheck.NotNullOrEmpty() || _fieldsToCheckMultiple.NotNullOrEmpty() || _predicateMethod.NotNullOrEmpty();
		
		private readonly string _fieldToCheck;
		private readonly bool _inverse;
		private readonly string[] _compareValues;

		private readonly string[] _fieldsToCheckMultiple;
		private readonly bool[] _inverseMultiple;
		private readonly string[] _compareValuesMultiple;

		private readonly string _predicateMethod;

		public ConditionalData(string fieldToCheck, bool inverse = false, params object[] compareValues)
			=> (_fieldToCheck, _inverse, _compareValues) =
				(fieldToCheck, inverse, compareValues.Select(c => c.ToString().ToUpper()).ToArray());

		public ConditionalData(string[] fieldToCheck, bool[] inverse = null, params object[] compare) =>
			(_fieldsToCheckMultiple, _inverseMultiple, _compareValuesMultiple) =
			(fieldToCheck, inverse, compare.Select(c => c.ToString().ToUpper()).ToArray());

		public ConditionalData(params string[] fieldToCheck) => _fieldsToCheckMultiple = fieldToCheck;

		// ReSharper disable once UnusedParameter.Local
		public ConditionalData(bool useMethod, string methodName, bool inverse = false) 
			=> (_predicateMethod, _inverse) = (methodName, inverse);


#if UNITY_EDITOR
		/// <summary>
		/// Iterate over Field Conditions
		/// </summary>
		public IEnumerator<(string Field, bool Inverse, string[] CompareAgainst)> GetEnumerator()
		{
			if (_fieldToCheck.NotNullOrEmpty()) yield return (_fieldToCheck, _inverse, _compareValues);
			if (_fieldsToCheckMultiple.NotNullOrEmpty())
			{
				for (var i = 0; i < _fieldsToCheckMultiple.Length; i++)
				{
					var field = _fieldsToCheckMultiple[i];
					bool withInverseValue = _inverseMultiple != null && _inverseMultiple.Length - 1 >= i;
					bool withCompareValue = _compareValuesMultiple != null && _compareValuesMultiple.Length - 1 >= i;
					var inverse = withInverseValue && _inverseMultiple[i];
					var compare = withCompareValue ? new[] { _compareValuesMultiple[i] } : null;

					yield return (field, inverse, compare);
				}
			}
		}

		/// <summary>
		/// Call and check Method Condition, if any
		/// </summary>
		public bool IsMethodConditionMatch(object owner)
		{
			if (_predicateMethod.IsNullOrEmpty()) return true;
			
			var predicateMethod = GetMethodCondition(owner);
			if (predicateMethod == null) return true;
			
			bool match = (bool)predicateMethod.Invoke(owner, null);
			if (_inverse) match = !match;
			return match;
		}
		
		
		private MethodInfo GetMethodCondition(object owner)
		{
			if (_predicateMethod.IsNullOrEmpty()) return null;
			if (_initializedMethodInfo) return _cachedMethodInfo;
			_initializedMethodInfo = true;

			var ownerType = owner.GetType();
			var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			var method = ownerType.GetMethods(bindings).SingleOrDefault(m => m.Name == _predicateMethod);

			if (method == null || method.ReturnType != typeof(bool))
			{
				ConditionalUtility.LogMethodNotFound((Object)owner, _predicateMethod);
				_cachedMethodInfo = null;
			}
			else _cachedMethodInfo = method;

			return _cachedMethodInfo;
		}

		private MethodInfo _cachedMethodInfo;
		private bool _initializedMethodInfo;
#endif
	}
}