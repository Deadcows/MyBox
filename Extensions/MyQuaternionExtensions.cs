using JetBrains.Annotations;
using UnityEngine;

namespace MyBox
{
	[PublicAPI]
	public static class MyQuaternionExtensions
	{
		public static Quaternion SetEulerX(this Quaternion quaternion, float x) 
			=> Quaternion.Euler(quaternion.eulerAngles.SetX(x));
		
		public static Quaternion SetEulerY(this Quaternion quaternion, float y)
			=> Quaternion.Euler(quaternion.eulerAngles.SetY(y));
		
		public static Quaternion SetEulerZ(this Quaternion quaternion, float z)
			=> Quaternion.Euler(quaternion.eulerAngles.SetZ(z));
		
		public static Quaternion SetEulerXY(this Quaternion quaternion, float x, float y)
			=> Quaternion.Euler(quaternion.eulerAngles.SetXY(x, y));
		
		public static Quaternion SetEulerXZ(this Quaternion quaternion, float x, float z)
			=> Quaternion.Euler(quaternion.eulerAngles.SetXZ(x, z));
		
		public static Quaternion SetEulerYZ(this Quaternion quaternion, float y, float z)
			=> Quaternion.Euler(quaternion.eulerAngles.SetYZ(y, z));
	}
}