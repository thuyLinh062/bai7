//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyThietBiPhongHoc
{
    public class Program
    {
        private static int _pass = 0;
        private static int _fail = 0;

        // In kết quả kiểm tra: ĐẠT / SAI
        private static void Check(string moTa, bool dieuKien)
        {
            if (dieuKien) _pass++; else _fail++;
            Console.WriteLine($"   [{(dieuKien ? "ĐẠT" : "SAI")}] {moTa}");
        }

        private static void Tieude(string text)
        {
            Console.WriteLine($"\n--- {text} ---");
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("================ KIỂM THỬ HỆ THỐNG QUẢN LÝ THIẾT BỊ PHÒNG HỌC ================");

            var pRepo = new InMemoryRepository<ProjectorRemote>();
            var acRepo = new InMemoryRepository<AirConditionerRemote>();
            var micRepo = new InMemoryRepository<WirelessMicrophone>();

            // ================= YÊU CẦU 1: Thêm thiết bị vào repository =================
            Tieude("Yêu cầu 1: Thêm thiết bị vào repository");

            // 3 máy chiếu pin khác nhau + 2 thiết bị không sẵn sàng (pin yếu, đang bảo trì)
            pRepo.Add(new ProjectorRemote("P01", "ĐK Máy chiếu Sony", "Tủ A1", DeviceStatus.Available, 40));
            pRepo.Add(new ProjectorRemote("P02", "ĐK Máy chiếu Panasonic", "Tủ A2", DeviceStatus.Available, 95));
            pRepo.Add(new ProjectorRemote("P03", "ĐK Máy chiếu Epson", "Kho chính", DeviceStatus.Available, 70));
            pRepo.Add(new ProjectorRemote("P04", "ĐK Máy chiếu Hitachi (pin yếu)", "Tủ A3", DeviceStatus.Available, 10));
            pRepo.Add(new ProjectorRemote("P05", "ĐK Máy chiếu NEC (bảo trì)", "Xưởng", DeviceStatus.UnderMaintenance, 99));

            // 2 điều hòa với tập phòng tương thích khác nhau
            acRepo.Add(new AirConditionerRemote("AC01", "ĐK ĐH Tầng 1", "Hộp KT 1", DeviceStatus.Available,
                new HashSet<string> { "P101", "P102" }));
            acRepo.Add(new AirConditionerRemote("AC02", "ĐK ĐH Tầng 2", "Hộp KT 2", DeviceStatus.Available,
                new HashSet<string> { "P201" }));

            // 3 micro pin và tín hiệu khác nhau
            micRepo.Add(new WirelessMicrophone("M01", "Micro Shure", "Tủ âm thanh 1", DeviceStatus.Available, 80, 90));      // điểm 85
            micRepo.Add(new WirelessMicrophone("M02", "Micro Sennheiser", "Tủ âm thanh 1", DeviceStatus.Available, 50, 75)); // điểm 62.5
            micRepo.Add(new WirelessMicrophone("M03", "Micro Sony", "Phòng thiết bị", DeviceStatus.Available, 90, 95));      // điểm 92.5

            Check("Kho máy chiếu có 5 thiết bị", pRepo.GetAll().Count == 5);
            Check("Kho điều hòa có 2 thiết bị", acRepo.GetAll().Count == 2);
            Check("Kho micro có 3 thiết bị", micRepo.GetAll().Count == 3);

            // ================= YÊU CẦU 2: Thêm trùng mã =================
            Tieude("Yêu cầu 2: Thêm thiết bị trùng mã");
            try
            {
                pRepo.Add(new ProjectorRemote("P01", "ĐK Trùng mã", "Tủ B1", DeviceStatus.Available, 100));
                Check("Phải ném DuplicateIdException khi trùng mã 'P01'", false);
            }
            catch (DuplicateIdException ex)
            {
                Console.WriteLine($"   -> {ex.Message}");
                Check("Bắt đúng DuplicateIdException, DuplicateId = 'P01'", ex.DuplicateId == "P01");
                Check("Kho vẫn còn 5 thiết bị (không thêm bản trùng)", pRepo.GetAll().Count == 5);
            }

            Tieude("Yêu cầu 2 (mở rộng): Thêm phần tử null");
            try
            {
                pRepo.Add(null!);
                Check("Phải ném ArgumentNullException khi thêm null", false);
            }
            catch (ArgumentNullException)
            {
                Check("Bắt đúng ArgumentNullException khi thêm null", true);
            }

            // ================= YÊU CẦU 3: Tìm theo mã =================
            Tieude("Yêu cầu 3: Tìm thiết bị theo mã");
            var found = pRepo.FindById("P02");
            Check("Tìm thấy 'P02'", found != null && found.Name == "ĐK Máy chiếu Panasonic");
            if (found != null)
                Console.WriteLine($"   -> [{found.Id}] {found.Name} (Vị trí: {found.StorageLocation})");
            Check("Mã không tồn tại 'XXX' trả về null", pRepo.FindById("XXX") == null);

            // ================= YÊU CẦU 4: Lọc thiết bị sẵn sàng =================
            Tieude("Yêu cầu 4: Lọc các thiết bị sẵn sàng bằng FindAll + IsReadyForUse");
            var readyProjectors = pRepo.FindAll(p => p.IsReadyForUse());
            foreach (var p in readyProjectors)
                Console.WriteLine($"   -> Sẵn sàng: [{p.Id}] {p.Name} - Pin {p.BatteryLevel}%");
            Check("Có đúng 3 máy chiếu sẵn sàng (P01, P02, P03)", readyProjectors.Count == 3);
            Check("P04 (pin 10%) KHÔNG sẵn sàng", !pRepo.FindById("P04")!.IsReadyForUse());
            Check("P05 (bảo trì, pin 99%) KHÔNG sẵn sàng", !pRepo.FindById("P05")!.IsReadyForUse());
            Check("IsReadyForUse không làm đổi trạng thái P05 (vẫn UnderMaintenance)",
                pRepo.FindById("P05")!.Status == DeviceStatus.UnderMaintenance);

            // ================= YÊU CẦU 5 + 10: Chọn máy chiếu điểm cao nhất, chuyển Borrowed =================
            Tieude("Yêu cầu 5 và 10: Máy chiếu điểm cao nhất, kiểm tra Borrowed");
            var projectorService = new AllocationService<ProjectorRemote>(
                pRepo,
                new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore()));

            var p02Before = pRepo.FindById("P02")!.Status;
            var selectedProj = projectorService.Allocate(d => true);
            Console.WriteLine($"   -> Được chọn: [{selectedProj.Id}] {selectedProj.Name} (Pin {selectedProj.BatteryLevel}%)");
            Console.WriteLine($"   -> Trạng thái trước: {p02Before}, sau cấp phát: {selectedProj.Status}");
            Check("Chọn đúng P02 (pin cao nhất trong thiết bị sẵn sàng, không chọn P05 đang bảo trì)", selectedProj.Id == "P02");
            Check("Trước cấp phát trạng thái là Available", p02Before == DeviceStatus.Available);
            Check("Yêu cầu 10: Sau cấp phát trạng thái là Borrowed", selectedProj.Status == DeviceStatus.Borrowed);
            Check("Thiết bị trong repository cùng đối tượng và cũng là Borrowed",
                pRepo.FindById("P02")!.Status == DeviceStatus.Borrowed);

            // Gọi lần 2: P02 đã Borrowed nên không được chọn lại
            var secondProj = projectorService.Allocate(d => true);
            Console.WriteLine($"   -> Lần cấp phát tiếp theo: [{secondProj.Id}] (Pin {secondProj.BatteryLevel}%)");
            Check("Lần 2 chọn P03 (pin 70), không cấp lại P02 đang Borrowed", secondProj.Id == "P03");

            // ================= YÊU CẦU 6: Điều hòa tương thích với phòng =================
            Tieude("Yêu cầu 6: Chọn điều khiển điều hòa tương thích với phòng");
            var acService = new AllocationService<AirConditionerRemote>(
                acRepo,
                new HighestPriorityStrategy<AirConditionerRemote>(d => d.GetDefaultPriorityScore()));

            var acP101 = acService.Allocate(d => d.IsCompatibleWith("P101"));
            Console.WriteLine($"   -> Phòng P101: [{acP101.Id}] {acP101.Name}");
            Check("Phòng P101 nhận AC01", acP101.Id == "AC01");
            Check("AC01 chuyển sang Borrowed", acP101.Status == DeviceStatus.Borrowed);

            var acP201 = acService.Allocate(d => d.IsCompatibleWith("p201")); // chữ thường để kiểm tra không phân biệt hoa thường
            Console.WriteLine($"   -> Phòng p201 (chữ thường): [{acP201.Id}] {acP201.Name}");
            Check("Phòng 'p201' nhận AC02 (so sánh không phân biệt hoa thường)", acP201.Id == "AC02");

            try
            {
                acService.Allocate(d => d.IsCompatibleWith("P999"));
                Check("Phòng P999 không có thiết bị tương thích, phải ném ngoại lệ", false);
            }
            catch (NoSuitableItemException<AirConditionerRemote>)
            {
                Check("Phòng P999: ném NoSuitableItemException (không có điều hòa tương thích)", true);
            }

            // ================= YÊU CẦU 7: Micro đầu tiên sẵn sàng =================
            Tieude("Yêu cầu 7: Chọn micro đầu tiên sẵn sàng (FirstAvailableStrategy)");
            var micService = new AllocationService<WirelessMicrophone>(micRepo, new FirstAvailableStrategy<WirelessMicrophone>());

            var firstMic = micService.Allocate(m => true);
            Console.WriteLine($"   -> Được chọn: [{firstMic.Id}] {firstMic.Name}");
            Check("FirstAvailable chọn M01 (đứng đầu danh sách)", firstMic.Id == "M01");
            Check("M01 chuyển sang Borrowed", firstMic.Status == DeviceStatus.Borrowed);

            // ================= YÊU CẦU 8: Đổi chiến lược lúc chạy =================
            Tieude("Yêu cầu 8: Đổi chiến lược trong thời gian chạy");
            firstMic.Return();
            Check("Trả M01 về kho: trạng thái Available", firstMic.Status == DeviceStatus.Available);

            // Cùng một service, cùng dữ liệu, nhưng đổi chiến lược
            micService.ChangeStrategy(new HighestPriorityStrategy<WirelessMicrophone>(m => m.GetDefaultPriorityScore()));
            var bestMic = micService.Allocate(m => true);
            Console.WriteLine($"   -> Trước đổi: {firstMic.Id} (điểm {firstMic.GetDefaultPriorityScore()})");
            Console.WriteLine($"   -> Sau đổi sang HighestPriority: {bestMic.Id} (điểm {bestMic.GetDefaultPriorityScore()})");
            Check("Sau ChangeStrategy chọn M03 (điểm 92.5 cao nhất), khác kết quả M01 trước đó", bestMic.Id == "M03");

            try
            {
                micService.ChangeStrategy(null!);
                Check("ChangeStrategy(null) phải ném ArgumentNullException", false);
            }
            catch (ArgumentNullException)
            {
                Check("ChangeStrategy(null) ném ArgumentNullException", true);
            }

            // ================= YÊU CẦU 9: Không có ứng viên phù hợp =================
            Tieude("Yêu cầu 9: Cấp phát khi không có ứng viên phù hợp");
            try
            {
                projectorService.Allocate(d => false);
                Check("Phải ném NoSuitableItemException", false);
            }
            catch (NoSuitableItemException<ProjectorRemote> ex)
            {
                Console.WriteLine($"   -> {ex.Message}");
                Check("Bắt đúng NoSuitableItemException khi điều kiện luôn sai", true);
            }

            // ================= YÊU CẦU 10 (mở rộng): Return và các ngoại lệ trạng thái =================
            Tieude("Yêu cầu 10 (mở rộng): Return(), Borrow() và các ngoại lệ trạng thái");
            var p03 = pRepo.FindById("P03")!; // đang Borrowed từ phần Yêu cầu 5

            try
            {
                p03.Borrow();
                Check("Mượn thiết bị đang Borrowed phải ném DeviceUnavailableException", false);
            }
            catch (DeviceUnavailableException ex)
            {
                Console.WriteLine($"   -> {ex.Message}");
                Check("Mượn lần 2 ném DeviceUnavailableException với trạng thái Borrowed",
                    ex.DeviceId == "P03" && ex.CurrentStatus == DeviceStatus.Borrowed);
            }

            p03.Return();
            Check("Return() đưa P03 về Available", p03.Status == DeviceStatus.Available);

            try
            {
                p03.Return();
                Check("Trả thiết bị chưa mượn phải ném DeviceNotBorrowedException", false);
            }
            catch (DeviceNotBorrowedException ex)
            {
                Console.WriteLine($"   -> {ex.Message}");
                Check("Trả lần 2 ném DeviceNotBorrowedException", true);
            }

            try
            {
                pRepo.FindById("P05")!.Borrow();
                Check("Mượn thiết bị đang bảo trì phải ném DeviceUnavailableException", false);
            }
            catch (DeviceUnavailableException ex)
            {
                Check("Mượn P05 (bảo trì) ném DeviceUnavailableException, trạng thái giữ nguyên",
                    ex.CurrentStatus == DeviceStatus.UnderMaintenance
                    && pRepo.FindById("P05")!.Status == DeviceStatus.UnderMaintenance);
            }

            // ================= Kiểm tra dữ liệu đầu vào không hợp lệ =================
            Tieude("Kiểm tra dữ liệu không hợp lệ (ca biên)");
            try
            {
                _ = new ProjectorRemote("P99", "Test", "Tủ", DeviceStatus.Available, 101);
                Check("Pin 101 phải ném ArgumentOutOfRangeException", false);
            }
            catch (ArgumentOutOfRangeException)
            {
                Check("Pin 101 ném ArgumentOutOfRangeException", true);
            }

            try
            {
                _ = new WirelessMicrophone("M99", "Test", "Tủ", DeviceStatus.Available, 50, -1);
                Check("Tín hiệu -1 phải ném ArgumentOutOfRangeException", false);
            }
            catch (ArgumentOutOfRangeException)
            {
                Check("Tín hiệu -1 ném ArgumentOutOfRangeException", true);
            }

            try
            {
                _ = new ProjectorRemote("", "Test", "Tủ", DeviceStatus.Available, 50);
                Check("Id rỗng phải ném ArgumentException", false);
            }
            catch (ArgumentException)
            {
                Check("Id rỗng ném ArgumentException", true);
            }

            // Biên của ngưỡng sẵn sàng
            var pBien20 = new ProjectorRemote("PB1", "Biên 20", "Tủ", DeviceStatus.Available, 20);
            var pBien19 = new ProjectorRemote("PB2", "Biên 19", "Tủ", DeviceStatus.Available, 19);
            Check("Máy chiếu pin đúng 20% sẵn sàng", pBien20.IsReadyForUse());
            Check("Máy chiếu pin 19% không sẵn sàng", !pBien19.IsReadyForUse());

            var mBien = new WirelessMicrophone("MB1", "Biên", "Tủ", DeviceStatus.Available, 30, 40);
            var mPinYeu = new WirelessMicrophone("MB2", "Pin yếu", "Tủ", DeviceStatus.Available, 29, 90);
            var mSongYeu = new WirelessMicrophone("MB3", "Sóng yếu", "Tủ", DeviceStatus.Available, 90, 39);
            Check("Micro pin 30% và tín hiệu 40% sẵn sàng", mBien.IsReadyForUse());
            Check("Micro pin 29% không sẵn sàng", !mPinYeu.IsReadyForUse());
            Check("Micro tín hiệu 39% không sẵn sàng", !mSongYeu.IsReadyForUse());

            // Remove
            Tieude("Remove()");
            Check("Remove('P04') trả về true", pRepo.Remove("P04"));
            Check("Remove('P04') lần 2 trả về false", !pRepo.Remove("P04"));
            Check("Sau khi xóa, FindById('P04') trả về null", pRepo.FindById("P04") == null);

            // GetAll không lộ collection nội bộ
            Tieude("GetAll() không làm lộ collection nội bộ");
            var snapshot = pRepo.GetAll();
            int countBefore = snapshot.Count;
            pRepo.Add(new ProjectorRemote("P06", "ĐK mới", "Tủ C", DeviceStatus.Available, 60));
            Check("Danh sách lấy trước đó là bản sao, không đổi khi repo thay đổi", snapshot.Count == countBefore);
            Check("Danh sách trả về là kiểu chỉ đọc (không ép sang List để sửa được)", snapshot is not List<ProjectorRemote>);

            // ================= YÊU CẦU 11: Repository tổng quát dùng chung =================
            Tieude("Yêu cầu 11: InMemoryRepository<T> dùng chung cho nhiều loại thiết bị");
            Console.WriteLine($"   -> Kho 1: {pRepo.GetType().Name} đang quản lý {pRepo.GetAll().Count} thiết bị");
            Console.WriteLine($"   -> Kho 2: {acRepo.GetType().Name} đang quản lý {acRepo.GetAll().Count} thiết bị");
            Console.WriteLine($"   -> Kho 3: {micRepo.GetType().Name} đang quản lý {micRepo.GetAll().Count} thiết bị");
            Check("Cả ba kho cùng là một lớp generic InMemoryRepository<>",
                pRepo.GetType().GetGenericTypeDefinition() == typeof(InMemoryRepository<>)
                && acRepo.GetType().GetGenericTypeDefinition() == typeof(InMemoryRepository<>)
                && micRepo.GetType().GetGenericTypeDefinition() == typeof(InMemoryRepository<>));
            Check("Các kho khác tham số kiểu T (ProjectorRemote, AirConditionerRemote, WirelessMicrophone)",
                pRepo.GetType() != acRepo.GetType() && acRepo.GetType() != micRepo.GetType());

            // ================= TỔNG KẾT =================
            Console.WriteLine("\n================ TỔNG KẾT ================");
            Console.WriteLine($"ĐẠT: {_pass}   SAI: {_fail}");
            Console.WriteLine(_fail == 0 ? "Tất cả kiểm thử đều đạt." : "Có kiểm thử SAI, xem lại các dòng [SAI] ở trên.");
        }
    }
}