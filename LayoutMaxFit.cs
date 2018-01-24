using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Set max width to layout element with text child
/// </summary>
[RequireComponent(typeof(LayoutElement))]
//[ExecuteInEditMode]
public class LayoutMaxFit : UIBehaviour, ILayoutElement
{

	public float MaxWidth;
	
	[SerializeField] private GameObject _calculateBy;

	private ILayoutElement _layoutToFit;
	private LayoutElement _layout;
	private LayoutGroup _group;

	public void CalculateLayoutInputHorizontal()
	{
		if (_layoutToFit == null)
		{
			if (_calculateBy != null)
			{
				_layoutToFit = _calculateBy.GetComponent<ILayoutElement>();
			}
			else
			{
				var childUI = GetComponentsInChildren<UIBehaviour>().FirstOrDefault(e => e.transform != transform);
				if (childUI != null) _layoutToFit = childUI as ILayoutElement;
			}
		}
#if UNITY_EDITOR
		if ((_layoutToFit as MonoBehaviour) == null)
		{
			_layoutToFit = null;
			return;
		}
#endif
		if (_layout == null) _layout = GetComponent<LayoutElement>();
		if (_layout == null) return;
		if (_group == null) _group = GetComponent<LayoutGroup>();

		var newWidth = _layoutToFit.preferredWidth > MaxWidth ? MaxWidth : _layoutToFit.preferredWidth;
		var padding = _group != null ? _group.padding.left + _group.padding.right : 0f;
		newWidth += padding;
		if (!Mathf.Approximately(newWidth, preferredWidth))
		{
			_layout.preferredWidth = newWidth;
		}

	}

	public void CalculateLayoutInputVertical()
	{
	}
	
	public float minWidth { get; private set; }
	public float preferredWidth { get; private set; }
	public float flexibleWidth { get; private set; }
	public float minHeight { get; private set; }
	public float preferredHeight { get; private set; }
	public float flexibleHeight { get; private set; }
	public int layoutPriority { get; private set; }

}
