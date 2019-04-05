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

public static class GameResources
{
    private static void LoadFonts()
    {
        NewFont("ArialLarge", "arial.ttf", 80);
        NewFont("Courier", "cour.ttf", 14);
        NewFont("CourierSmall", "cour.ttf", 8);
        NewFont("Menu", "ffaccess.ttf", 8);
    }

    private static void LoadImages()
    {
        // Backgrounds
        NewImage("Menu", "main_page.jpg");
        NewImage("Discovery", "discover.jpg");
        NewImage("Deploy", "deploy.jpg");

        // Deployment
        NewImage("LeftRightButton", "deploy_dir_button_horiz.png");
        NewImage("UpDownButton", "deploy_dir_button_vert.png");
        NewImage("SelectedShip", "deploy_button_hl.png");
        NewImage("PlayButton", "deploy_play_button.png");
        NewImage("RandomButton", "deploy_randomize_button.png");

        // Ships
        int i;
        for (i = 1; i <= 5; i++)
        {
            NewImage("ShipLR" + i, "ship_deploy_horiz_" + i + ".png");
            NewImage("ShipUD" + i, "ship_deploy_vert_" + i + ".png");
        }

        // Explosions
        NewImage("Explosion", "explosion.png");
        NewImage("Splash", "splash.png");
    }

    private static void LoadSounds()
    {
        NewSound("Error", "error.wav");
        NewSound("Hit", "hit.wav");
        NewSound("Sink", "sink.wav");
        NewSound("Siren", "siren.wav");
        NewSound("Miss", "watershot.wav");
        NewSound("Winner", "winner.wav");
        NewSound("Lose", "lose.wav");
    }

    private static void LoadMusic()
    {
        NewMusic("Background", "horrordrone.mp3");
    }

    /// <summary>
    /// Gets a Font Loaded in the Resources
    /// </summary>
    /// <param name="font">Name of Font</param>
    /// <returns>The Font Loaded with this Name</returns>
    public static Font GameFont(string font)
    {
        return Fonts[font];
    }

    /// <summary>
    /// Gets an Image loaded in the Resources
    /// </summary>
    /// <param name="image">Name of image</param>
    /// <returns>The image loaded with this name</returns>
    public static Bitmap GameImage(string image)
    {
        return Images[image];
    }

    /// <summary>
    /// Gets an sound loaded in the Resources
    /// </summary>
    /// <param name="sound">Name of sound</param>
    /// <returns>The sound with this name</returns>
    public static SoundEffect GameSound(string sound)
    {
        return Sounds[sound];
    }

    /// <summary>
    /// Gets the music loaded in the Resources
    /// </summary>
    /// <param name="music">Name of music</param>
    /// <returns>The music with this name</returns>
    public static Music GameMusic(string music)
    {
        return Music[music];
    }

    private static Dictionary<string, Bitmap> Images = new Dictionary<string, Bitmap>();
    private static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
    private static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
    private static Dictionary<string, Music> Music = new Dictionary<string, Music>();

    private static Bitmap Background;
    private static Bitmap Animation;
    private static Bitmap LoaderFull;
    private static Bitmap LoaderEmpty;
    private static Font LoadingFont;
    private static SoundEffect StartSound;

    /// <summary>
    /// The Resources Class stores all of the Games Media Resources, such as Images, Fonts
    /// Sounds, Music.
    /// </summary>
    public static void LoadResources()
    {
        int width, height;

        width = SwinGame.ScreenWidth();
        height = SwinGame.ScreenHeight();

        SwinGame.ChangeScreenSize(800, 600);

        ShowLoadingScreen();

        ShowMessage("Loading fonts...", 0);
        LoadFonts();
        SwinGame.Delay(100);

        ShowMessage("Loading images...", 1);
        LoadImages();
        SwinGame.Delay(100);

        ShowMessage("Loading sounds...", 2);
        LoadSounds();
        SwinGame.Delay(100);

        ShowMessage("Loading music...", 3);
        LoadMusic();
        SwinGame.Delay(100);

        SwinGame.Delay(100);
        ShowMessage("Game loaded...", 5);
        SwinGame.Delay(100);
        EndLoadingScreen(width, height);
    }

