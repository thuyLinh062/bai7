//Họ và Tên: Trần Thùy Linh
//MSSV: 202418935
using System;

namespace QuanLyThietBiPhongHoc
{
    public class ProjectorRemote : TeachingDevice
    {
        public int BatteryLevel { get; }

        public ProjectorRemote(
            string id, 
            string name, 
            string storageLocation,
            DeviceStatus status,
            int batteryLevel
        ) : base (id, name, storageLocation, status)
        {
            if(batteryLevel < 0 || batteryLevel >100)
            {
                throw new ArgumentOutOfRangeException(nameof(batteryLevel), "Mức pin phải từ 0 đến 100");
            }

            BatteryLevel = batteryLevel;
        }

        public override bool IsReadyForUse()
            {
                if(BatteryLevel < 20)
                {
                    Console.WriteLine("Mức pin dưới 20%");
                    return false;
                }

                Status = DeviceStatus.Available;
                return true;
            }

        public override double GetDefaultPriorityScore()
            {
                return BatteryLevel;
            }
    }

    public class AirConditionerRemote : TeachingDevice
    {
        public IReadOnlySet<string> CompatibleRooms { get; }

        public AirConditionerRemote(
            string id, 
            string name, 
            string storageLocation,
            DeviceStatus status,
            IReadOnlySet<string> compatibleRooms
        ) : base(id, name, storageLocation, status)
        {
            CompatibleRooms = compatibleRooms;
        }

        public bool IsCompatibleWith(string roomId)
        {
            foreach (string room in CompatibleRooms )
            {
                if(room.Equals(roomId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsReadyForUse()
        {
            if(Status == DeviceStatus.Available)
            {
                return true;
            }
            return false;
        }

        public override double GetDefaultPriorityScore()
        {
            return 100;
        }
    }
    public class WirelessMicrophone : TeachingDevice
    {
        public int BatteryLevel { get; }
        public int SignalQuality { get; }

        public WirelessMicrophone(
            string id, 
            string name, 
            string storageLocation,
            DeviceStatus status,
            int batteryLevel,
            int signalQuality
        ) : base(id, name, storageLocation, status)
        {
            if(batteryLevel < 0 || batteryLevel >100)
            {
                throw new ArgumentOutOfRangeException(nameof(batteryLevel), "Mức pin phải từ 0 đến 100");
            }

            if(signalQuality < 0 || signalQuality > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(signalQuality), "Mức pin phải từ 0 đến 100");
            }

            BatteryLevel = batteryLevel;
            SignalQuality = signalQuality;
        }

        public override bool IsReadyForUse()
        {
            if(BatteryLevel >=30 && SignalQuality >=40)
            {
                Status = DeviceStatus.Available;
                return true;
            }
            return false;
        }

        public override double GetDefaultPriorityScore()
        {
            return (BatteryLevel + SignalQuality)/2.0;
        }
    }
}