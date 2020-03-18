using System;
using System.Collections.Generic;

namespace HotelManagerSimulator.Logic
{
    [Serializable]
    struct FamilyRequirements
    {
        public ERoomType roomType;
        public float minCost;
        public float maxCost;
        public List<string> furniture;

        public FamilyRequirements(ERoomType roomType, float minCost, float maxCost, List<string> furniture)
        {
            this.roomType = roomType;
            this.minCost = minCost;
            this.maxCost = maxCost;
            this.furniture = furniture;
        }

        public override string ToString()
        {
            string text = "Требования:\n";
            text += "     Тип комнаты: " + roomType.ToString();
            text += "\n     Цена от: " + minCost.ToString() + "\n\tдо " + maxCost.ToString();
            if (furniture != null && furniture.Count > 0)
            {
                text += "\n     Мебель: ";
                foreach (string item in furniture)
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
        
        public List<Human> Members { get; set; }
        public int MembersCount { get; set; }
        public short RoomNumber { get; set; }
        public DateTime EndSettle { get; set; }
        public FamilyRequirements Requirements { get; }
        public float Money { get; set; }


        public Family()
        {}

        public Family(List<Human> members, float money, short roomNumber = 0)
        {
            Members = members;
            RoomNumber = roomNumber;
            MembersCount = members.Count;
            EndSettle = new DateTime(DateTime.Now.Ticks + new Random(DateTime.Now.Millisecond).Next(300000000, 1500000000));
            Requirements = GenerateRequirements();
            Money = money;
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
