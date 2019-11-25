using System;
using System.Collections.Generic;
using System.Linq;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origin.Mission;

namespace Origin.Display
{
    class ContextMenu
    {
        public List<ContextMenuItem> Items = new List<ContextMenuItem>();
        private Rectangle rectangle;
        private BitmapFontGroup font = BitmapFontGroup.Mia24Font;
        private ContextMenuItem mouseOverItem;
        private const int ITEMHEIGHT = 30;
        public bool ScheduledForElimination { get; private set; }

        private void setVisuals(int realx, int realy)
        {
            realx += 1;
            realy += 1;
            int maxwidth = 0;
            foreach(ContextMenuItem item in Items)
            {
                int thisWidth = (int) font.Regular.MeasureString(item.Name).Width + 25 + ITEMHEIGHT;
                if (thisWidth > maxwidth)
                {
                    maxwidth = thisWidth;
                }
            }
            int maxheight = Items.Count*ITEMHEIGHT;
            if (realx + maxwidth > Root.ScreenWidth)
            {
                realx = Root.ScreenWidth - maxwidth;
            }
            if (realy + maxheight > Root.ScreenHeight)
            {
                realy = Root.ScreenHeight - maxheight;
            }
            rectangle = new Rectangle(realx, realy, maxwidth, maxheight);
            for (int i = 0; i < Items.Count; i++)
            {
                ContextMenuItem item = Items[i];
                if (item.Tooltip != null)
                {
                    Rectangle requestedTooltipRectangle = Tooltip.GetOptimumTooltipRectangle(item.Tooltip,
                        new Point(rectangle.X + maxwidth, rectangle.Y + ITEMHEIGHT * i),
                        250,
                        rectangle
                    );
                    item.OptimumTooltipRectangle = requestedTooltipRectangle;
                }
            }
        }
        
        
        public static ContextMenu Generate(int x, int y, List<Interaction> interactions)
        {
            ContextMenu contextMenu = new ContextMenu();
            foreach (var interaction in interactions)
            {
                contextMenu.Items.Add(new ContextMenuItem(Assets.TextureFromCard(Illustration.Fire32), interaction.Caption, interaction.Description,
                    () =>
                    {
                        interaction.FullExecute();
                    }));
            }
            
            // Calculate the context menu size
            contextMenu.setVisuals(x,y);
            return contextMenu;
        }


        public void HandleLeftClick()
        {
            if (mouseOverItem == null)
            {
                this.ScheduledForElimination = true;
                return;
            }
            mouseOverItem.Action();
            this.ScheduledForElimination = true;
            Root.WasMouseLeftClick = false;
        }

        public void Draw()
        {
            mouseOverItem = null;
            Primitives.DrawAndFillRectangle(rectangle, Color.Brown, Color.Black, 1);
            int y = rectangle.Y;
            int x = rectangle.X;
            int width = rectangle.Width;
            int height = ITEMHEIGHT;
            for(int index = 0; index < Items.Count; index++)
            {
                Rectangle rectThisItem = new Rectangle(x,y,width,height);
                Color colorInner = Color.SandyBrown;
                ContextMenuItem item = Items[index];
                if (Root.IsMouseOver(rectThisItem))
                {
                    colorInner = Color.White;
                    mouseOverItem = item;
                    // Tooltip
                    if (item.Tooltip != null)
                    {
                        Tooltip.DrawTooltip(item.OptimumTooltipRectangle, item.Tooltip);
                    }
                }
                Primitives.DrawAndFillRectangle(rectThisItem, colorInner, Color.Black, 1);
                Primitives.DrawImage(item.Icon, new Rectangle(rectThisItem.X+2,rectThisItem.Y+2,rectThisItem.Height-4,rectThisItem.Height-4));
                Writer.DrawString(item.Name, new Rectangle(rectThisItem.X + rectThisItem.Height + 4, rectThisItem.Y, rectThisItem.Width - rectThisItem.Height - 6, rectThisItem.Height), Color.Black, font, Writer.TextAlignment.Left);
                y += height;
            }
            if (!Root.IsMouseOver(rectangle.Extend(40,40)))
            {
                this.ScheduledForElimination = true;
            }
        }
      
        
    }

    class ContextMenuItem
    {
        public Texture2D Icon;
        public delegate void ContextMenuAction();
        public ContextMenuAction Action;
        public string Name;
        public string Tooltip;
        public Rectangle OptimumTooltipRectangle;
        public ContextMenuItem(Texture2D icon,
            string name,
            string tooltip,
            ContextMenuAction action)
        {
            Icon = icon;
            Action = action;
            Name = name;
            Tooltip = tooltip;
        }
    }
}