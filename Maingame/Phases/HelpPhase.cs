using System;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origin.Display;

namespace Origin.Phases
{
    public class HelpPhase : DisplayLongTextPhase
    {

        protected override void Initialize(Game game)
        {
            base.Initialize(game);
            Treasure.Instance.ReadInstructions = true;
        }

        public override string ComposeHelpString()
        {
            string mainHelp =
                "You lead a rescue team of {b}magical paladins{/b}! Defend Lonaelon’s loyal citizens from monsters, fires and injury.\n\nDuring an {b}emergency{/b}, your goal is to defeat all enemies, extinguish all fires, and rescue or heal all injured. You lose the mission if all of your paladins die, the fires spread out of control or too many civilians die.\n\nDuring an emergency, left-click a character to select them. Then, right-click any tile to move the character or to interact with whatever’s on that tile.\n\n{i}Attributions: Icons: freepik.com; Pipoya RPG set; Lorc, Delapouite, Viscious Speed (misc). Sounds: Alexander Ehlers (music), devlab, qubodup{/i}";

            mainHelp += "\n\nClick anywhere to go back to the main menu.";

            return mainHelp;

        }

        protected override void OnClick()
        {
            Root.PopFromPhase();
        }
    }
}