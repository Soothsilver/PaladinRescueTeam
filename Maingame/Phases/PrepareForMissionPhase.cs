using System.Collections.Generic;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origin.Characters;
using Origin.Display;
using Origin.Levels;
using Origin.Mission;

namespace Origin.Phases
{
    public class PrepareForMissionPhase : AuxiGamePhase
    {
        public LevelSheet Ls { get; }
        private List<CharacterSheet> chosenPaladins = new List<CharacterSheet>();
        private List<CharacterSheet> allPaladins = new List<CharacterSheet>();

        public PrepareForMissionPhase(LevelSheet ls)
        {
            Ls = ls;
            allPaladins = ls.AllPaladins;
            for (int i = 0; i < ls.NumberOfPaladins; i++)
            {
                if (allPaladins.Count >= i + 1)
                {
                    chosenPaladins.Add(allPaladins[i]);
                }
            }
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            base.Draw(sb, game, elapsedSeconds);
            Primitives.FillRectangle(Root.Screen, Color.CornflowerBlue);
            Writer.DrawString("Your rescue team:", new Rectangle(5, 5, 600, 40),
                alignment: Writer.TextAlignment.Left);
            Rectangle paladinChoiceR = new Rectangle(5,45, 400, Root.ScreenHeight - 300);
            UX.DrawPaladinChoice(paladinChoiceR, true, allPaladins, chosenPaladins );


            UX.DrawButton("Difficulty: Easy", new Rectangle(5, paladinChoiceR.Bottom + 5, paladinChoiceR.Width, 60),
                () => { this.ChosenDifficulty = Difficulty.Easy; }, null, ChosenDifficulty == Difficulty.Easy);
            UX.DrawButton("Difficulty: Normal", new Rectangle(5, paladinChoiceR.Bottom + 65, paladinChoiceR.Width, 60),
                () => { this.ChosenDifficulty = Difficulty.Normal; }, null, ChosenDifficulty == Difficulty.Normal);
            UX.DrawButton("Difficulty: Meaningful", new Rectangle(5, paladinChoiceR.Bottom + 125, paladinChoiceR.Width, 60),
                () => { this.ChosenDifficulty = Difficulty.Meaningful; }, null, ChosenDifficulty == Difficulty.Meaningful); 
            
            
            Rectangle missionDescRect = new Rectangle(410, 300, Root.ScreenWidth - 420, 500);
            Writer.DrawString("{b}Emergency: " + Ls.Name + "{/b}",
                new Rectangle(missionDescRect.X, missionDescRect.Top - 200, missionDescRect.Width, 200),
                font: BitmapFontGroup.MiaFont,
                alignment: Writer.TextAlignment.Middle);
            Primitives.DrawAndFillRectangle(missionDescRect, Color.LightBlue, Color.DarkBlue);
            string desc = Ls.Intro + "\n\n{b}Difficulty: " + ChosenDifficulty + ".{/b} {i}" + DescribeDifficulty(ChosenDifficulty) + "{/i}"; 
            Writer.DrawString(desc, missionDescRect.Extend(-3, -3), degrading: true);
            
            
            
            bool goAhead = true;
            string caption = "{b}Respond to this emergency{/b}";
            if (chosenPaladins.Count == 0)
            {
                goAhead = false;
                caption = "Select at least one paladin";
            }

            else if (chosenPaladins.Count < Ls.NumberOfPaladins)
            {
                goAhead = true;
                caption = "Respond to this emergency (understaffed)";
            }

            if (chosenPaladins.Count > Ls.NumberOfPaladins)
            {
                goAhead = false;
                caption = "You can't have more than " + Ls.NumberOfPaladins + " paladins.";
            }
            UX.DrawButton(
                    caption, new Rectangle(410, Root.ScreenHeight - 80, 390, 75), () =>
                    {
                        Root.PopFromPhase();
                        Root.PushPhase(new EmergencyPhase(Ls, chosenPaladins, ChosenDifficulty));
                    }, null, disabled: !goAhead
                    );
            UX.DrawButton(
                "Return to Main Menu", new Rectangle(800, Root.ScreenHeight - 80, 400, 75),
                () =>
                {
                    Root.PopFromPhase();
                });
            
        }

        private string DescribeDifficulty(Difficulty chosenDifficulty)
        {
            switch (chosenDifficulty)
            {
                case Difficulty.Easy:
                    return
                        "Injured don't bleed out on their own, fires spread very slowly, and you can't lose except by retreating. As much of a relaxing experience as being an emergency battle paladin can be.";
                case Difficulty.Normal:
                    return "Injured bleed out extremely slowly and fires spread slowly. You can take time figuring out your plan.";
                case Difficulty.Meaningful:
                    return
                        "The game as it's meant to be played. Injured bleed out at a standard pace. Fires spread. In the Haunted House, you will have to make decisions, and the first couple of playthroughs, your decisions will probably lead to many deaths.";
                default:
                    return "";
            }
        }

        public Difficulty ChosenDifficulty { get; set; } = Difficulty.Meaningful;
    }
}