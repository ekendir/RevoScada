using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RevoScada.DesktopApplication.Helpers
{
    public static class TreeViewHelpers
    {
        public static void ClearTreeViewSelection(TreeView tv)
        {
            if (tv != null)
                ClearTreeViewItemsControlSelection(tv.Items, tv.ItemContainerGenerator);
        }

        private static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
        {
            if ((ic != null) && (icg != null))
                for (int i = 0; i < ic.Count; i++)
                {
                    TreeViewItem treeViewItem = icg.ContainerFromIndex(i) as TreeViewItem;
                    if (treeViewItem != null)
                    {
                        ClearTreeViewItemsControlSelection(treeViewItem.Items, treeViewItem.ItemContainerGenerator);
                        treeViewItem.IsSelected = false;
                    }
                }
        }

        /// <summary>
        /// It'll set expand property to true of all the treeview items.
        /// </summary>
        /// <param name="treeItem"></param>
        public static void ExpandAllNodes(TreeViewItem treeItem)
        {
            treeItem.IsExpanded = true;
            foreach (var childItem in treeItem.Items.OfType<TreeViewItem>())
            {
                ExpandAllNodes(childItem);
            }
        }

        public static void JumpToFolder(TreeView tv, string node)
        {
            bool done = false;
            ItemCollection ic = tv.Items;

            while (!done)
            {
                bool found = false;

                foreach (TreeViewItem tvi in ic)
                {
                    if (node.StartsWith(tvi.Header.ToString()))
                    {
                        found = true;
                        tvi.IsExpanded = true;
                        ic = tvi.Items;
                        if (node == tvi.Header.ToString()) done = true;
                        break;
                    }
                }

                done = (found == false && done == false);
            }
        }

        public static void JumpToFolderByName(TreeView tv, string name)
        {
            bool done = false;
            ItemCollection ic = tv.Items;

            while (!done)
            {
                bool found = false;

                foreach (TreeViewItem tvi in ic)
                {
                    if (name.StartsWith(tvi.Name.ToString()))
                    {
                        found = true;
                        tvi.IsExpanded = true;
                        ic = tvi.Items;
                        if (name == tvi.Name.ToString()) done = true;
                        break;
                    }
                }

                done = (found == false && done == false);
            }
        }

        public static void ExpandSpecificNode(TreeViewItem treeItem, string treeItemName)
        {
            treeItem.IsExpanded = true;
            foreach (var childItem in treeItem.Items.OfType<TreeViewItem>())
            {
                if(childItem.Name == treeItemName)
                    ExpandAllNodes(childItem);
            }
        }

        /// <summary>
        /// Gets the selected TreeView's parent if there are any.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        public static bool SetSelectedItem(this TreeView treeView, object item)
        {
            return SetSelected(treeView, item);
        }

        public static void UnselectTreeViewItem(TreeView pTreeView)
        {
            if (pTreeView.SelectedItem == null)
                return;

            if (pTreeView.SelectedItem is TreeViewItem)
            {
                (pTreeView.SelectedItem as TreeViewItem).IsSelected = false;
            }
            else
            {
                TreeViewItem item = pTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsSelected = false;
                }
            }
        }

        private static bool SetSelected(ItemsControl parent, object child)
        {
            if (parent == null || child == null)
                return false;

            TreeViewItem childNode = parent.ItemContainerGenerator
            .ContainerFromItem(child) as TreeViewItem;

            if (childNode != null)
            {
                childNode.Focus();
                return childNode.IsSelected = true;
            }

            if (parent.Items.Count > 0)
            {
                foreach (object childItem in parent.Items)
                {
                    ItemsControl childControl = parent
                      .ItemContainerGenerator
                      .ContainerFromItem(childItem)
                      as ItemsControl;

                    if (SetSelected(childControl, child))
                        return true;
                }
            }

            return false;
        }
    }
}
