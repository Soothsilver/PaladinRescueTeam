using Auxiliary;
using Microsoft.Xna.Framework;

namespace Origin.Phases
{
    public abstract class AuxiGamePhase : GamePhase
    {
        protected override void Update(Game game, float elapsedSeconds)
        {
            base.Update(game, elapsedSeconds);
            if (Root.WasMouseLeftClick && UX.MouseOverAction != null)
            {
                Root.WasMouseLeftClick = false;
                UX.MouseOverAction();
            }
        }
    }
}