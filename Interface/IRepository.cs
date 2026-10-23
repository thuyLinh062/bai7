//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public interface IRepository<T>
        where T : IIdentifiable
    {
    public void Add(T item);
    public bool Remove(string id);
    public T? FindById(string id);
    public IReadOnlyList<T> GetAll();
    public IReadOnlyList<T> FindAll(Predicate<T> condition);
    }
}