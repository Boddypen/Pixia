using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Pixia.Tiles;
using Pixia.Levels;
using Pixia.Entities;
using Pixia.Resources;

namespace Pixia
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		// Static Game Stuff
		public static readonly String
			GAMENAME = "Pixia",
			GAMEVERSION = "0.1";
		
		// XNA
		public GraphicsDeviceManager graphics;
		public SpriteBatch spriteBatch;
		public KeyboardState k, kOld = Keyboard.GetState();
		public MouseState m, mOld = Mouse.GetState();

		// Display
		public RenderTarget2D
			target;
		public Color
			skyColor = new Color(0.32F, 0.53F, 0.80F);

		// Enums
		public enum GameState
		{
			Menu,
			Exploring,
			Inventory,
			Generating,
			Quitting
		}
		public enum FocusState
		{
			Focused,
			Unfocused
		}

		// Enum Variables
		public GameState gameState = GameState.Menu;
		public FocusState focusState = FocusState.Unfocused;

		// Game
		public Random random;
		public Level level;
		public Player player;
		public System.Timers.Timer frameTimer;
		//public List<Resource> inventory;
		public ResourceStack[,] inventory;
		public ResourceStack handStack;

		// Data
		public int
			tickCounter = 0,
			displacementX,
			displacementY,
			gameWidth,
			gameHeight,
			cursorWidth = 16,
			cursorHeight = 16,
			scale = 2,
			focusTargetTileX = 0,
			focusTargetTileY = 0,
			menuTitleWidth = 144,
			menuTitleHeight = 48,
			frames = 0, framesDisplay = 0,
			updates = 0, updatesDisplay = 0,
			inventoryX = 100,
			inventoryY = 100,
			inventoryWidth = 9,
			inventoryHeight = 5,
			inventorySelectionX = -1,
			inventorySelectionY = -1;
		public Boolean
			showDebug = false;
		public Vector2
			cameraPosition,
			cameraTarget,
			playerSize,
			focusPosition;

		// XNA Content Pipeline
		public SpriteFont
			uiFont;
		public Texture2D
			mainSheet,
			cursorTexture;
		public Rectangle
			inventorySlotTextureRect = new Rectangle(80, 240, 16, 16),
			blankTextureRect = new Rectangle(96, 240, 1, 1);
		public SoundEffect
			placeSound,
			breakSound,
			pauseSound,
			spawnSound,
			focusSound,
			openSound,
			closeSound;

		public Game()
		{
			// Graphics Adapter
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 768;
			gameWidth = graphics.PreferredBackBufferWidth;
			gameHeight = graphics.PreferredBackBufferHeight;
			graphics.IsFullScreen = false;
			graphics.SynchronizeWithVerticalRetrace = true;

			IsMouseVisible = false;

			// Window
			Window.Title = "Pixia";
			Window.AllowUserResizing = false;
			
			// Content Directory
			Content.RootDirectory = "Content";
		}

		#region Timer Methods

		private void showFrames(object sender, System.Timers.ElapsedEventArgs e)
		{
			Console.WriteLine(frames + " fps, " + updates + " ups");

			framesDisplay = frames;
			frames = 0;

			updatesDisplay = updates;
			updates = 0;
		}

		#endregion

		protected override void Initialize()
		{
			// Random Number Generator
			random = new Random();

			// Data
			cameraTarget = new Vector2(Tile.tileWidth * 1024, Tile.tileHeight * 128);
			cameraPosition = cameraTarget;

			focusPosition = Vector2.Zero;
			
			// Create the inventory
			//inventory = new List<Resource>();
			//inventory.Add(new DirtResource());
			inventory = new ResourceStack[inventoryWidth, inventoryHeight];
			for(int x = 0; x < inventoryWidth; x++)
				for(int y = 0; y < inventoryHeight; y++)
					inventory[x, y] = new ResourceStack(64);
			handStack = new ResourceStack(64);

			// Calculate the inventory position
			inventoryX = (graphics.PreferredBackBufferWidth / (2 * scale)) - ((inventoryWidth * Tile.tileWidth) / 2);
			inventoryY = (graphics.PreferredBackBufferHeight / (2 * scale)) - ((inventoryHeight * Tile.tileHeight) / 2);

			// Create a player for the game level
			playerSize = new Vector2(36, 64);

			// Timers
			frameTimer = new System.Timers.Timer(1000);
			frameTimer.Elapsed += showFrames;
			frameTimer.Start();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch(GraphicsDevice);
			target = new RenderTarget2D(GraphicsDevice,
				graphics.PreferredBackBufferWidth / scale,
				graphics.PreferredBackBufferHeight / scale);

			// Fonts
			uiFont = Content.Load<SpriteFont>("spritefont\\ui");

			// Textures
			mainSheet = Content.Load<Texture2D>("png\\main");
			cursorTexture = Content.Load<Texture2D>("png\\cursor");

			// Sounds
			placeSound = Content.Load<SoundEffect>("wav\\place");
			breakSound = Content.Load<SoundEffect>("wav\\break");
			pauseSound = Content.Load<SoundEffect>("wav\\pause");
			spawnSound = Content.Load<SoundEffect>("wav\\spawn");
			focusSound = Content.Load<SoundEffect>("wav\\focus");
			openSound = Content.Load<SoundEffect>("wav\\open");
			closeSound = Content.Load<SoundEffect>("wav\\close");
		}

		protected override void UnloadContent()
		{
			// Stop all timers
			frameTimer.Stop();
		}
		
		protected override void Update(GameTime gameTime)
		{
			// Get the keyboard and mouse states
			k = Keyboard.GetState();
			m = Mouse.GetState();
			
			// Update displacements
			displacementX = (graphics.PreferredBackBufferWidth / 2) / scale;
			displacementY = (graphics.PreferredBackBufferHeight / 2) / scale;

			// Make sure the game is active
			if (this.IsActive)
			{
				if(focusState.Equals(FocusState.Unfocused))
				{
					focusState = FocusState.Focused;
					focusSound.Play();
				}
			}
			else
			{
				focusState = FocusState.Unfocused;
			}

			switch(focusState)
			{
				case FocusState.Focused:
					switch(gameState)
					{
						case GameState.Quitting:
							#region Quitting Update Logic

							// Everything needed to be done before quitting is done here

							this.Exit();

							#endregion
							break;

						case GameState.Menu:
							#region Menu Update Logic

							// Allow the user to enter the game
							if (k.IsKeyDown(Keys.Enter))
							{
								gameState = GameState.Generating;

								// Generate the level
								level = new Level(1024, 128, 0.18F);
								level.generateLevel();

								// Load the player onto the level
								player = new Player(new Vector2(Tile.tileWidth * 128, Tile.tileHeight * 64),
													playerSize,
													Vector2.Zero);
								level.player = player;

								// Move the camera
								cameraTarget.X = level.player.position.X;
								cameraTarget.Y = level.player.position.Y;
								cameraPosition.X = cameraTarget.X;
								cameraPosition.Y = cameraTarget.Y;
							}

							// Allow the user to quit the game
							if (k.IsKeyDown(Keys.Q))
								gameState = GameState.Quitting;

							#endregion
							break;

						case GameState.Exploring:
							#region Exploring Update Logic
							// Update the level
							level.update(level, gameState, k, kOld, m, mOld);

							// TEMPORARY
							//for(int i = 0; i < level.entities.Count; i++)
							//{
							//	//inventory.Add(level.entities[i].dropResource);
							//	if(inventory[random.Next(0, inventoryWidth), random.Next(0, inventoryHeight)].addItem(level.entities[i].dropResource))
							//		level.entities.RemoveAt(i);
							//}

							// Allow the player to exit to the menu
							if (k.IsKeyDown(Keys.Escape))
							{
								gameState = GameState.Menu;
								pauseSound.Play();
							}

							// Allow the user to open their inventory
							if(k.IsKeyDown(Keys.Tab) && kOld.IsKeyUp(Keys.Tab))
							{
								gameState = GameState.Inventory;
								openSound.Play();
							}

							// Tick?
							if (tickCounter >= 3)
							{
								level.tick(level, cameraPosition, displacementX, displacementY);

								tickCounter = 0;
							}

							// Move the focus
							focusPosition.X += ((focusTargetTileX * Tile.tileWidth) - focusPosition.X) / 3.0F;
							focusPosition.Y += ((focusTargetTileY * Tile.tileHeight) - focusPosition.Y) / 3.0F;
							focusTargetTileX = (((m.X / scale) - displacementX) + (int)cameraPosition.X) / Tile.tileWidth;
							focusTargetTileY = (((m.Y / scale) - displacementY) + (int)cameraPosition.Y) / Tile.tileHeight;

							// Placing Blocks
							if (m.RightButton.Equals(ButtonState.Pressed)
								&& !m.RightButton.Equals(mOld.RightButton))
							{
								if (level.getTile(
									focusTargetTileX,
									focusTargetTileY).ID <= 0)
								{
									level.setTile(
										focusTargetTileX,
										focusTargetTileY,
										new WoodPlankTile(0, random));

									placeSound.Play();
								}
							}

							// Destroying Blocks
							if (m.LeftButton.Equals(ButtonState.Pressed)
								&& !m.LeftButton.Equals(mOld.LeftButton))
							{
								if (level.getTile(
									focusTargetTileX,
									focusTargetTileY).ID > 0)
								{
									level.removeTile(level,
										focusTargetTileX,
										focusTargetTileY);

									breakSound.Play();
								}
							}

							// Move the camera
							cameraPosition.X += (cameraTarget.X - cameraPosition.X) / 10;
							cameraPosition.Y += (cameraTarget.Y - cameraPosition.Y) / 10;
							cameraTarget.X = level.player.position.X;
							cameraTarget.Y = level.player.position.Y;

							// Increment the tick counter
							tickCounter++;
							#endregion
							break;

						case GameState.Inventory:
							#region Inventory Update Logic

							// Allow the player to exit to the main menu
							if(k.IsKeyDown(Keys.Escape))
							{
								gameState = GameState.Menu;
								pauseSound.Play();
							}

							// Allow the player to exit the inventory
							if(k.IsKeyDown(Keys.Tab) && kOld.IsKeyUp(Keys.Tab))
							{
								// If the player is holding an item in their hand, throw the items out before continuing.
								if(handStack.items.Count > 0)
								{

								}

								gameState = GameState.Exploring;
								closeSound.Play();
							}

							// Find out what item stack the user is pointing at
							if(m.X / scale >= inventoryX && m.X / scale < inventoryX + (inventoryWidth * Tile.tileWidth)
								&& m.Y / scale >= inventoryY && m.Y / scale < inventoryY + (inventoryHeight * Tile.tileHeight))
							{
								inventorySelectionX = ((m.X / scale) - inventoryX) / Tile.tileWidth;
								inventorySelectionY = ((m.Y / scale) - inventoryY) / Tile.tileHeight;
							}
							else
							{
								inventorySelectionX = -1;
								inventorySelectionY = -1;
							}

							// The player has clicked!
							if((m.LeftButton.Equals(ButtonState.Pressed) && mOld.LeftButton.Equals(ButtonState.Released))
								|| m.RightButton.Equals(ButtonState.Pressed) && mOld.RightButton.Equals(ButtonState.Released))
							{
								// First, make sure a slot in the inventory is selected.

								int iX = inventorySelectionX;
								int iY = inventorySelectionY;

								if(iX >= 0 && iY >= 0)
								{
									if (m.LeftButton.Equals(ButtonState.Pressed)
										&& m.RightButton.Equals(ButtonState.Released))
									{
										if (handStack.items.Count > 0
											&& inventory[iX, iY].items.Count <= 0)
										{
											// Player drops items into an empty inventory slot
											if (inventory[iX, iY].addStack(handStack))
												handStack.items.Clear();
										}
										else if (handStack.items.Count > 0
											&& inventory[iX, iY].items.Count > 0
											&& handStack.items[handStack.items.Count - 1].ID
												.Equals(inventory[iX, iY].items[inventory[iX, iY].items.Count - 1].ID))
										{
											// Player adds items to an existing pile in the inventory
											if (inventory[iX, iY].addStack(handStack))
												handStack.items.Clear();
										}
										else if (handStack.items.Count <= 0
											&& inventory[iX, iY].items.Count > 0)
										{
											// Player picks up a pile of items from the inventory
											if (handStack.addStack(inventory[iX, iY]))
												inventory[iX, iY].items.Clear();
										}
									}
								}
								else
								{
									// Allow the player to throw items
									if(handStack.items.Count > 0)
									{
										if (m.LeftButton.Equals(ButtonState.Pressed)
											&& m.RightButton.Equals(ButtonState.Released))
										{
											foreach (Resource r in handStack.items)
												level.addEntity(new DropEntity(cameraPosition, Vector2.Zero, r));

											handStack.items.Clear();
										}
										else if(m.LeftButton.Equals(ButtonState.Released)
											&& m.RightButton.Equals(ButtonState.Pressed))
										{
											level.addEntity(new DropEntity(cameraPosition, Vector2.Zero, handStack.getOne()));
										}
									}
								}
							}

							#endregion
							break;

						case GameState.Generating:
							#region Generating Update Logic

							if (level.genDone)
							{
								gameState = GameState.Exploring;
								spawnSound.Play();
							}

							#endregion
							break;
					}
					break;

				case FocusState.Unfocused:
					#region Unfocused Update Logic

					#endregion
					break;
			}

			// Turn on/off the debug information
			if (k.IsKeyDown(Keys.F3) && kOld.IsKeyUp(Keys.F3))
				showDebug = !showDebug;

			// Update the old keyboard and mouse variables
			kOld = k;
			mOld = m;

			updates++;
			base.Update(gameTime);
		}
		
		/// <summary>
		/// Go through all components of the game (entities, tiles, etc.) and draw them onto the screen.
		/// This also checks whether or not the game is focused or not.
		/// </summary>
		/// <param name="gameTime"></param>
		protected override void Draw(GameTime gameTime)
		{
			// Set the render target to the pre-buffer
			GraphicsDevice.SetRenderTarget(target);

			#region Drawing
			
			// Begin the spritebatch
			spriteBatch.Begin(SpriteSortMode.Deferred,
				BlendState.NonPremultiplied,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullNone);

			switch (focusState)
			{
				case FocusState.Focused:
					switch(gameState)
					{
						case GameState.Menu:
							#region Menu Drawing Logic

							// Clear the screen
							GraphicsDevice.Clear(Color.Black);
							background(mainSheet,
								Tile.getTileSheetRect(1),
								Tile.tileWidth,
								Tile.tileHeight);

							// Menu Title
							spriteBatch.Draw(mainSheet,
								new Rectangle(displacementX - (menuTitleWidth / 2),
											 (menuTitleHeight / 2) + 80,
											 menuTitleWidth,
											 menuTitleHeight),
								new Rectangle(0, 208, 48, 16),
								Color.White);
							spriteBatch.Draw(mainSheet,
								new Rectangle(displacementX - (menuTitleWidth / 2),
											 (menuTitleHeight / 2) + 80,
											 menuTitleWidth,
											 menuTitleHeight),
								new Rectangle(48, 208, 48, 16),
								Color.White);

							// Menu Options
							String playOption = "Press ENTER to play.";
							int playOptionWidth = (int)uiFont.MeasureString(playOption).X;
							int playOptionHeight = (int)uiFont.MeasureString(playOption).Y;
							String quitOption = "Press Q to quit.";
							int quitOptionWidth = (int)uiFont.MeasureString(quitOption).X;
							int quitOptionHeight = (int)uiFont.MeasureString(quitOption).Y;
							drawShadowString(spriteBatch,
								uiFont, playOption,
								new Vector2(displacementX - (playOptionWidth / 2),
											displacementY - (playOptionHeight / 2)),
								Color.White);
							drawShadowString(spriteBatch,
								uiFont, quitOption,
								new Vector2(displacementX - (quitOptionWidth / 2),
											displacementY - (quitOptionHeight / 2) + 32),
								Color.White);

							#endregion
							break;

						case GameState.Exploring:
							#region Exploring Drawing Logic

							// Clear the screen
							GraphicsDevice.Clear(skyColor);

							// Draw the level
							level.draw(spriteBatch,
								displacementX,
								displacementY,
								cameraPosition,
								mainSheet,
								level);

							level.player.draw(spriteBatch,
											  level,
											  displacementX,
											  displacementY,
											  cameraPosition,
											  mainSheet);

							// Draw the focus
							spriteBatch.Draw(mainSheet,
								new Rectangle(displacementX + (int)Math.Round(focusPosition.X) - (int)cameraPosition.X,
									displacementY + (int)Math.Round(focusPosition.Y) - (int)cameraPosition.Y,
									Tile.tileWidth, Tile.tileHeight),
								new Rectangle(112, 240, 16, 16),
								Color.White);

							#endregion
							break;

						case GameState.Inventory:
							#region Inventory Drawing Logic

							// Clear the screen
							GraphicsDevice.Clear(skyColor);

							// Draw the level, as a background
							level.draw(spriteBatch,
								displacementX,
								displacementY,
								cameraPosition,
								mainSheet,
								level);

							// Draw the inventory menu
							spriteBatch.Draw(mainSheet,
								new Rectangle(inventoryX + 4,
											  inventoryY + 4,
											  inventoryWidth * Tile.tileWidth,
											  inventoryHeight * Tile.tileHeight),
								blankTextureRect,
								Color.Black);
							for (int x = inventoryWidth - 1; x >= 0; x--)
								for (int y = inventoryHeight - 1; y >= 0; y--)
								{
									Rectangle rect = new Rectangle(inventoryX + (x * Tile.tileWidth),
																   inventoryY + (y * Tile.tileHeight),
																   Tile.tileWidth,
																   Tile.tileHeight);

									spriteBatch.Draw(mainSheet,
												rect,
												inventorySlotTextureRect,
												Color.White);

									inventory[x, y].draw(spriteBatch,
										mainSheet,
										uiFont,
										rect);

									// If this slot is the currently selected one
									if(inventorySelectionX == x && inventorySelectionY == y)
									{
										spriteBatch.Draw(mainSheet,
											rect,
											blankTextureRect,
											new Color(1.0F, 1.0F, 1.0F, 0.3F));
									}
								}

							#endregion
							break;

						case GameState.Generating:
							#region Generating Drawing Logic

							// Clear the screen
							GraphicsDevice.Clear(Color.Black);
							background(mainSheet,
								Tile.getTileSheetRect(1),
								Tile.tileWidth,
								Tile.tileHeight);

							//TODO: Create a method that draws a string to the center of the screen.

							// Message
							String genMessage = "Generating level...";
							int genMessageWidth = (int)uiFont.MeasureString(genMessage).X;
							int genMessageHeight = (int)uiFont.MeasureString(genMessage).Y;
							drawShadowString(spriteBatch,
								uiFont, genMessage,
								new Vector2(displacementX - (genMessageWidth / 2),
											displacementY - (genMessageHeight / 2)),
								Color.White);

							#endregion
							break;
					}
					break;

				case FocusState.Unfocused:
					#region Unfocused Drawing Logic

					// Clear the screen
					GraphicsDevice.Clear(new Color(0.3F, 0.3F, 0.3F));

					// Warn the player that the game is unfocused
					String warnMessage = "Paused: Click to Focus!";
					int warnMessageWidth = (int)uiFont.MeasureString(warnMessage).X;
					int warnMessageHeight = (int)uiFont.MeasureString(warnMessage).Y;
					drawShadowString(spriteBatch,
						uiFont,
						warnMessage,
						new Vector2(displacementX - (warnMessageWidth / 2),
									displacementY - (warnMessageHeight / 2)),
						Color.White);

					#endregion
					break;
			}
			
			// Draw debug info
			drawShadowString(spriteBatch,
				uiFont, GAMENAME + " " + GAMEVERSION + "  " + framesDisplay + " FPS (" + updatesDisplay + ")",
				new Vector2(5, 5),
				Color.White);
			if (showDebug)
			{
				drawShadowString(spriteBatch,
					uiFont, "T: " + focusTargetTileX + ", " + focusTargetTileY,
					new Vector2(5, 25),
					Color.White);
				drawShadowString(spriteBatch,
					uiFont, "FS: " + focusState.ToString(),
					new Vector2(5, 45),
					Color.White);
				drawShadowString(spriteBatch,
					uiFont, "GS: " + gameState.ToString(),
					new Vector2(5, 65),
					Color.White);
				drawShadowString(spriteBatch,
					uiFont, "IS: " + inventorySelectionX + ", " + inventorySelectionY,
					new Vector2(5, 85),
					Color.White);
				drawShadowString(spriteBatch,
					uiFont, "OG: " + (level.player.onGround > 0),
					new Vector2(5, 105),
					Color.White);
			}

			// Draw the mouse cursor
			if (handStack.items.Count > 0)
			{
				handStack.draw(spriteBatch,
					mainSheet,
					uiFont,
					new Rectangle((m.X / scale) - (Tile.tileWidth / 2),
								  (m.Y / scale) - (Tile.tileHeight / 2),
								  Tile.tileWidth,
								  Tile.tileHeight));
			}
			spriteBatch.Draw(cursorTexture,
				new Rectangle(m.X / scale, m.Y / scale,
					cursorWidth, cursorHeight),
				Color.White);

			// End the spritebatch
			spriteBatch.End();

			#endregion

			// Set the render target to the back buffer
			GraphicsDevice.SetRenderTarget(null);

			// Clear the screen
			GraphicsDevice.Clear(Color.Black);

			// Begin the spritebatch
			spriteBatch.Begin(SpriteSortMode.Deferred,
				BlendState.NonPremultiplied,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullNone);

			spriteBatch.Draw(target,
				new Rectangle(0, 0, gameWidth, gameHeight),
				Color.White);
			
			// End the spritebatch
			spriteBatch.End();

			frames++;
			base.Draw(gameTime);
		}

		/// <summary>
		/// Draws a blank rectangle on the screen at the specified location, with the specified color.
		/// </summary>
		/// <param name="Rectangle"></param>
		/// <param name="Color"></param>
		private void drawBlankRectangle(Rectangle rectangle, Color color)
		{
			spriteBatch.Draw(mainSheet,
				rectangle,
				blankTextureRect,
				color);
		}

		/// <summary>
		/// Draws a tessalating background of whatever texture is supplied.
		/// </summary>
		/// <param name="Texure"></param>
		/// <param name="Texture Width"></param>
		/// <param name="Texture Height"></param>
		private void background(Texture2D texture, int textureWidth, int textureHeight)
		{
			int positionX = 0;
			int positionY = 0;
			
			while(positionY < graphics.PreferredBackBufferHeight)
			{
				while(positionX < graphics.PreferredBackBufferWidth)
				{
					spriteBatch.Draw(texture,
						new Rectangle(positionX,
									  positionY,
									  textureWidth,
									  textureHeight),
						Color.White);

					positionX += textureWidth;
				}

				positionX = 0;
				positionY += textureHeight;
			}
		}

		/// <summary>
		/// Draws a tessalating background of whatever texture is supplied.
		/// </summary>
		/// <param name="Texture"></param>
		/// <param name="Source Rectangle"></param>
		/// <param name="Texture Width"></param>
		/// <param name="Texture Height"></param>
		private void background(Texture2D texture, Rectangle sourceRectangle, int textureWidth, int textureHeight)
		{
			int positionX = 0;
			int positionY = 0;

			while (positionY < graphics.PreferredBackBufferHeight)
			{
				while (positionX < graphics.PreferredBackBufferWidth)
				{
					spriteBatch.Draw(texture,
						new Rectangle(positionX,
									  positionY,
									  textureWidth,
									  textureHeight),
						sourceRectangle,
						Color.White);

					positionX += textureWidth;
				}

				positionX = 0;
				positionY += textureHeight;
			}
		}

		public void drawShadowString(SpriteBatch spriteBatch,
			SpriteFont font,
			String text,
			Vector2 position,
			Color textColor)
		{
			spriteBatch.DrawString(font,
				text,
				new Vector2(position.X + 2, position.Y + 2),
				Color.Black);

			spriteBatch.DrawString(font,
				text,
				position,
				textColor);
		}
	}
}
