using System;

using Microsoft.Xna.Framework;

using Pixia.Levels;
using Pixia.Tiles;
using Pixia.Entities;
using Pixia.Resources;

namespace Pixia.Tiles
{
	public class DirtTile : Tile
	{
		private Random random;

		/// <summary>
		/// Moist soil, great for growing crops on.
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="Random Number Generator"></param>
		public DirtTile(int data, Random random)
		{
			this.random = random;

			this.ID = 1;
			this.data = data;

			this.name = "Dirt";
			this.solid = true;

			this.health = 12;

			this.type = TileType.Soft;
		}

		/// <summary>
		/// If there is open air above the dirt tile, have a small chance of grass growing.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		public override void update(Level level, int xt, int yt)
		{
			if(random.Next(0, 1500) == 0)
			{
				if (!level.getTile(xt, yt - 1).isSolid())
					level.setTile(xt, yt, new GrassTile(0, random));
			}
		}

		public override void remove(Level level, int xt, int yt)
		{
			level.addEntity(new DropEntity(new Vector2((xt * Tile.tileWidth) + (Tile.tileWidth / 2),
													   (yt * Tile.tileHeight) + (Tile.tileHeight / 2)),
				new Vector2((random.Next(0, 10) - random.Next(0, 10)) / 5.0F,
							(random.Next(0, 10) - random.Next(0, 10)) / 5.0F),
				new DirtResource()));
		}
	}
}
