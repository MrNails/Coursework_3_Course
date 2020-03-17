using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelManagerSimulator.Logic;



namespace HotelManagerSimulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum GameType : byte
        {
            Load = 1,
            NewGame
        }

        private enum MoveDirection : byte
        {
            Left = 1,
            Right
        }

        private bool isRefused;
        public static RoutedCommand MyCommand = new RoutedCommand();


        public MainWindow()
        {
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            InitializeComponent();
            isRefused = false;
            Tab1Image.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\Background_field.jfif")))).ImageSource;
            Tab2Image.Source = Tab1Image.Source;
        }

        private void GameInit()
        {
            try
            {

                Game.Start(SpawnFamily, CheckEndSettle);

                Game.Manager.PaymentError += () => MessageBox.Show("Не хватает денег на поселение");
                Game.Manager.PeopleCountError += () => MessageBox.Show("Вы не можете поселить, так как в комнате есть люди");

                this.DataContext = Game.Manager;

                FillGrid(GameType.NewGame);

                ListRoom.ItemsSource = Game.Manager.GetFreeRoom(Game.Floors);

                BackgroundImage.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\Background_field.jfif")))).ImageSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message:\n\t{ex.Message}\nStackTrace:\n\t{ex.StackTrace}");
            }
        }

        private void GameOver()
        {
            Game.StopGame();

            Game.Manager.SettledPeopleCount = 0;
            Game.Manager.Score = 0;
            Game.Peoples.Clear();
            Game.Floors.Clear();

            CloseAllTip();

            for (int i = 0; i < MainField.Children.Count; i++)
            {
                if (MainField.Children[i] is Canvas)
                {
                    MainField.Children.RemoveAt(i);
                    i--;
                }
            }

            PriceCheckBox.IsChecked = false;
            TypeCheckBox.IsChecked = false;

            SetFilterConditionClick(null, null);

            if (PauseTab.IsSelected || MessageBox.Show("Вы проиграли\nВыйти в главное меню?", "Проигрыш", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            {
                MainMenuTab.IsSelected = true;
            }
            else
            {
                MessageBox.Show("+");
                Game.Start(SpawnFamily, CheckEndSettle);
                FillGrid(GameType.NewGame);
            }
        }


        //Создаёт картинку и семью
        private void SpawnFamily(object sender, EventArgs e)
        {
            bool exit = true;
            Canvas canvas1 = new Canvas();
            Image image1 = new Image();
            Button button = new Button();
            ToolTip tool = new ToolTip();
            for (int i = MainField.Children.Count - 1; i >= 0 && exit; i--)
            {
                if (MainField.Children[i] is Canvas)
                {
                    Canvas mainCanv = (Canvas)MainField.Children[i];
                    mainCanv.PreviewMouseDown += MoveFamily;

                    image1.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\Person.png")))).ImageSource;

                    tool.Content = Game.SpawnFamily();
                    tool.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
                    tool.PlacementTarget = button;
                    tool.HasDropShadow = true;
                    tool.Foreground = new SolidColorBrush(Colors.Black);
                    tool.FontSize = 12;
                    tool.HorizontalOffset = 70;
                    tool.VerticalOffset = -35;


                    button.Margin = new Thickness(20);
                    button.Height = 100;
                    button.Width = 55;
                    button.Opacity = 0;
                    button.ToolTip = tool;

                    Canvas.SetLeft(button, 85);
                    Canvas.SetTop(button, 20);

                    canvas1.Children.Add(image1);
                    canvas1.Children.Add(button);
                    canvas1.Width = 300;
                    canvas1.Height = 300;

                    SetMoveAnimation(canvas1, MoveDirection.Right, (o, arg) =>
                    {
                        foreach (var item in canvas1.Children)
                        {
                            if (item is Button)
                            {
                                ((ToolTip)((Button)item).ToolTip).IsOpen = true;
                            }
                        }
                        button.PreviewMouseDown += MoveFamily;
                        button.PreviewMouseDown += RefuseAction;

                        mainCanv.PreviewMouseDown += RefuseAction;
                        mainCanv.MouseRightButtonUp += AnswerButtonClick;
                    });

                    mainCanv.Children.Add(canvas1);

                    Game.CurrentSpawnPeopleCount++;
                    Game.SpawnPeople();

                    exit = false;
                }

            }
        }

        //Делает опрос всей сетки с комнатами на дату выселения
        private void CheckEndSettle(object sender, EventArgs e)
        {
            if (Game.Manager.Score < 0)
            {
                GameOver();
            }

            ToolTip tool = new ToolTip();
            Border border = null;
            Room room = null;
            WrapPanel panel = null;

            //DebugTextBlock.Text = Game.FamilyWaitingTime.ToString();

            foreach (var child in MainField.Children)
            {
                if (child is Canvas)
                {
                    foreach (var canvChild in ((Canvas)child).Children)
                    {
                        if (canvChild is Border)
                        {
                            border = (Border)canvChild;
                            room = (Room)((ToolTip)((Button)border.Child).ToolTip).Content;
                            if (room.Guests != null && border.ToolTip == null && room.EndSettleGuest <= DateTime.Now)
                            {
                                tool.Content = "Время для поселения вышло";
                                tool.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
                                tool.PlacementTarget = border;
                                tool.HasDropShadow = true;
                                tool.Foreground = new SolidColorBrush(Colors.Black);
                                tool.FontSize = 12;
                                tool.HorizontalOffset = 70;
                                tool.VerticalOffset = -35;
                                tool.IsOpen = true;

                                border.ToolTip = tool;
                            }
                        }

                        if (canvChild is Canvas && Game.FamilyWaitingTime > 0 && Game.FamilyWaitingTime != byte.MaxValue)
                        {
                            Game.FamilyWaitingTime--;
                        }
                        else if (canvChild is Canvas && Game.FamilyWaitingTime == 0)
                        {
                            int score = 0;
                            Game.FamilyWaitingTime = byte.MaxValue;
                            foreach (var item in ((Canvas)canvChild).Children)
                            {
                                if (item is Button)
                                {
                                    ((Canvas)child).PreviewMouseDown -= RefuseAction;
                                    ((Canvas)child).MouseRightButtonUp -= AnswerButtonClick;
                                    ((Button)item).PreviewMouseDown -= RefuseAction;
                                    ((Button)item).PreviewMouseDown -= MoveFamily;
                                    ((ToolTip)((Button)item).ToolTip).IsOpen = false;
                                    score = ((Family)((ToolTip)((Button)item).ToolTip).Content).MembersCount;
                                }

                            }

                            SetMoveAnimation((Canvas)canvChild, MoveDirection.Left, (o, arg) =>
                            {
                                ((Canvas)child).Children.Remove((Canvas)canvChild);
                                Game.FamilyWaitingTime = 25;
                                Game.Manager.Score -= score;
                                Game.CurrentSpawnPeopleCount--;
                                Game.SpawnPeople();
                            });

                        }

                        if (canvChild is WrapPanel && (Game.FamilyWaitingTime == 0 || Game.FamilyWaitingTime == byte.MaxValue))
                        {
                            panel = (WrapPanel)canvChild;
                        }
                    }

                    if (panel != null)
                    {
                        ((Canvas)child).Children.Remove(panel);
                        panel = null;
                    }
                }
            }
        }

        //Механизм Drag&Drop
        private void MoveFamily(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button && ((Button)sender).ToolTip is ToolTip)
            {
                DragDrop.DoDragDrop((Button)sender, ((ToolTip)((Button)sender).ToolTip).Content, DragDropEffects.Move);
            }
        }

        //Механизм Drag&Drop
        private void FamilyDrop(object sender, DragEventArgs e)
        {
            Room room = null;
            Family family = null;
            if (sender is Button && ((Button)sender).ToolTip is ToolTip && Game.FamilyWaitingTime != byte.MaxValue)
            {
                room = (Room)((ToolTip)((Button)sender).ToolTip).Content;
                family = (Family)e.Data.GetData(typeof(Family));

                if (Game.Manager.RecieveGuest(room, family) == true)
                {
                    Game.FamilyWaitingTime = 25;
                    ChangePicture(room.Number);
                    DeletePeople();
                }
            }
        }



        //Заполнение сетки разными компонентами
        private void FillGrid(GameType gameType)
        {
            try
            {
                string imagePath;

                switch (gameType)
                {
                    case GameType.Load:
                        imagePath = "";
                        break;
                    case GameType.NewGame:
                        imagePath = System.IO.Path.GetFullPath(@".\Resource\Image\A.T.P._Engineer.png");
                        break;
                }

                Canvas canvas;
                Image image;
                TextBlock textBlock;
                Button button;
                ToolTip toolTip;
                Border border;
                Binding binding, binding1;

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        canvas = new Canvas();
                        image = new Image();
                        textBlock = new TextBlock();
                        button = new Button();
                        toolTip = new ToolTip();
                        border = new Border();
                        binding = new Binding();
                        binding1 = new Binding();

                        if (i < Game.Floors.Count && j < Game.Floors[i].Rooms.Count)
                        {
                            textBlock.Text = Game.Floors[Game.Floors.Count - i - 1].Rooms[j].Number.ToString();
                            toolTip.Content = Game.Floors[Game.Floors.Count - i - 1].Rooms[j];
                            button.ToolTipOpening += (o, e) =>
                            {
                                ToolTip tool = new ToolTip();
                                tool.Content = ((ToolTip)((Button)o).ToolTip).Content;
                                ((Button)o).ToolTip = tool;
                            };
                        }

                        toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
                        toolTip.HasDropShadow = true;
                        toolTip.Foreground = new SolidColorBrush(Colors.Black);
                        toolTip.FontSize = 12;
                        toolTip.HorizontalOffset = 70;
                        toolTip.VerticalOffset = -35;

                        Canvas.SetLeft(textBlock, 77);
                        Canvas.SetTop(textBlock, 39);
                        Canvas.SetLeft(border, 72);
                        Canvas.SetTop(border, 60);

                        textBlock.FontSize = 10;
                        textBlock.Width = 38;
                        textBlock.Height = 14;
                        textBlock.Margin = new Thickness(10, 0, 0, 0);

                        border.Width = 45;
                        border.Height = 79;
                        border.BorderBrush = new SolidColorBrush(Colors.Red);
                        border.ToolTip = null;

                        button.ToolTip = toolTip;
                        button.AllowDrop = true;
                        button.Drop += FamilyDrop;
                        button.PreviewMouseUp += GetOutPeople;
                        canvas.PreviewMouseUp += GetOutPeople;

                        canvas.MouseRightButtonUp += AnswerButtonClick;


                        if (i != 4)
                        {
                            image.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\EmptyRoom.bmp")))).ImageSource;
                        }
                        else if (j == 0)
                        {
                            image.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\Hall.bmp")))).ImageSource;
                        }


                        binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
                        binding.RelativeSource.AncestorType = typeof(Canvas);
                        binding.Path = new PropertyPath("ActualWidth");
                        image.SetBinding(Image.WidthProperty, binding);

                        binding1.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
                        binding1.RelativeSource.AncestorType = typeof(Canvas);
                        binding1.Path = new PropertyPath("ActualHeight");
                        image.SetBinding(Image.HeightProperty, binding1);
                        image.Stretch = Stretch.Fill;

                        border.Child = button;


                        if (i != 4)
                        {
                            canvas.Children.Add(image);
                            canvas.Children.Add(textBlock);
                            canvas.Children.Add(border);
                        }

                        Grid.SetColumn(canvas, j + 1);
                        Grid.SetRow(canvas, i);

                        if (i == 4 && j == 0)
                        {
                            Grid.SetColumnSpan(canvas, 5);
                            canvas.Children.Add(image);

                            canvas.MouseRightButtonUp -= AnswerButtonClick;
                            canvas.PreviewMouseUp -= GetOutPeople;
                        }

                        MainField.Children.Add(canvas);

                        if (i == 4 && j == 0)
                        {
                            image = new Image();
                            Grid.SetColumn(image, 5);
                            Grid.SetRow(image, i);
                            image.Margin = new Thickness(100, 0, 0, 0);
                            image.ToolTip = "Вы";
                            image.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\A.T.P._Engineer.png")))).ImageSource;
                            MainField.Children.Add(image);

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");

            }
        }


        private void ChangePicture(int roomNumber)
        {
            foreach (var child in MainField.Children)
            {
                if (child is Canvas)
                {
                    Image image = null;
                    foreach (var canvChild in ((Canvas)child).Children)
                    {
                        if (canvChild is Image)
                        {
                            image = (Image)canvChild;
                        }

                        if (canvChild is TextBlock && Int32.Parse(((TextBlock)canvChild).Text) == roomNumber && image != null)
                        {
                            SetFilterConditionClick(null, null);
                            image.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\FullRoom.bmp")))).ImageSource;

                        }
                    }
                }
            }
        }


        private void DeletePeople()
        {
            WrapPanel panel = null;
            Canvas canvas = null;
            foreach (var child in MainField.Children)
            {
                if (child is Canvas)
                {
                    foreach (var canvChild in ((Canvas)child).Children)
                    {
                        if (canvChild is Canvas)
                        {
                            ((Canvas)child).PreviewMouseDown -= RefuseAction;
                            ((Canvas)child).MouseRightButtonUp -= AnswerButtonClick;

                            if (((Canvas)((Canvas)child).Children[1]).Children[1] is Button)
                            {
                                ((ToolTip)((Button)((Canvas)((Canvas)child).Children[1]).Children[1]).ToolTip).IsOpen = false;
                                Game.CurrentSpawnPeopleCount--;
                                if (!PauseTab.IsSelected)
                                {
                                    Game.SpawnPeople();
                                }
                            }
                            canvas = (Canvas)canvChild;
                        }

                        if (canvChild is WrapPanel)
                        {
                            panel = (WrapPanel)canvChild;
                        }
                    }

                    if (panel != null)
                    {
                        ((Canvas)child).Children.Remove(panel);
                    }

                    if (canvas != null)
                    {
                        ((Canvas)child).Children.Remove(canvas);
                        return;
                    }

                }
            }
        }

        //Анимирует выбранную комнату в листбоксе
        private void BorderAnimation(object sender, MouseButtonEventArgs e)
        {
            Room room = ListRoom.SelectedItem as Room;
            if (room != null)
            {
                foreach (var child in MainField.Children)
                {
                    if (child is Canvas)
                    {
                        foreach (var canvChild in ((Canvas)child).Children)
                        {
                            if (canvChild is TextBlock)
                            {
                                if (((TextBlock)canvChild).Text != "" && Convert.ToInt32(((TextBlock)canvChild).Text) != room.Number)
                                {
                                    break;
                                }
                            }
                            else if (canvChild is Border)
                            {
                                ThicknessAnimation borderSizeAnimation = new ThicknessAnimation();
                                borderSizeAnimation.From = new Thickness(0);
                                borderSizeAnimation.To = new Thickness(6);
                                borderSizeAnimation.Duration = TimeSpan.FromSeconds(1);
                                borderSizeAnimation.AutoReverse = true;

                                ColorAnimation colorAnimation = new ColorAnimation();
                                colorAnimation.From = Colors.Blue;
                                colorAnimation.To = Colors.Red;
                                colorAnimation.Duration = TimeSpan.FromSeconds(1);
                                colorAnimation.AutoReverse = true;


                                ((Border)canvChild).BeginAnimation(Border.BorderThicknessProperty, borderSizeAnimation);
                                ((Border)canvChild).BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                                break;
                            }
                        }
                    }
                }
            }
        }

        //Создаёт панель с кнопками, которые нужны для удаления гостей
        private void GetOutPeople(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas)
            {
                ToolTip tool = new ToolTip();

                foreach (var child in ((Canvas)sender).Children)
                {

                    if (child is Border)
                    {
                        tool.Content = (Room)((ToolTip)((Button)((Border)child).Child).ToolTip).Content;
                    }

                    if (child is WrapPanel)
                    {
                        return;
                    }
                }

                if (((Room)tool.Content).Guests == null)
                {
                    return;
                }

                tool.Opacity = 0;

                WrapPanel panel = CreatePanelWithButton("Выселить?", AnswerButtonClick, 1, tool);

                Canvas.SetTop(panel, 60);
                Canvas.SetLeft(panel, 45);

                ((Canvas)sender).Children.Add(panel);
            }
        }

        private void RefuseAction(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas)
            {
                ToolTip tool = new ToolTip();

                foreach (var child in ((Canvas)sender).Children)
                {
                    if (child is Canvas)
                    {
                        tool.Content = (Canvas)child;
                    }

                    if (child is WrapPanel)
                    {
                        return;
                    }
                }

                tool.Opacity = 0;

                WrapPanel panel = CreatePanelWithButton("Отказать?", AnswerButtonClick, 2, tool);

                Canvas.SetTop(panel, 60);
                Canvas.SetLeft(panel, 45);

                ((Canvas)sender).Children.Add(panel);
            }
        }


        private WrapPanel CreatePanelWithButton(string text, MouseButtonEventHandler handler, int tag = -1, ToolTip tool = null)
        {
            WrapPanel panel = new WrapPanel();
            Button button = new Button();
            TextBlock textBlock = new TextBlock();

            textBlock.Text = text;
            textBlock.Margin = new Thickness(20, 0, 0, 0);
            panel.Children.Add(textBlock);


            for (int i = 0; i < 2; i++)
            {
                button = new Button();
                if (i == 0)
                {
                    button.Content = "Да";
                    button.MouseRightButtonUp += handler;
                    button.Tag = tag;
                    if (tool != null)
                    {
                        button.ToolTip = tool;
                    }
                }
                else
                {
                    button.Content = "Нет";
                    button.MouseRightButtonUp += handler;
                }
                button.Padding = new Thickness(5);
                button.Margin = new Thickness(3);
                button.Opacity = 100;
                button.Width = 45;
                button.Height = 30;

                panel.Children.Add(button);

            }
            panel.Height = 60;
            panel.Width = 110;

            return panel;
        }

        //Удаляет гостей с их комнаты вместе с кнопками
        private void AnswerButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {

                if (sender is Button)
                {
                    Button button = (Button)sender;
                    if (button.Content is string && (string)button.Content == "Да")
                    {
                        if (button.Tag is int && (int)button.Tag == 1)
                        {
                            Game.Manager.SetRoomFree((Room)((ToolTip)button.ToolTip).Content, Game.Peoples);
                        }
                        else if (button.Tag is int && (int)button.Tag == 2)
                        {
                            isRefused = true;
                            Game.FamilyWaitingTime = byte.MaxValue;

                            SetMoveAnimation((Canvas)((ToolTip)button.ToolTip).Content, MoveDirection.Left, (obj, arg) =>
                            {
                                Game.FamilyWaitingTime = 25;
                                ((ToolTip)button.ToolTip).Content = null;
                                DeletePeople();
                                Game.RefusePeople();
                            });
                        }
                    }
                }

                if (sender is Canvas)
                {
                    Canvas canvas = (Canvas)sender;
                    Image image = null;
                    Button button = null;
                    foreach (var child in canvas.Children)
                    {
                        if (child is Image)
                        {
                            image = (Image)child;
                        }

                        if (child is Border)
                        {
                            button = (Button)((Border)child).Child;
                            if (((Room)((ToolTip)button.ToolTip).Content).Guests == null && image != null)
                            {
                                if (((Border)child).ToolTip != null)
                                {
                                    ((ToolTip)((Border)child).ToolTip).IsOpen = false;
                                    ((Border)child).ToolTip = null;
                                }

                                SetFilterConditionClick(null, null);
                                image.Source = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\Resource\Image\EmptyRoom.bmp")))).ImageSource;
                            }
                        }

                        if (child is Canvas && isRefused)
                        {
                            ((Canvas)sender).PreviewMouseDown -= RefuseAction;
                            ((Canvas)sender).MouseRightButtonUp -= AnswerButtonClick;

                            foreach (var item in ((Canvas)child).Children)
                            {
                                if (item is Button)
                                {
                                    ((Button)item).PreviewMouseDown -= MoveFamily;
                                    ((Button)item).PreviewMouseDown -= RefuseAction;
                                }
                            }

                            isRefused = false;
                        }

                        if (child is WrapPanel)
                        {
                            canvas.Children.Remove((WrapPanel)child);
                            return;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void HelpClick(object sender, MouseButtonEventArgs e)
        {
            Game.StopGame();
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Owner = this;
            helpWindow.ShowDialog();
            Game.ResumeGame();
        }

        //Устанавливает анимацию для человечка
        private void SetMoveAnimation(Canvas canvas, MoveDirection moveDirection, EventHandler completedAction = null)
        {
            ThicknessAnimation moveAnimation = new ThicknessAnimation();
            switch (moveDirection)
            {
                case MoveDirection.Left:
                    moveAnimation.From = canvas.Margin;
                    moveAnimation.To = new Thickness(-500, 0, 0, 0);
                    break;
                case MoveDirection.Right:
                    moveAnimation.From = new Thickness(-500, 0, 0, 0);
                    moveAnimation.To = new Thickness(600, 0, 0, 0);
                    break;
            }

            if (completedAction != null)
            {
                moveAnimation.Completed += completedAction;
            }

            moveAnimation.Duration = TimeSpan.FromSeconds(3);

            canvas.BeginAnimation(Canvas.MarginProperty, moveAnimation);
        }

        private void SetFilterConditionClick(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                sender = new object();
            }

            if (e == null)
            {
                e = new RoutedEventArgs();
            }

            Logic.Condition condition = new Logic.Condition(null, null, null);

            if (PriceCheckBox.IsChecked == true)
            {
                int min, max;
                if (!int.TryParse(MinPriceTextBox.Text, out min))
                {
                    PriceCheckBox.IsChecked = false;
                    return;
                }

                if (!int.TryParse(MaxPriceTextBox.Text, out max))
                {
                    PriceCheckBox.IsChecked = false;
                    return;
                }

                condition.maxValue = max;
                condition.minValue = min;

            }
            else if (PriceCheckBox.IsChecked == null)
            {
                PriceCheckBox.IsChecked = false;
            }

            if (TypeCheckBox.IsChecked == true)
            {
                string type;
                if (sender is ComboBoxItem)
                {
                    type = (string)((ComboBoxItem)sender).Content;
                }
                else
                {
                    type = RoomTypeComboBox.Text;
                }

                switch (type)
                {
                    case "Economy":
                        condition.roomType = type;
                        break;
                    case "Standart":
                        condition.roomType = type;
                        break;
                    case "Superior":
                        condition.roomType = type;
                        break;
                    case "Deluxe":
                        condition.roomType = type;
                        break;
                    case "Junior Suite":
                        condition.roomType = "JuniorSuite";
                        break;
                    case "Luxe":
                        condition.roomType = type;
                        break;
                    default:
                        TypeCheckBox.IsChecked = false;
                        return;
                }
            }
            else if (TypeCheckBox.IsChecked == null)
            {
                PriceCheckBox.IsChecked = false;
            }

            ListRoom.ItemsSource = Game.Manager?.GetFreeRoom(Game.Floors, condition);
        }

        private void CloseAllTip()
        {
            for (int i = 0; i < MainField.Children.Count; i++)
            {
                if (MainField.Children[i] is Canvas)
                {
                    foreach (var item in ((Canvas)MainField.Children[i]).Children)
                    {
                        if (item is Border && ((Border)item).ToolTip != null)
                        {
                            ((ToolTip)((Border)item).ToolTip).IsOpen = false;
                            ((Border)item).ToolTip = null;
                            break;
                        }

                        if(item is Canvas)
                        {
                            foreach (var innerItem in ((Canvas)item).Children)
                            {
                                if(innerItem is Button)
                                {
                                    ((ToolTip)((Button)innerItem).ToolTip).IsOpen = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SelectItem(object sender, RoutedEventArgs e)
        {
            SetFilterConditionClick(sender, e);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFilterConditionClick(sender, new RoutedEventArgs());
        }


        private void NewGameClick(object sender, MouseButtonEventArgs e)
        {
            GameTab.IsSelected = true;
            GameInit();
        }

        private void ExitClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ResumeClick(object sender, MouseButtonEventArgs e)
        {
            Game.ResumeGame();
            GameTab.IsSelected = true;
        }

        private void ExitMainMenuClick(object sender, MouseButtonEventArgs e)
        {
            GameOver();
        }

        private void PauseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (GameTab.IsSelected)
            {
                Game.StopGame();
                CloseAllTip();
                PauseTab.IsSelected = true;
            }
        }
    }
}
