using System.Windows.Forms;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origin.Display;
using Origin.Levels;
using MessageBoxButtons = Auxiliary.MessageBoxButtons;

namespace Origin.Phases
{
    public class MainMenuPhase : AuxiGamePhase
    {
        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            Primitives.FillRectangle(Root.Screen, Color.LightBlue);
            Primitives.DrawImage(Assets.TextureFromCard(Illustration.MainTitle), new Rectangle(0, 0, Root.ScreenWidth, 400),
                null, true, false);
            Writer.DrawString("Welcome to the monastery, commander. Are you ready to serve?",
                new Rectangle(50, 405, 1000, 140), Color.Black);

            DrawRespondToEmergencyButtons();
            int y = 410;
            /*
            UX.DrawButton("Enter your monastery",
                new Rectangle(Root.ScreenWidth - 450, y, 400, 40),
                () => { Root.PushPhase(new MonasteryPhase()); },
                "The monastery is your base of operations. In the monastery, you can {b}review{/b} your rescue team and choose {b}new powers{/b} for your paladins.");
            y += 40;
            UX.DrawButton("Reset all progress",
                new Rectangle(Root.ScreenWidth - 450, y, 400, 40),
                () => {    Root.PushPhase(new MessageBoxPhase("You will reset back to level 1 with two starting paladins, as if you just started the game for the first time. Is that okay?",
                    "Reset all progress?", GuiIcon.Question, MessageBoxButtons.YesNo, (str) =>
                    {
                        Treasure.Clear();
                        Treasure.Instance.Save();
                    }));
                },
                "You will reset back to level 1 with two starting paladins, as if you just started the game for the first time.");*/
            y += 40;
            UX.DrawButton("Help",
                new Rectangle(Root.ScreenWidth - 450, y, 400, 40),
                () => { Root.PushPhase(new HelpPhase()); },
                Treasure.Instance.IsFirstLaunch ? "Maybe you should read these instructions before you respond to an emergency..."
                    : "Read about how to become a better commander of your rescue service.");
            y += 40;
            UX.DrawButton("Quit game",
                new Rectangle(Root.ScreenWidth - 450, y, 400, 40),
                () =>
                {
                    Root.PushPhase(new MessageBoxPhase("Do you really want to quit?",
                        "Quit Game", GuiIcon.Question, MessageBoxButtons.YesNo, (str) => { Root.Quit(); }));
                },
                "Please don't...");
        }

        private void DrawRespondToEmergencyButtons()
        {
            int y = 500;
            for (int i = 0; i < Campaign.Levels.Count; i++)
            {
                LevelSheet ls = Campaign.Levels[i];
                bool legal = Treasure.Instance.CheatMode || i <= (Treasure.Instance.LastCompletedLevel + 1);
               
                UX.DrawButton("Respond to emergency: {b}" + ls.Name + "{/b}",
                    new Rectangle(10, y, 600, 40),
                    () =>
                    {
                        Root.PushPhase(new PrepareForMissionPhase(ls));
                    }, disabled: !legal);
                
                y += 40;
            }       
        }
    }
}