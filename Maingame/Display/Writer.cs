﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using Auxiliary;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collections;
using SharpDX.WIC;

namespace Origin.Display
{
    public static class Writer
    {
        public static SpriteBatch SpriteBatch;
        private static readonly Dictionary<MultilineString,MultilineString> MultilineStringCache = new Dictionary<MultilineString,MultilineString>();

        /// <summary>
        /// Draws a multiline text using the Primitives spritebatch.
        /// </summary>
        /// <param name="text">Text to be drawn.</param>
        /// <param name="rectangle">The rectangle bounding the text.</param>
        /// <param name="color">Text color.</param>
        /// <param name="font">Text font (Verdana 14, if null).</param>
        /// <param name="degrading"></param>
        /// <param name="alignment">Text alignment.</param>
        public static void DrawString(string text,
            Rectangle rectangle,
            Color? color = null,
            BitmapFontGroup font = null,
            TextAlignment alignment = TextAlignment.TopLeft,
            bool degrading = false)
        {
            // TODO complete degrading
            Color baseColor = color ?? Color.Black;
            BitmapFontGroup baseFont = font ?? BitmapFontGroup.DefaultFont;
            MultilineString ms = new MultilineString(text, rectangle, alignment, baseFont,null, baseColor, degrading);
            MultilineString msCache = MultilineStringCache.GetValueOrDefault(ms, null);
            if (msCache != null)
            {
                foreach (MultilineFragment line in msCache.CachedLines)
                {
                    if (line.Icon != null)
                    {
                        SpriteBatch.Draw(line.Icon,
                            new Rectangle(rectangle.X + (int) line.PositionOffset.X,
                                rectangle.Y + (int) line.PositionOffset.Y,
                                line.IconWidth, line.IconWidth), Color.White);
                    }
                    else
                    {
                        SpriteBatch.DrawString(line.Font,
                            line.Text,
                            new Vector2(rectangle.X + (int) line.PositionOffset.X,
                                rectangle.Y + (int) line.PositionOffset.Y),
                            line.Color);

                    }
                }

                return;
            }


            Rectangle rrr;
            SetupNewMultilineString(baseFont, text, rectangle, baseColor, alignment, out rrr, out List<MultilineFragment> cachedLines, degrading);
            var multilineString = new MultilineString(text, rectangle, alignment, baseFont, cachedLines, baseColor, degrading);
            MultilineStringCache.Add(multilineString, multilineString);
            DrawString(text, rectangle, baseColor, baseFont, alignment);
        }
        

        /// <summary>
        /// If the text were written to the specified rectangle, how much width and height would it actually use?
        /// This method ignores the rectangle's X and Y properties.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="rectangle">Rectangle bounding the text.</param>
        /// <param name="font">Font to use.</param>
        public static Rectangle GetMultiLineTextBounds(string text, Rectangle rectangle, BitmapFontGroup font = null)
        {
            Rectangle bounds;
            List<MultilineFragment> _;
            SetupNewMultilineString(font ?? BitmapFontGroup.DefaultFont, text, rectangle, Color.Black, TextAlignment.TopLeft,  out bounds, out _, false);
            return bounds;
        }
        /// <summary>
        /// Specifies text alignment.
        /// </summary>
        public enum TextAlignment
        {
            /// <summary>
            /// Align to top.
            /// </summary>
            Top,
            /// <summary>
            /// Align to left.
            /// </summary>
            Left,
            /// <summary>
            /// Align to middle.
            /// </summary>
            Middle,
            /// <summary>
            /// Align to right.
            /// </summary>
            Right,
            /// <summary>
            /// Align to bottom.
            /// </summary>
            Bottom,
            /// <summary>
            /// Align to top left.
            /// </summary>
            TopLeft,
            /// <summary>
            /// Allign to top right.
            /// </summary>
            TopRight,
            /// <summary>
            /// Align to bottom left.
            /// </summary>
            BottomLeft,
            /// <summary>
            /// Align to bottom right.
            /// </summary>
            BottomRight
        }

