//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
    {
    public T Select(IReadOnlyList<T> candidates)
        {
            if (candidates == null || candidates.Count == 0)
            {
                throw new NoSuitableItemException<T>(candidates!);
            }
            return candidates[0];
        }
    }
}