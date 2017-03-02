using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Pixia.Levels;

namespace Pixia.Tiles
{
	public class Tile
	{
		public static int tileWidth = 48, tileHeight = 48;
		
		public enum TileType
		{
			Soft,
			Hard,
			Wood,
			Living,
			None
		}

		public int ID;
		public int data;
		public int health;

		public String name;
		public Boolean solid;

		public TileType type;

		public Tile()
		{
			clear();
		}

		public Tile(int ID, int data, String name, Boolean solid, int health, TileType type)
		{
			clear();

			this.ID = ID;
			this.data = data;

			this.name = name;
			this.solid = solid;

			this.health = health;

			this.type = type;
		}

		private void clear()
		{
			this.ID = 0;
			this.data = 0;

			this.name = null;
			this.solid = false;

			this.health = 1;

			this.type = TileType.Hard;
		}

		/// <summary>
		/// This method is called when the tile is acted upon by a tool.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		public virtual void interact(Level level, int xt, int yt) { }

		/// <summary>
		/// Updates the tile.
		/// This will be called every tick.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		public virtual void update(Level level, int xt, int yt) { }
		
		public virtual String getName()
		{
			return name;
		}

		/// <summary>
		/// Returns whether or not the tile is a solid object.
		/// A tile that is solid will have collisions.
		/// </summary>
		/// <returns></returns>
		public virtual Boolean isSolid()
		{
			return solid;
		}

		/// <summary>
		/// The code here will run when the tile is destroyed.
		/// The item drops from any blocks must be put here.
		/// </summary>
		/// <param name="level"></param>
		public virtual void remove(Level level, int xt, int yt)
		{
			clear();
		}

		/// <summary>
		/// This draws the tile.
		/// </summary>
		/// <param name="SpriteBatch"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		/// <param name="Displacement X"></param>
		/// <param name="Displacement Y"></param>
		/// <param name="Camera Position"></param>
		/// <param name="Main Spritesheet"></param>
		public virtual void draw(SpriteBatch spriteBatch,
			int xt, int yt,
			int displacementX, int displacementY,
			Vector2 cameraPosition,
			Texture2D sheet)
		{
			if (this.ID == 0) // If air, don't do anything (to save resources)
				return;

			// Get the rectangle of the spritesheet for the tile
			Rectangle tileSheetRect = getTileSheetRect(this.ID);

			// Draw the tile
			spriteBatch.Draw(sheet,
				new Rectangle(displacementX - (int)cameraPosition.X + (xt * tileWidth),
					displacementY - (int)cameraPosition.Y + (yt * tileHeight),
					tileWidth,
					tileHeight),
				new Rectangle(tileSheetRect.X,
					tileSheetRect.Y,
					tileSheetRect.Width,
					tileSheetRect.Height),
				Color.White);
		}

		/// <summary>
		/// Returns the English name of the tile of the specified ID.
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public static String getTileName(int ID)
		{
			switch(ID)
			{
				case 0: return "Air";
				case 1: return "Dirt";
				case 2: return "Grass";
				case 3: return "Stone";
				case 4: return "Slab";
				case 5: return "Wood Planks";
				case 6: return "Starkstone";
				case 7: return "Stone Bricks";

				default: return "N/A";
			}
		}

		/// <summary>
		/// Returns the rectangle object which determines the tile's texture position on the main spritesheet.
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public static Rectangle getTileSheetRect(int ID)
		{
			switch(ID)
			{
				case 0: return new Rectangle(0, 0, 16, 16);
				case 1: return new Rectangle(16, 0, 16, 16);
				case 2: return new Rectangle(32, 0, 16, 16);
				case 3: return new Rectangle(48, 0, 16, 16);
				case 4: return new Rectangle(64, 0, 16, 16);
				case 5: return new Rectangle(80, 0, 16, 16);
				case 6: return new Rectangle(96, 0, 16, 16);
				case 7: return new Rectangle(112, 0, 16, 16);

				default: return new Rectangle(0, 0, 16, 16);
			}
		}
	}
}
