using System;

using Microsoft.Xna.Framework;

using Pixia.Entities;
using Pixia.Levels;
using Pixia.Resources;

namespace Pixia.Tiles
{
	public class WoodPlankTile : Tile
	{
		private Random random;

		/// <summary>
		/// Planks, laid together in a pattern, coated in a thin layer of varnish.
		/// </summary>
		/// <param name="Data"></param>
		public WoodPlankTile(int data, Random random)
		{
			this.random = random;

			this.ID = 5;
			this.data = data;

			this.name = "Wood Planks";
			this.solid = true;

			this.health = 25;

			this.type = TileType.Wood;
		}

		public override void remove(Level level, int xt, int yt)
		{
			level.addEntity(new DropEntity(new Vector2((xt * Tile.tileWidth) + (Tile.tileWidth / 2),
													   (yt * Tile.tileHeight) + (Tile.tileHeight / 2)),
				new Vector2((random.Next(0, 10) - random.Next(0, 10)) / 5,
							(random.Next(0, 10) - random.Next(0, 10)) / 5),
				new PlankResource()));
		}
	}
}
