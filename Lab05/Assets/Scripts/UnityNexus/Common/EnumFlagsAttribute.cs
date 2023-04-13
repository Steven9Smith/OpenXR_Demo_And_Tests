using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
namespace UnityNexus.Common
{
    public sealed class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }

        public static List<int> GetSelectedIndexes<T>(T val) where T : IConvertible
        {
            List<int> selectedIndexes = new List<int>();
            for (int i = 0; i < System.Enum.GetValues(typeof(T)).Length; i++)
            {
                int layer = 1 << i;
                if ((Convert.ToInt32(val) & layer) != 0)
                {
                    selectedIndexes.Add(i);
                }
            }
            return selectedIndexes;
        }
        public static List<string> GetSelectedStrings<T>(T val) where T : IConvertible
        {
            List<string> selectedStrings = new List<string>();
            for (int i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
            {
                int layer = 1 << i;
                if ((Convert.ToInt32(val) & layer) != 0)
                {
                    selectedStrings.Add(Enum.GetValues(typeof(T)).GetValue(i).ToString());
                }
            }
            return selectedStrings;
        }

        public static uint EnumToBitmask(List<int> indicies)
        {
            if (indicies == null || indicies.Count == 0) return 0u;
            else
            {
                uint mask = 1u << indicies[0];
                for (int i = 1; i < indicies.Count; i++)
                    mask = mask | (1u << indicies[i]);
                return mask;
            }
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
#endif
}