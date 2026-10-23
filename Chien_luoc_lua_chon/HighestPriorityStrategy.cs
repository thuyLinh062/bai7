//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
    {
        private readonly Func<T, double> scoreSelector;

        public HighestPriorityStrategy(Func<T, double> scoreSelector)
        {
            this.scoreSelector = scoreSelector;
        }

        public T Select(IReadOnlyList<T> candidates)
        {
            // Kiểm tra nếu danh sách ứng viên rỗng thì báo lỗi bằng tiếng Việt
            if (candidates == null || candidates.Count == 0)
            {
                throw new NoSuitableItemException<T>(candidates!);
            }

            // Khởi tạo thiết bị tốt nhất tạm thời là phần tử đầu tiên
            T bestCandidate = candidates[0];
            double maxScore = scoreSelector(bestCandidate);

            // Dùng foreach để duyệt qua từng ứng viên một cách cực kỳ gọn gàng
            foreach (var candidate in candidates)
            {
                double score = scoreSelector(candidate);
                
                // Nếu tìm thấy thiết bị có điểm cao hơn thì cập nhật lại
                if (score > maxScore)
                {
                    maxScore = score;
                    bestCandidate = candidate;
                }
            }

            return bestCandidate;
        }
    }
}