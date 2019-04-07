using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SwinGameSDK;

/// <summary>

/// The menu controller handles the drawing and user interactions
/// from the menus in the game. These include the main menu, game
/// menu and the settings menu.
/// </summary>

static class MenuController
{

    /// <summary>
    /// The menu structure for the game.
    /// </summary>
    /// <remarks>
    /// These are the text captions for the menu items.
    /// </remarks>
    private readonly static string[][] _menuStructure = new[] { new string[] { "PLAY", "SETUP", "SCORES", "QUIT" }, new string[] { "RETURN", "SURRENDER", "QUIT" }, new string[] { "EASY", "MEDIUM", "HARD" } };

    private const static int MenuTop = 575;
    private const static int MenuLeft = 30;
    private const static int MenuGap = 0;
    private const static int ButtonWidth = 75;
    private const static int ButtonHeight = 15;
    private const static int ButtonSep = ButtonWidth + MenuGap;
    private const static int TextOffset = 0;

    private const static int MainMenu = 0;
    private const static int GameMenu = 1;
    private const static int SetupMenu = 2;

    private const static int MainMenuPlayButton = 0;
    private const static int MainMenuSetupButton = 1;
    private const static int MainMenuTopScoresButton = 2;
    private const static int MainMenuQuitButton = 3;

    private const static int SetupMenuEasyButton = 0;
    private const static int SetupMenuMediumButton = 1;
    private const static int SetupMenuHardButton = 2;
    private const static int SetupMenuExitButton = 3;

    private const static int GameMenuReturnButton = 0;
    private const static int GameMenuSurrenderButton = 1;
    private const static int GameMenuQuitButton = 2;

    private readonly static Color MenuColour = SwinGame.RGBAColor(2, 167, 252, 255);
    private readonly static Color HighlightColour = SwinGame.RGBAColor(1, 57, 86, 255);

    /// <summary>
    /// Handles the processing of user input when the main menu is showing
    /// </summary>
    public static void HandleMainMenuInput()
    {
        HandleMenuInput(MainMenu, 0, 0);
    }

    /// <summary>
    /// Handles the processing of user input when the main menu is showing
    /// </summary>
    public static void HandleSetupMenuInput()
    {
        bool handled;
        handled = HandleMenuInput(SetupMenu, 1, 1);

        if (!handled)
            HandleMenuInput(MainMenu, 0, 0);
    }

    /// <summary>
    /// Handle input in the game menu.
    /// </summary>
    /// <remarks>
    /// Player can return to the game, surrender, or quit entirely
    /// </remarks>
    public static void HandleGameMenuInput()
    {
        HandleMenuInput(GameMenu, 0, 0);
    }

