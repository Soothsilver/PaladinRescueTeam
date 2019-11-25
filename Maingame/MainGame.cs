using System;
using System.Linq;
using System.Windows.Forms;
using Auxiliary;
using Auxiliary.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Origin.Display;
using Origin.Phases;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Origin
{
    public class MainGame : Game
    {
        private bool ImmediatelyFullscreenize = false;
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private static MainGame instance;
        public Form form;
        private bool isFullScreen = true;
        
        public static MainGame Instance => MainGame.instance;

        public MainGame()
        {
            if (MainGame.instance != null)
            {
                throw new Exception("You cannot create more than one instance of the main game class.");
            }
            string[] args = Environment.GetCommandLineArgs();
           
            MainGame.instance = this;
            this.graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            this.Content.RootDirectory = "Content";
            IntPtr hWnd = this.Window.Handle;
            var control = Control.FromHandle(hWnd);
            this.form = control.FindForm();
            this.form.FormBorderStyle = FormBorderStyle.None;
            //  form.TopMost = true;
            this.form.Width = 800;
            this.form.Height = 600;
            this.form.WindowState = FormWindowState.Maximized;
            
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Window.Title = "Paladin Rescue Team";

            Resolution resolution = Utilities.GetSupportedResolutions().Last();


            // Init call mandated by Auxiliary library.
            Root.Init(this, this.spriteBatch, this.graphics, resolution);
            Root.BeforeLastDraw = () =>
            {
                UX.Clear();
                Tooltip.Clear();
            };
            Writer.SpriteBatch = this.spriteBatch;


            if (this.ImmediatelyFullscreenize)
            {
                Root.SetResolution(Utilities.GetSupportedResolutions().FindLast(m => true));
                Root.IsFullscreen = true;
            }


            Assets.LoadAll(Content);
            SFX.Load(Content);



            Primitives.Fonts.Add(FontFamily.Small,
                new FontGroup(Assets.FontLittle, Assets.FontLittleItalic, Assets.FontLittleBold,
                    Assets.FontLittleBoldItalic));
            Primitives.Fonts.Add(FontFamily.Normal,
                new FontGroup(Assets.FontNormal, Assets.FontNormal, Assets.FontNormalBold, Assets.FontNormalBold));
            Primitives.Fonts.Add(FontFamily.Big,
                new FontGroup(Assets.FontBig, Assets.FontBig, Assets.FontBigBold, Assets.FontBigBold));


            // Make the buttons orange.
            GuiSkin mainSkin = GuiSkin.DefaultSkin;
            mainSkin.Font = Assets.FontNormal;
            mainSkin.InnerBorderThickness = 1;
            mainSkin.OuterBorderThickness = 1;
            mainSkin.GreyBackgroundColor = Color.DarkOrange;
            mainSkin.GreyBackgroundColorMouseOver = Color.Orange;
            mainSkin.InnerBorderColor = Color.Red;
            mainSkin.InnerBorderColorMouseOver = Color.Red;
            mainSkin.InnerBorderColorMousePressed = Color.DarkRed;

            Root.Display_DisplayFpsCounter = false;
            Root.Display_DisplayFpsCounterWhere = new Vector2(5, 55);
            // Go to main menu.
            Root.PushPhase(new MainMenuPhase());
        }
        


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // We will not accept any input when the game window is not in focus.
            if (!this.IsActive)
            {
                return;
            }

            if (Root.WasKeyPressed(Keys.F1, ModifierKey.Ctrl))
            {
                Treasure.Instance.CheatMode = !Treasure.Instance.CheatMode;
                Treasure.Instance.Save();
            }
            
            // Toggle fullscreen
            if (Root.WasKeyPressed(Keys.Enter, ModifierKey.Alt))
            {
                if (isFullScreen)
                {
                    isFullScreen = false;
                    Root.SetResolution(1280, 1024);
                    this.form.FormBorderStyle = FormBorderStyle.FixedSingle;
                    this.form.WindowState = FormWindowState.Normal;
                }
                else
                {
                    isFullScreen = true;
                    Root.SetResolution(Utilities.GetSupportedResolutions().FindLast(m => true));
                    this.form.FormBorderStyle = FormBorderStyle.None;
                    this.form.WindowState = FormWindowState.Maximized;
                }
            }
            // Accept input, do actions
            
            Root.Update(gameTime);
            
            // Exit when we pop out of the main menu
            if (Root.PhaseStack.Count == 0)
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Auxiliary handles all drawing.
            this.GraphicsDevice.Clear(Color.CornflowerBlue);
            Tooltip.Clear();
            UX.Clear();
            this.spriteBatch.Begin();
            Root.DrawPhase(gameTime);
            Tooltip.DrawTooltipReally();
            Root.DrawOverlay(gameTime);
            this.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}