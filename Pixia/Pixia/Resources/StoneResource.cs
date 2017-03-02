using System;

using Pixia.Tiles;
using Pixia.Levels;

namespace Pixia.Resources
{
	public class StoneResource : Resource
	{
		public StoneResource()
		{
			this.ID = 3;

			this.attack = 1;

			this.durability = -1;
			this.effectiveType = Tile.TileType.None;

			this.placeable = true;
		}

		public override bool place(Level level, int xt, int yt)
		{
			return level.getTile(xt, yt).ID <= 0;
		}
	}
}
