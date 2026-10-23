//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public interface ISelectionStrategy<T>
    {
    T Select(IReadOnlyList<T> candidates);
    }
}