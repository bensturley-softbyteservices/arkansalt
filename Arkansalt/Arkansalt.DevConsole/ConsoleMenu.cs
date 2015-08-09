using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkansalt.DevConsole
{
    public class ConsoleMenu
    {
        public const bool DisableFolders = false;


        #region menu item class

        public delegate void MenuItemAction();

        public class MenuItem
        {
            public MenuItem(int itemIndex, string text, MenuItemAction action)
            {
                this.ItemIndex = itemIndex;
                this.Text = text;
                this.Action = action;
                this.IsHilited = false;
                this.IsSelected = false;
                this.IsSeperator = false;
            }
            public MenuItem(int itemIndex)
            {
                this.ItemIndex = itemIndex;
                this.Text = null;
                this.Action = null;
                this.IsHilited = false;
                this.IsSelected = false;
                this.IsSeperator = true;
            }

            public int ItemIndex { get; set; }

            public string Text { get; set; }
            public MenuItemAction Action { get; set; }
            public bool IsSelected { get; set; }
            public bool IsHilited { get; set; }
            public bool IsSeperator { get; set; }

            public ConsoleColor ForeColour { get; set; }
            public ConsoleColor BackColour { get; set; }
            public ConsoleColor HiliteForeColour { get; set; }
            public ConsoleColor HiliteBackColour { get; set; }

            public MenuFolder Folder 
            {
                get { return this._folder; }
                set
                {
                    this._folder = value;
                    if (this._folder != null)
                        this._folder.AddItem(this);
                }
            }

            private MenuFolder _folder = null;

        }

        public class MenuFolder : MenuItem
        {
            public MenuFolder(int itemIndex, string text) : base(itemIndex, text, null)
            {
                this.items = new List<MenuItem>();
            }

            public MenuFolder(int itemIndex) : base(itemIndex)
            {
                this.items = new List<MenuItem>();
            }

            public void AddItem(MenuItem item)
            {
                this.items.Add(item);
            }

            public MenuItem[] Items { get { return this.items.ToArray(); } }
            private List<MenuItem> items { get; set; }

            public bool IsOpen { get; set; }

        }

        #endregion

        public ConsoleMenu(
            string title
            , int x, int y
            , ConsoleColor itemForeColour, ConsoleColor itemBackColour
            , ConsoleColor itemHiliteForeColour, ConsoleColor itemHiliteBackColour
            )
        {
            this.Title = title;
            this.X = x;
            this.Y = y;
            this.ItemForeColour = itemForeColour;
            this.ItemBackColour = itemBackColour;
            this.ItemHiliteForeColour = itemHiliteForeColour;
            this.ItemHiliteBackColour = itemHiliteBackColour;

            this.RedrawMenuAfterItemAction = false;
            this.ItemActionInvoker = null;

            this.items = new List<MenuItem>();
        }

        #region properties

        public string Title { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public ConsoleColor ItemForeColour { get; set; }
        public ConsoleColor ItemBackColour { get; set; }
        public ConsoleColor ItemHiliteForeColour { get; set; }
        public ConsoleColor ItemHiliteBackColour { get; set; }

        public bool RedrawMenuAfterItemAction { get; set; }

        public delegate void MenuItemActionInvoker(MenuItemAction action, ConsoleMenu menu);
        public MenuItemActionInvoker ItemActionInvoker { get; set; }

        //public string SeperatorText = "-";
        //public bool ExplodeSeperatorText = true;
        public string SeperatorText = string.Empty;
        public bool ExplodeSeperatorText = false;

        #endregion

        #region items

        public MenuItem[] Items { get { return this.items.ToArray(); } }
        private List<MenuItem> items { get; set; }

        public MenuItem AddMenuItem(string text, MenuItemAction action)
        {
            MenuItem item = new MenuItem(this.items.Count, text, action)
            {
                ForeColour = this.ItemForeColour,
                BackColour = this.ItemBackColour,
                HiliteForeColour = this.ItemHiliteForeColour,
                HiliteBackColour = this.ItemHiliteBackColour
            };
            this.items.Add(item);
            return item;
        }
        public MenuItem AddSeperatorItem()
        {
            MenuItem item = new MenuItem(this.items.Count)
            {
                ForeColour = this.ItemForeColour,
                BackColour = this.ItemBackColour,
                HiliteForeColour = this.ItemHiliteForeColour,
                HiliteBackColour = this.ItemHiliteBackColour
            };
            this.items.Add(item);
            return item;
        }

        public MenuFolder AddFolder(string text)
        {
            MenuFolder folder = new MenuFolder(this.items.Count, text)
            {
                ForeColour = this.ItemForeColour,
                BackColour = this.ItemBackColour,
                HiliteForeColour = this.ItemHiliteForeColour,
                HiliteBackColour = this.ItemHiliteBackColour
            };
            
            if (!ConsoleMenu.DisableFolders)
                this.items.Add(folder);

            return folder;
        }

        #endregion

        #region execution

        public void DrawMenu()
        {
            Console.CursorVisible = false;

            int x = this.X;
            int y = this.Y;

            Console.SetCursorPosition(x, y);
            Console.Write(this.Title);

            // add space to end of longest item text if that item is in a folder (hilite bar looks silly without space at end)
            MenuItem longestItem = (
                from mi in this.items
                where !mi.IsSeperator
                orderby mi.Text.Length descending
                select mi
                ).First();
            if (longestItem.Folder != null && longestItem.Text.Substring(longestItem.Text.Length - 1) != " ")
                longestItem.Text += " ";

            this.items.ForEach(this.DrawMenuItem);
        }

        private void DrawMenuItem(MenuItem item)
        {
            if (item is MenuFolder & ConsoleMenu.DisableFolders)
                return;

            if (ConsoleMenu.DisableFolders || item.Folder == null || item.Folder.IsOpen)
            {
                int x = this.X;
                int y = this.Y + 2 + item.ItemIndex;

                if (!ConsoleMenu.DisableFolders)
                {
                    int previousFolderCount = this.items.Count(mi => mi.ItemIndex < item.ItemIndex && mi.Folder != null && !mi.Folder.IsOpen);
                    y -= previousFolderCount;
                }

                MenuItem longestItem = (
                    from mi in this.items
                    where !mi.IsSeperator
                    orderby mi.Text.Length descending
                    select mi
                    ).First();

                string itemText = item.Text ?? string.Empty;
                string newItemText = itemText;
                if (itemText.Length < longestItem.Text.Length)
                    for (int i = 0; i < longestItem.Text.Length - itemText.Length; i++)
                        newItemText += " ";
                itemText = newItemText;

                string text = string.Format(" {0} ", itemText);

                if (!item.IsHilited)
                {
                    Console.ForegroundColor = item.ForeColour;
                    Console.BackgroundColor = item.BackColour;
                }
                else
                {
                    Console.ForegroundColor = item.HiliteForeColour;
                    Console.BackgroundColor = item.HiliteBackColour;
                }

                if (item.IsSeperator)
                {
                    text = this.SeperatorText;
                    if (this.ExplodeSeperatorText)
                    {
                        text = " " + text.PadRight(longestItem.Text.Length, this.SeperatorText[0]);
                    }
                }

                if (item.Folder != null)
                {
                    if (text.Length > 0)
                        text = " " + text.Remove(text.Length -1);
                }

                Console.SetCursorPosition(x, y);
                //Console.Write(item.IsSeperator ? this.SeperatorText : text);
                Console.Write(text);
            }
            else
            {
                //this.Y--;
            }
        }

        public Action RedrawScreen { get; set; }

        public void RunMenu()
        {
            if (this.items.Count > 0)
            {
                this.DrawMenu();

                MenuItem currentItem = this.items.First();
                int currentItemIndex = currentItem.ItemIndex;

                bool movingUp = false;
                bool movingDown = false;

                while (!this._stopRunningMenu)
                {
                    // draw current item
                    currentItem.IsHilited = true;
                    this.DrawMenuItem(currentItem);

                    // get input
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    movingUp = false;
                    movingDown = false;
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Escape:
                            this.ExitMenu();
                            break;

                        case ConsoleKey.UpArrow:
                        case ConsoleKey.PageUp:
                        case ConsoleKey.Backspace:
                            currentItemIndex--;
                            if (currentItemIndex < 0)
                                currentItemIndex = this.items.Last().ItemIndex;
                            movingUp = true;
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.PageDown:
                        case ConsoleKey.Tab:
                            currentItemIndex++;
                            if (currentItemIndex > this.items.Last().ItemIndex)
                                currentItemIndex = 0;
                            movingDown = true;
                            break;

                        case ConsoleKey.Home:
                            currentItemIndex = 0;
                            movingUp = true;
                            break;

                        case ConsoleKey.End:
                            currentItemIndex = this.items.Last().ItemIndex;
                            movingDown = true;
                            break;

                        case ConsoleKey.Enter:
                        case ConsoleKey.Spacebar:
                        case ConsoleKey.Select:
                        case ConsoleKey.Execute:

                            if (currentItem is MenuFolder)
                            {
                                this.ToggleFolder(currentItem as MenuFolder);
                            }
                            else
                            {
                                if (this.ItemActionInvoker == null)
                                {
                                    currentItem.Action.Invoke();
                                }
                                else
                                {
                                    this.ItemActionInvoker.Invoke(currentItem.Action, this);
                                }

                                if (this.RedrawMenuAfterItemAction)
                                {
                                    this.DrawMenu();
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    // lolite old current item
                    currentItem.IsHilited = false;
                    this.DrawMenuItem(currentItem);

                    // choose new current item
                    currentItem = this.items.Find(mi => mi.ItemIndex == currentItemIndex);

                    // make sure it isn't a seperator or hidden in a non-open folder
                    while (currentItem.IsSeperator || (currentItem.Folder != null && !currentItem.Folder.IsOpen))
                    {
                        if (movingUp)
                        {
                            currentItemIndex--;
                            if (currentItemIndex < 0)
                                currentItemIndex = this.items.Last().ItemIndex;
                        }
                        else if (movingDown)
                        {
                            currentItemIndex++;
                            if (currentItemIndex > this.items.Last().ItemIndex)
                                currentItemIndex = 0;
                        }

                        currentItem = this.items.Find(mi => mi.ItemIndex == currentItemIndex);
                    }

                }
            }
        }

        private void ToggleFolder(MenuFolder folder)
        {
            if (folder.IsOpen)
            {
                folder.IsOpen = false;

                if (this.RedrawScreen != null)
                    this.RedrawScreen();
                this.DrawMenu();
            }
            else
            {
                folder.IsOpen = true;
                folder.Items.ToList().ForEach(this.DrawMenuItem);
            }
            items.Where(mi => mi.ItemIndex > folder.ItemIndex).ToList().ForEach(this.DrawMenuItem);
        }

        public void ExitMenu()
        {
            this._stopRunningMenu = true;
        }

        private bool _stopRunningMenu = false;

        #endregion

    }
}
