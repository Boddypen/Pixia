using System;

using Microsoft.Xna.Framework;

using Pixia.Levels;
using Pixia.Resources;
using Pixia.Entities;

namespace Pixia.Tiles
{
	public class GrassTile : Tile
	{
		private Random random;

		/// <summary>
		/// A block of dirt, with lush grass growing out the top.
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="Random Number Generator"></param>
		public GrassTile(int data, Random random)
		{
			this.random = random;

			this.ID = 2;
			this.data = data;

			this.name = "Grass";
			this.solid = true;

			this.health = 15;

			this.type = TileType.Soft;
		}

		/// <summary>
		/// If there's a solid tile above this grass tile, have a small chance of killing the grass.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Tile X"></param>
		/// <param name="Tile Y"></param>
		public override void update(Level level, int xt, int yt)
		{
			if (level.getTile(xt, yt - 1).isSolid())
			{
				if (random.Next(0, 500) == 0)
				{
					level.setTile(xt, yt, new DirtTile(0, random));
				}
			}
		}

		public override void remove(Level level, int xt, int yt)
		{
			level.addEntity(new DropEntity(new Vector2((xt * Tile.tileWidth) + (Tile.tileWidth / 2),
													   (yt * Tile.tileHeight) + (Tile.tileHeight / 2)),
				new Vector2((random.Next(0, 10) - random.Next(0, 10)) / 5,
							(random.Next(0, 10) - random.Next(0, 10)) / 5),
				new DirtResource()));
		}
	}
}
