using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // Add a top-level category
        public void AddCategory(string name)
        {
            var category = new Category(name);
            Categories.Add(category);
        }

        // Add a subcategory to an existing category
        public bool AddSubCategory(string parentCategoryName, string subCategoryName)
        {
            var parentCategory = FindCategoryByName(parentCategoryName);
            if (parentCategory != null)
            {
                var subCategory = new Category(subCategoryName, parentCategoryName);
                parentCategory.Subcategories.Add(subCategory);
                return true;
            }
            return false;
        }

        // Remove a category and its subcategories by name
        public bool RemoveCategory(string categoryName)
        {
            var categoryToRemove = FindCategoryByName(categoryName);
            if (categoryToRemove != null)
            {
                // Remove from parent if it's a subcategory
                if (!string.IsNullOrEmpty(categoryToRemove.ParentName))
                {
                    var parentCategory = FindCategoryByName(categoryToRemove.ParentName);
                    parentCategory?.Subcategories.Remove(categoryToRemove);
                }
                else
                {
                    // If it is a top-level category, remove it from the Categories collection
                    Categories.Remove(categoryToRemove);
                }
                return true;
            }
            return false;
        }

        // Find a category by its name (including subcategories)
        public Category? FindCategoryByName(string categoryName)
        {
            return FindCategoryInList(Categories, categoryName);
        }

        // Recursive search for category in the list (including subcategories)
        private Category? FindCategoryInList(ObservableCollection<Category> categoryList, string categoryName)
        {
            foreach (var category in categoryList)
            {
                if (category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                    return category;

                // Recursively search in subcategories
                var foundSubCategory = FindCategoryInList(category.Subcategories, categoryName);
                if (foundSubCategory != null)
                    return foundSubCategory;
            }
            return null;
        }

        // Get all subcategories for a given category by name
        public ObservableCollection<Category> GetSubCategories(string categoryName)
        {
            var parentCategory = FindCategoryByName(categoryName);
            return parentCategory?.Subcategories ?? new ObservableCollection<Category>();
        }

        // Check if a category exists
        public bool CategoryExists(string categoryName)
        {
            return FindCategoryByName(categoryName) != null;
        }
    }
}
