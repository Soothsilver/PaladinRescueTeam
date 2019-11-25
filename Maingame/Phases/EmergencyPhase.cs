using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoreLinq;
using Origin.Characters;
using Origin.Display;
using Origin.Levels;
using Origin.Mission;
using SharpDX.MediaFoundation;

namespace Origin.Phases
{
    public class EmergencyPhase : AuxiGamePhase
    {
        public Difficulty ChosenDifficulty { get; }
        public EmergencyRecords Records = new EmergencyRecords();
        public TSession Session;
        public Vector2 Offset = Vector2.Zero;
        private float KEYBOARD_OFFSET_SPEED = 22;
        private float MOUSE_OFFSET_SPEED = 22;
        private int MOUSE_BORDER = 3;
        private int TILESIZE = 32;
        public Character SelectedCharacter;
        private Tile mouseOverTile = null;
        private Character mouseOverCharacter = null;
        private ContextMenu contextMenu;
        private float TimeDilation = 1;

        public EmergencyPhase(LevelSheet ls, List<CharacterSheet> chosenPaladins, Difficulty chosenDifficulty)
        {
            ChosenDifficulty = chosenDifficulty;
            Session = new TSession(ls.MapFileName, chosenPaladins,ls.Id, chosenDifficulty);
            Offset = Session.Characters.First(chara => !chara.IsNPC).Position;
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            if (Treasure.Instance.TimeDilationFast)
            {
                elapsedSeconds *= 4;
            }

            elapsedSeconds *= TimeDilation;
            mouseOverTile = null;
            mouseOverCharacter = null;
            base.Draw(sb, game, elapsedSeconds);
            Primitives.FillRectangle(Root.Screen, Color.Black);
            Rectangle start = ToReal(new Rectangle(0, 0, 1, 1));
            Primitives.DrawRectangle(new Rectangle(start.X - 3, start.Y - 3, Session.MapWidth * TILESIZE + 6, Session.MapHeight * TILESIZE + 6), Color.White, 3);
            for (int x = 0; x < Session.MapWidth; x++)
            {
                for (int y = 0; y < Session.MapHeight; y++)
                {
                    var rectThisTile = new Rectangle(start.X + x * TILESIZE, start.Y + y * TILESIZE, TILESIZE, TILESIZE);
                    if (rectThisTile.Right < 0 || rectThisTile.Bottom < 0 || rectThisTile.X >= Root.ScreenWidth ||
                        rectThisTile.Y >= Root.ScreenHeight)
                    {
                        continue;
                    }
                    Session.Map[x, y].Draw(rectThisTile);
                    if (Root.IsMouseOver(rectThisTile))
                    {
                        mouseOverTile = Session.Map[x, y];
                    }
                }
            }

            foreach (var character in Session.Characters)
            {
                Point p = ToReal(character.Position);
                Rectangle rect = new Rectangle(p.X, p.Y, TILESIZE, TILESIZE);

                if (Root.IsMouseOver(rect))
                {
                    if (mouseOverCharacter == null || mouseOverCharacter.IsNPC || mouseOverCharacter.Dead)
                    {
                        mouseOverCharacter = character;
                    }
                }

                if (character.Dead)
                {
                    rect = new Rectangle(rect.X + 8, rect.Y + 8, rect.Width -16, rect.Height - 16);
                }

                Primitives.DrawImage(Assets.TextureFromCard(character.Illustration), rect, character.Dead ? Color.Red : Color.White);
                if (character.ImmediateActivity != null)
                {
                    Writer.DrawProgressBar(new Rectangle(rect.X, rect.Y - 6, rect.Width,4), Color.Yellow, character.ImmediateActivity.SecondsProgressed,
                        character.ImmediateActivity.SecondsToComplete, null);
                }

                if (character == SelectedCharacter)
                {
                    Primitives.DrawRectangle(rect, Color.White);
                }
            }

            if (!Treasure.Instance.CheatMode)
            {
                for (int x = 0; x < Session.MapWidth; x++)
                {
                    for (int y = 0; y < Session.MapHeight; y++)
                    {
                        var rectThisTile = new Rectangle(start.X + x * TILESIZE, start.Y + y * TILESIZE, TILESIZE,
                            TILESIZE);
                        if (rectThisTile.Right < 0 || rectThisTile.Bottom < 0 || rectThisTile.X >= Root.ScreenWidth ||
                            rectThisTile.Y >= Root.ScreenHeight)
                        {
                            continue;
                        }

                        Tile tile = Session.Map[x, y];
                        if (tile.Blackened)
                        {
                            Primitives.FillRectangle(rectThisTile, Color.DarkBlue);
                        }

                    }
                }
            }

            for (int x = 0; x < Session.MapWidth; x++)
            {
                for (int y = 0; y < Session.MapHeight; y++)
                {
                    var rectThisTile =
                        new Rectangle(start.X + x * TILESIZE, start.Y + y * TILESIZE, TILESIZE, TILESIZE);
                    if (rectThisTile.Right < 0 || rectThisTile.Bottom < 0 || rectThisTile.X >= Root.ScreenWidth ||
                        rectThisTile.Y >= Root.ScreenHeight)
                    {
                        continue;
                    }
                    Tile tile = Session.Map[x,y];

                    foreach (Overhead overhead in tile.Overheads)
                    {
                        overhead.Draw(rectThisTile);
                    }

                    tile.Overheads.RemoveAll(oh => oh.UpdateAndPossiblyDelete(elapsedSeconds));
                }
            }

            for (int wi = 0; wi < Session.Particles.Count; wi++)
            {
                WaterParticle pp = Session.Particles[wi];
                Point asReal = ToReal(pp.Position);
                Primitives.DrawPoint(new Vector2(asReal.X, asReal.Y), Color.Blue, 6);
                pp.Position += pp.Speed * elapsedSeconds;
                pp.TimeLeft -= elapsedSeconds;
                if (pp.TimeLeft <= 0)
                {
                    Session.Particles.RemoveAt(wi);
                    wi--;
                }
            }

            DrawTopBar();
            DrawBottomBar();
            contextMenu?.Draw();
        }

