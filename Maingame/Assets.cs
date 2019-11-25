using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Origin.Characters;
using Origin.Display;

namespace Origin
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class Assets
    {
        public static SpriteFont FontNormal;
        public static SpriteFont FontNormalBold;
        public static SpriteFont FontBig;
        public static SpriteFont FontBigBold;
        public static SpriteFont FontLittle;
        public static SpriteFont FontLittleItalic;
        public static SpriteFont FontLittleBold;
        public static SpriteFont FontLittleBoldItalic;
        public static Texture2D QuestionMark;

        private static ContentManager content;

        public static void LoadAll(ContentManager contentParameter)
        {
            Assets.content = contentParameter;
            Assets.LoadSpriteFont(out Assets.FontLittle, "Fonts\\Little");
            Assets.LoadSpriteFont(out Assets.FontLittleBold, "Fonts\\LittleBold");
            Assets.LoadSpriteFont(out Assets.FontLittleBoldItalic, "Fonts\\LittleBoldItalics");
            Assets.LoadSpriteFont(out Assets.FontNormal, "Fonts\\Standard");
            Assets.LoadSpriteFont(out Assets.FontNormalBold, "Fonts\\StandardBold");
            Assets.LoadSpriteFont(out Assets.FontBig, "Fonts\\Big");
            Assets.LoadSpriteFont(out Assets.FontBigBold, "Fonts\\BigBold");
            Assets.LoadSpriteFont(out Assets.FontLittleItalic, "Fonts\\LittleItalics");

            BitmapFont annaRegular = content.Load<BitmapFont>("Fonts\\Anna\\AnnaRegular");
            BitmapFontGroup.AnnaFont = new BitmapFontGroup(annaRegular,annaRegular,annaRegular,annaRegular);
            BitmapFont mia = content.Load<BitmapFont>("Fonts\\Anna\\MiaRegular");
            BitmapFont miaBold = content.Load<BitmapFont>("Fonts\\Anna\\MiaBoldBitmap");
            BitmapFont miaItalic = content.Load<BitmapFont>("Fonts\\Anna\\MiaItalic");
            BitmapFont mia2 = content.Load<BitmapFont>("Fonts\\Mia\\MiaSmallRegular");
            BitmapFont mia2Bold = content.Load<BitmapFont>("Fonts\\Mia\\MiaSmallBold");
            BitmapFont mia2Italic = content.Load<BitmapFont>("Fonts\\Mia\\MiaSmallItalics");
            BitmapFontGroup.MiaSmallFont = new BitmapFontGroup(mia2,mia2Italic,mia2Bold,mia2Bold);
            BitmapFont mia24 = content.Load<BitmapFont>("Fonts\\Mia\\Mia24");
            BitmapFont mia24Bold = content.Load<BitmapFont>("Fonts\\Mia\\Mia24Bold");
            BitmapFont mia24Italics = content.Load<BitmapFont>("Fonts\\Mia\\Mia24Italics");
            
            BitmapFont mia18 = content.Load<BitmapFont>("Fonts\\Mia\\Mia18");
            BitmapFont mia18Italics = content.Load<BitmapFont>("Fonts\\Mia\\Mia18Italics");
            BitmapFont mia18Bold = content.Load<BitmapFont>("Fonts\\Mia\\Mia18Bold");
            
            BitmapFont mia12 = content.Load<BitmapFont>("Fonts\\Mia\\Mia12Regular");
            BitmapFont mia12Italics = content.Load<BitmapFont>("Fonts\\Mia\\Mia12Italics");
            BitmapFont mia12Bold = content.Load<BitmapFont>("Fonts\\Mia\\Mia12Bold");
            
            BitmapFontGroup.Mia12Font = new BitmapFontGroup(mia12,mia12Italics,mia12Bold,mia12Bold);
            BitmapFontGroup.Mia18Font = new BitmapFontGroup(mia18,mia18Italics,mia18Bold,mia18Bold, BitmapFontGroup.Mia12Font);
            BitmapFontGroup.Mia24Font = new BitmapFontGroup(mia24,mia24Italics,mia24Bold,mia24Bold, BitmapFontGroup.Mia18Font);
            BitmapFontGroup.MiaFont = new BitmapFontGroup(mia,miaItalic,miaBold,miaBold, BitmapFontGroup.Mia24Font);
            
            Assets.LoadTexture(out Assets.QuestionMark, "QuestionMark");
            
            StartFetching();
        }
        

        private static void LoadSpriteFont(out SpriteFont spriteFont, string assetName)
        {
            spriteFont = Assets.content.Load<SpriteFont>(assetName);
        }

        private static void LoadTexture(out Texture2D texture2D, string assetName)
        {
            texture2D = Assets.content.Load<Texture2D>(assetName);
        }
        

        static object mutex = new object();
        static bool fetching;
        
        private static readonly ConcurrentQueue<Illustration> FetchWhat = new ConcurrentQueue<Illustration>();
        private static readonly ConcurrentDictionary<Illustration, Texture2D> ConcurrentCardTextures = new ConcurrentDictionary<Illustration, Texture2D>();
        static void StartFetching()
        {
            lock (mutex)
            {
                var f = fetching;
                if (f) return;
                Illustration fetchWhat;
                if (FetchWhat.TryDequeue(out  fetchWhat))
                {
                    Task.Factory.StartNew(() =>
                    {
                        Texture2D result = content.Load<Texture2D>("Illustrations\\" + fetchWhat.ToString());
                        ConcurrentCardTextures[fetchWhat] = result;
                        lock (mutex)
                        {
                            fetching = false;
                        }
                        StartFetching();
                    });
                    fetching = true;
                }
            }
        }
        public static Texture2D TextureFromCard(Illustration c)
        {
            Texture2D value;
            if (ConcurrentCardTextures.TryGetValue(c, out value))
            {
                if (value != null)
                {
                    return value;
                }
            }
            else
            {
                ConcurrentCardTextures[c] = null;
                FetchWhat.Enqueue(c);
                StartFetching();
            }
            return Assets.QuestionMark;
        }
        public static Texture2D Crop(Texture2D texture, Rectangle newBounds)
        {
            // Get your texture

            // Calculate the cropped boundary

            // Create a new texture of the desired size
            Texture2D croppedTexture = new Texture2D(Root.GraphicsDevice, newBounds.Width, newBounds.Height);

            // Copy the data from the cropped region into a buffer, then into the new texture
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            texture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);
            return croppedTexture;
        }

        private static Illustration[] allIllustrations = (Illustration[]) Enum.GetValues(typeof(Illustration));
        public static Illustration IllustrationFromPath(string sourceStr)
        {
            string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(sourceStr);
            foreach (var illustration in allIllustrations)
            {
                if (illustration.ToString().Equals(withoutExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return illustration;
                }
            }
            throw new Exception("nonexistent illustration");
        }
    }

    public enum Illustration
    {
        None,
        MainTitle,
        church,
        AutumnTree,
        Bag,
        BlueCivilian,
        BrownBrick,
        BrownDoor,
        Bush,
        Chest,
        Chest2,
        Cobweb,
        CultistOfFire,
        Fire16,
        Fire20,
        Fire24,
        Fire32,
        Ghoul,
        Grass,
        Grass2,
        Grass3,
        Grave,
        Grave2,
        GreenCivilian,
        PurpleCivilian,
        Rock,
        Sidewalk,
        WarriorPaladin,
        WaterPaladin,
        Weed,
        Water,
        WoodenGrave,
        Evac,
        Ash,
        BaseCharacter
    }
}