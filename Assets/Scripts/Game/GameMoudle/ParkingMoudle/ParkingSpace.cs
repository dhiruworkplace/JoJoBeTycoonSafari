using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum ParkingSpaceState
    {
        /// <summary>
        /// 空位
        /// </summary>
        Free,

        /// <summary>
        /// 被锁定
        /// </summary>
        BeLocked,

        /// <summary>
        /// 停上车了
        /// </summary>
        Parking,
    }

    public class ParkingSpace
    {
        /// <summary>
        /// 组编号
        /// </summary>
        public int groupID;

        /// <summary>
        /// 组内编号
        /// </summary>
        public int idx;

        /// <summary>
        /// 停放的汽车
        /// </summary>
        public EntityGroundParkingCar parkingCar;

        public ParkingSpaceState spaceState;

        public ParkingSpace(int groupID, int idx)
        {
            this.groupID = groupID;
            this.idx = idx;
            spaceState = ParkingSpaceState.Free;
        }

        public bool IsFree()
        {
            return spaceState == ParkingSpaceState.Free;
        }

        public bool IsParking()
        {
            return spaceState == ParkingSpaceState.Parking;
        }

        public void SetBeLocked()
        {
            spaceState = ParkingSpaceState.BeLocked;
        }

        public void SetBeLocked(EntityGroundParkingCar car)
        {
            parkingCar = car;
            spaceState = ParkingSpaceState.BeLocked;
        }

        public void SetParking()
        {
#if UNITY_EDITOR
            if (parkingCar == null)
            {
                string e = string.Format("SetParking 时上面没车 groupID={0}, idx={1}", groupID, idx);
                throw new System.Exception(e);
            }
#endif
            spaceState = ParkingSpaceState.Parking;
        }

        public void SetFree()
        {
#if UNITY_EDITOR
            if (parkingCar == null)
            {
                string e = string.Format("SetFree 时上面没车 groupID={0}, idx={1}", groupID, idx);
                throw new System.Exception(e);
            }
#endif
            spaceState = ParkingSpaceState.Free;
            parkingCar = null;
        }
    }

    public class GroupParkingSpace
    {
        /// <summary>
        /// 组编号
        /// </summary>
        public int groupID;

        /// <summary>
        /// 停放的车
        /// </summary>
        public List<ParkingSpace> parkingspaces = new List<ParkingSpace>();

        public GroupParkingSpace(int groupID, int numParkingSpace)
        {
            this.groupID = groupID;
            for (int i = 0; i < numParkingSpace; i++)
            {
                var parkingSpace = new ParkingSpace(groupID, i);
                parkingspaces.Add(parkingSpace);
            }
        }

        public ParkingSpace GetFreeParingSpace()
        {
            ParkingSpace parkingSpace = null;
            for (int i = 0; i < parkingspaces.Count; i++)
            {
                parkingSpace = parkingspaces[i];
                if (parkingSpace.IsFree())
                {
                    return parkingSpace;
                }
            }

            return null;
        }

        public ParkingSpace GetParkingSpace()
        {
            ParkingSpace parkingSpace = null;
            for (int i = 0; i < parkingspaces.Count; i++)
            {
                parkingSpace = parkingspaces[i];
                if (parkingSpace.IsParking())
                {
                    return parkingSpace;
                }
            }

            return null;
        }

        public ParkingSpace GetSpace(int idx)
        {
            return parkingspaces[idx];
        }

    }
}

