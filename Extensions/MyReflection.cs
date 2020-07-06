namespace MyBox
{
	public static class MyReflection
	{
		public static bool HasMethod(this object target, string methodName)
		{
			return target.GetType().GetMethod(methodName) != null;
		}

		public static bool HasField(this object target, string fieldName)
		{
			return target.GetType().GetField(fieldName) != null;
		}

		public static bool HasProperty(this object target, string propertyName)
		{
			return target.GetType().GetProperty(propertyName) != null;
		}
	}
}