//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{   
    public abstract class TeachingDevice : IIdentifiable
    {
        // Các thuộc tính chung

        //Mã thiết bị
        public string Id { get; init; }

        //Tên thiết bị
        public string Name { get; set; }

        //Vị trí lưu trữ
        public string StorageLocation { get; }

        //Trạng thái hoạt động 
        public DeviceStatus Status {get; set;}
        
        protected TeachingDevice(
            string id, 
            string name, 
            string storageLocation,
            DeviceStatus status)
        {
            if(string.IsNullOrWhiteSpace(Id))
            {
                throw new ArgumentException("ID không được để trống");
            }

            if(string.IsNullOrWhiteSpace(Name))
            {
                    throw new ArgumentException("ID không được để trống");
            }

            if(string.IsNullOrWhiteSpace(StorageLocation))
            {
                    throw new ArgumentException("ID không được để trống");
            }      

            Id = id;
            Name = name;
            StorageLocation = storageLocation;
            Status = status;     
        }

        //Cung cấp phương thức trừu tượng tính chi phí bảo trì dự kiến trong một năm.
        public abstract bool IsReadyForUse();

        public abstract double GetDefaultPriorityScore();

        public void Borrow()
        {
            if(IsReadyForUse())
            {
                Status = DeviceStatus.Borrowed;
            }
            else
            {
                throw new DeviceUnavailableException(Id, Status);
            }
        }

        public void Return()
        {
            if(Status == DeviceStatus.Borrowed)
            {
                Status = DeviceStatus.Available;
            }
            else
            {
                throw new DeviceNotBorrowedException(Id);
            }
        }
    }
}