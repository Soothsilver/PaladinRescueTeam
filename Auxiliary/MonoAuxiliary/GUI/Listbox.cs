using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Auxiliary.GUI
{
    /// <summary>
    /// Represents an Auxiliary list box control.
    /// </summary>
    /// <typeparam name="T">This type should override ToString().</typeparam>
    [Serializable]
    public class Listbox<T> : UIElement
    {
    
        /// <summary>
        /// Triggers whenever an item is selected. It is guaranteed the selected item will not be null when this is called.
        /// </summary>
      
        public event Action<Listbox<T>, object> ItemSelected;
        /// <summary>
        /// Calls the ItemSelected event.
        /// </summary>
        protected virtual void OnItemSelected(Listbox<T> listbox, object item)
        {
            if (ItemSelected != null)
                ItemSelected(listbox, item);
        }
        /// <summary>
        /// Triggers whenever an item is clicked or is selected and the Enter key is pressed.
        /// </summary>
        public event Action<Listbox<T>, object> ItemConfirmed;
        /// <summary>
        /// Calls the ItemConfirmed event.
        /// </summary>
        protected virtual void OnItemConfirmed(Listbox<T> listbox, object item)
        {
            if (ItemConfirmed != null)
                ItemConfirmed(listbox, item);
        }

        /// <summary>
        /// The collection of objects in this listbox.
        /// </summary>
        public List<T> Items = new List<T>();
        private int selectedIndex = -1;
        private int mouseOverIndex = -1;

        /// <summary>
        /// Gets or sets the index of the selected item. Returns -1 when no item is selected. Deselects anything if set to an invalid index.
        /// </summary>
        public int SelectedIndex
        {
            get { if (selectedIndex >= Items.Count) return -1; else return selectedIndex; }
            set
            {
                if (value >= Items.Count || value < -1)
                    selectedIndex = -1;
                else
                {
                    selectedIndex = value;
                    OnItemSelected(this, SelectedItem);
                }
            }
        }
        /// <summary>
        /// Gets or sets the selected item. Returns null if no item is selected. Throws exception of set to an object not in the listbox.
        /// </summary>
        public T SelectedItem
        {
            get { if (selectedIndex == -1) return default(T); else return Items[selectedIndex]; }
            set
            {
                if (value is T)
                {
                    T input = (T)value;
                    if (Items.Contains(input))
                    {
                        SelectedIndex = Items.IndexOf(input);
                    }
                    else throw new Exception("This item is not in the listbox.");
                }
                else throw new Exception("This item is not of the type accepted by this Listbox<T>.");
            }
        }

        /// <summary>
        /// Update the control. The base method causes the control to become active and consumes the left-click if clicked.
        /// </summary>
        public override void Update()
        {
            if (Root.WasMouseLeftClick)
            {
                if (Root.IsMouseOver(Rectangle))
                {
                    Root.ConsumeLeftClick();
                    if (mouseOverIndex != -1)
                    {
                        if (Multiselect)
                        {
                            if (SelectedIndices.Contains(mouseOverIndex))
                            {
                                SelectedIndices.Remove(mouseOverIndex);
                            }
                            else
                            {
                                SelectedIndices.Add(mouseOverIndex);
                            }
                        }
                        if (mouseOverIndex == SelectedIndex)
                            OnItemConfirmed(this, SelectedItem);
                        else
                        {
                            SelectedIndex = mouseOverIndex;
                        }
                    }
                    else SelectedIndex = -1;
                    this.Activate();
                }
            }
            if (Root.WasKeyPressed(Keys.Down) && this.IsActive)
            {
                if (SelectedIndex < Items.Count - 1) SelectedIndex++;
            }
            if (Root.WasKeyPressed(Keys.Up) && this.IsActive)
            {
                if (SelectedIndex > 0) SelectedIndex--;
            }
            if (Root.WasKeyPressed(Keys.Home))
                if (Items.Count > 0) SelectedIndex = 0;
            if (Root.WasKeyPressed(Keys.End))
                SelectedIndex = Items.Count - 1;
            if (Root.WasKeyPressed(Keys.Enter))
            {
                if (SelectedIndex != -1) OnItemConfirmed(this, SelectedItem);
                if (Multiselect)
                {
                    if (SelectedIndices.Contains(SelectedIndex))
                    {
                        SelectedIndices.Remove(SelectedIndex);
                    } else
                    {
                        SelectedIndices.Add(SelectedIndex);
                    }
                }
            }
            base.Update();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        public override void Draw()
        {
            mouseOverIndex = -1;
            Color outerBorderColor = Skin.OuterBorderColor;
            Color innerBorderColor = Skin.InnerBorderColor;
            Color innerButtonColor = Skin.WhiteBackgroundColor;
            Primitives.FillRectangle(Rectangle, this.IsActive ? Skin.InnerBorderColorMouseOver : innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            int maxItems = InnerRectangle.Height / Skin.ListItemHeight;
            int colsize = (InnerRectangle.Width - 2) / 3;
            for (int i = TopOfList; i < Items.Count; i++)
            {
                Rectangle rectItem = new Rectangle(InnerRectangle.X + 1, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList) + 1, InnerRectangle.Width - 2, Skin.ListItemHeight);
                if (ThreeColumn)
                {
                    if (i < maxItems)
                    {
                        rectItem = new Rectangle(InnerRectangle.X + 1, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList) + 1, colsize, Skin.ListItemHeight);

                    }
                    else if (i < maxItems * 2)
                    {
                        rectItem = new Rectangle(InnerRectangle.X + 1 + colsize, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList - maxItems) + 1,
                         colsize, Skin.ListItemHeight);
                    }
                    else
                    {
                        rectItem = new Rectangle(InnerRectangle.X + 1 + colsize * 2, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList - maxItems * 2) + 1,
                         colsize, Skin.ListItemHeight);
                    }
                }


                if (Root.IsMouseOver(rectItem))
                    mouseOverIndex = i;

                if (selectedIndex == i || (Multiselect && SelectedIndices.Contains(i)))
                    Primitives.FillRectangle(rectItem, Skin.ItemSelectedBackgroundColor);
                else if (mouseOverIndex == i)
                    Primitives.FillRectangle(rectItem, Skin.ItemMouseOverBackgroundColor);
                if ((Multiselect && SelectedIndices.Contains(i)))
                {
                    Primitives.FillCircleQuick(new Vector2(rectItem.Right - rectItem.Height / 2 - 4, rectItem.Y + rectItem.Height / 2), rectItem.Height / 2 - 1, Color.Black);
                }
                    Primitives.DrawSingleLineText(Items[i].ToString(), new Vector2(rectItem.X + 4, rectItem.Y + 1), Skin.TextColor, Skin.Font);
                Primitives.DrawLine(new Vector2(rectItem.X - 1, rectItem.Bottom-1),
                                    new Vector2(rectItem.Right+1, rectItem.Bottom-1),
                                    outerBorderColor, Skin.OuterBorderThickness);
            }
        }

        private const int TopOfList = 0;
        public bool Multiselect;
        public List<int> SelectedIndices = new List<int>();
        public bool ThreeColumn;

        public IEnumerable<T> SelectedItems
        {
            get
            {
                foreach(int i in SelectedIndices)
                {
                    if (i < 0) continue;
                    if (i >= Items.Count) continue;
                    yield return Items[i];
                }
            }
        }

        /// <summary>
        /// Creates a new Auxiliary listbox.
        /// </summary>
        /// <param name="rect">Space occupied by the listbox.</param>
        public Listbox(Rectangle rect)
        {
            Rectangle = rect;
        }
    }
}