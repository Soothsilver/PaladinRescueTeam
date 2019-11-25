using Auxiliary;

namespace Origin.Phases
{
    internal class ObjectivesPhase : DisplayLongTextPhase
    {
        public override string ComposeHelpString()
        {
            return
                "You {b}win{/b} when all fires are extinguished, there is no injured, trapped or panicked civilian, and there is no threatening enemy.\n\nYou {b}lose{/b} when all of your paladins are helpless, when too many civilians die or when too many tiles are either burning or burnt out.\n\nLeft-click a character to select them.\n\nRight-click an empty tile to move the selected character there.\n\nRight-click a character or a nonempty tile to have the selected character interact with that character or tile.";

        }

        protected override void OnClick()
        {
            Root.PopFromPhase();
        }
    }
}