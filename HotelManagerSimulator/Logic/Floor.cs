using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace HotelManagerSimulator.Logic
{
    [Serializable]
    class Floor 
    {
        public List<Room> Rooms { get; set; }

        public Floor() {}
        public Floor(byte number, List<Room> rooms)
        {
            Rooms = rooms;
            Number = number;
        }

        public byte Number { get; set; }


    }
}
