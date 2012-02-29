using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace LoT
{
    //A set of flags indicating four different directions.
    public enum Direction
    {
        None = 0x0000,
        Up = 0x0001,
        Right = 0x0002,
        Down = 0x0004,
        Left = 0x0008
    };

    /// <summary>
    /// A texture container that allows passing around the texture while it is null. Once loaded,
    /// anyone with a reference to the container can access the texture
    /// </summary>
    class TextureContainer
    {
        public Texture2D texture;
    };
    static class Global
    {
        public static Random rnd = new Random();

        /// <summary>
        /// Used to assist rounding so we can figure out which tile we're coliding with
        /// </summary>
        /// <param name="val">Value to be rounded</param>
        /// <param name="amount">How close we can be to zero before we're consided zero</param>
        /// <returns>The rounded value</returns>
        public static float TendToZero(float val, float amount)
        {
            if (val > 0f && (val -= amount) < 0f) return 0f;
            if (val < 0f && (val += amount) > 0f) return 0f;
            return val;
        }

        /// <summary>
        /// Checks to see if casting a float to an int would round the data
        /// </summary>
        /// <param name="value">Float to be checked</param>
        /// <returns>Resulting value</returns>
        public static bool IsInt(float value)
        {
            return ((float)((int)value) == value);
        }
    }
}
