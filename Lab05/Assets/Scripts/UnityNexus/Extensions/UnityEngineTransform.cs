using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class UnityEngineTransform
{
    public static Transform SearchForChild(this Transform self,string name)
    {
        if (name == null || name == "") return null;
        var children = new List<Transform>();
        GetAllChildren(self, children);
        foreach (var child in children)
            if (child.name == name) return child;
        return null;
    }
    public static List<Transform> GetAllChildren(this Transform self)
    {
        var children = new List<Transform>();
        GetAllChildren(self, children);
        return children;
    }
    public static void GetAllChildren(Transform currentTransform, List<Transform> children)
    {
        children.Add(currentTransform);
        if (currentTransform.childCount > 0)
        {
            for (int i = 0; i < currentTransform.childCount; i++)
                GetAllChildren(currentTransform.GetChild(i), children);
        }
    }
    // I know i need to move this but i dont' want to right now
    public static void RemoveComponent<T>(this GameObject obj) where T : Component
    {
        if (obj == null) return;
        T component = obj.GetComponent<T>();

        if (component != null)
            Object.Destroy(component);
    }
}
