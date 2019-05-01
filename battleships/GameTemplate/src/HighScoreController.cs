using System;
using System.Collections.Generic;
using System.IO;
using SwinGameSDK;

/// <summary>
/// Controls displaying and collecting high score data.
/// </summary>
/// <remarks>
/// Data is saved to a file.
/// </remarks>
static class HighScoreController
{
    private const int Name_Width = 3;
    private const int Scores_Left = 490;

    /// <summary>
    /// The score structure is used to keep the name and
    /// score of the top players together.
    /// </summary>
    private struct Score : IComparable
    {
        public string Name;
        public int Value;

        /// <summary>
        /// Allows scores to be compared to facilitate sorting
        /// </summary>
        /// <param name="obj">the object to compare to</param>
        /// <returns>a value that indicates the sort order</returns>
        public int CompareTo(object obj)
        {
            if (obj is Score)
            {
                Score other = (Score)obj;

                return other.Value - this.Value;
            }
            else
                return 0;
        }
    }

    private static List<Score> _Scores = new List<Score>();

    /// <summary>
    /// Loads the scores from the highscores text file.
    /// </summary>
    /// <remarks>
    /// The format is
    /// # of scores
    /// NNNSSS
    /// Where NNN is the name and SSS is the score
    /// </remarks>
    private static void LoadScores()
    {
        string filename;
        filename = SwinGame.PathToResource("highscores.txt");

        StreamReader input;
        input = new StreamReader(filename);

        // Read in the # of scores
        int numScores;
        numScores = Convert.ToInt32(input.ReadLine());

        _Scores.Clear();

        int i;

        for (i = 1; i <= numScores; i++)
        {
            Score s;
            string line;

            line = input.ReadLine();
            string[] bothDimensions = line.Split(' ');
            //s.Name = line.Substring(0, Name_Width);
            s.Name = bothDimensions[0];
            s.Value = Convert.ToInt32(bothDimensions[1]);
            _Scores.Add(s);
        }
        input.Close();
    }

    /// <summary>
    /// Saves the scores back to the highscores text file.
    /// </summary>
    /// <remarks>
    /// The format is
    /// # of scores
    /// NNNSSS
    /// 
    /// Where NNN is the name and SSS is the score
    /// </remarks>
    private static void SaveScores()
    {
        string filename;
        filename = SwinGame.PathToResource("highscores.txt");

        StreamWriter output;
        output = new StreamWriter(filename);

        output.WriteLine(_Scores.Count);

        foreach (Score s in _Scores)
            output.WriteLine(s.Name + " " + s.Value);

        output.Close();
    }

    /// <summary>
    /// Draws the high scores to the screen.
    /// </summary>
    public static void DrawHighScores()
    {
        const int Scores_Heading = 40;
        const int Scores_Top = 80;
        const int Score_Gap = 30;

        if (_Scores.Count == 0)
            LoadScores();

        SwinGame.DrawText("   High Scores   ", Color.White, GameResources.GameFont("Courier"), Scores_Left, Scores_Heading);

        // For all of the scores
        for (int i = 0; i <= _Scores.Count - 1; i++)
        {
            Score s;

            s = _Scores[i];

            // for scores 1 - 9 use 01 - 09
            if (i < 9)
                SwinGame.DrawText(" " + (i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, GameResources.GameFont("Courier"), Scores_Left, Scores_Top + i * Score_Gap);
            else
                SwinGame.DrawText(i + 1 + ":   " + s.Name + "   " + s.Value, Color.White, GameResources.GameFont("Courier"), Scores_Left, Scores_Top + i * Score_Gap);
        }
    }

    /// <summary>
    /// Handles the user input during the top score screen.
    /// </summary>
    /// <remarks></remarks>
    public static void HandleHighScoreInput()
    {
        if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.EscapeKey) || SwinGame.KeyTyped(KeyCode.EscapeKey))
        { // buf fix- save scores
            SaveScores();
            GameController.EndCurrentState();
        }
        
    }

    /// <summary>
    /// Read the user's name for their highsSwinGame.
    /// </summary>
    /// <param name="value">the player's sSwinGame.</param>
    /// <remarks>
    /// This verifies if the score is a highsSwinGame.
    /// </remarks>
    public static void ReadHighScore(int value)
    {
        const int Entry_Top = 500;

        if (_Scores.Count == 0)
            LoadScores();

        // is it a high score
        if (value > _Scores[_Scores.Count - 1].Value)
        {
            Score s = new Score();
            s.Value = value;

            GameController.AddNewState(GameState.ViewingHighScores);

            int x;
            x = Scores_Left + SwinGame.TextWidth(GameResources.GameFont("Courier"), "Name: ");

            SwinGame.StartReadingText(Color.White, Name_Width, GameResources.GameFont("Courier"), x, Entry_Top);

            // Read the text from the user
            while (SwinGame.ReadingText())
            {
                SwinGame.ProcessEvents();

                UtilityFunctions.DrawBackground();
                DrawHighScores();
                SwinGame.DrawText("Name: ", Color.White, GameResources.GameFont("Courier"), Scores_Left, Entry_Top);
                SwinGame.RefreshScreen();
            }

            s.Name = SwinGame.TextReadAsASCII();

            if (s.Name.Length < 3)
                s.Name = s.Name + new string(System.Convert.ToChar(" "), 3 - s.Name.Length);

            _Scores.RemoveAt(_Scores.Count - 1);
            _Scores.Add(s);
            _Scores.Sort();

            GameController.EndCurrentState();
        }
    }
}
