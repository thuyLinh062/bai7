//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public class InMemoryRepository<T> : IRepository<T>
    where T : IIdentifiable
    {
        private readonly Dictionary<string, T> _items = new();

        public void Add(T item)
        {
            if(item == null)
            {
                throw new ArgumentNullException(nameof(item)); // Không chấp nhận phần tử null
            }

            if(_items.ContainsKey(item.Id))
            {
                throw new DuplicateIdException(item.Id);
            }
            _items[item.Id] = item;
        }
        public bool Remove(string id)
        {
            return _items.Remove(id);
        }
        public T? FindById(string id)
        {
            return _items.TryGetValue(id, out var item) ? item : default;
        }

        public IReadOnlyList<T> GetAll()
        {
            return _items.Values.ToList().AsReadOnly();
        }

        public IReadOnlyList<T> FindAll(Predicate<T> condition)
        {
            if (condition == null) 
            throw new ArgumentNullException(nameof(condition));

        // Lọc các phần tử thỏa mãn điều kiện truyền vào mà không phụ thuộc loại thiết bị cụ thể[cite: 4]
        return _items.Values.Where(item => condition(item)).ToList().AsReadOnly();
        }
    }
}