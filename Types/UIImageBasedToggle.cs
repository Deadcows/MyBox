using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyBox
{
	public class UIImageBasedToggle : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField] private Sprite _checked;
		[SerializeField] private Sprite _checkedHighlighted;
#pragma warning restore 0649

		[Tooltip("Unique Toggle Event")] public UnityEvent Checked;


		private Sprite _regular;
		private Sprite _regularHighlighted;
		private Image _source;
		private Toggle _toggle;


		private void Start()
		{
			_source = GetComponent<Image>();
			_regular = _source.sprite;
			_toggle = GetComponent<Toggle>();
			_regularHighlighted = _toggle.spriteState.highlightedSprite;

			_toggle.onValueChanged.AddListener(value =>
			{
				_source.sprite = value ? _checked : _regular;
				var ss = _toggle.spriteState;
				ss.highlightedSprite = value ? _checkedHighlighted : _regularHighlighted;
				_toggle.spriteState = ss;
				if (value)
				{
					Checked.Invoke();
				}
			});

			if (_toggle.isOn)
				Checked.Invoke();

			_source.sprite = _toggle.isOn ? _checked : _regular;
			if (_toggle.isOn)
			{
				var newState = _toggle.spriteState;
				newState.highlightedSprite = _checkedHighlighted;
				_toggle.spriteState = newState;
			}
		}
	}
}