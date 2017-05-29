using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    public class MusicalTrack
    {
        public string trackName { get; set; }
        public int instType { get; set; }
        public MusicInfo musicInfo { get; set; }
        public List<Note> notelist { get; set; } 

        public MusicalTrack(string trackname)
        {
            notelist = new List<Note>();
            this.trackName = trackname;
            musicInfo = new MusicInfo();
        }

        public MusicalTrack(string name, int baseTempo, int baseTimeSigNumer, int baseTimeSigDenom , int instType)
        {
            this.instType = instType;
            notelist = new List<Note>();
            this.trackName = name;
            musicInfo = new MusicInfo(baseTempo, baseTimeSigNumer, baseTimeSigDenom);
        }

        public void AddNote(Note note)
        {
            notelist.Add(note);
        }

        public void ChangeNotelist(List<Note> notelist)
        {
            this.notelist = notelist;
        }

        public NomalNote this[int noteNum]
        {
            get
            {
                foreach(Note nt in notelist)
                {
                    if (nt is NomalNote)
                    {
                        if (((NomalNote)nt).noteNum == noteNum)
                        {
                            return (NomalNote)nt;
                        }
                    }
                    else if (nt is Harmony)
                    {
                        foreach (NomalNote nn in ((Harmony)nt).nomalNotes)
                        {
                            if (nn.noteNum == noteNum)
                            {
                                return nn;
                            }
                        }
                    }
                }
                return null;
            }
        }
        

        public override string ToString()
        {
            string buf = string.Empty;

            int count = 0;
            foreach (Note nt in notelist)
            {
                count++;
                buf += nt.ToString();
                if (count != notelist.Count)
                {
                    buf += " / ";
                }
            }











































































            return buf;
        }
    }
}
