using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using WPFFolderBrowser;
using Microsoft.Win32;
using System.Xml.Linq;

/*
MIT License

Copyright (c) 2023 AugustLC

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace AppMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer;
        private List<Playlist> playlists;
        private SaveManager sm;

        public MainWindow()
        {
            InitializeComponent();

            //загрузка настроек из файла
            playlists = new List<Playlist>();

            LoadManager lm = new LoadManager();
            rootPathDirMusic = lm.Path;
            CheckBoxFindMusic.IsChecked = lm.FindingMusic;
            playlists = lm.Playlists;
            UpdateComboBoxPlaylists();

            sm = new SaveManager();
            sm.Path = rootPathDirMusic;

            
            //начальные настройки
            PanelChange("Radio");
            mediaPlayer = new MediaPlayer();
            
            SliderTempMusic.Maximum = 100;
            SliderTempMusic.Value = 30;
            sliderMusicVolume.Maximum = 100;
            sliderMusicVolume.Value = 30;

            mediaPlayer.Volume = 0.3;

            TextBoxMusicRootDir.Text = rootPathDirMusic;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }


        //////////////////////////////////////////////////////
        /// <summary>
        /// Верхняя панель
        /// </summary>
        //////////////////////////////////////////////////////

        // перетаксивание окна мышью за верхнюю панель
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // кнопка выхода на верхней панели
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        // кнопка свернуть на верхней панели
        private void RollUp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //////////////////////////////////////////////////////
        /// <summary>
        /// Панель навигации
        /// </summary>
        //////////////////////////////////////////////////////

        // переключение видимости панелей
        private void PanelChange(string name)
        {
            Radio.Visibility = Visibility.Hidden;
            Playlist.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;

            switch (name)
            {
                case "Radio":
                    Radio.Visibility = Visibility.Visible;
                    break;
                case "Playlist":
                    Playlist.Visibility = Visibility.Visible;
                    break;
                case "Settings":
                    Settings.Visibility = Visibility.Visible;
                    break;
            }
        }

        // перключение на вкладку "Радио"
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            PanelChange("Radio");
            mediaPlayer.Stop();
            mediaPlayer.Volume = sliderMusicVolume.Value / 100;
            sliderMusic.Value = 0;
        }

        // перключение на вкладку "Плейлисты"
        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PanelChange("Playlist");
            mediaPlayer.Stop();
            ComboBoxChoosePlaylist.SelectedIndex = -1;
            LabelPlaylistName.Content = "Название плейлиста";
            LabelMusicName.Content = "<Название>";
            LabelMusicTime.Content = "0:00 / 0:00";
            mediaPlayer.Volume = SliderTempMusic.Value / 100;
        }

        // переключение на вкладку "Настройки"
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            PanelChange("Settings");
        }


        //////////////////////////////////////////////////////
        /// <summary>
        /// Настройки
        /// </summary>
        //////////////////////////////////////////////////////

        // корневая папка с музыкой
        private string rootPathDirMusic = "";
        // флажок для функции поиска файла, если его нет по указанному пути
        private bool findingMusic;

        // выбор флажка
        private void CheckBoxFindMusic_Checked(object sender, RoutedEventArgs e)
        {
            findingMusic = true;
        }

        // снятие выбора флажка
        private void CheckBoxFindMusic_Unchecked(object sender, RoutedEventArgs e)
        {
            findingMusic = false;
        }

        private void ChooseMusicRootDir_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowserDialog dd = new WPFFolderBrowserDialog();
            var result = dd.ShowDialog();
            if (result.HasValue)
            {
                TextBoxMusicRootDir.Text = dd.FileName;
            }
        }

        // кнопка сохранения настроек
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            rootPathDirMusic = TextBoxMusicRootDir.Text;
            //сохранение настроек в отдельный файл
            SaveManager sm = new SaveManager();
            sm.Path = rootPathDirMusic;
            sm.FindingMusic = findingMusic;
            sm.SaveSettings();
        }


        //////////////////////////////////////////////////////
        /// <summary>
        /// Плейлисты
        /// </summary>
        //////////////////////////////////////////////////////

        // функция обновления списка плейлистов
        public void UpdateComboBoxPlaylists()
        {
            ComboBoxChoosePlaylist.Items.Clear();
            ComboBoxPlaylists.Items.Clear();
            foreach (var item in playlists)
            {
                ComboBoxChoosePlaylist.Items.Add(item.Name);
                ComboBoxPlaylists.Items.Add(item.Name);
            }
        }

        // функция добавления списка музыки из выбранного плейлиста в элемент ListBox
        // по порядковому номеру плейлиста
        public void UpdateListBoxPlaylistMusics(int num)
        {
            ListBoxPlaylistMusics.Items.Clear();
            foreach (var item in playlists[num].GetMusics())
            {
                ListBoxPlaylistMusics.Items.Add(item.Name);
            }
        }

        // данные для отображения файлов и папок корневой папки

        // список директорий до текущей подпапки корневой папки
        public List<string> dirs = new List<string>();
        // список типов элементов в элементе ListBox
        public List<string> itemCommand = new List<string>();
        // данные элементов в элементе ListBox
        public List<string> itemName = new List<string>();

        // функция добавляющая в элемент ListBox папки и файлы из корневой папки
        public void UpdateFilesFromRootMusicDir()
        {
            string path = rootPathDirMusic;

            // путь до текущей папки, относительно корневой папки
            foreach(var dir in dirs)
            {
                path += "\\" + dir;
            }

            // очистка
            itemCommand.Clear();
            itemName.Clear();
            FilesFromRootMusicDir.Items.Clear();

            // если текущая папка - не корневая, то добавляется
            // элемент для перехода к предыдущей папке
            if(dirs.Count != 0)
            {
                FilesFromRootMusicDir.Items.Add("📂 ...");
                itemCommand.Add("BackDirectory");
                itemName.Add("");
            }
                
            // вывод всех директорий в текущей папке
            var files = Directory.GetDirectories(path, "");
            foreach (var item in files)
            {
                FilesFromRootMusicDir.Items.Add("📂 " + item.Replace(path + "\\", ""));
                itemCommand.Add("Directory");
                itemName.Add(item.Replace(path + "\\", ""));
            }

            // вывод всех файлов в текущей папке
            files = Directory.GetFiles(path, "*.*").Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg") || s.EndsWith(".wav") || s.EndsWith(".flac")).ToArray();
            foreach (var item in files)
            {
                FilesFromRootMusicDir.Items.Add(item.Replace(path + "\\", ""));
                itemCommand.Add("File");
                itemName.Add(item);
            }
        }

        // кнопка выбора нового лого для плейлиста
        private void ChooseNewPlaylistPathToLogo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                TextBoxNewPlaylistPathToLogo.Text = dialog.FileName;
            }
        }

        // кнопка создания нового плейлиста
        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Playlist playlistTemp = new Playlist();
            playlistTemp.Name = TextBoxNewPlaylist.Text;
            TextBoxNewPlaylist.Text = "";

            if (TextBoxNewPlaylistPathToLogo.Text != "")
            {
                string path = @"img//" + playlistTemp.Name + "-" + DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss") + ".png";
                File.Copy(TextBoxNewPlaylistPathToLogo.Text, path);
                playlistTemp.LogoPath = path;
                TextBoxNewPlaylistPathToLogo.Text = "";
            }
            else
            {
                playlistTemp.LogoPath = "";
            }

            playlists.Add(playlistTemp);

            UpdateComboBoxPlaylists();
        }

        // кнопка проигрывания выбранной музыки из выбранной папки
        private void OpenMusicFile_Click(object sender, RoutedEventArgs e)
        {
            if(itemCommand.Count == 0)
            {
                MessageBox.Show("Выберите корневую папку с музыкой и нажмите \"Обновить\".");
            }
            else
            {
                if(FilesFromRootMusicDir.SelectedIndex == -1)
                {
                    if (ListBoxPlaylistMusics.SelectedIndex == -1)
                    {
                        MessageBox.Show("Выберите файл музыки.");
                        return;
                    }
                }
                if (FilesFromRootMusicDir.SelectedIndex != -1)
                {
                    if(itemCommand[FilesFromRootMusicDir.SelectedIndex] == "File")
                    {
                        mediaPlayer.Open(new Uri(itemName[FilesFromRootMusicDir.SelectedIndex]));
                    }
                    else
                    {
                        MessageBox.Show("Выберите файл музыки.");
                        return;
                    }
                }
                if (ListBoxPlaylistMusics.SelectedIndex != -1)
                {
                    mediaPlayer.Open(new Uri(playlists[selectedPlaylistNum].GetMusic(ListBoxPlaylistMusics.SelectedIndex).Path));
                }


                mediaPlayer.Play();
                while (!mediaPlayer.NaturalDuration.HasTimeSpan) { }
                sliderPositionPreView.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliderPositionPreView.Value = 0.0;
            }
        }

        // изменение позиции проигрывания выбранной музыки
        private void sliderPositionPreView_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(sliderPositionPreView.Value);
        }

        // остановка проигрывания выбранной музыки
        private void StopMusicFile_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        // кнопка обновления списка файлов и папок
        private void ButtonUpdateFilesFromRootMusicDir_Click(object sender, RoutedEventArgs e)
        {
            dirs.Clear();
            UpdateFilesFromRootMusicDir();
        }

        // двойное нажатие на элемент в списке папок и файлов
        private void FilesFromRootMusicDir_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // переход на предыдущую папку
            if (itemCommand[FilesFromRootMusicDir.SelectedIndex] == "BackDirectory")
            {
                dirs.RemoveAt(dirs.Count - 1);
                UpdateFilesFromRootMusicDir();
            }
            // переход на следующую папку
            else if (itemCommand[FilesFromRootMusicDir.SelectedIndex] == "Directory")
            {
                dirs.Add(itemName[FilesFromRootMusicDir.SelectedIndex]);
                UpdateFilesFromRootMusicDir();
            }
        }

        // порядковый номер выбранного плейлиста
        public int selectedPlaylistNum;

        private void ComboBoxPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPlaylistNum = ComboBoxPlaylists.SelectedIndex;
            if(ComboBoxPlaylists.SelectedIndex != -1)
                UpdateListBoxPlaylistMusics(selectedPlaylistNum);
        }

        // добавление выбранной музыки из списка файлов и папок
        // в выбранный плейлист
        private void SelectedMusicAddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (itemCommand.Count == 0)
            {
                MessageBox.Show("Выберите корневую папку с музыкой и нажмите \"Обновить\".");
            }
            else
            {
                if (FilesFromRootMusicDir.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите файл музыки.");
                }
                else if (ComboBoxPlaylists.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите плейлист.");
                }
                else if (itemCommand[FilesFromRootMusicDir.SelectedIndex] == "File")
                {
                    Music musicTemp = new Music();
                    musicTemp.Path = itemName[FilesFromRootMusicDir.SelectedIndex];
                    musicTemp.Name = System.IO.Path.GetFileName(musicTemp.Path);

                    playlists[selectedPlaylistNum].AddMusic(musicTemp);
                    UpdateListBoxPlaylistMusics(selectedPlaylistNum);
                }
                else
                {
                    MessageBox.Show("Выберите файл музыки.");
                }
            }
        }

        // удаление выбранной музыки из плейлиста
        private void ButtonDeleteMusicFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if(ListBoxPlaylistMusics.SelectedIndex != -1)
            {
                int num = ListBoxPlaylistMusics.SelectedIndex;
                playlists[selectedPlaylistNum].RemoveMusic(ListBoxPlaylistMusics.SelectedIndex);
                UpdateListBoxPlaylistMusics(selectedPlaylistNum);
                if(num <= ListBoxPlaylistMusics.Items.Count - 1)
                {
                    ListBoxPlaylistMusics.SelectedIndex = num;
                }
                else
                {
                    ListBoxPlaylistMusics.SelectedIndex = ListBoxPlaylistMusics.Items.Count - 1;
                }
            }
            else
            {
                MessageBox.Show("Музыка в плейлисте не выбрана.");
            }
        }

        // если выбирается файл из списка файлов и папок
        // убирается фокус на элементе в списке музыки выбранного плейлиста
        private void FilesFromRootMusicDir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FilesFromRootMusicDir.SelectedIndex != -1)
                ListBoxPlaylistMusics.SelectedIndex = -1;
        }

        // если выбирается файл из списка музыки выбранного плейлиста
        // убирается фокус на элементе в списке файлов и папок
        private void ListBoxPlaylistMusics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxPlaylistMusics.SelectedIndex != -1)
                FilesFromRootMusicDir.SelectedIndex = -1;
        }

        // кнопка изменения имени плейлиста
        private void ButtonChangeSelectedPlaylistName_Click(object sender, RoutedEventArgs e)
        {
            playlists[selectedPlaylistNum].Name = TextBoxNewNameForSelectedPlaylist.Text;
            UpdateComboBoxPlaylists();
            ComboBoxPlaylists.SelectedIndex = selectedPlaylistNum;
            TextBoxNewNameForSelectedPlaylist.Text = "";
        }

        // изменение громкости выбранной музыки
        private void SliderTempMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = SliderTempMusic.Value/100;
        }

        // выбор нового лого для созданного плейлиста
        private void ButtonNewLogoForSelectedPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                NewLogoPathForSelectedPlaylist.Text = dialog.FileName;
            }
        }

        // изменение/добавление лого для созданного плейлиста
        private void ButtonChangeLogoToSelectedPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (NewLogoPathForSelectedPlaylist.Text != "")
            {
                if (playlists[selectedPlaylistNum].LogoPath != "")
                {
                    File.Delete(playlists[selectedPlaylistNum].LogoPath);
                }

                string path = @"img//" + playlists[selectedPlaylistNum].Name + "-" + DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss") + ".png";
                File.Copy(NewLogoPathForSelectedPlaylist.Text, path);
                playlists[selectedPlaylistNum].LogoPath = path;
                NewLogoPathForSelectedPlaylist.Text = "";

                MessageBox.Show("Лого плейлиста изменен.");
            }
        }

        // удаление плейлиста
        private void ButtonDeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            playlists[ComboBoxPlaylists.SelectedIndex].Destroy();
            playlists.RemoveAt(ComboBoxPlaylists.SelectedIndex);
            UpdateComboBoxPlaylists();
            ListBoxPlaylistMusics.Items.Clear();
        }

        // перемещение файла музыки в плейлисте выше
        private void ButtonMusicUp_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxPlaylistMusics.SelectedIndex != -1)
            {
                if (ListBoxPlaylistMusics.SelectedIndex != 0)
                {
                    playlists[ComboBoxPlaylists.SelectedIndex].MusicUp(ListBoxPlaylistMusics.SelectedIndex);
                    int num = ListBoxPlaylistMusics.SelectedIndex - 1;
                    UpdateListBoxPlaylistMusics(ComboBoxPlaylists.SelectedIndex);
                    ListBoxPlaylistMusics.SelectedIndex = num;
                }
            }

        }

        // перемещение файла музыки в плейлисте ниже
        private void ButtonMusicDown_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxPlaylistMusics.SelectedIndex != -1)
            {
                if (ListBoxPlaylistMusics.SelectedIndex != ListBoxPlaylistMusics.Items.Count - 1)
                {
                    playlists[ComboBoxPlaylists.SelectedIndex].MusicDown(ListBoxPlaylistMusics.SelectedIndex);
                    int num = ListBoxPlaylistMusics.SelectedIndex + 1;
                    UpdateListBoxPlaylistMusics(ComboBoxPlaylists.SelectedIndex);
                    ListBoxPlaylistMusics.SelectedIndex = num;
                }
            }
        }

        // сохранение плейлистов в файл
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            //здесь функция сохранения
            sm.Playlists = playlists;
            sm.SavePlaylists();
        }

        //////////////////////////////////////////////////////
        /// <summary>
        /// Радио
        /// </summary>
        //////////////////////////////////////////////////////

        // текущий плейлист
        private Playlist playlistTemp;
        // общая продолжительность выбранной музыки
        // для отображения в проигрывателе
        private string musicTime = "";

        // перевод из секунд в форму для отображения в проигрывателе
        public string SecondsToString(int seconds)
        {
            int totalSec = seconds;
            int min = totalSec / 60;
            int sec = totalSec - min * 60;
            string secStr;
            if (sec < 10)
                secStr = "0" + sec.ToString();
            else
                secStr = sec.ToString();
            return min.ToString() + ":" + secStr;
        }

        // функция для проигрывания музыки
        public void PlayMusic(int num)
        {
            string name = playlistTemp.GetMusic(num).Name;
            string path = playlistTemp.GetMusic(num).Path;
            if (!File.Exists(path))
            {
                // файла нет по указанному пути
                bool isFind = false;
                // поиск файла по имени по корневой папке и её подпапках
                if (findingMusic)
                {
                    string[] paths = Directory.GetFiles(rootPathDirMusic, "", SearchOption.AllDirectories);
                    foreach (var pathTemp in paths)
                    {
                        if (System.IO.Path.GetFileName(pathTemp) == name)
                        {
                            Music music = playlistTemp.GetMusic(num);
                            music.Path = pathTemp;
                            path = pathTemp;
                            playlistTemp.ChangeMusic(num, music);
                            isFind = true;
                            break;
                        }
                    }
                }
                if(!isFind)
                {
                    LabelMusicName.Content = "Файл не найден";
                    sliderMusic.Value = 0.0;
                    LabelMusicTime.Content = "0:00 / 0:00";
                    MessageBox.Show("Файл не найден в корневой папке и её подпапках.");
                    return;
                }
            }
            mediaPlayer.Open(new Uri(path));
            while (!mediaPlayer.NaturalDuration.HasTimeSpan) { }
            sliderMusic.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            sliderMusic.Value = 0.0;
            LabelMusicName.Content = name;
            musicTime = SecondsToString((int)mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
            LabelMusicTime.Content = "0:00 / " + musicTime;
        }

        // выбор плейлиста из выпадающего списка
        private void ComboBoxChoosePlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxChoosePlaylist.SelectedIndex != -1)
            {
                playlistTemp = playlists[ComboBoxChoosePlaylist.SelectedIndex];
                LabelPlaylistName.Content = playlistTemp.Name;
                if(playlistTemp.LogoPath != "")
                {
                    PlaylistLogo.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//" + playlistTemp.LogoPath, UriKind.Absolute));
                }
                else
                {
                    PlaylistLogo.Source = null;
                }
                if(playlistTemp.GetCountMusics() == 0)
                {
                    LabelMusicName.Content = "<В плейлисте нет музыки>";
                    MessageBox.Show("В плейлисте нет музыки.");
                    return;
                }
                PlayMusic(0);
                mediaPlayer.Play();
                numMusic = 0;
            }
        }

        // изменение громкости проигрывателя
        private void sliderMusicVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = sliderMusicVolume.Value / 100;
        }

        // изменение положения проигрывания выбранной музыки
        private void sliderMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LabelMusicTime.Content = SecondsToString((int)sliderMusic.Value) + " / " + musicTime;
            mediaPlayer.Position = TimeSpan.FromSeconds(sliderMusic.Value);
        }

        // таймер для автоматического изменения текущей музыки
        // и динамичного отображения текущего положения проигрывания
        // выбранной музыки
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LabelMusicTime.Content = SecondsToString((int)mediaPlayer.Position.TotalSeconds) + " / " + musicTime;
            sliderMusic.Value = mediaPlayer.Position.TotalSeconds;

            // переход на следующую песню если закончилась текущая
            if(mediaPlayer.NaturalDuration.HasTimeSpan)
                if((int)mediaPlayer.Position.TotalSeconds == (int)mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds)
                {
                    UpdateOrderPlaying();
                }
        }

        // кнопка для проигрывания музыки
        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (playlistTemp == null)
            {
                MessageBox.Show("Плейлист не выбран");
                return;
            }
            if (playlistTemp.GetCountMusics() == 0)
            {
                MessageBox.Show("Плейлист пуст");
                return;
            }
            mediaPlayer.Position = TimeSpan.FromSeconds(sliderMusic.Value);
            mediaPlayer.Play();
        }

        // кнопка паузы
        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (playlistTemp == null)
            {
                MessageBox.Show("Плейлист не выбран");
                return;
            }
            if (playlistTemp.GetCountMusics() == 0)
            {
                MessageBox.Show("Плейлист пуст");
                return;
            }
            mediaPlayer.Pause();
        }


        // режим проигрывания плейлиста
        private string reqime = "standart";

        private int numMusic = 0;

        // режим стандартного проигрывания по порядку в плейлисте
        private void RadioButtonRegimeStandart_Checked(object sender, RoutedEventArgs e)
        {
            reqime = "standart";
        }

        // режим проигрывания файлов музыки из плейлиста в случайном порядке
        private void RadioButtonRegimeRandom_Checked(object sender, RoutedEventArgs e)
        {
            reqime = "random";
        }

        // режим цикличного проигрывания выбранной музыки из плейлиста
        private void RadioButtonRegimeOneCycle_Checked(object sender, RoutedEventArgs e)
        {
            reqime = "onecycle";
        }


        // выбор файла музыки в плейлисте
        // после окончания проигрывания текущей музыки
        // или при нажатии кнопки ">>"
        private void UpdateOrderPlaying()
        {
            switch (reqime)
            {
                case "standart":
                    numMusic++;
                    if (numMusic >= playlistTemp.GetCountMusics())
                        numMusic = 0;
                    break;
                case "random":
                    Random r = new Random();
                    numMusic = r.Next(0, playlistTemp.GetCountMusics());
                    break;
                case "onecycle":
                    mediaPlayer.Position = TimeSpan.FromSeconds(0);
                    break;
            }
            PlayMusic(numMusic);
            mediaPlayer.Play();
        }
        
        // кнопка "<<"
        private void ButtonLastMusic_Click(object sender, RoutedEventArgs e)
        {
            if(playlistTemp.GetCountMusics() == 0)
            {
                return;
            }
            // если продолжительность проигрывания текущей муызки меньше 5 секунд
            // то переход на предыдущую музыку
            if(sliderMusic.Value < 5)
            {
                switch (reqime)
                {
                    case "standart":
                        numMusic--;
                        if (numMusic < 0)
                            numMusic = playlistTemp.GetCountMusics();
                        break;
                    case "random":
                        Random r = new Random();
                        numMusic = r.Next(0, playlistTemp.GetCountMusics());
                        break;
                    case "onecycle":
                        mediaPlayer.Position = TimeSpan.FromSeconds(0);
                        break;
                }
                PlayMusic(numMusic);
                mediaPlayer.Play();
            }
            // иначе сброс продолжительности проигрывания музыки до до 0
            else
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(0);
            }
        }

        // кнопка ">>"
        private void ButtonNextMusic_Click(object sender, RoutedEventArgs e)
        {
            if(playlistTemp.GetCountMusics() != 0)
                UpdateOrderPlaying();
        }

    }
}
