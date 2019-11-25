using System;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origin.Display;

namespace Origin.Phases
{
    public abstract class DisplayLongTextPhase : AuxiGamePhase
    {

        protected DisplayLongTextPhase()
        {
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds)
        {
            base.Draw(sb, game, elapsedSeconds);
            
            Rectangle rectHelp = new Rectangle(Root.ScreenWidth / 2 -500, Root.ScreenHeight /2 - 450,
                1000,800);
            Primitives.DrawAndFillRectangle(rectHelp, Color.Wheat, Color.Black, 3);
            string helpString = ComposeHelpString();
            Writer.DrawString(helpString, rectHelp.Extend(-7, -7), Color.Black, degrading:true);
        }

        public abstract string ComposeHelpString();
        
        

        protected override void Update(Game game, float elapsedSeconds)
        {
            base.Update(game, elapsedSeconds);
            if (Root.WasMouseLeftClick || Root.WasMouseRightClick)
            {
                OnClick();
            }
        }

        protected abstract void OnClick();
    }
}