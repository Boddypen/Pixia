using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pixia.Levels;
using Pixia.Resources;

namespace Pixia.Entities
{
	public class DropEntity : Entity
	{
		public DropEntity(Vector2 position, Vector2 velocity, Resource dropResource)
		{
			this.ID = 0;
			this.age = 0;

			this.size = new Vector2(1, 1);

			this.position = position;
			this.velocity = velocity;

			this.dropResource = dropResource;
		}

		public override void draw(SpriteBatch spriteBatch,
			Level level,
			int displacementX,
			int displacementY,
			Vector2 cameraPosition,
			Texture2D sheet)
		{
			spriteBatch.Draw(sheet,
				new Rectangle(displacementX + (int)position.X - (int)cameraPosition.X - 8,
							  displacementY + (int)position.Y - (int)cameraPosition.Y - 8,
							  16, 16),
				Resource.getResourceSpriteRect(dropResource.ID),
				Color.White);
		}
	}
}
