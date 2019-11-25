using Auxiliary;
using Microsoft.Xna.Framework;

namespace Origin.Display
{
    class Tooltip
    {
        public static Rectangle GetOptimumTooltipRectangle(string text, Point requestedTopLeft, int tooltipWidth, Rectangle avoidRectangle)
        {
            Rectangle optimumTooltip = new Rectangle(0, 0, tooltipWidth - 8, 1000);
            Rectangle tooltipDimensions = Writer.GetMultiLineTextBounds(text, optimumTooltip, BitmapFontGroup.Mia24Font);
            Rectangle requestedTooltipRectangle = new Rectangle(
                requestedTopLeft.X,
                requestedTopLeft.Y,
                tooltipWidth,
                tooltipDimensions.Height + 8);
            if (requestedTooltipRectangle.Bottom > Root.ScreenHeight)
            {
                requestedTooltipRectangle.Y = Root.ScreenHeight - requestedTooltipRectangle.Height;
            }
            if (requestedTooltipRectangle.Right > Root.ScreenWidth)
            {
                requestedTooltipRectangle.X = Root.ScreenWidth - requestedTooltipRectangle.Width;
                if (requestedTooltipRectangle.X > avoidRectangle.Left &&
                    requestedTooltipRectangle.X < avoidRectangle.Right)
                {
                    requestedTooltipRectangle.X = avoidRectangle.Left - requestedTooltipRectangle.Width;
                }
            }
            return requestedTooltipRectangle;
        }

        private static bool tooltipExists;
        private static Rectangle tooltipRect;
        private static string _tooltipText;

        public static void DrawTooltip(Rectangle tooltipRectangle, string tooltipText)
        {
            Tooltip.tooltipRect = tooltipRectangle;
            Tooltip._tooltipText = tooltipText;
            Tooltip.tooltipExists = true;
        }
        public static void DrawTooltipReally()
        {
            if (!Tooltip.tooltipExists)
            {
                return;
            }
            
            Primitives.DrawAndFillRoundedRectangle(
                tooltipRect,
                Color.SandyBrown.Alpha(240),
                Color.Brown,
                1);
            Writer.DrawString(_tooltipText, tooltipRect.Extend(-4, -4), Color.Black,
                                        BitmapFontGroup.Mia24Font);
        }

        public static void Clear()
        {
            Tooltip.tooltipExists = false;
        }

        public static void DrawTooltipAround(Rectangle rectangle, string tooltip)
        {
            Rectangle tooltipBest = GetOptimumTooltipRectangle(tooltip, new Point(rectangle.Right, rectangle.Top), 400, rectangle);
            DrawTooltip(tooltipBest, tooltip);
        }
    }
}

    