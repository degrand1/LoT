using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LoT
{
    class Player : GameObject
    {
        private const float PLAYER_DECEL = 0.02f; //The amount the player slows down
        private const float PLAYER_ACCEL = 0.008f; // The amount the player accelerates by
        private const float PLAYER_MAX_SPEED = 0.35f; // Fastest player can move

        private static TextureContainer image = new TextureContainer();
        private static Texture2D light;

        float scale = 15.0f;
        bool visible = true;

        public Player()
        {
            base.texture = image;

            //Hard code the initial position of the player
            position.X = 5;
            position.Y = 62;
        }

        public void LoadContent(ContentManager content)
        {
            image.texture = content.Load<Texture2D>("player");
            light = content.Load<Texture2D>("Glow");

        }

        /// <summary>
        /// Move the player around the given map based on the state of the game pad.
        /// </summary>
        /// <param name="map">Map to move the player</param>
        public override void Update(Map map, GamePadState gamePadState, KeyboardState keyboardState)
        {
            //Move the player and detect any wall collisions.
            Direction hitdirn = base.Move(map);

            // If we hit a direction horizontally set the vector's x component to 0
            if ((hitdirn & Direction.Left) != 0 || (hitdirn & Direction.Right) != 0)
                vector.X = 0f;

            // If we hit a direction vertically set the vector's y component to 0
            if ((hitdirn & Direction.Up) != 0 || (hitdirn & Direction.Down) != 0)
                vector.Y = 0f;

            // Check if the player is pushing left or right
            if (gamePadState.ThumbSticks.Left.X > 0.5f || keyboardState.IsKeyDown(Keys.D))
            {
                // Player is pushing right
                vector.X += PLAYER_ACCEL + PLAYER_DECEL;

                // Prevent X vector from going above the maximum speed.
                if (vector.X > PLAYER_MAX_SPEED)
                    vector.X = PLAYER_MAX_SPEED;
            }
            else if (gamePadState.ThumbSticks.Left.X < -0.5f || keyboardState.IsKeyDown(Keys.A))
            {
                // Player is pushing left
                vector.X -= PLAYER_ACCEL + PLAYER_DECEL;

                // Prevent X vector from going above the maximum speed.
                if (vector.X < -PLAYER_MAX_SPEED)
                    vector.X = -PLAYER_MAX_SPEED;
            }

            if (gamePadState.ThumbSticks.Left.Y > 0.5f || keyboardState.IsKeyDown(Keys.W))
            {
                vector.Y -= PLAYER_ACCEL + PLAYER_DECEL;

                // Prevent Y vector from going above the maximum speed.
                if (vector.Y < -PLAYER_MAX_SPEED)
                    vector.Y = -PLAYER_MAX_SPEED;
            }
            else if (gamePadState.ThumbSticks.Left.Y < -0.5f || keyboardState.IsKeyDown(Keys.S))
            {
                vector.Y += PLAYER_ACCEL + PLAYER_DECEL;

                // Prevent Y vector from going above the maximum speed.
                if (vector.Y > PLAYER_MAX_SPEED)
                    vector.Y = PLAYER_MAX_SPEED;
            }

            // Tend the x vector towards zero so we stop when the player isn't pushing on the joystick
            vector.X = Global.TendToZero(vector.X, PLAYER_DECEL);
            vector.Y = Global.TendToZero(vector.Y, PLAYER_DECEL);

            // Update the position of the map so it centers on the player.
            map.setViewPos(position);

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                scale += 0.1f;
                if (!visible)
                    visible = true;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                scale -= 0.1f;
                if (scale <= 0.0f)
                {
                    scale = 0.1f;
                    visible = false;
                }
            }
        }

        public void DrawLight(SpriteBatch batch, Map map)
        {
            if(visible)
                map.DrawOnMap(batch, light, position, scale);
        }
    }
}
