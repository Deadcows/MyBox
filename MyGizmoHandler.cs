using System;
using UnityEngine;

namespace MyBox.Utility
{
	public class MyGizmoHandler : MonoBehaviour
	{
		public Action DrawGizmos;
		public Action DrawGizmosSelected;

		private void OnDrawGizmos()
		{
			if (DrawGizmos != null) DrawGizmos.Invoke();
		}

		private void OnDrawGizmosSelected()
		{
			if (DrawGizmosSelected != null) DrawGizmosSelected.Invoke();
		}
	}
}