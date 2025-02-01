using System.Collections.ObjectModel;

namespace AutoCare.Models
{
    public class Category
    {
        public string Name { get; set; }
        public Category? Parent { get; set; }

        public ObservableCollection<Category> Subcategories { get; set; }

        public Category(string name, Category? parent = null)
        {
            Name = name;
            Parent = parent;
            Subcategories = new ObservableCollection<Category>();
        }

        public string GetLink()
        {
            return (Parent != null) ? Parent.GetLink() + ">" + Name : Name;
        }
    }

}
