using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [Flags]
    public enum SongDifficulty
    {
        Debut=0x01,
        Regular=0x02,
        Pro=0x04,
        Master=0x08,
        MasterPlus=0x10,
    }

    [DataContract]
    public class SongData
    {
        public SongData(SongDifficulty difficulty, int level, int notes, int duration)
        {
            Difficulty = difficulty;
            Level = level;
            Notes = notes;
            Duration = duration;
        }

        [DataMember]
        public SongDifficulty Difficulty
        {
            get; private set;
        }

        [DataMember]
        public int Level
        {
            get; private set;
        }

        [DataMember]
        public int Notes
        {
            get; private set;
        }

        [DataMember]
        public int Duration
        {
            get; private set;
        }
    }

    [DataContract]
    public class Song
    {
        public Song(string title, IdolCategory type, params SongData[] data)
        {
            Title = title;
            Data = data == null ? new Dictionary<SongDifficulty, SongData>() : data.ToDictionary(x => x.Difficulty);
            Type = type;
        }

        [DataMember]
        public Dictionary<SongDifficulty, SongData> Data
        {
            get;
            set;
        }

        [DataMember]
        public string Title
        {
            get; private set;
        }

        [DataMember]
        public IdolCategory Type
        {
            get; private set;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
