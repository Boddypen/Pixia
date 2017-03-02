using System;

using Microsoft.Xna.Framework;

using Pixia.Levels;
using Pixia.Entities;
using Pixia.Resources;

namespace Pixia.Tiles
{
	public class StoneTile : Tile
	{
		private Random random;

		/// <summary>
		/// Raw stone from the depths of the planet.
		/// </summary>
		/// <param name="data"></param>
		public StoneTile(int data, Random random)
		{
			this.random = random;

			this.ID = 3;
			this.data = data;

			this.name = "Stone";
			this.solid = true;

			this.health = 30;

			this.type = TileType.Hard;
		}

		public override void remove(Level level, int xt, int yt)
		{
			level.addEntity(new DropEntity(new Vector2((xt * Tile.tileWidth) + (Tile.tileWidth / 2),
													   (yt * Tile.tileHeight) + (Tile.tileHeight / 2)),
				new Vector2((random.Next(0, 10) - random.Next(0, 10)) / 5,
							(random.Next(0, 10) - random.Next(0, 10)) / 5),
				new StoneResource()));
		}
	}
}
