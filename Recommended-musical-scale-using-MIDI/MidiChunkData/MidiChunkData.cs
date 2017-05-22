
using MyMusic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m
{
    public class MidiChunkData
    {
        public string midiName { get; private set; }
        public MidiHeaderChunkData header { get; private set; }
        public List<MidiTrackChunkData> trackList { get; private set; }

        /*****************************************미디분석*****************************************/
        public MidiChunkData(string filepath)
        {
            int startindex = filepath.LastIndexOf('\\') + 1;
            int lastindex = filepath.LastIndexOf('.');
            midiName = new string(filepath.ToCharArray(startindex, lastindex - startindex));

            trackList = new List<MidiTrackChunkData>();
            BinaryReader br = new BinaryReader(new FileStream(filepath, FileMode.Open));
            header = new MidiHeaderChunkData(br);
            for (int i = 0; i < header.trackcount; i++)
            {
                trackList.Add(new MidiTrackChunkData(br, header.timedivision, i));
            }
            br.Close();
        }
        /*****************************************미디생성*****************************************/
        public MidiChunkData(MusicalTrack mt, string midiName)
        {
            trackList = new List<MidiTrackChunkData>();
            header = new MidiHeaderChunkData(2);
            this.midiName = midiName;
            trackList.Add(new MidiTrackChunkData(mt.musicInfo, header.timedivision, 0, midiName));
            trackList.Add(new MidiTrackChunkData(mt, header.timedivision, 1));
        }

        public MidiChunkData(List<MusicalTrack> mtList, string midiName)
        {
            trackList = new List<MidiTrackChunkData>();
            header = new MidiHeaderChunkData(mtList.Count + 1);
            this.midiName = midiName;
            int trackCount = 0;
            trackList.Add(new MidiTrackChunkData(mtList[0].musicInfo, header.timedivision, trackCount, midiName));

            foreach (MusicalTrack mt in mtList)
            {
                trackCount++;
                trackList.Add(new MidiTrackChunkData(mt, header.timedivision, trackCount));
            }
        }
        /*****************************************미디저장*****************************************/
        public void SaveMidiFile(string filepath)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(filepath, FileMode.Create));
            header.SaveMidiFile(bw);
            foreach (MidiTrackChunkData mtcd in trackList)
            {

                mtcd.SaveMidiFile(bw);
            }
            bw.Close();

        }
        /******************************************************************************************/
        public Dictionary<int, List<int>> GetChList()
        {
            Dictionary<int, List<int>> TrackCh = new Dictionary<int, List<int>>();

            foreach (MidiTrackChunkData mtcd in trackList)
            {
                if (mtcd.chList != null && mtcd.chList.Count != 0)
                {
                    TrackCh[mtcd.trackNum] = mtcd.chList;
                }
            }
            return TrackCh;
        }
       
        //public void ModifyMidiname(string midiname)
        //{
        //    this.midiName = midiname;
        //}
        //public void ModifyMetaEvent(string metaEventType, string strData)
        //{
        //    foreach (MidiTrackChunkData track in trackList)
        //    {
        //        track.RemoveMetaEventType(metaEventType);
        //    }
        //    trackList[0].AddMetaEvent(metaEventType, strData);
        //}
        //public void ModifyMetaEvent(string metaEventType, string strData, int trackNum)
        //{
        //    trackList[trackNum].RemoveMetaEventType(metaEventType);
        //    trackList[trackNum].AddMetaEvent(metaEventType, strData);
        //}

        public List<MusicalTrack> GetMusicalTrackList()
        {
            List<MusicalTrack> mtList = new List<MusicalTrack>();
            foreach (MidiTrackChunkData track in trackList)
            {
                if (track.musicalTrack != null)
                {
                    mtList.Add(track.musicalTrack);
                }
            }
            return mtList;
        }

        /// <summary>
        /// 미디파일 이름변경
        /// </summary>
        /// <param name="midiname"></param>
        public void ChangeMidiName(string midiname)
        {
            this.midiName = midiname;
         
            
        }
        /// <summary>
        /// 노트음 변경
        /// </summary>
        /// <param name="trackNum">트랙선택</param>
        /// <param name="noteNum">노트cnt</param>
        /// <param name="scale">음계 MusicScale</param>
        public void ChangeNoteScale(int trackNum, int noteNum, MusicScale scale)
        {
            trackList[trackNum].ChangeNoteScale(noteNum, scale);
        }
        /// <summary>
        /// 노트 박자변경
        /// </summary>
        /// <param name="trackNum">트랙선택</param>
        /// <param name="noteNum">노트cnt</param>
        /// <param name="beat">MusicBeat</param>
        public void ChangeNoteBeat(int trackNum, int noteNum, MusicBeat beat)
        {
            trackList[trackNum].ChangeNoteBeat(noteNum, beat);
        }

        public void ChangeNoteBeat2(int trackNum, int noteNum, MusicBeat beat, bool afterNotesPull)
        {
            trackList[trackNum].ChangeNoteBeat2(noteNum, beat, afterNotesPull);
        }
        /// <summary>
        /// 노트제거
        /// </summary>
        /// <param name="trackNum">트랙선택</param>
        /// <param name="noteNum">노트cnt</param>
        /// <param name="afterNotesPull">악보의eof</param>
        public void RemoveNote(int trackNum, int noteNum, bool afterNotesPull)
        {
            trackList[trackNum].RemoveNote(noteNum, afterNotesPull);
        }

        /// <summary>
        /// 트렉추가
        /// </summary>
        /// <param name="trackName">트랙이름</param>
        /// <param name="InstType">악기타입</param>
        /// <param name="staffType">스템포타입</param>
        public void AddNewTrack(string trackName, int InstType, int staffType)
        {

            MusicalTrack mt = new MusicalTrack(trackName);

            mt.musicInfo.tempoList = MidiTrackChunkData.metaEventInfo.tempoList;
            mt.musicInfo.timeSignatureList = MidiTrackChunkData.metaEventInfo.timeSignatureList;
            mt.musicInfo.key = MidiTrackChunkData.metaEventInfo.GetKeySignatureDataAnalsis();
            mt.instType = InstType;
            mt.musicInfo.staffType = staffType;

            trackList.Add(new MidiTrackChunkData(mt, header.timedivision, trackList.Count));
            header.TrackCountIncrease();

        }
        /// <summary>
        /// 트렉삭제
        /// </summary>
        /// <param name="trackNum">트렉cnt</param>
        public void RemoveTrack(int trackNum)
        {
            trackList.RemoveAt(trackNum);

            List<int> removechlit = new List<int>();
            foreach (KeyValuePair<int, List<int>> pair in MidiTrackChunkData.ChDic)
            {
                foreach (int track in pair.Value)
                {
                    if (track == trackNum)
                    {
                        removechlit.Add(pair.Key);
                    }
                }
            }

            foreach (int ch in removechlit)
            {
                MidiTrackChunkData.ChDic[ch].Remove(trackNum);
            }


            foreach (MidiTrackChunkData track in trackList)
            {
                if (track.trackNum > trackNum)
                {
                    track.trackNum--;
                }
            }

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < MidiTrackChunkData.ChDic[i].Count; j++)
                {
                    if (MidiTrackChunkData.ChDic[i][j] > trackNum)
                    {
                        MidiTrackChunkData.ChDic[i][j] = MidiTrackChunkData.ChDic[i][j] - 1;
                    }
                }
            }

            header.TrackCountDecrease();

            string[] newTrackNames = new string[header.trackcount];

            for (int i = 0; i < MidiTrackChunkData.metaEventInfo.trackNames.Length; i++)
            {
                if (i < trackNum)
                {
                    newTrackNames[i] = MidiTrackChunkData.metaEventInfo.trackNames[i];
                }
                else if (i > trackNum)
                {
                    newTrackNames[i - 1] = MidiTrackChunkData.metaEventInfo.trackNames[i];
                }
            }
            MidiTrackChunkData.metaEventInfo.trackNames = newTrackNames;

        }
    }
}
