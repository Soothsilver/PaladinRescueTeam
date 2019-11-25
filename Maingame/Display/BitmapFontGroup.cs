using MonoGame.Extended.BitmapFonts;

namespace Origin.Display
{
    public class BitmapFontGroup
    {
        public static BitmapFontGroup AnnaFont;
        public static BitmapFontGroup MiaFont;
        public static BitmapFontGroup DefaultFont => MiaFont;
        public static BitmapFontGroup MiaSmallFont { get; set; }
        public static BitmapFontGroup Mia24Font { get; set; }
        public static BitmapFontGroup Mia18Font { get; set; }
        public static BitmapFontGroup Mia12Font { get; set; }

        public readonly BitmapFont Regular;
        public readonly BitmapFont Italics;
        public readonly BitmapFont Bold;
        public readonly BitmapFont BoldItalics;
        public BitmapFontGroup DegradesTo { get; }
        
        public BitmapFontGroup(BitmapFont regular, BitmapFont italics, BitmapFont bold, BitmapFont boldItalics, BitmapFontGroup degradesTo = null)
        {
            DegradesTo = degradesTo;
            Regular = regular; Italics = italics; Bold = bold; BoldItalics = boldItalics;
        }

    }
}