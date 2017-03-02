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
using Pixia.Tiles;
using Pixia.Resources;

namespace Pixia.Entities
{
	public class Entity
	{
		public Vector2 position, velocity;
		public Vector2 size;

		public int ID;
		public int age;
		public Resource dropResource;

		public Entity()
		{
			this.position = new Vector2(0.0F, 0.0F);
			this.velocity = Vector2.Zero;

			this.age = 0;
		}

		public Entity(int ID, Vector2 position, Vector2 size, Vector2 velocity, Resource dropResource)
		{
			this.ID = ID;
			this.age = 0;

			this.position = position;
			this.size = size;
			this.velocity = velocity;

			this.dropResource = dropResource;
		}

		/// <summary>
		/// Private method:
		/// Performs a movement tick.
		/// </summary>
		private void move(Level level)
		{
			// Change this to a lower value if the AABB is lagging
			int collisionExpanse = 16;

			// First, get the entity's rectangle for use in AABB
			Vector4 entityRectangle = new Vector4(position.X, position.Y, size.X, size.Y);

			// Now find out all the nearby tiles of the entity
			Rectangle nearby = level.getLocalBoundary(position,
													 (int)size.X * collisionExpanse,
													 (int)size.Y * collisionExpanse);
			
			#region AABB

			Boolean collidesHorizontal = false;
			Boolean collidesVertical = false;
			
			// Go through all nearby tiles to find out if the entity will collide or not
            for (int x = nearby.X; x < nearby.X + nearby.Width; x++)
			{
				for (int y = nearby.Y; y < nearby.Y + nearby.Height; y++)
				{
					if (level.getTile(x, y).isSolid())
					{
						// Check if the entity collides with the tile
						if (floatCollides(entityRectangle + new Vector4(velocity.X, 0, 0, 0),
							new Vector4(x * Tile.tileWidth,
										y * Tile.tileHeight,
										Tile.tileWidth,
										Tile.tileHeight)))
							collidesHorizontal = true;

						if (floatCollides(entityRectangle + new Vector4(0, velocity.Y, 0, 0),
							new Vector4(x * Tile.tileWidth,
										y * Tile.tileHeight,
										Tile.tileWidth,
										Tile.tileHeight)))
							collidesVertical = true;
					}
				}
            }

			// Create the rectangle of the entity one step into the future
			Vector4 future = new Vector4(entityRectangle.X + velocity.X,
										 entityRectangle.Y + velocity.Y,
										 entityRectangle.W,
										 entityRectangle.Z);
			/*
			if (collidesHorizontal && !collidesVertical)
			{
				if (velocity.X > 0)
					position.X = ((int)Math.Ceiling(position.X / Tile.tileWidth) * Tile.tileWidth) - size.X;
				else
					position.X = ((int)Math.Floor(position.X / Tile.tileWidth) * Tile.tileWidth);

				velocity.X = 0;
			}
			if (collidesVertical && !collidesHorizontal)
			{
				if (velocity.Y > 0)
					position.Y = (int)Math.Floor(position.Y / Tile.tileHeight) * Tile.tileHeight;
				else
					position.Y = (int)Math.Ceiling(position.Y / Tile.tileHeight) * Tile.tileHeight;

				velocity.Y = 0;
			}
			if (collidesHorizontal && collidesVertical)
			{
				velocity = Vector2.Zero;
			}
			if (!collidesVertical && !collidesHorizontal)
			{
				position += velocity;
			}
			*/

			#endregion
		}

		/// <summary>
		/// Find out whether or not two rectangles collide.
		/// </summary>
		/// <param name="First Rectangle"></param>
		/// <param name="Second Rectangle"></param>
		/// <returns></returns>
		private Boolean collides(Rectangle first, Rectangle second)
		{
			return (first.Intersects(second));
		}

		/// <summary>
		/// Find out whether or not the two vector-based rectangles collide.
		/// X = X Position
		/// Y = Y Position
		/// W = Width
		/// Z = Height
		/// </summary>
		/// <param name="First Rectangle"></param>
		/// <param name="Second Rectangle"></param>
		/// <returns></returns>
		private Boolean floatCollides(Vector4 first, Vector4 second)
		{
			return !(second.X > first.X + first.W
				  || second.X + second.W < first.X
				  || second.Y > first.Y + first.Z
				  || second.Y + first.Z < first.Y);

			//return (Math.Abs(first.X - second.X) * 2 < (first.W + second.W))
			//	&& (Math.Abs(first.Y - second.Y) * 2 < (first.Z + second.Z));

			//return !(first.X > second.X + second.W
			//	  || first.X + first.W < second.X
			//	  || first.Y > second.Y + second.Z
			//	  || first.Y + first.Z < second.Y);
		}

		/// <summary>
		/// Updates the entity.
		/// This will return TRUE if the entity dies.
		/// </summary>
		/// <param name="Level"></param>
		public virtual Boolean update(Level level)
		{
			// Move the entity
			this.move(level);
			velocity.Y += level.gravity;

			// Increase the entity's age
			age++;

			// Return false to say that the entity is not dead.
			return false;
		}

		/// <summary>
		/// Updates the entity.
		/// This also takes in a keyboard and mouse state, so the entity can respond to player input.
		/// </summary>
		/// <param name="Level"></param>
		/// <param name="Keyboard State"></param>
		/// <param name="Mouse State"></param>
		/// <returns></returns>
		public virtual Boolean update(Level level, KeyboardState k, KeyboardState kOld, MouseState m, MouseState mOld)
		{
			// Change the velocity based on the keyboard input
			if (k.IsKeyDown(Keys.A)) velocity.X -= 0.5F;
			if (k.IsKeyDown(Keys.D)) velocity.X += 0.5F;
			if (k.IsKeyDown(Keys.Space) && kOld.IsKeyUp(Keys.Space)) velocity.Y -= 1.0F;

			// Move the entity
			this.move(level);
			velocity.Y += level.gravity;

			// Increase the entity's age
			age++;

			// Return false to say that the entity is not dead
			return false;
		}

		/// <summary>
		/// Draw the entity onto the screen.
		/// </summary>
		/// <param name="SpriteBatch"></param>
		/// <param name="Level"></param>
		/// <param name="Displacement X"></param>
		/// <param name="Displacement Y"></param>
		/// <param name="Camera Position"></param>
		/// <param name="Main Spritesheet"></param>
		public virtual void draw(SpriteBatch spriteBatch,
			Level level,
			int displacementX, int displacementY,
			Vector2 cameraPosition,
			Texture2D sheet)
		{
			Rectangle entitySheetRect = getEntitySheetRect(this.ID);

			spriteBatch.Draw(sheet,
				new Rectangle(displacementX - (int)cameraPosition.X + (int)position.X,
					displacementY - (int)cameraPosition.Y + (int)position.Y,
					(int)size.X, (int)size.Y),
				new Rectangle(entitySheetRect.X * Tile.tileWidth,
					entitySheetRect.Y * Tile.tileHeight,
					entitySheetRect.Width * Tile.tileWidth,
					entitySheetRect.Height * Tile.tileHeight),
				Color.White);
		}

		/// <summary>
		/// Returns the position of the entity's texture from the main spritesheet.
		/// </summary>
		/// <param name="Entity ID"></param>
		/// <returns></returns>
		public static Rectangle getEntitySheetRect(int ID)
		{
			switch (ID)
			{
				case 0: return new Rectangle(0, 0, 1, 1);
				case 1: return new Rectangle(1, 0, 1, 1);

				default: return new Rectangle(0, 0, 1, 1);
			}
		}
	}
}