        /// <summary>
        /// This class is used only internally when calculating how to print out a multiline string.
        /// </summary>
        private class Data
        {
            public StringBuilder ReadyFragment;
            public StringBuilder Builder;
            /// <summary>
            /// Font that will be used to print text now,maybe bold or italic.
            /// </summary>
            public BitmapFont CurrentFont;
            /// <summary>
            /// Total width of the fragments that were already committed to the line that's being constructed.
            /// </summary>
            public float CurrentX;
            /// <summary>
            /// Vertical coordinates of the line that's being constructed, starting at 0, incrementing by LineHeight.
            /// </summary>
            public int CurrentY;
            /// <summary>
            /// Number of lines that are already finalized.
            /// </summary>
            public int TotalNumberOfLines;
            /// <summary>
            /// Maximum permissible width of this multiline string,
            /// </summary>
            public int Width;
            /// <summary>
            /// Maximum permissible height of this multiline text.
            /// </summary>
            public int Height;
            /// <summary>
            /// Constant. Height of a single line.
            /// </summary>
            public int LineHeight;
            /// <summary>
            /// Fonts used in this multiline string.
            /// </summary>
            public BitmapFontGroup FontGroup;
            /// <summary>
            /// If true, then we have already overreached our vertical bounds and must stop constructing additional text.
            /// </summary>
            public bool End;
            /// <summary>
            /// List of already constructed lines
            /// </summary>
            public List<List<MultilineFragment>> Lines;
            /// <summary>
            /// Commmitted fragments of the line that's being constructed
            /// </summary>
            public List<MultilineFragment> ThisLine;
            /// <summary>
            /// What color are we currently using to write.
            /// </summary>
            public Color Color;
            /// <summary>
            /// The width of the widest line committed so far
            /// </summary>
            public float Maximumwidthencountered;
            /// <summary>
            /// Whether we are now writing bold text.
            /// </summary>
            internal bool IsBold;
            /// <summary>
            /// Whether we are now writing italics text.
            /// </summary>
            internal bool IsItalics;

            internal void UpdateFont()
            {
                if (this.IsBold && this.IsItalics)
                    this.CurrentFont = FontGroup.BoldItalics;
                else if (this.IsBold)
                    this.CurrentFont = FontGroup.Bold;
                else if (this.IsItalics)
                    this.CurrentFont = FontGroup.Italics;
                else
                    this.CurrentFont = FontGroup.Regular;
            }
        }

