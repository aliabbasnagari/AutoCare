using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Models
{
    public class Category
    {
        public string Name { get; set; }
        public string? ParentName { get; set; }

        public ObservableCollection<Category> Subcategories { get; set; }

        public Category(string name, string? parentName = null)
        {
            Name = name;
            ParentName = parentName;
            Subcategories = new ObservableCollection<Category>();
        }
    }

}
