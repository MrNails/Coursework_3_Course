using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagerSimulator.Logic
{
    struct Condition
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string RoomType { get; set; }

        public Condition(int? minValue, int? maxValue, string roomType)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.RoomType = roomType;
        }
    }

    [Serializable]
    abstract class Human
    {

        private static int id = 0;

        public Human()
        {}
        public Human(string name, int age, int id = -1)
        {
            Name = name;
            Age = age;
            if (id == -1)
            {
                Id = Human.id;
                Human.id++;
            } else
            {
                Id = id;
            }


        }

        public int Id { get; }
        public int Age { get; set; }
        public string Name { get; set; }

    }

    [Serializable]
    class Guest : Human
    {

        public Guest()
        {}

        public Guest(string name, int age, int id = -1) : base(name, age, id)
        {
        }

    }

    [Serializable]
    class Manager : Human, INotifyPropertyChanged
    {
        public delegate void HumanAction();
        public event HumanAction PaymentError;
        public event HumanAction PeopleCountError;

        private int settledPeopleCount;
        private int score;

        [field:NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Manager()
        {
        }
        public Manager(string name, int age, int settledPeopleCount, int score, int id = -1) : base(name, age, id)
        {
            SettledPeopleCount = settledPeopleCount;
            Score = score;
        }


        public int SettledPeopleCount
        {
            get { return settledPeopleCount; }
            set { 
                settledPeopleCount = value;
                OnPropertyChanged("SettledPeopleCount");
            }
        }

        public int Score
        {
            get { return score; }
            set
            {
                score = value;
                OnPropertyChanged("Score");
            }
        }


        public bool RecieveGuest(Room room, Family family)
        {
            if (room != null && family != null)
            {
                if(room.Guests != null)
                {
                    PeopleCountError?.Invoke();
                    return false;
                }

                if (room.Cost > family.Money)
                {
                    PaymentError?.Invoke();
                    return false;
                }

                room.Guests = family;
                room.IsFree = false;
                SettledPeopleCount += family.MembersCount;

                Score += CalculateScore(family.Requirements, room);

                room.EndSettleGuest = family.EndSettle;

                return true;
            }

            return false;
        }

        public void SetRoomFree(Room room, List<Family> families)
        {
            if(room != null && room.Guests != null)
            {
                families.Remove(room.Guests);

                room.Guests = null;
                room.IsFree = true;
                room.EndSettleGuest = DateTime.Now;
            }
        }

        public List<Room> GetFreeRoom(List<Floor> floors)
        {
            return (from floor in floors
                    where floor.Rooms.Count > 0
                    from room in floor.Rooms
                    where room.IsFree == true
                    select room).ToList<Room>();
        }

        public List<Room> GetFreeRoom(List<Floor> floors, Condition condition)
        {
            return (from floor in floors
                    where floor.Rooms.Count > 0
                    from room in floor.Rooms
                    where room.IsFree == true 
                    && !(condition.MinValue.HasValue && room.Cost < condition.MinValue.Value) 
                    && !(condition.MaxValue.HasValue && room.Cost > condition.MaxValue.Value)
                    && !(condition.RoomType != null && room.RoomType.ToString() != condition.RoomType)
                    select room).ToList<Room>();
        }

        private int CalculateScore(FamilyRequirements requirements, Room room)
        {
            int score = 0;

            if(requirements.RoomType == room.RoomType)
            {
                switch (room.RoomType)
                {
                    case ERoomType.Economy:
                        score += 1;
                        break;
                    case ERoomType.Standart:
                        score += 2;
                        break;
                    case ERoomType.Superior:
                        score += 3;
                        break;
                    case ERoomType.Deluxe:
                        score += 4;
                        break;
                    case ERoomType.JuniorSuite:
                        score += 5;
                        break;
                    case ERoomType.Luxe:
                        score += 6;
                        break;
                }
            } else if(requirements.RoomType > room.RoomType)
            {
                score += (-1) *(requirements.RoomType - room.RoomType);
            } else
            {
                if((requirements.MaxCost + requirements.MinCost) / 2 - room.Cost > 100)
                {
                    score -= (int)(((requirements.MaxCost + requirements.MinCost) / 2 - room.Cost) / 100);
                }
            }

            return score;
        }
    }
}
