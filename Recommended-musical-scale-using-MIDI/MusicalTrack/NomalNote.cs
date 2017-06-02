using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    public class NomalNote:Note
    {
        public static int noteCount = 0;

        public int noteNum { get; set; }
        public MusicBeat beat{ get; set; }     //32분음표의 개수(32 = 1 / 16 = 2 / 16. = 3 / 8 = 4 / 8. = 6 / 4 = 8 / 4. = 12 / 2 = 16 / 2. = 24 / 1 = 32)
        public MusicScale scale { get; set; }          //음 높이 (... / 60 = 5Octave도 / 61 = 5Octave도# / ...)
        public bool afterNoteTie { get; set; }   //뒤 음표와 붙임줄 여부

        public NomalNote(int noteNum, MusicBeat beat, MusicScale scale, bool afterNoteTie = false)
        {

            this.noteNum = noteNum;
            this.beat = beat;
            this.scale = scale;
            this.afterNoteTie = afterNoteTie;

            if (noteNum >= noteCount)
            {
                noteCount = noteNum+1;
            }
        }

        public NomalNote(MusicBeat beat, MusicScale scale, bool afterNoteTie = false)
        {
            this.noteNum = noteCount;
            this.beat = beat;
            this.scale = scale;
            this.afterNoteTie = afterNoteTie;
            noteCount++;
        }

        public override string ToString()
        {
            int oct = (int)scale/12;
            int note = (int)scale % 12;
            //string notestr = string.Empty;
            ////디버깅용
            //switch (note)
            //            {
            //                case 0: notestr = "도"; break;
            //                case 1: notestr = "도#"; break;
            //                case 2: notestr = "레"; break;
            //                case 3: notestr = "레#"; break;
            //                case 4: notestr = "미"; break;
            //                case 5: notestr = "파"; break;
            //                case 6: notestr = "파#"; break;
            //                case 7: notestr = "솔"; break;
            //                case 8: notestr = "솔#"; break;
            //                case 9: notestr = "라"; break;
            //                case 10: notestr = "라#"; break;
            //                case 11: notestr = "시"; break;
            //            }


            return scale + " Term(" + GetNoteTime((int)beat) + ")";
        }

        
    }
}
