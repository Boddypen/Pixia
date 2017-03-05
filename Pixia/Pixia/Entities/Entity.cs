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

		public int onGround = 0;

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
			int collisionExpanse = 8;

			// First, get the entity's rectangle for use in AABB
			Vector4
				entityRectangle = new Vector4(position.X,
											  position.Y,
											  size.X,
											  size.Y),

			// Create the rectangle of the entity one step into the future
				futureRectangle = new Vector4(position.X + velocity.X,
											  position.Y + velocity.Y,
											  size.X,
											  size.Y);

			// Now find out all the nearby tiles of the entity
			Rectangle nearby = level.getLocalBoundary(position,
													 (int)size.X * collisionExpanse,
													 (int)size.Y * collisionExpanse);

			#region AABB

			// Collision Booleans
			Boolean
				collides = false,
				collidesHorizontal = false,
				collidesVertical = false;

			// Collision Normals
			Vector2
				horizontalNormal = Vector2.Zero, // X = X Position, Y = Width
				verticalNormal = Vector2.Zero; // X = Y Position, Y = Width

			Vector2 newPosition = position;

			// Go through all nearby tiles to find out if the entity will collide or not
            for (int x = nearby.X; x < nearby.X + nearby.Width; x++)
			{
				for (int y = nearby.Y; y < nearby.Y + nearby.Height; y++)
				{
					if (level.getTile(x, y).isSolid())
					{
						// Create a temporary variable to hold the tile position data
						Vector4 tileVector4
							= new Vector4(x * Tile.tileWidth,
										  y * Tile.tileHeight,
										  Tile.tileWidth,
                                          Tile.tileHeight);

						// Check if the entity collides with the tile
						if (floatCollides(futureRectangle, tileVector4))
							collides = true;

						// .. horizontally
						if (floatCollides(entityRectangle + new Vector4(velocity.X, 0, 0, 0), tileVector4))
						{
							collidesHorizontal = true;

							// Set the normal, so the player can 'snap' onto the tile
							horizontalNormal.X = x * Tile.tileWidth;
							horizontalNormal.Y = Tile.tileWidth;
						}

						// .. vertically
						if (floatCollides(entityRectangle + new Vector4(0, velocity.Y, 0, 0), tileVector4))
						{
							collidesVertical = true;

							// Again, set the normal
							verticalNormal.X = y * Tile.tileHeight;
							verticalNormal.Y = Tile.tileHeight;
						}
					}
				}
            }

			// Find out if the entity is on the ground
			if (onGround > 0) onGround--;

			// If the player has collided with something, find out the normal of the tile
			if (collides)
			{
				if(collidesHorizontal)
				{
					if(velocity.X < 0.0F)
					{
						// Moving to the left

						// Snap the player onto the tile
						newPosition.X = horizontalNormal.X + horizontalNormal.Y;

						// Stop moving the player horizontally
						velocity.X = 0.0F;
					}
					else if(velocity.X > 0.0F)
					{
						// Moving to the right

						// Snap the player onto the tile
						newPosition.X = horizontalNormal.X - size.X;

						// Stop moving the player horizontally
						velocity.X = 0.0F;
					}
				}
				
				if(collidesVertical)
				{
					if (velocity.Y < 0.0F)
					{
						// Moving up

						// Snap the player onto the ceiling
						newPosition.Y = verticalNormal.X + verticalNormal.Y;

						// Stop moving the player vertically
						velocity.Y = 0.0F;
					}
					else if(velocity.Y > 0.0F)
					{
						// Moving down

						// Snap the entity onto the floor
						newPosition.Y = verticalNormal.X - size.Y;

						// Stop moving the entity downward
						velocity.Y = 0.0F;

						onGround = 3;
					}
				}

				position = newPosition;
			}

			#endregion

			// Move the entity
			position += velocity;
			velocity.Y += level.gravity;

			// Slow the entity down if it's on the ground
			if (onGround > 0)
				velocity.X *= 0.8F;
			else
				velocity.X *= 0.97F;
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
			//first.X += first.Z / 2.0F;
			//first.Y += first.W / 2.0F;

			//first.Z /= 2.0F;
			//first.W /= 2.0F;

			//if (Math.Abs(first.X - second.X) < first.Z + second.Z)
			//	if (Math.Abs(first.Y - second.Y) < first.W + second.W)
			//		return true; return false;
			
			Rectangle firstR  = new Rectangle((int)first.X, (int)first.Y, (int)first.Z, (int)first.W);
			Rectangle secondR = new Rectangle((int)second.X, (int)second.Y, (int)second.Z, (int)second.W);

			return firstR.Intersects(secondR);

			//return !(second.X > first.X + first.W
			//	  || second.X + second.W < first.X
			//	  || second.Y > first.Y + first.Z
			//	  || second.Y + first.Z < first.Y);
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
			if (onGround > 0)
			{
				if (k.IsKeyDown(Keys.A)) velocity.X -= 0.5F;
				if (k.IsKeyDown(Keys.D)) velocity.X += 0.5F;
			}
			else
			{
				if (k.IsKeyDown(Keys.A)) velocity.X -= 0.15F;
				if (k.IsKeyDown(Keys.D)) velocity.X += 0.15F;
			}
			if (k.IsKeyDown(Keys.Space) && kOld.IsKeyUp(Keys.Space) && onGround > 0) velocity.Y -= 5.0F;

			// Move the entity
			this.move(level);

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
