using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppMusic
{
    class SaveManager
    {
        // путь к корневой папке с музыкой
        public string Path { get; set; }
        // флажок для функции поиска файла, если его нет по указанному пути
        public bool FindingMusic { get; set; }

        // сохранение настроек
        public void SaveSettings()
        {
            string path = "data\\settings.txt";

            // полная перезапись файла 
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(Path);
                writer.WriteLine(FindingMusic?"1":"0");
            }
        }

        // список плейлистов
        public List<Playlist> Playlists { get; set; }

        // сохранение плейлистов
        public void SavePlaylists()
        {
            string path = "data\\playlists.txt";

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.Write(Playlists.Count.ToString()); writer.WriteLine();
                foreach (Playlist playlist in Playlists)
                {
                    writer.Write(playlist.Name); writer.WriteLine();
                    writer.Write(playlist.LogoPath); writer.WriteLine();
                    writer.Write(playlist.GetCountMusics().ToString()); writer.WriteLine();
                    foreach (Music music in playlist.GetMusics())
                    {
                        writer.Write(music.Name); writer.WriteLine();
                        writer.Write(music.Path); writer.WriteLine();
                    }
                }
            }
        }
    }
}
