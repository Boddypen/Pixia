using System;

using Pixia.Levels;
using Pixia.Tiles;
using Pixia.Entities;

namespace Pixia.Resources
{
	public class PlankResource : Resource
	{
		public PlankResource()
		{
			this.ID = 4;

			this.attack = 2;

			this.durability = -1;
			this.effectiveType = Tile.TileType.Living;

			this.placeable = true;
		}

		public override bool place(Level level, int xt, int yt)
		{
			return level.getTile(xt, yt).ID <= 0;
		}
	}
}
