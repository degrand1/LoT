#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace LoT
{

    /// <summary>
    /// The level select screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class LevelSelectMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry[] levels;        

        const int numberOfLevels = 1;

        string[] levelString = { "Level One" };
        string[] previewPictureNames = { "Preview One" };
        ContentManager content;
        Texture2D[] previewPictures;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelSelectMenuScreen()
            : base("Level Select")
        {
            levels = new MenuEntry[numberOfLevels];
            for (int i = 0; i < numberOfLevels; i++)
            {
                // Create our menu entries.
                levels[i] = new MenuEntry(string.Empty);
                // Hook up menu event handlers.
                levels[i].Selected += LevelOneMenuEntrySelected;
                // Add entries to the menu.
                MenuEntries.Add(levels[i]);
            }

            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            for (int i = 0; i < numberOfLevels; i++)
            {
                levels[i].Text = levelString[i];
            }

        }


        #endregion

        #region Load Content

        /// <summary>
        /// Loads content for the level select menu
        /// </summary>
        public override void  LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            previewPictures = new Texture2D[numberOfLevels];
            for (int i = 0; i < numberOfLevels; i++)
            {
                previewPictures[i] = content.Load<Texture2D>(previewPictureNames[i]);
            }
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Level One menu entry is selected.
        /// </summary>
        void LevelOneMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new LevelOneScreen());
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!IsExiting)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                spriteBatch.Begin();
                spriteBatch.Draw(previewPictures[selectedEntry], new Rectangle(275, 125, 586, 344), Color.White);
                spriteBatch.End();
            }
        }

        #endregion

    }
}
