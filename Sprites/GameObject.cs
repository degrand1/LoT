using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LoT
{
    class GameObject
    {
        protected TextureContainer texture; // Texture used for the object, set by derived class
        
        protected Vector2 position; //World position of object
        private Vector2 size = new Vector2(1.0f, 1.0f); //Size of the object
        protected Vector2 vector = Vector2.Zero; //Objects movement vector


        /// <summary>
        /// Update the object's position and velocity
        /// </summary>
        /// <param name="map">Map the object is on</param>
        public virtual void Update(Map map, GamePadState gamePadState, KeyboardState keyBoardState)
        {
            return;
        }

        public virtual void Draw(SpriteBatch batch, Map map)
        {
            map.DrawOnMap(batch, texture.texture, position);
        }

        /// <summary>
        /// Move the object's position based on the value of 'vector'.
        /// Object will be blocked by any walls
        /// </summary>
        /// <param name="map">Map the object is moving on</param>
        /// <returns>Set of flags indicating the directions the object hit a wall</returns>
        protected virtual Direction Move(Map map)
        {
            Direction retVal = Direction.None;
            Vector2 movement = vector;

            if (movement.X > 0.0f)
            {
                //We're moving to the right, create a bounding box in the area we're moving through.
                Vector2 collpos = new Vector2(position.X + size.X, position.Y);
                Vector2 collsize = new Vector2(movement.X, size.Y);
                float newX;

                //Check the area we want to move through against the map.
                if (map.Collide(collpos, collsize, Direction.Right, out newX))
                    retVal |= Direction.Right;
                // Update the object's position based on how far we could move.
                position.X = newX - size.X;
            }
            else if (movement.X < 0.0f)
            {
                // We're moving to the left, create a bounding box in the area we're moving through.
                Vector2 collPos = new Vector2(position.X + movement.X, position.Y);
                Vector2 collSize = new Vector2(-movement.X, size.Y);
                float newX;

                // Check the area we want to move through against the map.
                if (map.Collide(collPos, collSize, Direction.Left, out newX))
                    retVal |= Direction.Left;

                // Update the object's x position based on how far we could move.
                position.X = newX;
            }

            if (movement.Y > 0.0f)
            {
                //We're moving down, create a bounding box in the area we're moving through.
                Vector2 collPos = new Vector2(position.X, position.Y + size.Y);
                Vector2 collSize = new Vector2(size.X, movement.Y);
                float newY;

                if (map.Collide(collPos, collSize, Direction.Down, out newY))
                    retVal |= Direction.Down;
                
                //Update the object's y position based on how far we could move
                position.Y = newY - size.Y;
            }
            else if (movement.Y < 0.0f)
            {
                // We're moving up, create a bounding box in the area we're moving through
                Vector2 collPos = new Vector2(position.X, position.Y + movement.Y);
                Vector2 collSize = new Vector2(size.X, -movement.Y);
                float newY;

                // Check the area we want to move through against the map
                if (map.Collide(collPos, collSize, Direction.Up, out newY))
                    retVal |= Direction.Up;

                //Update the object's y position based on how far we could move.
                position.Y = newY;
            }

            // Return the set of flags indicating which directions we hit.
            return retVal;
        }
    }
}