    /// <summary>
    /// Handles input for the specified menu.
    /// </summary>
    /// <param name="menu">the identifier of the menu being processed</param>
    /// <param name="level">the vertical level of the menu</param>
    /// <param name="xOffset">the xoffset of the menu</param>
    /// <returns>false if a clicked missed the buttons. This can be used to check prior menus.</returns>
    private static bool HandleMenuInput(int menu, int level, int xOffset)
    {
        if (SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
        {
            EndCurrentState();
            return true;
        }

        if (SwinGame.MouseClicked(MouseButton.LeftButton))
        {
            int i;
            for (i = 0; i <= _menuStructure[menu].Length - 1; i++)
            {
                // IsMouseOver the i'th button of the menu
                if (IsMouseOverMenu(i, level, xOffset))
                {
                    PerformMenuAction(menu, i);
                    return true;
                }
            }

            if (level > 0)
                // none clicked - so end this sub menu
                EndCurrentState();
        }

        return false;
    }

    /// <summary>
    /// Draws the main menu to the screen.
    /// </summary>
    public static void DrawMainMenu()
    {
        // Clears the Screen to Black
        // SwinGame.DrawText("Main Menu", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(MainMenu);
    }

    /// <summary>
    /// Draws the Game menu to the screen
    /// </summary>
    public static void DrawGameMenu()
    {
        // Clears the Screen to Black
        // SwinGame.DrawText("Paused", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(GameMenu);
    }

    /// <summary>
    /// Draws the settings menu to the screen.
    /// </summary>
    /// <remarks>
    /// Also shows the main menu
    /// </remarks>
    public static void DrawSettings()
    {
        // Clears the Screen to Black
        // SwinGame.DrawText("Settings", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(MainMenu);
        DrawButtons(SetupMenu, 1, 1);
    }

    /// <summary>
    /// Draw the buttons associated with a top level menu.
    /// </summary>
    /// <param name="menu">the index of the menu to draw</param>
    private static void DrawButtons(int menu)
    {
        DrawButtons(menu, 0, 0);
    }

    /// <summary>
    /// Draws the menu at the indicated level.
    /// </summary>
    /// <param name="menu">the menu to draw</param>
    /// <param name="level">the level (height) of the menu</param>
    /// <param name="xOffset">the offset of the menu</param>
    /// <remarks>
    /// The menu text comes from the _menuStructure field. The level indicates the height
    /// of the menu, to enable sub menus. The xOffset repositions the menu horizontally
    /// to allow the submenus to be positioned correctly.
    /// </remarks>
    private static void DrawButtons(int menu, int level, int xOffset)
    {
        int btnTop;
        Rectangle toDraw;

        btnTop = MenuTop - (MenuGap + ButtonHeight) * level;
        int i;
        for (i = 0; i <= _menuStructure[menu].Length - 1; i++)
        {
            int btnLeft;

            btnLeft = MenuLeft + ButtonSep * (i + xOffset);
            // SwinGame.FillRectangle(Color.White, btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT)
            toDraw.X = btnLeft + TextOffset;
            toDraw.Y = btnTop + TextOffset;
            toDraw.Width = ButtonWidth;
            toDraw.Height = ButtonHeight;
            SwinGame.DrawTextLines(_menuStructure[menu](i), MenuColour, Color.Black, GameFont("Menu"), FontAlignment.AlignCenter, toDraw);

            if (SwinGame.MouseDown(MouseButton.LeftButton) & IsMouseOverMenu(i, level, xOffset))
                SwinGame.DrawRectangle(HighlightColour, btnLeft, btnTop, ButtonWidth, ButtonHeight);
        }
    }

    /// <summary>
    /// Determined if the mouse is over one of the button in the main menu.
    /// </summary>
    /// <param name="button">the index of the button to check</param>
    /// <returns>true if the mouse is over that button</returns>
    private static bool IsMouseOverButton(int button)
    {
        return IsMouseOverMenu(button, 0, 0);
    }

    /// <summary>
    /// Checks if the mouse is over one of the buttons in a menu.
    /// </summary>
    /// <param name="button">the index of the button to check</param>
    /// <param name="level">the level of the menu</param>
    /// <param name="xOffset">the xOffset of the menu</param>
    /// <returns>true if the mouse is over the button</returns>
    private static bool IsMouseOverMenu(int button, int level, int xOffset)
    {
        int btnTop = MenuTop - (MenuGap + ButtonHeight) * level;
        int btnLeft = MenuLeft + ButtonSep * (button + xOffset);

        return IsMouseInRectangle(btnLeft, btnTop, ButtonWidth, ButtonHeight);
    }

    /// <summary>
    /// A button has been clicked, perform the associated action.
    /// </summary>
    /// <param name="menu">the menu that has been clicked</param>
    /// <param name="button">the index of the button that was clicked</param>
    private static void PerformMenuAction(int menu, int button)
    {
        switch (menu)
        {
            case MainMenu:
                {
                    PerformMainMenuAction(button);
                    break;
                }

            case SetupMenu:
                {
                    PerformSetupMenuAction(button);
                    break;
                }

            case GameMenu:
                {
                    PerformGameMenuAction(button);
                    break;
                }
        }
    }

    /// <summary>
    /// The main menu was clicked, perform the button's action.
    /// </summary>
    /// <param name="button">the button pressed</param>
    private static void PerformMainMenuAction(int button)
    {
        switch (button)
        {
            case MainMenuPlayButton:
                {
                    StartGame();
                    break;
                }

            case MainMenuSetupButton:
                {
                    AddNewState(GameState.AlteringSettings);
                    break;
                }

            case MainMenuTopScoresButton:
                {
                    AddNewState(GameState.ViewingHighScores);
                    break;
                }

            case MainMenuQuitButton:
                {
                    EndCurrentState();
                    break;
                }
        }
    }

    /// <summary>
    /// The setup menu was clicked, perform the button's action.
    /// </summary>
    /// <param name="button">the button pressed</param>
    private static void PerformSetupMenuAction(int button)
    {
        switch (button)
        {
            case SetupMenuEasyButton:
                {
                    SetDifficulty(AIOption.Hard);
                    break;
                }

            case SetupMenuMediumButton:
                {
                    SetDifficulty(AIOption.Hard);
                    break;
                }

            case SetupMenuHardButton:
                {
                    SetDifficulty(AIOption.Hard);
                    break;
                }
        }
        // Always end state - handles exit button as well
        EndCurrentState();
    }

    /// <summary>
    /// The game menu was clicked, perform the button's action.
    /// </summary>
    /// <param name="button">the button pressed</param>
    private static void PerformGameMenuAction(int button)
    {
        switch (button)
        {
            case GameMenuReturnButton:
                {
                    EndCurrentState();
                    break;
                }

            case GameMenuSurrenderButton:
                {
                    EndCurrentState(); // end game menu
                    EndCurrentState(); // end game
                    break;
                }

            case GameMenuQuitButton:
                {
                    AddNewState(GameState.Quitting);
                    break;
                }
        }
    }
}