    private static void ShowLoadingScreen()
    {
        Background = SwinGame.LoadBitmap(SwinGame.PathToResource("SplashBack.png", ResourceKind.BitmapResource));
        SwinGame.DrawBitmap(Background, 0, 0);
        SwinGame.RefreshScreen();
        SwinGame.ProcessEvents();

        Animation = SwinGame.LoadBitmap(SwinGame.PathToResource("SwinGameAni.jpg", ResourceKind.BitmapResource));
        LoadingFont = SwinGame.LoadFont(SwinGame.PathToResource("arial.ttf", ResourceKind.FontResource), 12);
        StartSound = Audio.LoadSoundEffect(SwinGame.PathToResource("SwinGameStart.ogg", ResourceKind.SoundResource));

        LoaderFull = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_full.png", ResourceKind.BitmapResource));
        LoaderEmpty = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_empty.png", ResourceKind.BitmapResource));

        PlaySwinGameIntro();
    }

    private static void PlaySwinGameIntro()
    {
        const int AniCellCount = 11;

        Audio.PlaySoundEffect(StartSound);
        SwinGame.Delay(200);

        int i;
        for (i = 0; i <= AniCellCount - 1; i++)
        {
            SwinGame.DrawBitmap(Background, 0, 0);
            SwinGame.Delay(20);
            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();
        }

        SwinGame.Delay(1500);
    }

    private static void ShowMessage(string message, int number)
    {
        const int Tx = 310;
        const int Ty = 493;
        const int Tw = 200;
        const int Th = 25;
        const int Steps = 5;
        const int BgX = 279;
        const int BgY = 453;

        int fullW;
        Rectangle toDraw;

        fullW = 260 * number / Steps;
        SwinGame.DrawBitmap(LoaderEmpty, BgX, BgY);
        SwinGame.DrawCell(LoaderFull, 0, BgX, BgY);
        // SwinGame.DrawBitmapPart(_LoaderFull, 0, 0, fullW, 66, BG_X, BG_Y)

        toDraw.X = Tx;
        toDraw.Y = Ty;
        toDraw.Width = Tw;
        toDraw.Height = Th;
        SwinGame.DrawTextLines(message, Color.White, Color.Transparent, LoadingFont, FontAlignment.AlignCenter, toDraw);
        // SwinGame.DrawTextLines(message, Color.White, Color.Transparent, _LoadingFont, FontAlignment.AlignCenter, TX, TY, TW, TH)

        SwinGame.RefreshScreen();
        SwinGame.ProcessEvents();
    }

    private static void EndLoadingScreen(int width, int height)
    {
        SwinGame.ProcessEvents();
        SwinGame.Delay(500);
        SwinGame.ClearScreen();
        SwinGame.RefreshScreen();
        SwinGame.FreeFont(LoadingFont);
        SwinGame.FreeBitmap(Background);
        SwinGame.FreeBitmap(Animation);
        SwinGame.FreeBitmap(LoaderEmpty);
        SwinGame.FreeBitmap(LoaderFull);
        Audio.FreeSoundEffect(StartSound);
        SwinGame.ChangeScreenSize(width, height);
    }

    private static void NewFont(string fontName, string fileName, int size)
    {
        Fonts.Add(fontName, SwinGame.LoadFont(SwinGame.PathToResource(fileName, ResourceKind.FontResource), size));
    }

    private static void NewImage(string imageName, string fileName)
    {
        Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(fileName, ResourceKind.BitmapResource)));
    }

    private static void NewTransparentColorImage(string imageName, string fileName, Color transColor)
    {
        Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(fileName, ResourceKind.BitmapResource)));
    }

    private static void NewTransparentColourImage(string imageName, string fileName, Color transColor)
    {
        NewTransparentColorImage(imageName, fileName, transColor);
    }

    private static void NewSound(string soundName, string fileName)
    {
        Sounds.Add(soundName, Audio.LoadSoundEffect(SwinGame.PathToResource(fileName, ResourceKind.SoundResource)));
    }

    private static void NewMusic(string musicName, string fileName)
    {
        Music.Add(musicName, Audio.LoadMusic(SwinGame.PathToResource(fileName, ResourceKind.SoundResource)));
    }

    private static void FreeFonts()
    {
        Font obj;
        foreach (var obj in Fonts.Values)
            SwinGame.FreeFont(obj);
    }

    private static void FreeImages()
    {
        Bitmap obj;
        foreach (var obj in Images.Values)
            SwinGame.FreeBitmap(obj);
    }

    private static void FreeSounds()
    {
        SoundEffect obj;
        foreach (var obj in Sounds.Values)
            Audio.FreeSoundEffect(obj);
    }

    private static void FreeMusic()
    {
        Music obj;
        foreach (var obj in Music.Values)
            Audio.FreeMusic(obj);
    }

    public static void FreeResources()
    {
        FreeFonts();
        FreeImages();
        FreeMusic();
        FreeSounds();
        SwinGame.ProcessEvents();
    }
}
