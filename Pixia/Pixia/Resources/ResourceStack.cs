using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pixia.Resources;

namespace Pixia.Resources
{
	public class ResourceStack
	{
		public List<Resource> items;

		public int maximumSize = 0;

		public ResourceStack(int maximumSize)
		{
			this.maximumSize = maximumSize;

			this.items = new List<Resource>();
		}

		/// <summary>
		/// Add a resource item to the stack.
		/// Will return FALSE if the stack is full.
		/// </summary>
		/// <param name="Resource"></param>
		/// <returns></returns>
		public Boolean addItem(Resource r)
		{
			if (items.Count >= maximumSize)
				return false;

			items.Add(r);
			return true;
		}

		/// <summary>
		/// Adds a pile of items into the stack.
		/// Will return FALSE if the stack is full.
		/// </summary>
		/// <param name="Resource Stack"></param>
		/// <returns></returns>
		public Boolean addStack(ResourceStack rs)
		{
			if (items.Count + rs.items.Count >= maximumSize)
				return false;

			foreach (Resource r in rs.items)
				items.Add(r);
			return true;
		}

		/// <summary>
		/// Get the first half of the stack.
		/// </summary>
		/// <returns></returns>
		public ResourceStack getHalf()
		{
			ResourceStack newList = new ResourceStack(16);

			for(int i = 0; i < items.Count / 2; i++)
			{
				newList.addItem(items[0]);
				items.RemoveAt(0);
			}

			return newList;
		}

		/// <summary>
		/// Get the first item off the top of the stack.
		/// </summary>
		/// <returns></returns>
		public Resource getOne()
		{
			Resource toReturn = items[0];
			items.RemoveAt(0);

			return toReturn;
		}

		public void draw(SpriteBatch spriteBatch, Texture2D mainSheet, SpriteFont uiFont, Rectangle rectangle)
		{
			// Don't draw anything if the stack is empty.
			if (items.Count <= 0) return;

			spriteBatch.Draw(mainSheet,
				rectangle,
				Resource.getResourceSpriteRect(items[items.Count - 1].ID),
				Color.White);

			if (items.Count > 1)
			{
				int numberWidth = (int)uiFont.MeasureString(items.Count.ToString()).X;
				int numberHeight = (int)uiFont.MeasureString(items.Count.ToString()).Y;

				spriteBatch.DrawString(uiFont,
					items.Count.ToString(),
					new Vector2(rectangle.X + rectangle.Width - numberWidth,
								rectangle.Y + rectangle.Height - numberHeight),
					Color.Black);

				spriteBatch.DrawString(uiFont,
					items.Count.ToString(),
					new Vector2(rectangle.X + rectangle.Width - numberWidth - 2,
								rectangle.Y + rectangle.Height - numberHeight - 2),
					Color.White);
			}
		}
	}
}
