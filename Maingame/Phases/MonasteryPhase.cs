using System;
using System.Collections.Generic;
using System.Linq;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origin.Characters;
using Origin.Display;

namespace Origin.Phases
{/*
    public class MonasteryPhase : AuxiGamePhase
    {
        private readonly bool fromMainMenu;
        private List<CharacterSheet> selectedPaladins = new List<CharacterSheet>();
        public CharacterSheet SelectedPaladin => selectedPaladins.FirstOrDefault();

        public MonasteryPhase()
        {
            if (Treasure.Instance.Characters.Count > 0)
            {
                selectedPaladins.Add(Treasure.Instance.Characters[0]);
            }
        }
        
        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            base.Draw(sb, game, elapsedSeconds);
            Primitives.FillRectangle(Root.Screen, Color.CornflowerBlue);
            Primitives.DrawImage(Assets.TextureFromCard(Illustration.church), new Rectangle(10, 10, 128, 128));
            Writer.DrawString("{b}Your Monastery{/b}", new Rectangle(140, 25, 700, 50), Color.Black, BitmapFontGroup.MiaFont);
            Writer.DrawString("Left-click a paladin, then you can dismiss them or choose new powers for them..", new Rectangle(140, 60, 900, 150), Color.Black, BitmapFontGroup.MiaFont);
            UX.DrawPaladinChoice(new Rectangle(10, 250, 300, Root.ScreenHeight - 260), false, selectedPaladins);
            DrawForSelectedPaladin();
            UX.DrawButton("To Main Menu", new Rectangle(500, Root.ScreenHeight - 90, 400, 80),
                () =>
                {
                    Treasure.Instance.Save();
                    Root.PopFromPhase();
                });
        }

        private void DrawForSelectedPaladin()
        {
            if (SelectedPaladin != null)
            {
                Writer.DrawString(
                    SelectedPaladin.DescribeSelf() + "\n"
                    + (SelectedPaladin.CanSelectNewPowers ? "You can select a new power for this paladin!" : "This paladin doesn't have enough experience to gain a new power.") +
                    "\nThis paladin's powers:",
                    new Rectangle(600, 300, 600, 500));

                int y = 500;
                foreach (var knownPower in SelectedPaladin.Powers)
                {
                    UX.DrawPower(knownPower, new Rectangle(320, y, 400, 100));
                    y += 100;
                }
                // TODO learning powers
                y = 500;
                DrawLearningButton("Dismiss this paladin", "Permanently release this paladin from your rescue team.", ()=>
                {
                    Root.PushPhase(new MessageBoxPhase("Do you really want to dismiss this paladin?", "Dismiss?", GuiIcon.Question, MessageBoxButtons.YesNo,
                        (s) =>
                        {
                            
                            CharacterSheet paladin = SelectedPaladin;
                            selectedPaladins.Remove(paladin);
                            Treasure.Instance.Characters.Remove(paladin);
                            Treasure.Instance.Save();
                        }));
                }, ref y);
                if (SelectedPaladin.CanSelectNewPowers)
                {
                    foreach (Power power in PowerDb.LearnableBy(SelectedPaladin))
                    {
                        DrawLearningButton("Learn {b}" + power.Name + "{/b}",
                            power.Description,
                            () =>
                            {
                                SelectedPaladin.Powers.Add(power.Id);
                                SelectedPaladin.PowerPoints--;
                                Treasure.Instance.Save();
                            },ref y);
                    }
                }

            }
        }

        private void DrawLearningButton(string caption, string tooltip, Action action, ref int y)
        {
            Rectangle rectThis = new Rectangle(730, y, 400, 50);
            UX.DrawButton(caption, rectThis, action, tooltip);
            y += 50;
        }
    }*/
}