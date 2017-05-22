using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    public class Harmony : Note
    {
        public List<NomalNote> nomalNotes { get; set; }
        public Harmony(List<NomalNote> NomalNotes)
        {
            this.nomalNotes = NomalNotes;
        }
        public Harmony()
        {
            this.nomalNotes = new List<NomalNote>();
        }
        

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
