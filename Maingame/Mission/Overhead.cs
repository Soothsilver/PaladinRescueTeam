using Auxiliary;
using Microsoft.Xna.Framework;
using Origin.Display;

namespace Origin.Mission
{
    public class Overhead
    {
        public string Text;
        public int Width;
        public int Height;
        public float TimeRemaining = 5;
        public float YDisplacement;
        private static float SPEED = 20;

        public Overhead(string text)
        {
            Text = text;
            Rectangle bounds = Writer.GetMultiLineTextBounds(text, new Rectangle(0, 0, 400, 800), BitmapFontGroup.Mia24Font);
            Width = bounds.Width;
            Height = bounds.Height;
            YDisplacement = -38;
        }

        public void Draw(Rectangle rectTile)
        {
            Rectangle rectSpeechText = new Rectangle(rectTile.X + rectTile.Width / 2 - Width / 2,
                (int)(rectTile.Y + YDisplacement - Height), Width, Height);
            Primitives.DrawAndFillRoundedRectangle(rectSpeechText.Extend(4, 4), Color.White, Color.Black);
            Writer.DrawString(Text, rectSpeechText, font: BitmapFontGroup.Mia24Font, alignment: Writer.TextAlignment.Middle);
        }

        public bool UpdateAndPossiblyDelete(float elapsedSecond)
        {
            YDisplacement -= elapsedSecond * SPEED;
            TimeRemaining -= elapsedSecond;
            if (TimeRemaining <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}