        private void DrawBottomBar()
        {
            Rectangle rectBottomBar = new Rectangle(0, Root.ScreenHeight - 200, Root.ScreenWidth, 200);
            Primitives.DrawAndFillRectangle(rectBottomBar, Color.Wheat, Color.Blue);
            if (SelectedCharacter == null)
            {
                Writer.DrawString("Left-click a character to select, then right-click to interact.", rectBottomBar,
                    alignment: Writer.TextAlignment.Middle);
            }
            else
            {
                Writer.DrawString(SelectedCharacter.DescribeSelf(),
                    new Rectangle(rectBottomBar.X + 100, rectBottomBar.Y + 5, 600, 180));
                Writer.DrawProgressBar(new Rectangle(rectBottomBar.X + 400, rectBottomBar.Y + 5, 300, 50),
                    Color.Lime, SelectedCharacter.HP, SelectedCharacter.MaxHP, "Health");
                var activity = SelectedCharacter.ImmediateActivity;
                if (activity != null)
                {
                    Writer.DrawProgressBar(new Rectangle(rectBottomBar.X + 400, rectBottomBar.Y + 55, 300, 50),
                        Color.Yellow, activity.SecondsProgressed, activity.SecondsToComplete, activity.Progress);
                }

                if (SelectedCharacter.HeldItems.Count > 0)
                {
                    Writer.DrawString("Held items:\n" + string.Join("\n", SelectedCharacter.HeldItems.Select(hi => hi.DescribeSelf())), 
                        new Rectangle(rectBottomBar.X + 710, rectBottomBar.Y + 5, 400, 200), degrading: true);
                }
            }
        }

