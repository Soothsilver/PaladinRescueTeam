using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Auxiliary
{
    /// <summary>
    /// Provides an extension method that allows you to load multiple assets at the same time.
    /// </summary>
    public static class LoadContentExtensionMethod
    {
        /// <summary>
        /// Loads all files from the specified folder as the given type.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="contentFolder">Folder name from which to load assets.</param>
        /// <typeparam name="T">Type of asset. All assets in the folder must be of this type.</typeparam>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">This folder does not exist.</exception>
        /// <remarks>
        /// This code comes from Stack Overflow, created by Neil Knights (http://stackoverflow.com/users/410636/neil-knight). The post's page is http://stackoverflow.com/questions/4052532/xna-get-an-array-list-of-resources.
        /// </remarks>
        public static Dictionary<string, T> LoadContent<T>(this ContentManager contentManager, string contentFolder)
        {
            var dir = new DirectoryInfo(contentManager.RootDirectory
                                        + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            var result = new Dictionary<string, T>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string key = Path.GetFileNameWithoutExtension(file.Name) ?? file.Name;

                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }

            return result;
        }

    }

    /// <summary>
    /// Contains some textures and font. 
    /// This is a DrawableGameComponent because it needs to load content from an alternate content library.
    /// </summary>
    public class Library: DrawableGameComponent
    {
        private static ContentManager libContent;

        /// <summary>
        /// For the given icon description, returns an appropriate texture.
        /// </summary>
        /// <param name="icon"></param>
        public static Texture2D GetTexture2DFromGuiIcon(GuiIcon icon)
        {
            switch (icon)
            {
                case GuiIcon.Error: return Library.IconError;
                case GuiIcon.Information: return Library.IconInformation;
                case GuiIcon.Question: return Library.IconQuestion;
                case GuiIcon.Warning: return Library.IconWarning;
            }
            return null;
        }
        /// <summary>
        /// A 1x1 white square.
        /// </summary>
        public static Texture2D Pixel;
        /// <summary>
        /// A 1000x1000 full circle texture.
        /// </summary>
        public static Texture2D Circle1000X1000;
        /// <summary>
        /// A 1000x1000 outline of a circle.
        /// </summary>
        public static Texture2D EmptyCircle1000X1000;
        /// <summary>
        /// An information bubble icon.
        /// </summary>
        public static Texture2D IconInformation;
        /// <summary>
        /// A warning triangle icon.
        /// </summary>
        public static Texture2D IconWarning;
        /// <summary>
        /// An error bubble icon.
        /// </summary>
        public static Texture2D IconError;
        /// <summary>
        /// A question mark bubble icon.
        /// </summary>
        public static Texture2D IconQuestion;
        /// <summary>
        /// A "Play" media player button.
        /// </summary>
        public static Texture2D IconPlay;
        /// <summary>
        /// A "Pause" media player button.
        /// </summary>
        public static Texture2D IconPause;
        /// <summary>
        /// A "Stop" media player button.
        /// </summary>
        public static Texture2D IconStop;
        /// <summary>
        /// A "Switch to Fullscreen" media player button.
        /// </summary>
        public static Texture2D IconFullscreen;
        /// <summary>
        /// A 14px Courier New font.
        /// </summary>
        public static SpriteFont FontCourierNew;
        /// <summary>
        /// A 14px Verdana font.
        /// </summary>
        public static SpriteFont FontVerdana;

        internal Library(Game game)
            : base(game)
        {
            libContent = new ContentManager(game.Services)
                             {
                                 RootDirectory = "AuxiliaryContent 3"
                             };
        }
        public static Dictionary<string, Texture2D> Icons;
        internal static void LoadBaseTextures(bool doNotLoadXxlTextures = false)
        {

            Icons = new Dictionary<string, Texture2D>();

            string[] allIconFiles = System.IO.Directory.GetFiles("Data\\Icons\\Styling", "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string filename in allIconFiles)
            {
                if (filename.EndsWith(".txt"))
                {
                    continue;
                }
                string assetName = filename.Substring("Data\\Icons\\Styling\\".Length);
                Icons.Add(assetName,
                    Utilities.LoadTextureAtRuntime(filename));

            }

            Pixel = libContent.Load<Texture2D>("pixel");
            FontCourierNew = libContent.Load<SpriteFont>("fontCourierNew");
            FontVerdana = libContent.Load<SpriteFont>("fontVerdana");
            IconPlay = libContent.Load<Texture2D>("Icons\\play");
            IconPause = libContent.Load<Texture2D>("Icons\\pause");
            IconInformation = libContent.Load<Texture2D>("Icons\\information");
            IconError = libContent.Load<Texture2D>("Icons\\error");
            IconQuestion = libContent.Load<Texture2D>("Icons\\question");
            IconWarning = libContent.Load<Texture2D>("Icons\\warning");
            IconStop = libContent.Load<Texture2D>("Icons\\stop");
            IconFullscreen = libContent.Load<Texture2D>("Icons\\fullscreen");

            if (doNotLoadXxlTextures)
            {
                Circle1000X1000 = libContent.Load<Texture2D>("pixel");
                EmptyCircle1000X1000 = libContent.Load<Texture2D>("pixel");
            }
            else
            {
                Circle1000X1000 = libContent.Load<Texture2D>("circle1000x1000");
                EmptyCircle1000X1000 = libContent.Load<Texture2D>("emptyCircle1000x1000");
            }
        }
    }
}
