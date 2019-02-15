using UnityEngine;

namespace MyBox
{
	[ExecuteAlways]
	public class Billboard : MonoBehaviour
	{
		private static Camera _camera;

		private void Update()
		{
			if (_camera == null) FindCamera();
			if (_camera == null) return;
			transform.forward = _camera.transform.forward;
		}

		private void FindCamera()
		{
			_camera = FindObjectOfType<Camera>();
		}
	}
}