        private Rectangle ToReal(Rectangle rules)
        {
            int rulesX = rules.X;
            float rulesXVersusCenterOfScreen = (float)rulesX - Offset.X;
            float pixelsRightOffCenterScreen = rulesXVersusCenterOfScreen * TILESIZE;
            float pixelsToCenterScreenX = Root.ScreenWidth / 2f;
            float x = pixelsToCenterScreenX + pixelsRightOffCenterScreen;
            
            int rulesY = rules.Y;
            float rulesYVersusCenterOfScreen = (float)rulesY - Offset.Y;
            float pixelsBottomOffCenterScreen = rulesYVersusCenterOfScreen * TILESIZE;
            float pixelsToCenterScreenY = Root.ScreenHeight / 2f;
            float y = pixelsToCenterScreenY + pixelsBottomOffCenterScreen;

            return new Rectangle((int) x, (int) y, rules.Width * TILESIZE, rules.Height * TILESIZE);
        }
        private Point ToReal(Vector2 rules)
        {
            float rulesX = rules.X;
            float rulesXVersusCenterOfScreen = (float)rulesX - Offset.X;
            float pixelsRightOffCenterScreen = rulesXVersusCenterOfScreen * TILESIZE;
            float pixelsToCenterScreenX = Root.ScreenWidth / 2f;
            float x = pixelsToCenterScreenX + pixelsRightOffCenterScreen;
            
            float rulesY = rules.Y;
            float rulesYVersusCenterOfScreen = (float)rulesY - Offset.Y;
            float pixelsBottomOffCenterScreen = rulesYVersusCenterOfScreen * TILESIZE;
            float pixelsToCenterScreenY = Root.ScreenHeight / 2f;
            float y = pixelsToCenterScreenY + pixelsBottomOffCenterScreen;

            return new Point((int) x, (int) y);
        }

        protected override void Update(Game game, float elapsedSeconds)  
        {
            elapsedSeconds *= TimeDilation;
            if (Treasure.Instance.TimeDilationFast)
            {
                elapsedSeconds *= 4;
            }
           
            if (Root.WasKeyPressed(Keys.Escape))
            {
                Retreat();
            }

            if (Root.WasKeyPressed(Keys.F, ModifierKey.Ctrl))
            {
                Treasure.Instance.ShowFireMode = !Treasure.Instance.ShowFireMode;
                Treasure.Instance.Save();
            }  
            if (Root.WasKeyPressed(Keys.T, ModifierKey.Ctrl))
            {
                Treasure.Instance.TimeDilationFast = !Treasure.Instance.TimeDilationFast;
                Treasure.Instance.Save();
            }
            if (Root.WasKeyPressed(Keys.R, ModifierKey.Ctrl))
            {
                Session.AllTiles.Where(tl => tl.Trap != TrapId.NotTrapped).ForEach(tr => tr.TrapGoesOff());
            }

            CameraMovement(elapsedSeconds);
            Session.Update(elapsedSeconds);
            base.Update(game, elapsedSeconds);

            if (Session.Victory.HasValue)
            {
                SFX.StopAll();
                if (Session.Victory.Value)
                {
                    Root.PushPhase(new MissionCompletedPhase(new EmergencyRecords()
                    {
                        Reason = EndReason.Victory,
                        Text = Session.VictoryText
                    }));
                }
                else
                {
                    Root.PushPhase(new MissionCompletedPhase(new EmergencyRecords()
                    {
                        Reason = Session.VictoryEndReason
                    }));
                }
            }

            if (contextMenu?.ScheduledForElimination ?? false)
            {
                contextMenu = null;
            }

            if (Root.WasMouseLeftClick)
            { 
                if (contextMenu != null)
                {
                    contextMenu.HandleLeftClick();
                }
                else if (mouseOverCharacter != null)
                {
                    SelectedCharacter = mouseOverCharacter;
                }
                else
                {
                    SelectedCharacter = null;
                }
            }

            if (Root.WasMouseRightClick && SelectedCharacter != null && SelectedCharacter.IsLivingPaladin)
            {
                List<Interaction> interactions = new List<Interaction>();
                if (mouseOverCharacter != null)
                {
                    interactions = mouseOverCharacter.GetInteractionsBy(SelectedCharacter).ToList();
                }
                if (mouseOverTile != null && interactions.Count == 0)
                {
                    interactions = mouseOverTile.GetInteractionsBy(SelectedCharacter).ToList();
                }

                if (interactions.Count == 1 && interactions[0].IsDirectlyExecutable)
                {
                    interactions[0].FullExecute();
                }
                else if (interactions.Count > 0)
                {
                    this.contextMenu = ContextMenu.Generate(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, interactions);
                }
            }
            
        }

