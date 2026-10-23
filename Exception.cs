//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    //Thiết bị không ở trạng thái Available
    public class DeviceUnavailableException : Exception
    {
        public string DeviceId { get; }
        public DeviceStatus CurrentStatus { get; }

        public DeviceUnavailableException(string deviceId, DeviceStatus currentStatus)
        :base($"Thiết bị {deviceId} không còn có sẵn")
        {
            DeviceId = deviceId;
            CurrentStatus = currentStatus;
        }
    }

    //Trả một thiết bị ko được mượn
    public class DeviceNotBorrowedException : Exception
    {
        public DeviceNotBorrowedException(string deviceId)
            : base($"Thiết bị {deviceId} không được mượn.")
        {
        }
    }

    //Bị trùng mã
    public class DuplicateIdException : Exception
    {
        public string DuplicateId { get; }

        public DuplicateIdException(string duplicateId)
            : base($"Thiết bị {duplicateId} đã tồn tại")
        {
            DuplicateId = duplicateId;
        }
    }

    //Không tìm thấy lựa chọn phù hợp
    public class NoSuitableItemException : Exception
    {
    }
}