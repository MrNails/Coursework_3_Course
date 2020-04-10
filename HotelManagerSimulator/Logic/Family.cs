using System;
using System.Collections.Generic;

namespace HotelManagerSimulator.Logic
{
    [Serializable]
    struct FamilyRequirements
    {
        public ERoomType RoomType { get; set; }
        public float MinCost { get; set; }
        public float MaxCost { get; set; }
        public List<string> Furniture { get; set; }

        public FamilyRequirements(ERoomType roomType, float minCost, float maxCost, List<string> furniture)
        {
            this.RoomType = roomType;
            this.MinCost = minCost;
            this.MaxCost = maxCost;
            this.Furniture = furniture;
        }

        public override string ToString()
        {
            string text = "Требования:\n";
            text += "     Тип комнаты: " + RoomType.ToString();
            text += "\n     Цена от: " + MinCost.ToString() + "\n\tдо " + MaxCost.ToString();
            if (Furniture != null && Furniture.Count > 0)
            {
                text += "\n     Мебель: ";
                foreach (string item in Furniture)
                {
                    text += item + ", ";
                }
            }

            return text;
        }
    }

    [Serializable]
    class Family
    {
        
        public List<Guest> Members { get; set; }
        public int MembersCount { get; set; }
        public short RoomNumber { get; set; }
        public DateTime EndSettle { get; set; }
        public FamilyRequirements Requirements { get; }
        public float Money { get; set; }

        public Family()
        {
        
        }

        public Family(List<Guest> members, float money, LocalTime localTime = default(LocalTime), short roomNumber = 0)
        {
            localTime.Second = new Random(DateTime.Now.Millisecond).Next(60, 180);

            Members = members;
            RoomNumber = roomNumber;
            MembersCount = members.Count;
            EndSettle = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, localTime.Hour, localTime.Minute, localTime.Second);
            Requirements = GenerateRequirements();
            Money = money;
        }

        public Guest this[int index]
        {
            get 
            {
                if (index >= 0 && index < Members.Count)
                {
                    return Members[index];
                }
                else
                {
                    return null;
                }
            }
        }

        private FamilyRequirements GenerateRequirements()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            ERoomType roomType = ERoomType.Economy;
            int room = random.Next(0, 100);

            if(room > 0 && room < 3)
            {
                roomType = ERoomType.Luxe;
            } else if (room > 3 && room < 10)
            {
                roomType = ERoomType.JuniorSuite;
            } else if (room > 10 && room < 25)
            {
                roomType = ERoomType.Deluxe;
            } else if (room > 25 && room < 40)
            {
                roomType = ERoomType.Superior;
            } else if (room > 40 && room < 65)
            {
                roomType = ERoomType.Standart;
            } else
            {
                roomType = ERoomType.Economy;
            }

            float minCost = random.Next(50, 300);
            float maxCost = random.Next(320, 600);

            return new FamilyRequirements(roomType, minCost, maxCost, null);
        }

        public override string ToString()
        {
            string text = "";
            text += Requirements.ToString();
            text += "\n\nКоличество денег: " + Money.ToString();
            text += "\nДата выселения: " + EndSettle.ToString("T");

            return text;
        }
    }
}
