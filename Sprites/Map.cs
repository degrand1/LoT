using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LoT
{
    class Map
    {
        private const int TILE_SIZE = 32;  // Width and height in pixels of each tile.
        private const UInt16 mapWidth = 64; // Width of the map in tiles
        private const UInt16 mapHeight = 64; // Height of the map in tiles

        // This array stores the textures that map to the ids stored in the grid.
        private static Texture2D[] tiles = new Texture2D[2];

        // Stores which tile should be drawn in the center of the screen.
        private Vector2 center = new Vector2(8.0f, 56.0f);

        //Stores the scale of the map, shared by all assets drawn to the map.
        private float scale = 1.5f;
        private Color color = new Color(10, 10, 10);

        // This is the 2D array that represents our map. Only two values are used currently:
        // O - An empty tile
        // 1 - A brick tile
        private UInt16[,] grid;

        private SpriteBatch batch;

        public Map()
        {
            // Create the 2D grid which will store the map data.
            grid = new UInt16[mapWidth, mapHeight];

            // Create an edge around the map
            for(int y = 0; y < mapHeight; y++)
            {
                grid[0, y] = 1;
                grid[mapWidth - 1, y] = 1;
            }
            for (int x = 0; x < mapHeight; x++)
            {
                grid[x, 0] = 1;
                grid[x, mapHeight - 1] = 1;
            }

            // Create a load of 6 tile wide platforms at random positions
            for (int platform = 0; platform < 100; platform++)
            {
                int platX = Global.rnd.Next(mapWidth - 6);
                int platY = Global.rnd.Next(mapHeight);
                for (int tile = 0; tile < 6; tile++)
                    grid[platX + tile, platY] = 1;
            }
        }

        public void LoadContent(ContentManager content)
        {
            tiles[1] = content.Load<Texture2D>("brick");
        }

        /// <summary>
        /// Render a texture onto the map using the given spritebatch and map position.
        /// </summary>
        /// <param name="batch">Spritebatch that renders the texture</param>
        /// <param name="tex">Texture to render</param>
        /// <param name="position">Map position where the texture will be rendered</param>
        public void DrawOnMap(SpriteBatch batch, Texture2D tex, Vector2 position)
        {
            if (tex == null)
                return;

            // Find the screen position for the texture
            Vector2 scrpos = WorldToScreen(position);

            batch.Draw(tex, scrpos, null, Color.White, 0f, Vector2.Zero, 
                this.scale, SpriteEffects.None, 0f);
        }

        public void DrawOnMap(SpriteBatch batch, Texture2D tex, Vector2 position, Color color)
        {
            if (tex == null)
                return;

            // Find the screen position for the texture
            Vector2 scrpos = WorldToScreen(position);

            batch.Draw(tex, scrpos, null, color, 0f, Vector2.Zero,
                this.scale, SpriteEffects.None, 0f);
        }

        public void DrawOnMap(SpriteBatch batch, Texture2D tex, Vector2 position, float scale)
        {
            if (tex == null)
                return;

            // Find the screen position for the texture
            Vector2 scrpos = WorldToScreen(position);
            scrpos.X -= tex.Width * scale / 2 - 20;
            scrpos.Y -= tex.Height * scale / 2 - 20;
            batch.Draw(tex, scrpos, null, Color.White, 0f, Vector2.Zero,
                scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Render the map to the screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            batch = spriteBatch;

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 bottomRight = new Vector2(ScreenManager.screenWidth, ScreenManager.screenHeight);
            Vector2 topLeftTile, bottomRightTile;

            batch.Begin(SpriteBlendMode.None);

            // Find the tiles at the top-left and bottom-right of the screen by passing the
            // appropriate coordinates to the ScreenToWorld() method.
            topLeftTile = ScreenToWorld(topLeft);
            bottomRightTile = ScreenToWorld(bottomRight);

            // Loop through the grid of tiles we know to be on the screen.
            for (int y = (int)topLeftTile.Y; y <= (int)bottomRightTile.Y; y++)
            {
                for (int x = (int)topLeftTile.X; x <= (int)bottomRightTile.X; x++)
                {
                    // Get the id of the tile at the current position.
                    UInt16 id = Grid(x, y);

                    // Render the correct texture at the current position.
                    DrawOnMap(batch, tiles[id], new Vector2((float)x, (float)y), color);
                }
            }
            batch.End();
        }

        /// <summary>
        /// Returns true if the given tile is solid
        /// </summary>
        public bool SolidSquare(int x, int y)
        {
            return (Grid(x, y) != 0);
        }

        /// <summary>
        /// Check if an object moving through the given space would have hit a wall.
        /// </summary>
        /// <param name="pos">Top left coordinate of the movement area</param>
        /// <param name="size">Size of the movement area</param>
        /// <param name="dirn">Direction we want to move</param>
        /// <param name="newpos">Return Value indicating how far through the area an object can move</param>
        /// <returns>If an object would hit a wall moving through this area</returns>
        public bool Collide(Vector2 pos, Vector2 size, Direction dirn, out float newpos)
        {
            // Find the bounding box for the given area as whole tiles
            float maxxf = (pos.X + size.X);
            float maxyf = (pos.Y + size.Y);
            int minx = (int)pos.X;
            int miny = (int)pos.Y;
            int maxX = (int)maxxf;
            int maxY = (int)maxyf;

            if (Global.IsInt(maxxf)) maxX--;
            if (Global.IsInt(maxyf)) maxY--;

            if (dirn == Direction.Right)
            {
                //We are travelling right, we will reach the right edge
                // of the given area if we don't hit anything.
                newpos = pos.X + size.X;

                //Scan through the tiles we're moving through from left to right
                for (int x = minx; x <= maxX; x++)
                {
                    for (int y = miny; y <= maxY; y++)
                    {
                        if (SolidSquare(x, y))
                        {
                            //Hit a wall, return the position of the left hand side of it
                            newpos = (float)x;
                            return true;
                        }
                    }
                }
                return false;
            }

            if (dirn == Direction.Left)
            {
                //We are travelling left, we will reach the left edge
                // of the given area if we don't hit anything.
                newpos = pos.X;

                // Scan through the tiles we're travelling through from right to left.
                for (int x = maxX; x >= minx; x--)
                {
                    for (int y = miny; y <= maxY; y++)
                    {
                        if (SolidSquare(x, y))
                        {
                            //Hit a wall, return the position of the right hand side of it
                            newpos = (float)(x + 1);
                            return true;
                        }
                    }
                }
                return false;
            }

            if (dirn == Direction.Down)
            {
                // We are travelling down, we will reach the bottom edge
                // of the given area if we don't hit anything
                newpos = pos.Y + size.Y;

                //Scan through the tiles we're travelling through from top to bottom.
                for (int y = miny; y <= maxY; y++)
                {
                    for (int x = minx; x <= maxX; x++)
                    {
                        if (SolidSquare(x, y))
                        {
                            //Hit a wall, return the position of the top of it
                            newpos = (float)y;
                            return true;
                        }
                    }
                }
                return false;
            }

            if (dirn == Direction.Up)
            {
                //We are travelling up, we will reach the top edge
                // of the given area if we don't hit anything.
                newpos = pos.Y;

                //Scan through the tiles we're travelling through from bottom to top.
                for (int y = maxY; y >= miny; y--)
                {
                    for (int x = minx; x <= maxX; x++)
                    {
                        if (SolidSquare(x, y))
                        {
                            //Hit a wall, return the position of the bottom of it.
                            newpos = (float)(y + 1);
                            return true;
                        }
                    }
                }
                return false;
            }

            //Should never reach here, just avoiding compiler warning
            newpos = 0.0f;
            return false;
        }

        /// <summary>
        /// Set the world coordinate which should be rendered in the center of the screen
        /// </summary>
        /// <param name="pos">World coordinate to center screen on</param>
        public void setViewPos(Vector2 pos)
        {
            // Only move towards the specified position, don't move directly there
            center.X += (pos.X - center.X) * 0.1f;
            center.Y += (pos.Y - center.Y) * 0.1f;
        }

        /// <summary>
        /// Returns the screen position matching the given map position
        /// </summary>
        private Vector2 WorldToScreen(Vector2 pos)
        {
            pos -= center;
            pos.X *= scale * TILE_SIZE;
            pos.Y *= scale * TILE_SIZE;
            pos.X += ScreenManager.screenWidth / 2;
            pos.Y += ScreenManager.screenHeight / 2;
            return pos;
        }

        private Vector2 WorldToScreen(Vector2 pos, float scale)
        {
            pos -= center;
            pos.X *= scale * TILE_SIZE;
            pos.Y *= scale * TILE_SIZE;
            pos.X += ScreenManager.screenWidth / 2;
            pos.Y += ScreenManager.screenHeight / 2;
            return pos;
        }

        /// <summary>
        /// Returns the world position matching the given screen position
        /// </summary>
        private Vector2 ScreenToWorld(Vector2 pos)
        {
            pos.X -= ScreenManager.screenWidth / 2;
            pos.Y -= ScreenManager.screenHeight / 2;
            pos.X /= scale * TILE_SIZE;
            pos.Y /= scale * TILE_SIZE;
            pos += center;
            return pos;
        }

        /// <summary>
        /// Returns the tile at the given position. Provides bounds checking.
        /// </summary>
        private UInt16 Grid(int x, int y)
        {
            // Check coordinate exists on map.
            if (x >= 0 && x < mapWidth &&
                y >= 0 && y < mapHeight)
                return grid[x, y];

            // All tiles outside the map are empty.
            return 1;
        }
    }
}
