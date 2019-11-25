using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Origin.Extensions
{
    static class OtherExtensions
    {
        public static Rectangle ToRectangle(this Point point, int width, int height)
        {
            return new Rectangle(point.X - width / 2, point.Y - height / 2, width, height);
        }

        public static TValue ComputeIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> ifNotThere)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, ifNotThere());
            }    
            return dictionary[key];
            
        }
    }
}