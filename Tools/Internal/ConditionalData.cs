using System.Linq;
using System.Reflection;

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
		public ConditionalData(bool useMethod, string methodName) => PredicateMethod = methodName;
		
		
		#if UNITY_EDITOR
		public MethodInfo EditorCachePredicateMethod;
		#endif
	}
}