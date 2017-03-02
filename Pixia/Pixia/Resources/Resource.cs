using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pixia.Tiles;
using Pixia.Levels;

namespace Pixia.Resources
{
	public class Resource
	{
		public int ID;

		public int durability;
		public int attack;

		public Tile.TileType effectiveType;

		public Boolean placeable;
		public Boolean stackable;

		/// <summary>
		/// An item of the game. Can be a resource or tool.
		/// </summary>
		public Resource()
		{
			clear();
		}

		/// <summary>
		/// An item of the game. Can be a resource or tool.
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="Durability"></param>
		/// <param name="Attack Power"></param>
		/// <param name="Effective Against..."></param>
		public Resource(int ID, int durability, int attack, Tile.TileType effectiveType, Boolean stackable)
		{
			this.ID = ID;

			this.durability = durability;
			this.attack = attack;

			this.effectiveType = effectiveType;

			this.placeable = false;
			this.stackable = stackable;
		}

		/// <summary>
		/// This method is called when the resource is attempted to be 'placed'.
		/// Returns TRUE when the resource is successfully placed.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		/// <returns></returns>
		public virtual Boolean place(Level level, int xt, int yt)
		{
			return false;
		}

		/// <summary>
		/// This method is called when the resource is used on a tile (mining, chopping, etc.)
		/// This method returns TRUE if the resource has depleted all its durability and has broken.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		/// <returns></returns>
		public virtual Boolean use(Level level, int xt, int yt)
		{
			return false;
		}

		private void clear()
		{
			this.ID = 0;

			this.durability = -1;
			this.attack = 1;

			this.effectiveType = Tile.TileType.None;
			this.stackable = true;
		}

		/// <summary>
		/// Get the resource's name in String format.
		/// </summary>
		/// <param name="Resource ID"></param>
		/// <returns></returns>
		public static String getResourceName(int ID)
		{
			switch(ID)
			{
				case 0: return "N/A";
				case 1: return "Dirt";
				case 2: return "Log";
				case 3: return "Rock Shards";
				case 4: return "Wood Plank";
				
				default: return "N/A";
			}
		}

		/// <summary>
		/// Get the resource's description in String format.
		/// </summary>
		/// <param name="Resource ID"></param>
		/// <returns></returns>
		public static String getResourceDescription(int ID)
		{
			switch (ID)
			{
				case 0: return null;
				case 1: return "A pile of dirt.";
				case 2: return "A log from a tree, 5 paces long, stripped of its branches.";
				case 3: return "A pile of different sized shards of rock.";
				case 4: return "A plank of wood, a palm's width wide and 3 paces long.";
				
				default: return null;
			}
		}

		/// <summary>
		/// Draw the resource sprite onto the screen.
		/// </summary>
		/// <param name="SpriteBatch"></param>
		/// <param name="Main Spritesheet"></param>
		/// <param name="Sprite's Spritesheet Rectangle"></param>
		/// <param name="Screen Location"></param>
		public static void draw(SpriteBatch spriteBatch, Texture2D mainSheet, Rectangle spriteRectangle, Rectangle location)
		{
			spriteBatch.Draw(mainSheet,
				location,
				spriteRectangle,
				Color.White);
		}

		/// <summary>
		/// Returns the rectangle of the sprite of the main spritesheet.
		/// </summary>
		/// <param name="Resource ID"></param>
		/// <returns></returns>
		public static Rectangle getResourceSpriteRect(int ID)
		{
			switch(ID)
			{
				case 0: return new Rectangle(0, 0, 16, 16);
				case 1: return new Rectangle(128, 0, 16, 16);
				case 2: return new Rectangle(144, 0, 16, 16);
				case 3: return new Rectangle(160, 0, 16, 16);
				case 4: return new Rectangle(176, 0, 16, 16);  

				default: return new Rectangle(0, 0, 16, 16);
			}
		}
	}
}
