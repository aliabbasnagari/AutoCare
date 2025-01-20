using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Models
{
    public class CategoryManager
    {
        public ObservableCollection<Category> Categories { get; private set; }

        public CategoryManager()
        {
            Categories = new ObservableCollection<Category>();
        }

        public Category? AddCategory(string name, Category? parent = null)
        {
            if (CategoryExists(name, parent)) return null;
            var newCategory = new Category(name, parent);
            if (parent != null)
                parent.Subcategories.Add(newCategory);
            else
                Categories.Add(newCategory);
            return newCategory;
        }

        public void RemoveCategory(Category category, bool deleteSubcategories = true)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category.Parent == null)
            {
                Categories.Remove(category);
                if (deleteSubcategories)
                {
                    foreach (var subcategory in category.Subcategories.ToList())
                    {
                        RemoveCategory(subcategory, true);
                    }
                    category.Subcategories.Clear();
                }
                else
                {
                    foreach (var subcategory in category.Subcategories.ToList())
                    {
                        subcategory.Parent = null;
                        if (!CategoryExists(subcategory.Name, Categories)) Categories.Add(subcategory);
                    }
                }
            }
            else
            {
                Category parent = category.Parent;
                if (!deleteSubcategories)
                {
                    foreach (var subcategory in category.Subcategories.ToList())
                    {
                        subcategory.Parent = parent;
                        parent.Subcategories.Add(subcategory);
                    }
                }

                parent.Subcategories.Remove(category);
                if (deleteSubcategories)
                {
                    foreach (var subcategory in category.Subcategories.ToList())
                    {
                        RemoveCategory(subcategory, true);
                    }
                    category.Subcategories.Clear();
                }
                category.Parent = null;
                category.Subcategories.Clear();
            }
        }

        public bool CategoryExists(string categoryName, IEnumerable<Category> categories)
        {
            return categories.Any(c => c.Name == categoryName);
        }

        public bool CategoryExists(string categoryName, Category? parent)
        {
            if (parent == null) return Categories.Any(c => c.Name == categoryName);
            return parent.Subcategories.Any(c => c.Name == categoryName);
        }
    }
}
