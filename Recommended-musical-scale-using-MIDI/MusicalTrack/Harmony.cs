using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    public class Harmony : Note
    {
        public List<NomalNote> nomalNotes { get; set; }
        /// <summary>
        /// harmony노트 노트의 상속
        /// </summary>
        /// <param name="NomalNotes">노트의 리스트</param>
        public Harmony(List<NomalNote> NomalNotes)
        {
            this.nomalNotes = NomalNotes;
        }
        /// <summary>
        /// 매개변수없는 하모니  자체 NoteList 생성
        /// </summary>
        public Harmony()
        {
            this.nomalNotes = new List<NomalNote>();
        }
        
        /// <summary>
        /// 노트의 노말노트 생성 
        /// </summary>
        /// <param name="nn"></param>
        public void AddNomalNote(NomalNote nn)
        {
            nomalNotes.Add(nn);
        }

        public void BeatSort()
        {
            nomalNotes.Sort(new NoteComparer());
        }

        public override string ToString()
        {
            string buf = string.Empty;

            int count = 0;
            foreach(NomalNote nt in  nomalNotes)
            {
                count++;
                buf += nt.ToString();
                if(count!=nomalNotes.Count)
                {
                    buf += " + ";
                }
            }
            return buf;
        }
    }

    internal sealed class NoteComparer : IComparer<NomalNote>
    {
        public int Compare(NomalNote x, NomalNote y)
        {
            if (x == null || y == null)
            {
                throw new ApplicationException("NomalNote 가 아닙니다.");
            }

            if (x.beat != y.beat)
            {
                return x.beat.CompareTo(y.beat);
            }

            return x.scale.CompareTo(y.scale);
        }
    }
}
