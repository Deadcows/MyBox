using UnityEngine;

namespace MyBox.Internal
{
	public class MyBoxUpdateWindowLocation: ScriptableObject
	{
		public static MyBoxUpdateWindowLocation Instance
		{
			get
			{
				if (_instance != null) return _instance;
				return _instance = CreateInstance<MyBoxUpdateWindowLocation>();
			}
		}

		private static MyBoxUpdateWindowLocation _instance;
	}
}