        /// <summary>
        /// Draws a multi-string. 
        /// WARNING! This grows more CPU-intensive as the number of words grow (only if word wrap enabled). It is recommended to use the DrawMultiLineText method instead - it uses caching.
        /// </summary>
        /// <param name="fnt">A reference to a SpriteFont object.</param>
        /// <param name="text">The text to be drawn. <remarks>If the text contains \n it
        /// will be treated as a new line marker and the text will drawn acordingy.</remarks></param>
        /// <param name="r">The screen rectangle that the rext should be drawn inside of.</param>
        /// <param name="col">The color of the text that will be drawn.</param>
        /// <param name="align">Specified the alignment within the specified screen rectangle.</param>
        /// <param name="textBounds">Returns a rectangle representing the size of the bouds of
        /// the text that was drawn.</param>
        /// <param name="cachedLines">This parameter is internal. Do not use it, merely throw away the variable.</param>
        private static void SetupNewMultilineString(BitmapFontGroup fnt, string text, Rectangle r,
        Color col, TextAlignment align, out Rectangle textBounds, out List<MultilineFragment> cachedLines, bool degrading)
        {
            textBounds = r;
            if (text == string.Empty)
            {
                cachedLines = new List<MultilineFragment>();
                return;
            }

            Data d = new Data();
            d.FontGroup = fnt;
            d.ReadyFragment = new StringBuilder();
            d.Builder = new StringBuilder();
            d.CurrentFont = d.FontGroup.Regular;
            d.CurrentX = 0;
            d.CurrentY = 0;
            d.TotalNumberOfLines = 0;
            d.Width = r.Width;
            d.Height = r.Height;
            d.IsBold = false;
            d.IsItalics = false;
            d.LineHeight = d.FontGroup.Regular.LineHeight;
            d.Color = col;
            d.End = false;
            d.Lines = new List<List<MultilineFragment>>();
            d.ThisLine = new List<MultilineFragment>();
            if (d.Height >= d.LineHeight)
            {
                foreach (char t in text)
                {
                    if (t == '{')
                    {
                        // Flush and write.
                        FlushAndWrite(d);
                        // Ready to change mode.
                    }
                    else if (t == '}')
                    {
                        // Change mode.
                        switch (d.Builder.ToString())
                        {
                            case "b":
                                d.IsBold = !d.IsBold;
                                d.UpdateFont();
                                break;
                            case "i":
                                d.IsItalics = !d.IsItalics;
                                d.UpdateFont();
                                break;
                            case "/b":
                                d.IsBold = !d.IsBold;
                                d.UpdateFont();
                                break;
                            case "/i":
                                d.IsItalics = !d.IsItalics;
                                d.UpdateFont();
                                break;
                            default:
                                string tag = d.Builder.ToString();
                                if (tag.StartsWith("icon:"))
                                {
                                    Texture2D icon = Library.Icons[tag.Substring(5)];
                                    d.Builder.Clear();
                                    // Now add icon.
                                    int iconwidth = d.LineHeight;
                                    int remainingSpace = (int) (d.Width - d.CurrentX);
                                    if (remainingSpace > iconwidth + 3)
                                    {
                                        d.ThisLine.Add(new MultilineFragment(icon,
                                            new Vector2(d.CurrentX, d.CurrentY), iconwidth));
                                        d.CurrentX += iconwidth + 3;
                                    }
                                    else
                                    {
                                        FlushAndWrite(d);
                                        GoToNextLine(d);

                                        d.ThisLine.Add(new MultilineFragment(icon,
                                            new Vector2(d.CurrentX, d.CurrentY), iconwidth));
                                        d.CurrentX += iconwidth + 3;
                                    }

                                    break;
                                }
                                else if (tag[0] == '/')
                                {
                                    d.Color = col;
                                }
                                else
                                {
                                    d.Color = ColorFromString(tag);
                                }

                                break;
                        }

                        d.Builder.Clear();
                    }
                    else if (t == ' ')
                    {
                        // Flush. 
                        // Add builder to ready.
                        string without = d.ReadyFragment.ToString();
                        d.ReadyFragment.Append(d.Builder);
                        if (d.CurrentFont.MeasureString(d.ReadyFragment.ToString()).Width <= d.Width - d.CurrentX)
                        {
                            // It will fit.
                            d.ReadyFragment.Append(' ');
                            d.Builder.Clear();
                        }
                        else
                        {
                            // It would not fit.
                            d.ReadyFragment = new StringBuilder(without);
                            string newone = d.Builder.ToString();
                            d.Builder = new StringBuilder();
                            FlushAndWrite(d, true);
                            if (!d.End)
                            {
                                d.Builder.Append(newone);
                                FlushAndWrite(d);
                                d.Builder.Clear();
                                d.Builder.Append(' ');
                            }
                        }

                        // Write if overflowing.
                    }
                    else if (t == '\n')
                    {
                        // Flush and write.
                        FlushAndWrite(d);
                        // Skip to new line automatically.
                        GoToNextLine(d);
                    }
                    else
                    {
                        d.Builder.Append(t);
                    }

                    if (d.End) break;
                }

                // Flush and write.
                FlushAndWrite(d);
                if (d.ThisLine.Count > 0)
                {
                    FinishLine(d);
                    d.TotalNumberOfLines += 1;
                }
            }

            if (d.End && degrading && fnt.DegradesTo != null)
            {
                SetupNewMultilineString(fnt.DegradesTo, text, r, col, align, out textBounds, out cachedLines, degrading);
                return;
            }

            // Output.
            textBounds = new Rectangle(r.X, r.Y, (int)d.Maximumwidthencountered, d.TotalNumberOfLines * d.LineHeight);
            int yoffset = 0;            
            switch(align)
            {
                case TextAlignment.TopLeft:
                case TextAlignment.Top:
                case TextAlignment.TopRight:
                    break;
                case TextAlignment.Bottom:
                case TextAlignment.BottomLeft:
                case TextAlignment.BottomRight:
                    yoffset = r.Height - d.TotalNumberOfLines * d.LineHeight;
                    break;
                case TextAlignment.Middle:
                case TextAlignment.Left:
                case TextAlignment.Right:
                    yoffset = (r.Height - (d.TotalNumberOfLines * d.LineHeight)) / 2;
                    break;
            }
            cachedLines = new List<MultilineFragment>();
            foreach(var line in d.Lines)
            {
                foreach(var fragment in line)
                {
                    if (fragment.Text == "") continue;
                    float xoffset = 0;
                    float lineWidth = line.Sum(frg => frg.Width);
                    switch(align)
                    {
                        case TextAlignment.Right:
                        case TextAlignment.TopRight:
                        case TextAlignment.BottomRight:
                            xoffset = r.Width - lineWidth;
                            break;
                        case TextAlignment.Middle:
                        case TextAlignment.Top:
                        case TextAlignment.Bottom:
                            xoffset = (r.Width - lineWidth) / 2;
                            break;
                    }
                    fragment.PositionOffset += new Vector2(xoffset, yoffset);
                    fragment.PositionOffset = new Vector2((int)fragment.PositionOffset.X, (int)fragment.PositionOffset.Y);
                    cachedLines.Add(fragment);
                }
            }

        }

