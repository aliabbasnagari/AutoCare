using AutoCare.Components;
using AutoCare.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoCare.Views
{
    /// <summary>
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        private CategoryManager CategoryManager { get; set; }
        public Category? SelectedCategory { get; set; }
        public CategoriesPage()
        {
            InitializeComponent();
            CategoryManager = new CategoryManager();
            tvCategories.DataContext = CategoryManager;
            tvCategories.ItemsSource = CategoryManager.Categories;
            Category? cat = null;
            for (int i = 0; i < 10; i++)
            {
                cat = CategoryManager.AddCategory($"Categories {i}", cat);
            }

        }

        private TreeViewItem? FindTreeViewItem(ItemsControl parent, object item)
        {
            if (parent == null || item == null) return null;
            foreach (var child in parent.Items)
            {
                TreeViewItem? treeViewItem = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                if (treeViewItem != null)
                {
                    if (treeViewItem.DataContext == item)
                        return treeViewItem;

                    TreeViewItem? found = FindTreeViewItem(treeViewItem, item);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        private void ExpandToNewSubcategory(Category subcategory)
        {
            if (subcategory.Parent != null)
            {
                ExpandToNewSubcategory(subcategory.Parent);
            }
            TreeViewItem? item = FindTreeViewItem(tvCategories, subcategory);
            if (item != null)
            {
                item.IsExpanded = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (String.IsNullOrEmpty(newCategory.Text)) throw new ArgumentNullException("Category Name is required.");
                var category = CategoryManager.AddCategory(newCategory.Text, SelectedCategory);
                if (category != null)
                    ExpandToNewSubcategory(category);
            }
        }

        private void tvCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedCategory = (Category)e.NewValue;

            parentLink.Text = SelectedCategory != null ? SelectedCategory.GetLink() : "N/A";
        }

        private void tbFontAwesome_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Category cat)
            {
                Debug.WriteLine(cat.Name);
                var dialog = new CategoryDeleteDialog();
                if (dialog.ShowDialog() == true)
                {
                    CategoryManager.RemoveCategory(cat, dialog.DeleteSubcategories);
                }
            }
        }
    }
}
