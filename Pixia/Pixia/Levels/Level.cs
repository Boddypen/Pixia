using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Pixia.Tiles;
using Pixia.Entities;

namespace Pixia.Levels
{
	public class Level
	{
		private Random random;

		public Tile[,] tiles;

		public List<Entity> entities;
		public Player player;

		public int width, height;
		public float gravity;
		
		public Boolean genDone = false;

		public Level(int sizeWidth, int sizeHeight, float gravity)
		{
			// RNG
			random = new Random();

			// Set the level's size variables
			this.width = sizeWidth;
			this.height = sizeHeight;

			// Set the level's gravity
			this.gravity = gravity;

			// Create the array of tiles in the world
			this.tiles = new Tile[sizeWidth, sizeHeight];

			// Set up the list variable for the entities
			this.entities = new List<Entity>();
		}

		private void generate()
		{
			Thread.Sleep(1000);

			// Create the tiles
			int genHeight = height / 2;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (y == genHeight)
					{
						this.tiles[x, y] = new GrassTile(0, random);
					}
					else if (y > genHeight)
					{
						if (y < genHeight + 4 + random.Next(3))
						{
							this.tiles[x, y] = new DirtTile(0, random);
						}
						else
						{
							this.tiles[x, y] = new StoneTile(0, random);
						}
					}
					else
					{
						this.tiles[x, y] = new Tile();
					}
				}

				if (random.Next(0, 2) == 0)
					genHeight += random.Next(2) - random.Next(2);

				// Cliffs
				if (random.Next(0, 20) == 0)
					genHeight += random.Next(8) - random.Next(8);

				// Make sure the gen height stays within the boundaries of the world
				if (genHeight < 0) genHeight = 0;
				if (genHeight > height - 1) genHeight = height - 1;
			}

			genDone = true;
		}

		public void generateLevel()
		{
			Thread genThread = new Thread(generate);
			genThread.Start();
		}
		
		public void update(Level level, Game.GameState gameState, KeyboardState k, KeyboardState kOld, MouseState m, MouseState mOld)
		{
			// Update the player
			if(player.update(level, k, kOld, m, mOld))
			{
				// Player dies
				gameState = Game.GameState.Menu;
			}

			// Update all entities
			for(int i = 0; i < entities.Count; i++)
			{
				// Update the entity, and if it dies, remove it.
				if(entities[i].update(level))
				{
					entities.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// This will update every block in the area around the player.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Camera Position"></param>
		/// <param name="Displacement X"></param>
		/// <param name="Displacement Y"></param>
		public void tick(Level level, Vector2 cameraPosition, int displacementX, int displacementY)
		{
			// Find the bounds of the local area
			Rectangle bounds = getLocalBoundary(cameraPosition, displacementX, displacementY);

			// Go through the level and update every block
			for (int x = bounds.X; x < bounds.Width; x++)
				for (int y = bounds.Y; y < bounds.Height; y++)
					tiles[x, y].update(level, x, y);
		}

		public Rectangle getLocalBoundary(Vector2 position, int displacementX, int displacementY)
		{
			Rectangle bounds;

			bounds = new Rectangle((int)(position.X / Tile.tileWidth) - (displacementX / Tile.tileWidth),
							  	   (int)(position.Y / Tile.tileHeight) - (displacementY / Tile.tileHeight),
								   ((displacementX) / Tile.tileWidth) * 2,
								   ((displacementY) / Tile.tileHeight) * 2);

			if (bounds.X < 0) bounds.X = 0;
			if (bounds.Y < 0) bounds.Y = 0;

			return bounds;			
        }

		/// <summary>
		/// Add an entity to the level.
		/// </summary>
		/// <param name="Entity"></param>
		public void addEntity(Entity e)
		{
			entities.Add(e);
		}

		/// <summary>
		/// Set a particular tile in the level to something else.
		/// </summary>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		/// <param name="Tile"></param>
		public void setTile(int x, int y, Tile tile)
		{
			if (x >= 0 && x < tiles.GetLength(0)
				&& y >= 0 && y < tiles.GetLength(1))
			{
				tiles[x, y] = tile;
			}
			else return;
		}

		/// <summary>
		/// Returns the tile at the specified location in the level.
		/// </summary>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		/// <returns></returns>
		public Tile getTile(int x, int y)
		{
			if (x >= 0 && x < width - 1
				&& y >= 0 && y < height - 1)
			{
				return tiles[x, y];
			}
			else
			{
				// If outside the bounds of the level, return an empty tile.
				return new Tile();
			}
		}

		/// <summary>
		/// Remove a tile from the level.
		/// This will harvest all the resources from the tile. 
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		public void removeTile(Level level, int x, int y)
		{
			if(tiles[x, y].ID > 0)
			{
				tiles[x, y].remove(level, x, y);
				tiles[x, y] = new Tile();
			}
		}

		/// <summary>
		/// Draw the level.
		/// This will draw all the tiles onto the screen, along with all the level's entities and whatnot.
		/// </summary>
		/// <param name="SpriteBatch"></param>
		/// <param name="Displacement X"></param>
		/// <param name="Displacement Y"></param>
		/// <param name="Camera Position"></param>
		/// <param name="Main Spritesheet"></param>
		public void draw(SpriteBatch spriteBatch,
			int displacementX, int displacementY,
			Vector2 cameraPosition,
			Texture2D mainSheet,
			Level level)
		{
			// Draw the tiles of the level
			//for (int x = 0; x < this.width; x++)
			for (int x = (int)(cameraPosition.X / Tile.tileWidth) - (displacementX / Tile.tileWidth) - 2;
					 x < (int)(cameraPosition.X / Tile.tileWidth) + (displacementX / Tile.tileWidth) + 2;
					 x++)
			{
				//for (int y = 0; y < this.height; y++)
				for (int y = (int)(cameraPosition.Y / Tile.tileHeight) - (displacementY / Tile.tileHeight) - 2;
						 y < (int)(cameraPosition.Y / Tile.tileHeight) + (displacementY / Tile.tileHeight) + 2;
						 y++)
				{
					getTile(x, y).draw(spriteBatch,
						x, y,
						displacementX,
						displacementY,
						cameraPosition,
						mainSheet);
				}
			}

			// Draw the entities of the level
			for(int i = 0; i < entities.Count; i++)
			{
				entities[i].draw(spriteBatch,
					level,
					displacementX,
					displacementY,
					cameraPosition,
					mainSheet);
			}
		}
	}
}
