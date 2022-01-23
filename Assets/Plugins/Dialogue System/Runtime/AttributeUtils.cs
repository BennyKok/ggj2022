using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Blaze.Dialogue
{
    public class CollapsedEventAttribute : PropertyAttribute
    {
        public bool visible;

        public CollapsedEventAttribute()
        {

        }
    }

    public enum HelpBoxMessageType { None, Info, Warning, Error }

    public class HelpAttribute : PropertyAttribute
    {
        public string text;
        public HelpBoxMessageType messageType;

        public HelpAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None)
        {
            this.text = text;
            this.messageType = messageType;
        }
    }

    public class TitleAttribute : PropertyAttribute
    {
        public string text;

        public TitleAttribute(string text)
        {
            this.text = text;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(CollapsedEventAttribute))]
    public class CollapsedEventDrawer : UnityEventDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUI.BeginProperty(position, label, property);

            EditorGUI.indentLevel++;

            var attr = this.attribute as CollapsedEventAttribute;

            position.height = EditorGUIUtility.singleLineHeight;
            var temp = new GUIContent(label);

            SerializedProperty persistentCalls = property.FindPropertyRelative("m_PersistentCalls.m_Calls");
            if (persistentCalls != null)
                temp.text += " (" + persistentCalls.arraySize + ")";

#if UNITY_2019_1_OR_NEWER
            attr.visible = EditorGUI.BeginFoldoutHeaderGroup(position, attr.visible, temp);
#else
            attr.visible = EditorGUI.Foldout(position, attr.visible, temp, true);
#endif
            if (attr.visible)
            {
                label.text = null;
                position.height = base.GetPropertyHeight(property, label);
                position.y += EditorGUIUtility.singleLineHeight;
                base.OnGUI(position, property, label);
            }
#if UNITY_2019_1_OR_NEWER
            EditorGUI.EndFoldoutHeaderGroup();
#endif
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = this.attribute as CollapsedEventAttribute;

            return attr.visible ? base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(HelpAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            var helpBoxAttribute = attribute as HelpAttribute;
            if (helpBoxAttribute == null) return base.GetHeight();
            var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
            if (helpBoxStyle == null) return base.GetHeight();
            return helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 8;
        }

        public override void OnGUI(Rect position)
        {
            var helpBoxAttribute = attribute as HelpAttribute;
            if (helpBoxAttribute == null) return;
            position.y += 4;
            position.height -= 8;
            EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
        }

        private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
        {
            switch (helpBoxMessageType)
            {
                default:
                case HelpBoxMessageType.None: return MessageType.None;
                case HelpBoxMessageType.Info: return MessageType.Info;
                case HelpBoxMessageType.Warning: return MessageType.Warning;
                case HelpBoxMessageType.Error: return MessageType.Error;
            }
        }
    }

    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleAttributeDrawer : DecoratorDrawer
    {
        private static GUIStyle titleStyle;

        public override float GetHeight()
        {
            Init();

            var titleAttribute = attribute as TitleAttribute;
            if (titleAttribute == null) return base.GetHeight();
            return titleStyle.CalcHeight(new GUIContent(titleAttribute.text), EditorGUIUtility.currentViewWidth) + 16;
        }

        public override void OnGUI(Rect position)
        {
            Init();

            var titleAttribute = attribute as TitleAttribute;
            if (titleAttribute == null) return;
            position.y += 12;
            position.height -= 16;
            EditorGUI.LabelField(position, titleAttribute.text, titleStyle);
        }

        private void Init()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle("box");
                titleStyle.active.textColor = EditorStyles.label.active.textColor;
                // titleStyle.font = EditorStyles.boldFont;
            }
        }
    }

#endif

}
