using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace HotelManagerSimulator.Logic
{
    static class Game
    {
        private static DispatcherTimer timer { get; set; }
        private static DispatcherTimer SettleTimer { get; set; }
        public static List<Floor> Floors { get; set; }
        public static List<Family> Peoples { get; set; }
        public static Manager Manager { get; set; }
        public static byte MaxSpawnPeopleCount { get; set; }
        public static byte CurrentSpawnPeopleCount { get; set; }
        public static byte FamilyWaitingTime { get; set; }

        static Game() {
            Manager = null;
            Peoples = null;
            timer = null;
            SettleTimer = null;
        }

        public static void Start(EventHandler spawnFunc, EventHandler checkSettle)
        {

            MaxSpawnPeopleCount = 1;
            CurrentSpawnPeopleCount = 0;

            if (Floors == null)
            {
                Floors = new List<Floor>();
            }

            FamilyWaitingTime = 25;

            int econom = 5, standart = 4, superior = 4, deluxe =3, junior = 3, luxe =1;

            for (byte i = 1; i <= 4; i++)
            {
                List<Room> rooms = new List<Room>();
                for (short j = 1; j <= 5; j++)
                {
                    if (econom > 0)
                    {
                        rooms.Add(new Room((short)(i * 100 + j), 70, ERoomType.Economy, true, null, DateTime.Now, new List<string> {
                        "Кровать", "Телевизор", "Ванна", "Шкаф"
                        }));
                        econom--;
                    }
                    else if (standart > 0)
                    {
                        rooms.Add(new Room((short)(i * 100 + j), 150, ERoomType.Standart, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать", "Телевизор", "Ванна", "Шкаф"
                        }));
                        standart--;
                    }
                    else if(superior > 0)
                    {
                        rooms.Add(new Room((short)(i * 100 + j), 200, ERoomType.Superior, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать", "Телевизор", "Ванна", "Шкаф", "Кондиционер"
                        }));
                        superior--;
                    }
                    else if(deluxe > 0)
                    {
                        rooms.Add(new Room((short)(i * 100 + j), 300, ERoomType.Deluxe, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать x2", "Телевизор", "Ванна", "Шкаф", "Кондиционер"
                        }));
                        deluxe--;
                    }
                    else if(junior > 0)
                    {
                        rooms.Add(new Room((short)(i * 100 + j), 400, ERoomType.JuniorSuite, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать x2", "Телевизор", "Ванна", "Шкаф", "Кондиционер", "Мини бар"
                        }));
                        junior--;
                    }
                    else if(luxe > 0)
                    {
                        rooms.Add(new Room((short)(i * 100 + j), 550, ERoomType.Luxe, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать x2", "Телевизор", "Ванна", "Шкаф", "Кондиционер", "Мини бар"
                        }));
                        luxe--;
                    }
                }

                Floors.Add(new Floor
                {
                    Number = (byte)(i + 1),
                    Rooms = rooms
                }); 
            }

            if (Peoples == null)
            {
                Peoples = new List<Family>();
            }

            if (Manager == null)
            {
                Manager = new Manager();
            }

            if (timer == null && SettleTimer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += spawnFunc;
                timer.Start();

                SettleTimer = new DispatcherTimer();
                SettleTimer.Interval = TimeSpan.FromSeconds(1);
                SettleTimer.Tick += checkSettle;
                SettleTimer.Start();
            } else
            {
                timer.Start();
                SettleTimer.Start();
            }
        }

        public static void SpawnPeople()
        {
            if(CurrentSpawnPeopleCount < MaxSpawnPeopleCount && Manager.GetFreeRoom(Floors).Count > 0)
            {
                timer.Start();
            } else
            {
                timer.Stop();
            }
        }

        public static Family SpawnFamily()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int amount = random.Next(1, 5);
            List<Human> guests = new List<Human>();
            List<string> names = new List<string> { "Nick", "Julia", "Piter", "Mark", "Smith" };
            for (int i = 0; i < amount; i++)
            {
                if(i < 2) {
                    guests.Add(new Guest(names[i], random.Next(19, 35)));
                } else
                {
                    guests.Add(new Guest(names[i], random.Next(5, 17)));
                }
            }
            
            Family family = new Family(guests, random.Next(70, 600));
            Peoples.Add(family);

            return family;
        }

        public static void StopGame()
        {
            timer.Stop();
            SettleTimer.Stop();
        }
        public static void ResumeGame()
        {
            SpawnPeople();
            SettleTimer.Start();
        }
    }
}
