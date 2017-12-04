using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Utility
    {
        
        public static Rect SetWidth(this Rect current, float width)
        {
            return new Rect(current.x, current.y, width, current.height);
        }
        public static Rect SetHeight(this Rect current, float height)
        {
            return new Rect(current.x, current.y, current.width, height);
        }
        public static Rect SetX(this Rect current, float newX)
        {
            return new Rect(newX, current.y, current.width, current.height);
        }
        public static Rect SetY(this Rect current, float newY)
        {
            return new Rect(current.x, newY, current.width, current.height);
        }

        public static GameObject GetRootParent(this GameObject gameObject)
        {
            var current = gameObject.transform;
            while (current.transform.parent != null)
            {
                current = current.transform.parent;
            }
            return current.gameObject;
        }
    }

    public class Pair<S, U>
    {
        public S first;
        public U second;
        
        public Pair(S first, U second)
        {
            this.first = first;
            this.second = second;
        }
    }
    
    public class Tuples {
        
        public static Pair<S,U> pair<S, U>(S first, U second)
        {
            return new Pair<S, U>(first, second);
        }
    }
    
}