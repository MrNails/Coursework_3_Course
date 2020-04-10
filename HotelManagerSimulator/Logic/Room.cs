using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace HotelManagerSimulator.Logic
{
    enum ERoomType : byte
    {
        Economy = 1,
        Standart,
        Superior,
        Deluxe,
        JuniorSuite,
        Luxe
    }

    [Serializable]
    class Room : INotifyPropertyChanged
    {
        private bool isFree;
        private DateTime endSettle;
        private Family guests;


        public Room() {}
        public Room(short roomNumber, float cost, ERoomType roomType) : this(roomNumber, cost, roomType, true, null, DateTime.Now, new List<string>()) {}

        public Room(short roomNumber, float cost, ERoomType roomType, bool isFree, Family guests, DateTime endSettle, List<string> furniture)
        {
            Number = roomNumber;
            Cost = cost;
            RoomType = roomType;
            IsFree = isFree;
            Guests = guests;
            EndSettleGuest = endSettle;
            Furniture = furniture;
        }

        public List<string> Furniture { get; set; }
        public short Number { get; set; }
        public int MaxPeopleCount { get; set; }
        public float Cost { get; set; }
        public ERoomType RoomType { get; set; }

        public bool IsFree
        {
            get { return isFree; }
            set { 
                isFree = value;
                OnPropertyChanged("IsFree");
            }
        }

        public DateTime EndSettleGuest
        {
            get { return endSettle; }
            set
            {
                endSettle = value;
                OnPropertyChanged("EndSettleGuest");
            }
        }

        public Family Guests
        {
            get { return guests; }
            set
            {
                guests = value;
                OnPropertyChanged("Guests");
            }
        }

        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            string text= "";
            text += "Тип комнаты: " + RoomType.ToString() + "\n";
            //text += "Максимальное количество жильцов: " + MaxPeopleCount.ToString() + "\n";
            text += "Текущее количество жильцов: " + Guests?.MembersCount.ToString() + "\n";
            if (Guests == null)
            {
                text += "Дата выселения: нет";
            } else
            {
                text += "Дата выселения: " + EndSettleGuest.ToString("T");
            }

            return text;
        }
    }
}
