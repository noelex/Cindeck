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
        private static readonly Dictionary<string, int> SongIdMap = new Dictionary<string, int> {
            { "2nd SIDE",66 },{ "Absolute NIne",512 },
            { "Angel Breeze",44 },{ "BEYOND THE STARLIGHT",523 },
            { "Bright Blue",53 },{ "DOKIDOKIリズム",17 },
            { "GOIN'!!!",21 },{ "Happy×2 Days",23 },
            { "Hotel Moonside",63 },{ "-LEGNE- 仇なす剣 光の旋律",24 },
            { "LET'S GO HAPPY!!",27 },{ "Love∞Destiny",520 },
            { "M@GIC☆",36 },{ "Memories",25 },
            { "Naked Romance",40 },{ "Nation Blue",502 },
            { "Near to You",522 },{ "Never say never",10 },
            { "Orange Sapphire",503 },{ "ØωØver!!",26 },
            { "Rockin' Emotion",59 },{ "Romantic Now",33 },
            { "S(mile)ING!",9 },{ "Shine!!",30 },
            { "Snow Wings",509 },{ "Star!!",19 },
            { "TOKIMEKIエスカレート",37 },{ "Trancing Pulse",35 },
            { "Tulip",511 },{ "Twilight Sky",28 },
            { "We're the friends!",3 },{ "You're stars shine on me",31 },
            { "アタシポンコツアンドロイド",501 },{ "アップルパイ・プリンセス",51 },
            { "あんずのうた",15 },{ "ヴィーナスシンドローム",32 },
            { "エヴリデイドリーム",52 },{ "おねだり Shall We ～？",18 },
            { "オルゴールの小箱",57 },{ "お願い！シンデレラ",1 },
            { "お散歩カメラ",64 },{ "き・ま・ぐ・れ☆Café au lait！",68 },
            { "きみにいっぱい☆",518 },{ "ゴキゲンParty Night",510 },
            { "ゴキゲンParty Night cool version",47 },{ "ゴキゲンParty Night cute version",46 },
            { "ゴキゲンParty Night passion version",48 },{ "サマカニ！！",521 },
            { "ショコラ・ティアラ",34 },{ "ススメ☆オトメ ～jewel parade～",8 },
            { "ススメ☆オトメ ～jewel parade～ cool version",6 },{ "ススメ☆オトメ ～jewel parade～ cute version",5 },
            { "ススメ☆オトメ ～jewel parade～ passion version",7 },{ "つぼみ",0 },
            { "できたてEvo! Revo! Generation!",22 },{ "とどけ！アイドル",29 },
            { "ハイファイ☆デイズ",513 },{ "パステルピンクな恋",505 },
            { "ましゅまろ☆キッス",14 },{ "ミツボシ☆☆★",11 },
            { "メッセージ",4 },{ "メルヘンデビュー！",65 },
            { "ラブレター",524 },{ "花簪 HANAKANZASHI",60 },
            { "華蕾夢ミル狂詩曲～魂ノ導～",13 },{ "輝く世界の魔法",2 },
            { "咲いてJewel",0 },{ "純情Midnight伝説",0 },
            { "生存本能ヴァルキュリア",515 },{ "絶対特権主張しますっ！",507 },
            { "毒茸伝説",70 },{ "熱血乙女A",67 },
            { "薄荷 -ハッカ-",69 },{ "秘密のトワレ",71 },
            { "風色メロディ",12 },{ "夢色ハーモニー",504 },
            { "明日また会えるよね",516 },{ "夕映えプレゼント",20 },
            { "流れ星キセキ",508 }
        };
        private static readonly Dictionary<SongDifficulty, int> DifficultyIndex = new Dictionary<SongDifficulty, int> {
            {SongDifficulty.Debut,1 },
            {SongDifficulty.Regular,2 },
            {SongDifficulty.Pro,3 },
            {SongDifficulty.Master,4 },
            {SongDifficulty.MasterPlus,5 },
        };

        public async Task<Note[]> GetPattern(Song s, SongDifficulty d)
        {
            if (!SongIdMap.ContainsKey(s.Title))
            {
                return null;
            }

            var serializer = new DataContractJsonSerializer(typeof(Note[]));
            var id = $"{SongIdMap[s.Title]:d3}_{DifficultyIndex[d]}";
            string idHash;

            using (var hasher = SHA256.Create())
            {
                idHash = string.Join("", hasher.ComputeHash(Encoding.UTF8.GetBytes(id)).Select(x => x.ToString("x2")));
            }

            try
            {
                using (var fs = File.OpenRead($"data/patterns/{idHash}"))
                {
                    return ((Note[])serializer.ReadObject(fs)).Where(x => x.Type != 91 && x.Type != 92 && x.Type != 100).ToArray();
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
                        return ((Note[])serializer.ReadObject(stream)).Where(x => x.Type != 91 && x.Type != 92 && x.Type != 100).ToArray();
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
