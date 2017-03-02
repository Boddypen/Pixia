using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Pixia.Levels;

namespace Pixia.Entities
{
	public class Player : Entity
	{
		public Player(Vector2 position, Vector2 size, Vector2 velocity)
		{
			this.position = position;
			this.size = size;
			this.velocity = velocity;

			// Player ID
			this.ID = 0;
		}

		public override void draw(SpriteBatch spriteBatch,
			Level level,
			int displacementX,
			int displacementY,
			Vector2 cameraPosition,
			Texture2D sheet)
		{
			spriteBatch.Draw(sheet,
				new Rectangle(
					(int)position.X + displacementX - (int)cameraPosition.X,
					(int)position.Y + displacementY - (int)cameraPosition.Y,
					(int)size.X,
					(int)size.Y),
				new Rectangle(
					112,
					0,
					16, 
					16),
				Color.White);
		}
	}
}