        private static Color ColorFromString(string nameOfColor)
        {
            var prop = typeof(Color).GetProperty(nameOfColor);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return Color.Black;
        }

        /// <summary>
        /// Draws whatever is present in the readyFragment on the line. 
        ///  If the current buildfragment can also fit, it is drawn as well.
        ///  If the current buildfragment cannot fit, we go to the next line and it is drawn as well. 
        ///  At the end of this procedure, both readyfragment and buildfragment are empty and something is definitely in thisline (though it can be "").
        /// </summary>
        /// <param name="d"></param>
        private static void FlushAndWrite(Data d, bool forceEnd = false)
        {
            string total = d.ReadyFragment.ToString() + d.Builder;
            if (total == "") return;
            if (d.CurrentFont.MeasureString(total).Width <= (d.Width - d.CurrentX))
            {
                // Write it and move X.
                d.ReadyFragment.Append(d.Builder);
                MultilineFragment frag = new MultilineFragment(d.ReadyFragment.ToString(), new Vector2(d.CurrentX, d.CurrentY), d.Color, d.CurrentFont);
                d.ThisLine.Add(frag);
                d.CurrentX += d.CurrentFont.MeasureString(total).Width;
                d.Builder.Clear();
                d.ReadyFragment.Clear();
                if (forceEnd)
                {
                    GoToNextLine(d);
                }
            }
            else
            {
                // Write what is there and clear it.
                MultilineFragment frag = new MultilineFragment(d.ReadyFragment.ToString(), new Vector2(d.CurrentX, d.CurrentY), d.Color, d.CurrentFont);
                d.ThisLine.Add(frag);
                d.CurrentX += d.CurrentFont.MeasureString(d.ReadyFragment.ToString()).Width; 
                d.ReadyFragment.Clear();
                // Flush the line.
                GoToNextLine(d);
                if (!d.End)
                {
                    d.ReadyFragment.Append(d.Builder);
                    MultilineFragment frag2 = new MultilineFragment(d.ReadyFragment.ToString(),
                        new Vector2(d.CurrentX, d.CurrentY), d.Color, d.CurrentFont);
                    d.ThisLine.Add(frag2);
                    d.ReadyFragment.Clear();
                    d.CurrentX += d.CurrentFont.MeasureString(d.Builder.ToString()).Width;
                }
            }
            d.Builder.Clear();
        }

