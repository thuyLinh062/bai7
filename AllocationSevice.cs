//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public class AllocationService<T>
        where T : TeachingDevice
    {
        private readonly IRepository<T> repository;
        private ISelectionStrategy<T> strategy;

        public AllocationService(
        IRepository<T> repository,
        ISelectionStrategy<T> strategy)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }
        public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
        {
            this.strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
        }

        public T Allocate(Predicate<T>? additionalCondition = null)
        {
            var candidates = repository.GetAll()
                .Where(d => d.IsReadyForUse())
                .Where(d => additionalCondition == null || additionalCondition(d))
                .ToList()
                .AsReadOnly();

            var selected = strategy.Select(candidates);
            selected.Borrow();
            return selected;
        }
    }
}