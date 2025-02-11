namespace AutoCare.Services
{
    public class Paginator<T>
    {
        private List<T> _items;
        private int _pageSize;
        private int _currentPage;
        public int TotalItems => _items.Count;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / _pageSize);

        public Paginator(List<T> items, int pageSize)
        {
            _items = items;
            _pageSize = pageSize;
            _currentPage = 1;
        }

        public List<T> GetCurrentPage()
        {
            int pageIndex = _currentPage - 1;
            return _items.Skip(pageIndex * _pageSize).Take(_pageSize).ToList();
        }

        public void UpdateItems(List<T> items)
        {
            _currentPage = 1;
            _items = items;
        }

        public int PageNumber()
        {
            return _currentPage;
        }

        public List<T> NextPage()
        {
            if (_currentPage < TotalPages)
            {
                _currentPage++;
            }
            return GetCurrentPage();
        }

        public List<T> PreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
            }
            return GetCurrentPage();
        }

        public List<T> MoveToPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > TotalPages)
                throw new IndexOutOfRangeException("Invalid page number");
            _currentPage = pageNumber;
            return GetCurrentPage();
        }

        public List<T> MovePages(int step)
        {
            if (_currentPage + step <= TotalPages && _currentPage + step >= 1)
            {
                _currentPage += step;
            }
            return GetCurrentPage();
        }
    }
}
