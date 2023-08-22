using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Aqua
{
    // Simple static class that contains some frequently used functions
    public static class Utils
    {
        public static Vector2 GetViewportCenter(Viewport viewport)
        {
            return new Vector2(viewport.X + viewport.Width / 2, viewport.Y + viewport.Height / 2);
        }

        public static Rectangle GetViewportRectangle(Viewport viewport)
        {
            return new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        public static bool IsWithin<T>(this T value, T minimum, T maximum) where T : IComparable<T>
        {
            if (value.Equals(minimum) || value.Equals(maximum)) return false;

            if (value.CompareTo(minimum) < 0 )
                return false;
            if (value.CompareTo(maximum) > 0 )
                return false;
            
            return true;
        }

    }
}
