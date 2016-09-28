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
                        .Replace("？", "?")
                        .Replace("’", "'");
        }

        private Dictionary<string, SongInfo[]> ParseSongIdMap(Stream stream)
        {
            Dictionary<int, string> typeMap = new Dictionary<int, string>() {
                {1," cute version" },
                {2, " cool version" },
                {3," passion version" }
            };
            var serializer = new DataContractJsonSerializer(typeof(SongInfo[]));
            var list = new List<SongInfo>();
            var source = ((SongInfo[])serializer.ReadObject(stream));

            foreach (var song in source.OrderByDescending(x => x.Type))
            {
                if (list.Any(x => x.Title == NormalizeTitle(song.Title) && x != song && x.Type != song.Type && song.Type != 4))
                {
                    song.Title += typeMap[song.Type];
                }
                song.Title = NormalizeTitle(song.Title);
                list.Add(song);
            }

            return list.GroupBy(x => x.Title).ToDictionary(x => x.Key, x => x.ToArray());
        }

        private async Task<Dictionary<string, SongInfo[]>> GetSongIdMap(bool remote)
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

        public async Task<Note[]> GetPattern(Song s, SongDifficulty d, int expectedNotes)
        {
            var title = NormalizeTitle(s.Title);

            var songIdMap = await GetSongIdMap(false);
            if (songIdMap == null || !songIdMap.ContainsKey(title))
            {
                songIdMap = await GetSongIdMap(true);
                if (songIdMap == null || !songIdMap.ContainsKey(title))
                {
                    return null;
                }
            }

            foreach (var info in songIdMap[title])
            {
                if ((int)info.GetType().GetProperty(d.ToString()).GetValue(info) != 0)
                {
                    var id = $"{info.Id:d3}_{DifficultyIndex[d]}";
                    var pattern = await GetPattern(id);
                    if (pattern != null && expectedNotes == pattern.Length)
                    {
                        return pattern;
                    }
                }
            }

            return null;
        }

        private async Task<Note[]> GetPattern(string id)
        {
            string idHash;
            var serializer = new DataContractJsonSerializer(typeof(Note[]));

            using (var hasher = SHA256.Create())
            {
                idHash = string.Join("", hasher.ComputeHash(Encoding.UTF8.GetBytes(id)).Select(x => x.ToString("x2")));
            }

            var patternFile = $"data/patterns/{idHash}";

            try
            {
                using (var fs = File.OpenRead(patternFile))
                {
                    if (fs.Length > 0)
                    {
                        return ((Note[])serializer.ReadObject(fs)).Where(x => x.Type == 1 || x.Type == 2).ToArray();
                    }
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
                        if (data.Length > 0)
                        {
                            Directory.CreateDirectory("data/patterns");
                            File.WriteAllBytes(patternFile, data);
                        }
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