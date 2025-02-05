using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Services
{
    public class Paginator<T>
    {

        private List<T> _items;
        private int _pageSize;
        private int _currentPage;

        public int TotalPages => (_items.Count + _pageSize - 1) / _pageSize;
        public int TotalItems => _items.Count;

        public Paginator(List<T> items, int pageSize)
        {
            _items = items;
            _pageSize = pageSize;
            _currentPage = 0;
        }

        public List<T> GetPage(int pageIndex)
        {
            return _items.Skip(pageIndex * _pageSize).Take(_pageSize).ToList();
        }

        public List<T> CurrentPage()
        {
            return GetPage(_currentPage);
        }

        public List<T> NextPage()
        {
            if (_currentPage < TotalPages - 1)
            {
                _currentPage++;
            }
            return GetPage(_currentPage);
        }

        public List<T> PrevPage()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
            }
            return GetPage(_currentPage);
        }
    }
}
