using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public class PatternProvider
    {
        private static readonly Dictionary<SongDifficulty, int> DifficultyIndex = new Dictionary<SongDifficulty, int> {
            {SongDifficulty.Debut,1 },
            {SongDifficulty.Regular,2 },
            {SongDifficulty.Pro,3 },
            {SongDifficulty.Master,4 },
            {SongDifficulty.MasterPlus,5 },
        };

        private string NormalizeTitle(string title)
        {
            return title.Replace(" ", "")
                        .Replace("！", "!")
                        .Replace("？", "?");
        }

        private Dictionary<string, int> ParseSongIdMap(Stream stream)
        {
            Dictionary<int, string> typeMap = new Dictionary<int, string>() {
                {1," cute version" },
                {2, " cool version" },
                {3," passion version" }
            };
            var serializer = new DataContractJsonSerializer(typeof(SongInfo[]));
            var list = new List<SongInfo>();
            var source = ((SongInfo[])serializer.ReadObject(stream)).OrderByDescending(x => x.EventType);

            foreach (var song in source)
            {
                if (!list.Any(x => x.Title == song.Title && x.Type == song.Type && x != song))
                {
                    list.Add(song);
                }
            }

            foreach (var song in list.OrderBy(x=>x.Type))
            {
                if (list.Any(x => x.Title == song.Title && x != song && x.Type != song.Type && song.Type != 4))
                {
                    song.Title += typeMap[song.Type];
                }
                song.Title = NormalizeTitle(song.Title);
            }

            return list.ToDictionary(x => x.Title, x => x.Id);
        }

        private async Task<Dictionary<string, int>> GetSongIdMap(bool remote)
        {
            try
            {
                if (remote)
                {
                    using (var client = new WebClient())
                    {
                        var data = await client.DownloadDataTaskAsync(new Uri($"https://apiv2.deresute.info/data/live"));
                        using (var stream = new MemoryStream(data))
                        {
                            Directory.CreateDirectory("data/patterns");
                            File.WriteAllBytes($"data/patterns/index", data);
                            return ParseSongIdMap(stream);
                        }
                    }
                }
                else
                {
                    using (var fs = File.OpenRead($"data/patterns/index"))
                    {
                        return ParseSongIdMap(fs);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<Note[]> GetPattern(Song s, SongDifficulty d)
        {
            var title = NormalizeTitle(s.Title);

            var songIdMap = await GetSongIdMap(false);
            if (songIdMap==null || !songIdMap.ContainsKey(title))
            {
                songIdMap = await GetSongIdMap(true);
                if (songIdMap == null || !songIdMap.ContainsKey(title))
                {
                    return null;
                }
            }

            var serializer = new DataContractJsonSerializer(typeof(Note[]));
            var id = $"{songIdMap[title]:d3}_{DifficultyIndex[d]}";
            string idHash;

            using (var hasher = SHA256.Create())
            {
                idHash = string.Join("", hasher.ComputeHash(Encoding.UTF8.GetBytes(id)).Select(x => x.ToString("x2")));
            }

            try
            {
                using (var fs = File.OpenRead($"data/patterns/{idHash}"))
                {
                    return ((Note[])serializer.ReadObject(fs)).Where(x => x.Type == 1 || x.Type == 2).ToArray();
                }
            }
            catch
            {

            }

            try
            {
                using (var client = new WebClient())
                {
                    var data = await client.DownloadDataTaskAsync(new Uri($"https://apiv2.deresute.info/pattern/{id}"));
                    using (var stream = new MemoryStream(data))
                    {
                        Directory.CreateDirectory("data/patterns");
                        File.WriteAllBytes($"data/patterns/{idHash}", data);
                        return ((Note[])serializer.ReadObject(stream)).Where(x => x.Type == 1 || x.Type == 2).ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
