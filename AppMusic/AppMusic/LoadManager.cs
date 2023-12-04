using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace AppMusic
{
    class LoadManager
    {
        // путь к корневой папке с музыкой
        public string Path { get; set; }
        // флажок для функции поиска файла, если его нет по указанному пути
        public bool FindingMusic { get; set; }

        public LoadManager()
        {
            loadSettings();
            Playlists = loadPlaylists();
        }

        // загрузка настроек
        private void loadSettings()
        {
            string path = "data\\settings.txt";

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    Path = reader.ReadLine();
                    FindingMusic = reader.ReadLine()=="1"?true:false;
                }
            }
            else
            {
                Path = "";
            }
        }

        // список плейлистов
        public List<Playlist> Playlists { get; set; }

        // загрузка плейлистов
        private List<Playlist> loadPlaylists()
        {
            List<Playlist> playlists = new List<Playlist>();

            string path = "data\\playlists.txt";

            using (StreamReader reader = new StreamReader(path))
            {
                int countPlaylists = int.Parse(reader.ReadLine());
                for(int i=0; i<countPlaylists; i++)
                {
                    Playlist playlistTemp = new Playlist();
                    playlistTemp.Name = reader.ReadLine();
                    playlistTemp.LogoPath = reader.ReadLine();
                    int countMusics = int.Parse(reader.ReadLine());
                    for (int j = 0; j < countMusics; j++)
                    {
                        Music musicTemp = new Music();
                        musicTemp.Name = reader.ReadLine();
                        musicTemp.Path = reader.ReadLine();
                        playlistTemp.AddMusic(musicTemp);
                    }
                    playlists.Add(playlistTemp);
                }
            }

            return playlists;
        }
    }
}
