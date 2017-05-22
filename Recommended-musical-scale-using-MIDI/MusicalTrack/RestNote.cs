using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    public class RestNote:Note
    {
        public MusicBeat beat { get; set; }     //32분음표의 개수
        public bool afterNoteTie { get; set; }   //다음노트가 쉼표인지의 여부

        public RestNote(MusicBeat Count32Note, bool afterNoteTie = false)
        {
            this.afterNoteTie = afterNoteTie;
            this.beat = Count32Note;
        }
        public override string ToString()
        {
            return "RestTerm("+GetNoteTime((int)beat)+")";
        }

    }
}