        private static void GoToNextLine(Data d)
        {
            FinishLine(d);
            d.ThisLine = new List<MultilineFragment>();
            // Move one line down.
            d.CurrentY += d.LineHeight;
            d.TotalNumberOfLines += 1;
            d.CurrentX = 0;
            if (d.CurrentY + d.LineHeight > d.Height)
            {
                // End.
                d.End = true;
            }
        }

        private static void FinishLine(Data d)
        {
            d.Maximumwidthencountered = Math.Max(d.Maximumwidthencountered, d.CurrentX);
            d.Lines.Add(d.ThisLine);
        }




        /// <summary>
        /// Do not use this class outside Auxiliary 3.
        /// </summary>
        private class MultilineFragment
        {
            public readonly Color Color;
            public readonly BitmapFont Font;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public readonly string Text;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public Vector2 PositionOffset;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public MultilineFragment(string text, Vector2 position, Color clr, BitmapFont font)
            {
                Text = text;
                PositionOffset = position;
                Font = font;
                Color = clr;
            }
            public readonly Texture2D Icon;
            public readonly int IconWidth;
            public float Width => Icon == null ? Font.MeasureString(Text).Width : IconWidth + 1;
            public MultilineFragment(Texture2D icon, Vector2 position, int width)
            {
                Icon = icon;
                PositionOffset = position;
                IconWidth = width;
            }
        }
        private class MultilineString
        {
            private readonly string text;
            private readonly Rectangle rectangle;
            private readonly TextAlignment textAlignment;
            private readonly BitmapFontGroup font;
            private readonly Color color;
            private readonly bool degrading;

            public readonly List<MultilineFragment> CachedLines;
            
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                MultilineString ms = (MultilineString)obj;
                return text == ms.text &&
                    rectangle.Width == ms.rectangle.Width &&
                    rectangle.Height == ms.rectangle.Height &&
                    color == ms.color &&
                    textAlignment == ms.textAlignment 
                    && font == ms.font;
            }

            public override int GetHashCode()
            {
                // Does this hash really work?
                // Also, we are using implicit modulo.
                return (
                    color.GetHashCode() +
                    37 * degrading.GetHashCode() + 
                    33 * 33 * 33 * 33 * 33 * 33 * text.GetHashCode() +
                    33 * 33 * 33 * 33 * 33 * rectangle.GetHashCode() +
                    33 * 33 * 33 * 33 * textAlignment.GetHashCode() +
                    33 * 33 * 33 * font.GetHashCode()
                    );
            }
            public MultilineString(string text, Rectangle rect, TextAlignment alignment, BitmapFontGroup font,
                List<MultilineFragment> cachedLines, Color color, bool degrading)
            {
                this.CachedLines = cachedLines;
                this.text = text;
                rectangle = rect;
                textAlignment = alignment;
                this.font = font;
                this.color = color;
                this.degrading = degrading;
            }
        }

        public static void DrawNumberInRectangle(int caption, Rectangle rectangle)
        {
           DrawNumberInRectangle(caption.ToString(), rectangle);
        }
        public static void DrawNumberInRectangle(string caption, Rectangle rectangle, Color? innerColor = null)
        {
            Primitives.DrawAndFillRectangle(rectangle, innerColor ?? Color.LightBlue, Color.DarkBlue, 2);
            Writer.DrawString(caption.ToString(), rectangle, alignment: TextAlignment.Middle, degrading: true);
        }

        public static void DrawProgressBar(Rectangle rectangle, Color goodColor, float current, float maximum, string caption)
        {
            Primitives.FillRectangle(rectangle, Color.Gray);
            Primitives.FillRectangle(
                new Rectangle(rectangle.X, rectangle.Y, (int)(rectangle.Width * current / maximum), rectangle.Height),
                goodColor);
            int percentage = (int) (100 * current / maximum);
            if (caption != null)
            {
                Writer.DrawString(caption + ": " + percentage + "%", rectangle, alignment: TextAlignment.Middle,
                    degrading: true);
            }

            Primitives.DrawRectangle(rectangle, Color.Black);
        }
    }
}
