using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    

    public class MusicInfo
    {
        public List<Tempo> tempoList { get; set; }
        public string key { get; set; }
        public List<TimeSignature> timeSignatureList { get; set; }
        public int staffType { get; set; } //보표 종류 (0=높은음자리, 1=낮은음자리, 2=높은음자리+낮은음자리)

        public MusicInfo(int baseTempo = 100, int baseTimeSigNumer = 4, int baseTimeSigDenom = 4, int staffType = 0)
        {


            if (key == null || key == "")
            {
                key = "-";
            }

            if (tempoList == null)
            {
                tempoList = new List<Tempo>();

                Tempo tempo = new Tempo();
                tempo.position = 0;
                tempo.frame = 0;
                tempo.staffPosition = new StaffPosition();
                tempo.SetDataByTempo(baseTempo);
                tempoList.Add(tempo);
            }

            if (timeSignatureList == null)
            {
                timeSignatureList = new List<TimeSignature>();
                TimeSignature timeSignature = new TimeSignature();
                timeSignature.position = 0;
                timeSignature.frame = 0;
                timeSignature.staffPosition = new StaffPosition();
                timeSignature.numerator = baseTimeSigNumer;
                timeSignature.denominator = baseTimeSigDenom;
                timeSignatureList.Add(timeSignature);
            }

            this.staffType = staffType;
        }
    }
}
