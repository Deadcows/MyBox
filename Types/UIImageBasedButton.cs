using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyBox
{
	public class UIImageBasedButton : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
#pragma warning disable 0649
		[SerializeField, MustBeAssigned] private Sprite _regularSprite;
		[SerializeField, MustBeAssigned] private Sprite _regularSelectedSprite;
		[SerializeField, MustBeAssigned] private Sprite _clickedSprite;
		[SerializeField, MustBeAssigned] private Sprite _clickedSelectedSprite;
#pragma warning restore 0649


		public bool AlternativeSpriteset
		{
			get { return _alternative; }
			set { _alternative = value; }
		}

		private bool _alternative;
		private bool _selected;
		private Image _image;
		private Button _button;
		private UnityAction _event;


		private void Awake()
		{
			_image = GetComponent<Image>();
			_button = GetComponent<Button>();
			_event = ToggleSprites;
		}

		private void OnEnable()
		{
			_button.onClick.AddListener(_event);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveListener(_event);
		}

		private void ToggleSprites()
		{
			_alternative = !_alternative;
			UpdateSprites();
		}


		private void UpdateSprites()
		{
			_image.sprite = !_alternative ? !_selected ? _regularSprite : _regularSelectedSprite :
				!_selected ? _clickedSprite : _clickedSelectedSprite;
		}

		private void UpdateSprites(bool selected)
		{
			_selected = selected;
			UpdateSprites();
		}


		public void OnSelect(BaseEventData eventData)
		{
			UpdateSprites(true);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			UpdateSprites(false);
		}
	}
}