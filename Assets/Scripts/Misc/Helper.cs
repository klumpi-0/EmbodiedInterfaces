using UnityEngine;

public static class Helper
{
    public static T FindComponentInObjectOrChildren<T>(GameObject obj, bool includeInactive = false) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        return obj.GetComponentInChildren<T>(includeInactive);
    }
}
