
using MusicalTrackLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MidiChunkDataLib
{

              ////SequenceNum
              ////  Writer
              ////  Copyright
              ////  TrackName
              ////  InstrumentInfo
              ////  Lyric
              ////  Marker
              ////  CuePoint
              ////  ProgramName
              ////  DeviceName
              //// 32   MidiFreemax
              ////   MidiPort
              ////  33
              ////  47 track end  return false
              ////  81 Tempo
              ////  84 SmpteOffSet
              ////  88 TimeSignature
              ////  89 KeySignature
              ////  127 SeqEvent 
              // ... 나머진 해독중 ..
    public class MidiMetaEventInfo
    {
        public byte[] sequenceNum { get; set; }       //시퀀스번호
        public string writer { get; set; }           //작성자?
        public string copyright { get; set; }        //저작권 정보
        public string[] trackNames { get; set; }        //Track의 이름
        public string instrumentinfo { get; set; }   //악기
        public string lyric { get; set; }            //가사
        public string marker { get; set; }           //마크 ?
        public string cuePoint { get; set; }         //큐포인트 ?
        public string programname { get; set; }      //악기음색명 ?
        public string devicename { get; set; }       //디바이스명 ?
        public byte[] midifreemax { get; set; }        //프리맥스?
        public byte[] midiport { get; set; }           //미디포트 ?
        public List<Tempo> tempoList { get; set; }          //템포
        public byte[] smpteOffset { get; set; }       //smpteOffset ?

        public List<TimeSignature> timeSignatureList { get; set; }

        public byte[] keySignatureData { get; set; } //Track의 키 높이정보 (+샾 -플랫)

        public byte[] seqEvent { get; set; }

        /// <summary>
        /// 트랙수만큼 메타이벤트 인포 생성
        /// </summary>
        /// <param name="trackcount"></param>
        public MidiMetaEventInfo(int trackcount)
        {
            trackNames = new string[trackcount];
            tempoList = new List<Tempo>();
            timeSignatureList = new List<TimeSignature>();
        }

        public string GetKeySignatureDataAnalsis()
        {
            string strKeySig = string.Empty;
            if (keySignatureData == null)
            {
                return strKeySig;
            }


            if ((int)keySignatureData[0] == 0)
            {
                strKeySig = "-";
            }
            else if ((int)keySignatureData[0] < 100)
            {
                strKeySig = "#*";
                strKeySig += string.Format("{0}", (int)keySignatureData[0]);
            }
            else if ((int)keySignatureData[0] > 100)
            {
                strKeySig = "b*";
                strKeySig += string.Format("{0}", 256 - (int)keySignatureData[0]);
            }
            else
            {
                strKeySig = "?";
            }

            switch ((int)keySignatureData[1])
            {
                case 0: strKeySig += " major"; break;
                case 1: strKeySig += " minor"; break;
                default: strKeySig += " ?"; break;
            }
            return strKeySig;
        }
    }
}
