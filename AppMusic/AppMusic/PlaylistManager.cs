using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppMusic
{
    class Music
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public Music()
        {
        }
    }

    class Playlist
    {
        public string Name { get; set; }
        public string LogoPath { get; set; }
        private List<Music> musics;

        public Playlist()
        {
            musics = new List<Music>();
        }

        public int GetCountMusics()
        {
            return musics.Count;
        }

        public void AddMusic(Music music)
        {
            musics.Add(music);
        }

        public List<Music> GetMusics()
        {
            return musics;
        }

        public Music GetMusic(int num)
        {
            return musics[num];
        }

        public void RemoveMusic(int num)
        {
            musics.RemoveAt(num);
        }

        public void Destroy()
        {
            if(LogoPath != "")
                File.Delete(LogoPath);
        }

        public void ChangeMusic(int num, Music music)
        {
            musics[num] = music;
        }

        public void MusicUp(int num)
        {
            Music musicTemp;
            musicTemp = musics[num];
            musics[num] = musics[num - 1];
            musics[num - 1] = musicTemp;
        }

        public void MusicDown(int num)
        {
            Music musicTemp;
            musicTemp = musics[num];
            musics[num] = musics[num + 1];
            musics[num + 1] = musicTemp;
        }
    }

    class PlaylistManager
    {
        public PlaylistManager()
        {
        }
    }
}
