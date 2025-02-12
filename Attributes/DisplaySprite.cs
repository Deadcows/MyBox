using UnityEngine;


//Property Attribute
namespace MyBox {

    public class DisplaySprite : PropertyAttribute {
        public readonly int Width;
        public readonly int Height;


        //Set width/height
        public DisplaySprite(int width, int height) {
            Width = width;
            Height = height;
        }

    }

}

#if UNITY_EDITOR
namespace MyBox.Internal {
    using UnityEditor;


    [CustomPropertyDrawer(typeof(DisplaySprite))]
    public class DrawSpriteDrawer : PropertyDrawer {

        //Get Height of self
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            //If field isnt sprite, do default height
            if (fieldInfo.FieldType != typeof(Sprite)) {
                var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
                if (customDrawer != null) return customDrawer.GetPropertyHeight(property, label);

                return EditorGUI.GetPropertyHeight(property, label);
            }

            return ((DisplaySprite)attribute).Height;
        }

        //Draw GUI using provided Width/Height
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            if (property.isArray) WarningsPool.LogCollectionsNotSupportedWarning(property, nameof(DisplaySprite));

            if (fieldInfo.FieldType != typeof(Sprite)) WarningsPool.LogWarning("DisplaySprite does not support" + nameof(fieldInfo.FieldType));
            else {
                //Draw using width / height
                EditorGUI.ObjectField(new Rect(position.x + EditorGUIUtility.currentViewWidth / 2, position.y, ((DisplaySprite)attribute).Width, ((DisplaySprite)attribute).Height), property, typeof(Sprite), GUIContent.none);
                return;
            }
            //If property isnt sprite, do default 
            var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
            if (customDrawer != null) customDrawer.OnGUI(position, property, label);
            else EditorGUI.PropertyField(position, property, label, true);



        }






    }
}
#endif
