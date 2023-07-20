namespace MyBox
{
  [CustomPropertyDrawer(typeof(RangeVectorAttribute))]
  public class RangeVectorDrawer : PropertyDrawer
  {
    #region fields

    private const int HELP_HEIGHT = 24;

    #endregion

    #region properties

    private RangeVectorAttribute rangeVectorAttribute => attribute as RangeVectorAttribute;

    #endregion

    #region methods

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      HandleChanges(position, property, label);
    }

    private void HandleChanges(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginChangeCheck();

      switch (property.propertyType)
      {
        case SerializedPropertyType.Vector2:
        {
          var val = EditorGUI.Vector2Field(position, label, property.vector2Value);

          if (EditorGUI.EndChangeCheck())
          {
            val.x = Mathf.Clamp(val.x, rangeVectorAttribute.min.x, rangeVectorAttribute.max.x);
            val.y = Mathf.Clamp(val.y, rangeVectorAttribute.min.y, rangeVectorAttribute.max.y);
            property.vector2Value = val;
          }

          break;
        }
        case SerializedPropertyType.Vector2Int:
        {
          var val = EditorGUI.Vector2IntField(position, label, property.vector2IntValue);

          if (EditorGUI.EndChangeCheck())
          {
            val.x = Mathf.RoundToInt(Mathf.Clamp(val.x, rangeVectorAttribute.min.x, rangeVectorAttribute.max.x));
            val.y = Mathf.RoundToInt(Mathf.Clamp(val.y, rangeVectorAttribute.min.y, rangeVectorAttribute.max.y));
            property.vector2IntValue = val;
          }

          break;
        }
        case SerializedPropertyType.Vector3:
        {
          var val = EditorGUI.Vector3Field(position, label, property.vector3Value);

          if (EditorGUI.EndChangeCheck())
          {
            val.x = Mathf.Clamp(val.x, rangeVectorAttribute.min.x, rangeVectorAttribute.max.x);
            val.y = Mathf.Clamp(val.y, rangeVectorAttribute.min.y, rangeVectorAttribute.max.y);
            val.z = Mathf.Clamp(val.z, rangeVectorAttribute.min.z, rangeVectorAttribute.max.z);
            property.vector3Value = val;
          }

          break;
        }
        case SerializedPropertyType.Vector3Int:
        {
          var val = EditorGUI.Vector3IntField(position, label, property.vector3IntValue);

          if (EditorGUI.EndChangeCheck())
          {
            val.x = Mathf.RoundToInt(Mathf.Clamp(val.x, rangeVectorAttribute.min.x, rangeVectorAttribute.max.x));
            val.y = Mathf.RoundToInt(Mathf.Clamp(val.y, rangeVectorAttribute.min.y, rangeVectorAttribute.max.y));
            val.z = Mathf.RoundToInt(Mathf.Clamp(val.z, rangeVectorAttribute.min.z, rangeVectorAttribute.max.z));
            property.vector3IntValue = val;
          }

          break;
        }
        default:
          var helpPosition = position;
          helpPosition.height = HELP_HEIGHT;
          EditorGUI.HelpBox(helpPosition, "Use RangeVector with Vector2, Vector2Int, Vector3 or Vector3Int.",
            MessageType.Error);
          break;
      }
    }

    #endregion
  }
}