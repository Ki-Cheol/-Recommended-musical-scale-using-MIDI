using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicalTrackLib;

namespace MidiChunkDataLib
{
    public class Event
    {
        public string type { get; set; }
        public int trackNum { get; set; }

        //시간정보
        public int time { get; set; }
        public float frame { get; set; }
        public StaffPosition staffPosition { get; set; }
        public int position { get; set; }    //위치 (타임상의 위치)
    }

    public class MetaEvent : Event
    {
        public int[] msg { get; set; }
        public int dataLen { get; set; }
        public byte[] data { get; set; }

        public MetaEvent()
        {
            msg = new int[2];
            msg[0] = 255;
        }
    }

    public class MidiEvent : Event
    {
        public int noteEventNum { get; set; }

        public int msg { get; set; }
        public int ch { get; set; }
        public int data1 { get; set; }
        public int data2 { get; set; }
        public int term { get; set; }   //Next Same Note's time (다음 같은 채널의 같은 음 사이의 타임 텀)

        public int GetWinmmMsg()
        {
            int buf1 = data1 << 8;
            int buf2 = data2 << 16;

            return buf1 + buf2 + msg + ch;
        }
    }
}
