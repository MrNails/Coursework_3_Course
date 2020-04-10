using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HotelManagerSimulator.Logic
{
    [Serializable]
    public struct LocalTime
    {
        private int hour;
        private int minutes;
        private int seconds;

        public LocalTime(int hour, int minutes, int seconds)
            : this()
        {
            this.Hour = hour;
            this.Minute = minutes;
            this.Second = seconds;   
        }

        public LocalTime(DateTime time)
        {
            this.hour = time.Hour;
            this.seconds = time.Second;
            this.minutes = time.Minute;

        }

        public int Hour
        {
            get { return hour; }
            set
            {
                if (value < 0)
                {
                    hour = 0;
                }
                else if (value < 24)
                {
                    hour = value;
                } else
                {
                    while (value >= 24)
                    {
                        value -= 24;
                    }
                    hour = value;
                }
            }
        }

        public int Minute
        {
            get { return minutes; }
            set
            {
                if (value < 0)
                {
                    minutes = 0;
                }
                else if (value < 60)
                {
                    minutes = value;
                } else 
                {
                    while (value >= 60)
                    {
                        value -= 60;
                        Hour += 1;
                    }
                    minutes = value;
                }
            }
        }

        public int Second
        {
            get { return seconds; }
            set
            {
                if (value < 0)
                {
                    seconds = 0;
                }
                else if (value < 60)
                {
                    seconds = value;
                } else
                {
                    while (value >= 60)
                    {
                        value -= 60;
                        Minute += 1;
                    }
                    seconds = value;
                }
            }
        }

        public override string ToString()
        {
            string text = "";

            if(hour < 10)
            {
                text += "0" + hour.ToString();
            } else
            {
                text += hour.ToString();
            }

            text += ":";

            if (minutes < 10)
            {
                text += "0" + minutes.ToString();
            }
            else
            {
                text += minutes.ToString();
            }

            text += ":";

            if (seconds < 10)
            {
                text += "0" + seconds.ToString();
            }
            else
            {
                text += seconds.ToString();
            }

            return text;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(DateTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour == leftTime.Hour && rightTime.Minute == leftTime.Minute && rightTime.Second == leftTime.Second;
        }
        public static bool operator !=(DateTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour != leftTime.Hour && rightTime.Minute != leftTime.Minute && rightTime.Second != leftTime.Second;
        }
        public static bool operator >(DateTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour > leftTime.Hour && rightTime.Minute > leftTime.Minute && rightTime.Second > leftTime.Second;
        }
        public static bool operator <(DateTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour < leftTime.Hour && rightTime.Minute < leftTime.Minute && rightTime.Second < leftTime.Second;
        }
        public static bool operator >=(DateTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour >= leftTime.Hour && rightTime.Minute >= leftTime.Minute && rightTime.Second >= leftTime.Second;
        }
        public static bool operator <=(DateTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour <= leftTime.Hour && rightTime.Minute <= leftTime.Minute && rightTime.Second <= leftTime.Second;
        }

        public static bool operator ==(LocalTime rightTime, DateTime leftTime)
        {
            return rightTime.Hour == leftTime.Hour && rightTime.Minute == leftTime.Minute && rightTime.Second == leftTime.Second;
        }
        public static bool operator !=(LocalTime rightTime, DateTime leftTime)
        {
            return rightTime.Hour != leftTime.Hour && rightTime.Minute != leftTime.Minute && rightTime.Second != leftTime.Second;
        }
        public static bool operator >(LocalTime rightTime, DateTime leftTime)
        {
            return rightTime.Hour > leftTime.Hour && rightTime.Minute > leftTime.Minute && rightTime.Second > leftTime.Second;
        }
        public static bool operator <(LocalTime rightTime, DateTime leftTime)
        {
            return rightTime.Hour < leftTime.Hour && rightTime.Minute < leftTime.Minute && rightTime.Second < leftTime.Second;
        }
        public static bool operator >=(LocalTime rightTime, DateTime leftTime)
        {
            return rightTime.Hour >= leftTime.Hour && rightTime.Minute >= leftTime.Minute && rightTime.Second >= leftTime.Second;
        }
        public static bool operator <=(LocalTime rightTime, DateTime leftTime)
        {
            return rightTime.Hour <= leftTime.Hour && rightTime.Minute <= leftTime.Minute && rightTime.Second <= leftTime.Second;
        }

        public static bool operator ==(LocalTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour == leftTime.Hour && rightTime.Minute == leftTime.Minute && rightTime.Second == leftTime.Second;
        }
        public static bool operator !=(LocalTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour != leftTime.Hour && rightTime.Minute != leftTime.Minute && rightTime.Second != leftTime.Second;
        }
        public static bool operator >(LocalTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour > leftTime.Hour && rightTime.Minute > leftTime.Minute && rightTime.Second > leftTime.Second;
        }
        public static bool operator <(LocalTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour < leftTime.Hour && rightTime.Minute < leftTime.Minute && rightTime.Second < leftTime.Second;
        }
        public static bool operator >=(LocalTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour >= leftTime.Hour && rightTime.Minute >= leftTime.Minute && rightTime.Second >= leftTime.Second;
        }
        public static bool operator <=(LocalTime rightTime, LocalTime leftTime)
        {
            return rightTime.Hour <= leftTime.Hour && rightTime.Minute <= leftTime.Minute && rightTime.Second <= leftTime.Second;
        }

        public static explicit operator DateTime(LocalTime localTime)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, localTime.Hour, localTime.Minute, localTime.Second);
        }
    }

    static class Game
    {
        private static bool isLoaded;

        private static DispatcherTimer timer { get; set; }
        private static DispatcherTimer SettleTimer { get; set; }

        public static List<Floor> Floors { get; set; }
        public static List<Family> Peoples { get; set; }
        public static Manager Manager { get; set; }
        public static LocalTime LocalTime { get; set; }
        public static byte MaxSpawnPeopleCount { get; set; }
        public static byte CurrentSpawnPeopleCount { get; set; }
        public static byte FamilyWaitingTime { get; set; }
        public static int MaxRefusedPeople { get; set; }
        public static int RefusedPeopleCount { get; set; }

        static Game()
        {
            Manager = null;
            Peoples = null;
            timer = null;
            SettleTimer = null;
            isLoaded = false;
            LocalTime = new LocalTime(DateTime.Now);
        }

        public static void Start(EventHandler spawnFunc, EventHandler checkSettle)
        {
            MaxRefusedPeople = 5;
            RefusedPeopleCount = 0;
            MaxSpawnPeopleCount = 1;
            CurrentSpawnPeopleCount = 0;

            if (Floors == null)
            {
                Floors = new List<Floor>();
            }

            FamilyWaitingTime = 25;

            if (!isLoaded)
            {

                int econom = 5, standart = 4, superior = 4, deluxe = 3, junior = 3, luxe = 1;

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
                        else if (superior > 0)
                        {
                            rooms.Add(new Room((short)(i * 100 + j), 200, ERoomType.Superior, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать", "Телевизор", "Ванна", "Шкаф", "Кондиционер"
                        }));
                            superior--;
                        }
                        else if (deluxe > 0)
                        {
                            rooms.Add(new Room((short)(i * 100 + j), 300, ERoomType.Deluxe, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать x2", "Телевизор", "Ванна", "Шкаф", "Кондиционер"
                        }));
                            deluxe--;
                        }
                        else if (junior > 0)
                        {
                            rooms.Add(new Room((short)(i * 100 + j), 400, ERoomType.JuniorSuite, true, null, DateTime.Now, new List<string> {
                        "Двойная кровать x2", "Телевизор", "Ванна", "Шкаф", "Кондиционер", "Мини бар"
                        }));
                            junior--;
                        }
                        else if (luxe > 0)
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
            }
            else
            {
                timer.Start();
                SettleTimer.Start();
            }
        }

        public static void SpawnPeople()
        {
            if (CurrentSpawnPeopleCount < MaxSpawnPeopleCount && Manager.GetFreeRoom(Floors).Count > 0)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        public static Family SpawnFamily()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int amount = random.Next(1, 5);
            List<Guest> guests = new List<Guest>();
            List<string> names = new List<string> { "Nick", "Julia", "Piter", "Mark", "Smith" };
            for (int i = 0; i < amount; i++)
            {
                if (i < 2)
                {
                    guests.Add(new Guest(names[i], random.Next(19, 35)));
                }
                else
                {
                    guests.Add(new Guest(names[i], random.Next(5, 17)));
                }
            }

            Family family = new Family(guests, random.Next(70, 600), LocalTime);
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

        public static void RefusePeople()
        {
            if (MaxRefusedPeople < ++RefusedPeopleCount)
            {
                Manager.Score--;
                RefusedPeopleCount = 0;
            }
        }

        public static bool Save(string fileName)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                object[] saveFile = new object[4] { Manager, Peoples, Floors, LocalTime};

                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, saveFile);
                    
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool Load(string fileName)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                object[] loadFile = new object[4];

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    loadFile = formatter.Deserialize(fs) as object[];
                }

                Manager = (Manager)loadFile[0];
                Peoples = (List<Family>)loadFile[1];
                Floors = (List<Floor>)loadFile[2];
                LocalTime = (LocalTime)loadFile[3];

                isLoaded = true;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
