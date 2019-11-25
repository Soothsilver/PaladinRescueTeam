using System;
using System.Collections.Generic;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Origin.Characters;
using Origin.Display;

namespace Origin.Phases
{
    internal class UX
    {
        public static Action MouseOverAction;
        public static void DrawButton(string caption, Rectangle rectangle, Action action,
            string tooltip = null, bool disabled = false)
        {
            bool mo = Root.IsMouseOver(rectangle) && !disabled;
            bool clicking =  mo && Root.Mouse_NewState.LeftButton == ButtonState.Pressed;
            Primitives.FillRectangle(rectangle, clicking ? Color.Blue : (disabled ? Color.Gray : Color.CornflowerBlue));
            Primitives.DrawRectangle(rectangle, disabled ? Color.DarkGray : Color.Blue, 1);
            Primitives.DrawAndFillRectangle(rectangle.Extend(-2, -2),
                mo ? Color.White : Color.LightBlue, Color.Blue, 1);
            Writer.DrawString(caption, rectangle.Extend(-5,0), (disabled ? Color.DarkGray : Color.Black), BitmapFontGroup.Mia24Font, Writer.TextAlignment.Left, degrading: true);
            if (mo)
            {
                MouseOverAction = action;
                if (tooltip != null)
                {
                    Tooltip.DrawTooltipAround(rectangle, tooltip);
                }
            }
        }

        public static void Clear()
        {
            MouseOverAction = null;
        }

        public static void DrawPower(PowerName powerName, Rectangle rectangle)
        {
            Primitives.DrawAndFillRectangle(rectangle, Color.LightBlue, Color.DarkBlue, 1);
            Power power = PowerDb.GetPower(powerName);
            Writer.DrawString("{b}" + power.Name + "{/b}\n" + power.Description, rectangle.Extend(-5,-5), Color.Black, degrading: true);
        }

        public static void DrawPaladinChoice(Rectangle rectangle, bool multipleChoice, List<CharacterSheet> allPaladins, List<CharacterSheet> selectedPaladins)
        {
            Primitives.DrawAndFillRectangle(rectangle, Color.LightBlue, Color.Blue, 1);
            int y = rectangle.Y + 2;
            foreach (var paladin in allPaladins)
            {
                Rectangle rectThis = new Rectangle(rectangle.X + 2, y, rectangle.Width-4, 80);
                var isSelected = selectedPaladins.Contains(paladin);
                Primitives.DrawAndFillRectangle(rectThis, isSelected ? Color.White : Color.Wheat, Color.Blue,
                    isSelected ? 4 : 1);
                Writer.DrawString(paladin.DescribeSelf(), rectThis.Extend(-6, -6), degrading: true);
                if (Root.IsMouseOver(rectThis))
                {
                    if (isSelected)
                    {
                        UX.MouseOverAction = () => { selectedPaladins.Remove(paladin); };
                    }
                    else
                    {
                        UX.MouseOverAction = () =>
                        {
                            if (!multipleChoice)
                            {
                                selectedPaladins.Clear();
                            }
                            selectedPaladins.Add(paladin);
                        };
                    }
                }

                y += 80;
            }
        }
    }
}