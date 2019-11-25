using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Auxiliary.GUI
{
    /// <summary>
    /// Represents an Auxiliary button.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Checkbox : UIElement
    {
        public Checkbox AddTo(GamePhase phase)
        {
            phase.AddUIElement(this);
            return this;
        }
        public bool Checked;
        /// <summary>
        /// Text on the button.
        /// </summary>
        public string Caption {get; }
        /// <summary>
        /// Called whenever the user left-clicks the button.
        /// </summary>
        public event Action<Checkbox> Click;

        /// <summary>
        /// Calls the Click event.
        /// </summary>
        private void OnClick(Checkbox button)
        {
            Click?.Invoke(button);
        }

        /// <summary>
        /// Calls OnClick when the button is clicked.
        /// </summary>
        public override void Update()
        {
            if (Root.WasMouseLeftClick && Root.IsMouseOver(Rectangle))
            {
                Root.ConsumeLeftClick();
                this.Checked = !this.Checked;
                OnClick(this);
            }
            if (this.IsActive && Root.WasKeyPressed(Keys.Enter))
            {
                this.Checked = !this.Checked;
                OnClick(this);
            }
            base.Update();
        }
        private bool _isMouseOverThis;

        /// <summary>
        /// Creates an Auxiliary button.
        /// </summary>
        /// <param name="text">Text on the button.</param>
        /// <param name="rect">Space of the button.</param>
        /// <param name="beginsChecked">Whether the checkbox begins in a checked state.</param>
        public Checkbox(string text, Rectangle rect, bool beginsChecked = false)
        {
            Caption = text;
            Checked = beginsChecked;
            Rectangle = rect;
        }
        /// <summary>
        /// Draws the button.
        /// </summary>
        public override void Draw()
        {
            this._isMouseOverThis = Root.IsMouseOver(Rectangle);
            bool pressed = this._isMouseOverThis && Root.Mouse_NewState.LeftButton == ButtonState.Pressed;
            Rectangle rectSquare = new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Height, Rectangle.Height);
            Primitives.DrawAndFillRectangle(rectSquare, pressed ? Color.Red : (this._isMouseOverThis ? Color.Yellow : (this.IsActive ? Color.Yellow : Color.White)), Color.Black);
            if (Checked)
            {
                Primitives.FillRectangle(rectSquare.Extend(-2, -2), Color.Black);
            }
            Primitives.DrawMultiLineText(Caption, new Rectangle(Rectangle.X + Rectangle.Height + 2, Rectangle.Y, Rectangle.Width - Rectangle.Height - 2, Rectangle.Height), Color.Black, FontFamily.Normal, Primitives.TextAlignment.Left);
            Primitives.DrawMultiLineText(Caption, new Rectangle(Rectangle.X + Rectangle.Height + 2 +2, Rectangle.Y+2, Rectangle.Width - Rectangle.Height - 2, Rectangle.Height), Color.White, FontFamily.Normal, Primitives.TextAlignment.Left);
            
            /*
            Primitives.FillRectangle(Rectangle, innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawMultiLineText(Caption, InnerRectangle, textColor, Skin.Font, Primitives.TextAlignment.Middle);
            */
        }
    }
 
}