        private void CameraMovement(float elapsedSeconds)
        {
            if (Root.Keyboard_NewState.IsKeyDown(Keys.Down) || Root.Keyboard_NewState.IsKeyDown(Keys.S))
            {
                Offset = new Vector2(Offset.X, Offset.Y + elapsedSeconds * KEYBOARD_OFFSET_SPEED);
            }

            if (Root.Keyboard_NewState.IsKeyDown(Keys.Up) || Root.Keyboard_NewState.IsKeyDown(Keys.W))
            {
                Offset = new Vector2(Offset.X, Offset.Y - elapsedSeconds * KEYBOARD_OFFSET_SPEED);
            }

            if (Root.Keyboard_NewState.IsKeyDown(Keys.Left) || Root.Keyboard_NewState.IsKeyDown(Keys.A))
            {
                Offset = new Vector2(Offset.X - elapsedSeconds * KEYBOARD_OFFSET_SPEED, Offset.Y);
            }

            if (Root.Keyboard_NewState.IsKeyDown(Keys.Right) || Root.Keyboard_NewState.IsKeyDown(Keys.D))
            {
                Offset = new Vector2(Offset.X + elapsedSeconds * KEYBOARD_OFFSET_SPEED, Offset.Y);
            }

            if (Root.Mouse_NewState.Y >= Root.ScreenHeight - MOUSE_BORDER)
            {
                Offset = new Vector2(Offset.X, Offset.Y + elapsedSeconds * MOUSE_OFFSET_SPEED);
            }

            if (Root.Mouse_NewState.Y <= MOUSE_BORDER)
            {
                Offset = new Vector2(Offset.X, Offset.Y - elapsedSeconds * MOUSE_OFFSET_SPEED);
            }

            if (Root.Mouse_NewState.X <= MOUSE_BORDER)
            {
                Offset = new Vector2(Offset.X - elapsedSeconds * MOUSE_OFFSET_SPEED, Offset.Y);
            }

            if (Root.Mouse_NewState.X >= Root.ScreenWidth - MOUSE_BORDER)
            {
                Offset = new Vector2(Offset.X + elapsedSeconds * MOUSE_OFFSET_SPEED, Offset.Y);
            }
        }

        private void DrawTopBar()
        {
            Rectangle rectTopbar = new Rectangle(0,0,Root.ScreenWidth, 40);
            Primitives.FillRectangle(rectTopbar, Color.LightBlue);
            Primitives.DrawRectangle(new Rectangle(-3, -3, Root.ScreenWidth + 6, 43), Color.Black);
            Writer.DrawString("Wounded remaining: " +
                              Session.Characters.Count(ch => ch.Wounded && !ch.Dead) + "; dead: "
                              + Session.Characters.Count(ch => ch.Dead && !ch.Hostile) + "; hostiles: " + 
                              Session.Characters.Count(ch => ch.Hostile && !ch.Dead) + "; fires: " +
                              Session.TilesOnFire + "; burnt out: " +
                              Session.TilesBurntOut, rectTopbar.Extend(-3, 0), 
                Color.Black, BitmapFontGroup.Mia24Font,
                alignment: Writer.TextAlignment.Left);
            
            UX.DrawButton("1x", new Rectangle(Root.ScreenWidth - 640,0,80,rectTopbar.Height), () =>
                {
                    TimeDilation = 1;
                }, "Play the game at the pace it's meant to be played.", TimeDilation == 1 );
            
            UX.DrawButton("2x", new Rectangle(Root.ScreenWidth - 560,0,80,rectTopbar.Height), () =>
                {
                    TimeDilation = 2;
                }, "If you want more action, speed the game up two times.", TimeDilation == 2 );
            
            UX.DrawButton("3x", new Rectangle(Root.ScreenWidth - 480,0,80,rectTopbar.Height), () =>
                {
                    TimeDilation = 3;
                }, "If you have very good reflexes and/or are impatient, speed the game up three times!", TimeDilation == 3 );
            UX.DrawButton("Objectives", new Rectangle(Root.ScreenWidth - 400, 0, 190, rectTopbar.Height), () =>
            {
                Root.PushPhase(new ObjectivesPhase());
            });
            UX.DrawButton("Retreat", new Rectangle(Root.ScreenWidth - 200, 0, 190, rectTopbar.Height), () =>
            {
                Retreat();
            }, "Fail this emergency and return to your monastery.");
        }

        private void Retreat()
        {
            Root.PushPhase(new MessageBoxPhase(
                "End this mission so that you can attempt it again?",
                "Retreat?", GuiIcon.Question, MessageBoxButtons.YesNo, (s) =>
                {
                    SFX.StopAll();
                    Treasure.Instance.Save();
                    this.Records.Reason = EndReason.Retreat;
                    Root.PushPhase(new MissionCompletedPhase(this.Records));
                }));
        }
    }
}