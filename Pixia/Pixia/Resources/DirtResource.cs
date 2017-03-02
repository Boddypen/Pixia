using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pixia.Levels;
using Pixia.Tiles;

namespace Pixia.Resources
{
	public class DirtResource : Resource
	{
		public DirtResource()
		{
			this.ID = 1;

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
