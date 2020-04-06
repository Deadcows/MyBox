using UnityEngine;

namespace MyBox
{
	[ExecuteAlways]
	public class Billboard : MonoBehaviour
	{
		public Transform FacedObject;

		private Transform ActiveFacedObject
		{
			get
			{
				if (FacedObject != null) return FacedObject;
				if (_camera != null) return _camera.transform;
				_camera = FindObjectOfType<Camera>();
				
				return _camera == null ? null : _camera.transform;
			}
		}
		private static Camera _camera;

		private void Update()
		{
			if (ActiveFacedObject == null) return;
			transform.LookAt(ActiveFacedObject);
		}
	}
}

