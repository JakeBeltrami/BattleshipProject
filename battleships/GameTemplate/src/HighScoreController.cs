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
    private const static int NameWidth = 3;
    private const static int ScoresLeft = 490;

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

            s.Name = line.Substring(0, NameWidth);
            s.Value = Convert.ToInt32(line.Substring(NameWidth));
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
            output.WriteLine(s.Name + s.Value);

        output.Close();
    }

    /// <summary>
    /// Draws the high scores to the screen.
    /// </summary>
    public static void DrawHighScores()
    {
        const int ScoresHeading = 40;
        const int ScoresTop = 80;
        const int ScoreGap = 30;

        if (_Scores.Count == 0)
            LoadScores();

        SwinGame.DrawText("   High Scores   ", Color.White, GameFont("Courier"), ScoresLeft, ScoresHeading);

        // For all of the scores
        int i;
        for (i = 0; i <= _Scores.Count - 1; i++)
        {
            Score s;

            s = _Scores.Item[i];

            // for scores 1 - 9 use 01 - 09
            if (i < 9)
                SwinGame.DrawText(" " + (i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, GameFont("Courier"), ScoresLeft, ScoresTop + i * ScoreGap);
            else
                SwinGame.DrawText(i + 1 + ":   " + s.Name + "   " + s.Value, Color.White, GameFont("Courier"), ScoresLeft, ScoresTop + i * ScoreGap);
        }
    }

    /// <summary>
    /// Handles the user input during the top score screen.
    /// </summary>
    /// <remarks></remarks>
    public static void HandleHighScoreInput()
    {
        if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.VK_ESCAPE) || SwinGame.KeyTyped(KeyCode.VK_RETURN))
            EndCurrentState();
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
        const int EntryTop = 500;

        if (_Scores.Count == 0)
            LoadScores();

        // is it a high score
        if (value > _Scores.Item[_Scores.Count - 1].Value)
        {
            Score s = new Score();
            s.Value = value;

            AddNewState(GameState.ViewingHighScores);

            int x;
            x = ScoresLeft + SwinGame.TextWidth(GameFont("Courier"), "Name: ");

            SwinGame.StartReadingText(Color.White, NameWidth, GameFont("Courier"), x, EntryTop);

            // Read the text from the user
            while (SwinGame.ReadingText())
            {
                SwinGame.ProcessEvents();

                DrawBackground();
                DrawHighScores();
                SwinGame.DrawText("Name: ", Color.White, GameFont("Courier"), ScoresLeft, EntryTop);
                SwinGame.RefreshScreen();
            }

            s.Name = SwinGame.TextReadAsASCII();

            if (s.Name.Length < 3)
                s.Name = s.Name + new string(System.Convert.ToChar(" "), 3 - s.Name.Length);

            _Scores.RemoveAt(_Scores.Count - 1);
            _Scores.Add(s);
            _Scores.Sort();

            EndCurrentState();
        }
    }
}