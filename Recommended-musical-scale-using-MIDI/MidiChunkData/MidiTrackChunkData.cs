using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicalTrackLib;

namespace MidiChunkDataLib
{
  public class MidiTrackChunkData
    {
        
        public int trackNum { get; set; }  //트랙번호
        public string trackName { get; private set; } //트랙이름
        public int trackSize { get; private set; } //Track의 크기
        public int instType { get; private set; } //악기 타입

        public string TrackID { get; private set; }
        public List<Event> events { get; private set; }
        public List<int> chList { get; private set; }
        public MusicalTrack musicalTrack { get; private set; }

        public static MidiMetaEventInfo metaEventInfo { get; set; } //메타이벤트
        public static Dictionary<int, List<int>> ChDic { get; set; }

        private Dictionary<string, MidiEvent> MidiNoteBufDic = new Dictionary<string, MidiEvent>();

        static int TimeDivision;// 4분음표의 타임

        public int noteEventNumBuf = 0;
        int cumulativetime = 0; //누적 타임
        float cumulativeFrames = 0;//누적 프레임
        int beforEventMsg;  //이전 이벤트의 메세지
        int beforEventCh;   //이전 이벤트의 채널        
        int lowScaleCount = 0;
        int nomalScaleCount = 0;
        int highScaleCount = 0;
        #region 생성자

        /// <summary>
        /// 생성부분
        /// </summary>
        /// <param name="bw"></param>
        public MidiTrackChunkData(BinaryWriter bw, Instrument myins=Instrument.GrandPiano)
        {
            byte[] ID = new byte[4];
            ID[0] = 77;
            ID[1] = 84;
            ID[2] = 114;
            ID[3] = 107;
            bw.Write(ID);
            instType =(int)myins;
        }
        #region 미디 생성 분석
        /// <summary>
        /// 미디생성 
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="timedivision"></param>
        /// <param name="trackCount"></param>
        /// <param name="midiName"></param>
        public MidiTrackChunkData(MusicInfo mi, int timedivision, int trackCount, string midiName)
        {
            TrackInit();

            TimeDivision = timedivision;
            this.trackNum = trackCount;

            SetTrackNameEvent(midiName);
            SetkeySignatureEvent(mi.key);
            SetTimeSignatureNTempoEvent(mi);

            musicalTrack = new MusicalTrack(midiName);
            musicalTrack.musicInfo = mi;
            this.instType = musicalTrack.instType = -1;
        }
        private void SetTrackNameEvent(string trackname)
        {
            if (metaEventInfo.trackNames.Length <= trackNum)
            {
                string[] newTrackNames = new string[trackNum + 1];

                for (int i = 0; i < metaEventInfo.trackNames.Length; i++)
                {
                    newTrackNames[i] = metaEventInfo.trackNames[i];
                }

                newTrackNames[trackNum] = trackname;

                metaEventInfo.trackNames = newTrackNames;
                this.trackName = trackname;
            }
            else
            {
                metaEventInfo.trackNames[trackNum] = trackname;
                this.trackName = trackname;
            }

            MetaEvent me = new MetaEvent();
            me.type = "TrackName";
            me.trackNum = this.trackNum;

            me.time = 0;
            me.frame = 0;
            me.staffPosition = new StaffPosition();
            me.position = 0;

            me.msg[1] = 3;
            me.dataLen = trackname.Length;
            me.data = ConvertToByteArrbyString(trackname);

            events.Add(me);
        }
        //새롭게 데이터반환
        private byte[] ConvertToByteArrbyString(string data)
        {
            byte[] buf = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                buf[i] = (byte)data[i];
            }
            return buf;
        }
        private void SetkeySignatureEvent(string key)
        {
            byte[] keydata = new byte[2];
            if (key == null || key == "")
            {
                keydata[0] = 0;
                keydata[1] = 0;
            }
            else
            {
                if (key[0] == '-')
                {
                    keydata[0] = 0;
                    if (key.Length > 2)
                    {
                        key = key.Remove(0, 2);
                    }
                }
                else if (key[0] == '#')
                {
                    keydata[0] = (byte)int.Parse(key[2].ToString());
                    if (key.Length > 4)
                    {
                        key = key.Remove(0, 4);
                    }
                }
                else if (key[0] == 'b')
                {
                    int data = 256;
                    data -= int.Parse(key[2].ToString());
                    keydata[0] = (byte)data;
                    if (key.Length > 4)
                    {
                        key = key.Remove(0, 4);
                    }
                }

                if (key == "minor")
                {
                    keydata[1] = 1;
                }
                else
                {
                    keydata[1] = 0;
                }
            }

            metaEventInfo.keySignatureData = keydata;

            MetaEvent me = new MetaEvent();
            me.type = "KeySignature";
            me.trackNum = this.trackNum;

            me.time = 0;
            me.frame = 0;

            me.staffPosition = new StaffPosition();
            me.position = 0;

            me.msg[1] = 89;
            me.dataLen = 2;
            me.data = keydata;

            events.Add(me);
        }
        private void SetTimeSignatureNTempoEvent(MusicInfo mi)
        {
            int timeCount = 0;
            if (mi.timeSignatureList != null)
            {
                timeCount = mi.timeSignatureList.Count;
            }

            int tempoCount = 0;
            if (mi.tempoList != null)
            {
                tempoCount = mi.tempoList.Count;
            }

            metaEventInfo.timeSignatureList = mi.timeSignatureList;
            metaEventInfo.tempoList = mi.tempoList;

            int timeNum = 0;
            int tempoNum = 0;

            while (timeNum < timeCount || tempoNum < tempoCount)
            {
                if (timeNum != timeCount && tempoNum != timeCount)
                {
                    if (mi.timeSignatureList[timeNum].position < mi.tempoList[tempoNum].position)
                    {
                        SetTimeSignatureEvent(mi.timeSignatureList[timeNum]);
                        timeNum++;
                    }
                    else if (mi.timeSignatureList[timeNum].position > mi.tempoList[tempoNum].position)
                    {
                        SetTempoEvent(mi.tempoList[tempoNum]);
                        tempoNum++;
                    }
                    else if (mi.timeSignatureList[timeNum].position == mi.tempoList[tempoNum].position)
                    {
                        SetTimeSignatureEvent(mi.timeSignatureList[timeNum]);
                        timeNum++;
                        SetTempoEvent(mi.tempoList[tempoNum]);
                        tempoNum++;
                    }
                }
                else if (timeNum == timeCount)
                {
                    SetTempoEvent(mi.tempoList[tempoNum]);
                    tempoNum++;
                }
                else if (tempoNum == tempoCount)
                {
                    SetTimeSignatureEvent(mi.timeSignatureList[timeNum]);
                    timeNum++;
                }

            }
        }
        private void SetTimeSignatureEvent(TimeSignature timeSignature)
        {
            MetaEvent me = new MetaEvent();
            me.type = "TimeSignature";
            me.trackNum = this.trackNum;

            me.frame = timeSignature.frame;
            me.position = timeSignature.position;
            me.staffPosition = timeSignature.staffPosition;

            me.msg[1] = 88;
            me.dataLen = 4;

            byte[] data = new byte[4];
            data[0] = (byte)timeSignature.numerator;
            data[1] = (byte)((int)Math.Log(timeSignature.denominator, 2));
            data[2] = 24;
            data[3] = 8;
            me.data = data;

            events.Add(me);
        }
        private void SetTempoEvent(Tempo tempo)
        {
            MetaEvent me = new MetaEvent();
            me.type = "Tempo";
            me.trackNum = this.trackNum;

            me.frame = tempo.frame;
            me.position = tempo.position;
            me.staffPosition = tempo.staffPosition;

            me.msg[1] = 81;
            me.dataLen = 3;


            int data = tempo.data;
            byte[] bytedata = new byte[3];
            bytedata[0] = (byte)(data / 65536);
            bytedata[1] = (byte)((data % 65536) / 256);
            bytedata[2] = (byte)((data % 65536) % 256);

            me.data = bytedata;

            events.Add(me);
        }

        #endregion
        /// <summary>
        /// 읽어오는부분
        /// </summary>
        /// <param name="br"></param>
        public MidiTrackChunkData(BinaryReader br)
        {
           
            TrackID = GetId(br);
            if(TrackID != "MTrk")
            {
                new NotFiniteNumberException("잘못된 Track파일입니다.");
                return;
            }
            trackSize = (GetLength(br));
                
        }
        #endregion
        public MidiTrackChunkData(MusicalTrack mt, int timedivision, int trackCount)
        {
            TrackInit();
            this.trackNum = trackCount;
            musicalTrack = mt;

            int ch = -1;
            foreach (KeyValuePair<int, List<int>> pair in ChDic)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    if (pair.Value[i] == this.trackNum)
                    {
                        ch = pair.Key;
                    }
                }
            }
            if (ch == -1)
            {
                ch = findNotUsedCh();
                if (ch == -1)
                {
                    throw new NotImplementedException("모든 채널 사용중");
                }
            }

            if (mt.trackName != null && mt.trackName != "")
            {
                SetTrackNameEvent(mt.trackName);
            }
            instType = mt.instType;

            TimeDivision = timedivision;
            this.trackNum = trackCount;

            MidiEvent me = new MidiEvent();
            me.type = "Programe";
            me.trackNum = this.trackNum;
            me.msg = 192;
            me.ch = ch;
            me.data1 = mt.instType;

            me.time = 0;
            me.position = 0;
            me.staffPosition = SetMidiStaffPosition();
            me.frame = SetFrame(0);

            events.Add(me);

            int beforRestNotetime = 0;

            foreach (Note nt in mt.notelist)
            {
                if (nt is NomalNote)
                {
                    NomalNote nn = nt as NomalNote;
                    SetNomalNoteEvent(nn, beforRestNotetime, ch);
                    beforRestNotetime = 0;
                }
                else if (nt is Harmony)
                {
                    Harmony hmn = nt as Harmony;
                    SetHarmonyNoteEvent(hmn, beforRestNotetime, ch);
                    beforRestNotetime = 0;
                }
                else if (nt is RestNote)
                {
                    RestNote rn = nt as RestNote;
                    beforRestNotetime += (int)(((double)TimeDivision / (double)8) * (double)rn.beat);
                    cumulativetime += (int)(((double)TimeDivision / (double)8) * (double)rn.beat);
                }
            }
            MidiNoteBufDic.Clear();
        }
        private int findNotUsedCh()
        {
            for (int i = 0; i < 16; i++)
            {
                if (i != 9 && ChDic[i].Count == 0)
                {
                    ChDic[i].Add(this.trackNum);
                    return i;
                }
            }

            return -1;
        }
        private void SetNomalNoteEvent(NomalNote nn, int beforRestNotetime, int ch)
        {
            MidiEvent meOn = new MidiEvent();
            MidiEvent meOff = new MidiEvent();

            SetMidiNoteEvent(ref meOn, ref meOff, nn, ch);

            meOn.time = beforRestNotetime;
            meOn.position = cumulativetime;
            meOn.staffPosition = SetMidiStaffPosition();
            meOn.frame = SetFrame(meOn.time);

            meOff.time = (int)(((double)TimeDivision / (double)8) * (double)nn.beat);
            cumulativetime += meOff.time;
            meOff.position = cumulativetime;
            meOff.staffPosition = SetMidiStaffPosition();
            meOff.frame = SetFrame(meOff.time);

            GetTerm(meOn, true);
            GetTerm(meOff, true);
        }

        private void SetHarmonyNoteEvent(Harmony hmn, int beforRestNotetime, int ch)
        {
            int noteCount = hmn.nomalNotes.Count;
            MidiEvent[] meOn = new MidiEvent[noteCount];
            MidiEvent[] meOff = new MidiEvent[noteCount];

            hmn.BeatSort();

            for (int i = 0; i < noteCount; i++)
            {
                meOn[i] = new MidiEvent();
                meOff[i] = new MidiEvent();
                SetMidiNoteEvent(ref meOn[i], ref meOff[i], hmn.nomalNotes[i], ch);
            }

            for (int i = 0; i < noteCount; i++)
            {
                meOn[i].time = 0;
                if (i == 0)
                {
                    meOn[i].time = beforRestNotetime;
                }
                meOn[i].position = cumulativetime;
                meOn[i].staffPosition = SetMidiStaffPosition();
                meOn[i].frame = SetFrame(meOn[i].time);
                GetTerm(meOn[i], true);
            }


            if (hmn.nomalNotes[0].beat == hmn.nomalNotes[noteCount - 1].beat)
            {
                for (int i = 0; i < noteCount; i++)
                {
                    meOff[i].time = 0;
                    if (i == 0)
                    {
                        meOff[i].time = (int)(((double)TimeDivision / (double)8) * (double)hmn.nomalNotes[i].beat);
                        cumulativetime += meOff[i].time;
                    }
                    meOff[i].position = cumulativetime;
                    meOff[i].staffPosition = SetMidiStaffPosition();
                    meOff[i].frame = SetFrame(meOff[i].time);
                    GetTerm(meOff[i], true);
                }
            }
            else
            {
                int beforNoteCount32Note = 0;
                for (int i = 0; i < noteCount; i++)
                {
                    if (i == 0)
                    {
                        meOff[i].time = (int)(((double)TimeDivision / (double)8) * (double)hmn.nomalNotes[i].beat);
                    }
                    else
                    {
                        meOff[i].time = (int)(((double)TimeDivision / (double)8) * (double)(hmn.nomalNotes[i].beat - beforNoteCount32Note)); ;
                    }
                    cumulativetime += meOff[i].time;
                    meOff[i].position = cumulativetime;
                    meOff[i].staffPosition = SetMidiStaffPosition();
                    meOff[i].frame = SetFrame(meOff[i].time);
                    GetTerm(meOff[i], true);
                    beforNoteCount32Note = (int)hmn.nomalNotes[i].beat;
                }
            }

        }

        private void SetMidiNoteEvent(ref MidiEvent meOn, ref MidiEvent meOff, NomalNote nn, int ch)
        {
            meOn.type = "NoteOn";
            meOff.type = "NoteOff";
            meOn.trackNum = this.trackNum;
            meOff.trackNum = this.trackNum;
            meOn.msg = 144;
            meOff.msg = 128;
            meOn.ch = ch;
            meOff.ch = ch;
            meOn.data1 = (int)nn.scale;
            meOff.data1 = (int)nn.scale;
            meOn.data2 = 120;
            meOff.data2 = 0;
            meOn.noteEventNum = nn.noteNum;
            meOff.noteEventNum = nn.noteNum;
        }
        #region 트랙분석
        /// <summary>
        ///트랙 분석생성
        /// </summary>
        /// <param name="br"></param>
        /// <param name="timedivision"></param>
        /// <paramq name="TrackCount"></param>
        public MidiTrackChunkData(BinaryReader br, int timedivision, int TrackCount)
        {
            TrackInit();
            instType = -1;
            trackNum = TrackCount;
            TimeDivision = timedivision;
            if (GetId(br) != "MTrk")
            {
                throw new NotImplementedException("트랙아이디오류");
            }
            trackSize = GetLength(br);
            TrackAnalsis(br);
            SetMusicalTrackNote();
        }
        private void TrackAnalsis(BinaryReader br)
        {
            while (true)
            {
                //델타타임분석
                int time = GetTimeFromDeltaTime(br);

                byte bt = br.ReadByte();

                if ((int)bt == 255)
                {
                    MetaEvent mtev = new MetaEvent();
                    SetTimeInfo(mtev, time);
                    if (!MetaEventAnalsis(br, mtev))
                    {
                        return;
                    }
                }
                else
                {
                    MidiEvent mdev = new MidiEvent();

                    SetTimeInfo(mdev, time);

                    MidiEventAnalsis(br, mdev, bt);

                }
            }
        }
        private void SetMusicalTrackNote() //이벤트리스트들 사이에서 음표와 쉼표 박자 정보 구하는 함수
        {
            if (musicalTrack == null)
            {
                SetMusicalTrackMi();
            }

            MidiEvent beforNote = new MidiEvent();

            int notMidiEventsTime = 0; //노트온,오프가 아닌 이벤트들의 누적 타임
            int nowNoteOnCount = 0; //음을 내고 있는 노트의 개수
            StaffPosition beforNoteOffstaffPosition = new StaffPosition();

            foreach (Event ent in events)
            {
                if (ent is MidiEvent)
                {
                    MidiEvent nt = ent as MidiEvent;
                    if (nt.msg == 144 || nt.msg == 128) //노트온&노트오프
                    {
                        if (nt.msg == 144 && nt.data2 != 0) //노트온
                        {
                            bool isharmony = false;

                            if (beforNote.msg != 128 && beforNote.data2 != 0 && nt.time == 0)   //동시음여부 판단
                            {
                                isharmony = true;
                            }

                            // 쉼표 (위치가 0이 아닌 노트온이면서 음을 내고있는 노트가 없을때)
                            if (nowNoteOnCount == 0 && nt.position != 0 && (nt.time + notMidiEventsTime) != 0) 
                            {
                                RestNote rn = new RestNote((MusicBeat)Round(Get32NoteCount(TimeDivision, nt.time + notMidiEventsTime)));
                                rn.staffPosition = beforNoteOffstaffPosition;
                                musicalTrack.notelist.Add(rn);
                                notMidiEventsTime = 0;
                            }

                            nowNoteOnCount++;

                            if (isharmony)//동시음 일때
                            {
                                if (musicalTrack.notelist[musicalTrack.notelist.Count - 1] is Harmony) //마지막으로 추가된 노트가 하모니 일경우
                                {
                                    NomalNote nn = new NomalNote(nt.noteEventNum,(MusicBeat)Round(Get32NoteCount(TimeDivision, nt.term)), (MusicScale)nt.data1);
                                    nn.staffPosition = nt.staffPosition;
                                    ((Harmony)musicalTrack.notelist[musicalTrack.notelist.Count-1]).nomalNotes.Add(nn);
                                }
                                else
                                {
                                    Harmony hmn = new Harmony();
                                    hmn.staffPosition = nt.staffPosition;
                                    NomalNote nn1 = new NomalNote(((NomalNote)musicalTrack.notelist[musicalTrack.notelist.Count - 1]).noteNum,
                                                                  ((NomalNote)musicalTrack.notelist[musicalTrack.notelist.Count - 1]).beat, 
                                                                  ((NomalNote)musicalTrack.notelist[musicalTrack.notelist.Count - 1]).scale, 
                                                                  ((NomalNote)musicalTrack.notelist[musicalTrack.notelist.Count - 1]).afterNoteTie);
                                    nn1.staffPosition = musicalTrack.notelist[musicalTrack.notelist.Count - 1].staffPosition;
                                    hmn.nomalNotes.Add(nn1);
                                    NomalNote nn2 = new NomalNote(nt.noteEventNum, (MusicBeat)Round(Get32NoteCount(TimeDivision, nt.term)), (MusicScale)nt.data1);
                                    nn2.staffPosition = nt.staffPosition;
                                    hmn.nomalNotes.Add(nn2);
                                    musicalTrack.notelist.RemoveAt(musicalTrack.notelist.Count - 1);
                                    musicalTrack.notelist.Add(hmn);
                                }

                            }

                            else//동시음이 아닐때
                            {
                                NomalNote nn = new NomalNote(nt.noteEventNum,(MusicBeat)Round(Get32NoteCount(TimeDivision, nt.term)), (MusicScale)nt.data1);
                                nn.staffPosition = nt.staffPosition;
                                musicalTrack.notelist.Add(nn);
                            }
                        }
                        else if (nt.msg == 128 || (nt.msg == 144 && nt.data2 == 0))//노트오프
                        {
                            if (nowNoteOnCount <= 0)
                            {
                                throw new NotImplementedException("노트온이 아닌 상태에서 노트오프");
                            }
                            nowNoteOnCount--;
                            beforNoteOffstaffPosition = nt.staffPosition;
                        }

                        beforNote = nt;
                    }
                    else// 노트온,오프 제외 다른 이벤트
                    {
                        notMidiEventsTime += nt.time;
                    }
                }
            }
        }
        private void SetMusicalTrackMi()
        {
            musicalTrack = new MusicalTrack(trackName);

            musicalTrack.musicInfo.staffType = StaffTypeAnalsis();

            musicalTrack.musicInfo.key = MidiTrackChunkData.metaEventInfo.GetKeySignatureDataAnalsis();
            musicalTrack.instType = instType;

            if (MidiTrackChunkData.metaEventInfo.timeSignatureList.Count != 0)
            {
                musicalTrack.musicInfo.timeSignatureList = MidiTrackChunkData.metaEventInfo.timeSignatureList;
            }

            if (MidiTrackChunkData.metaEventInfo.tempoList.Count != 0)
            {
                musicalTrack.musicInfo.tempoList = MidiTrackChunkData.metaEventInfo.tempoList;
            }
        }
        private int StaffTypeAnalsis()
        {
            int allScaleCount = lowScaleCount + nomalScaleCount + highScaleCount;

            if (allScaleCount != 0)
            {
                if (lowScaleCount == 0)
                {
                    return 0;
                }

                if (highScaleCount == 0)
                {
                    return 1;
                }

                return 2;
            }
            return 0;
        }
        private int Round(double db)
        {
            return (int)Math.Round(db);
        }

        private void TrackInit()
        {
            chList = new List<int>();
            events = new List<Event>();

            if (ChDic == null)
            {
                ChDic = new Dictionary<int, List<int>>();
                for (int i = 0; i < 16; i++)
                {
                    ChDic[i] = new List<int>();
                }
            }
        }

        
        #endregion
        /// <summary>
        /// 미디이벤트분석
        /// </summary>
        /// <param name="br">파일읽기</param>
        /// <param name="mdev">새로생성한 미디이벤트</param>
        /// <param name="bt">현재이벤트</param>
        private void MidiEventAnalsis(BinaryReader br, MidiEvent mdev, byte bt)
        {
            if ((int)bt < 128)
            {
                byte[] eventData = new byte[2];
                eventData[0] = bt;
                mdev.type = "RunningStatus";

                switch (beforEventMsg)
                {
                    case 128: eventData[1] = br.ReadByte(); Event(eventData, mdev); break; //노트오프 
                    case 144: eventData[1] = br.ReadByte(); Event(eventData, mdev); break; //노트온
                    case 160: eventData[1] = br.ReadByte(); Event(eventData, mdev); break; //Note Aftertouch
                    case 176: eventData[1] = br.ReadByte(); Event(eventData, mdev); break; //Controller
                    case 192: Event(eventData[0], mdev); instType = eventData[0]; break; //악기소리결정 1
                    case 208: Event(eventData[0], mdev); break; //Channel Aftertouch 1
                    case 224: eventData[1] = br.ReadByte(); Event(eventData, mdev); break; //Pitch Bend
                    case 240:
                        {
                            byte b = eventData[0];
                            while (b != 0xF7)
                            { b = br.ReadByte(); }
                            Event(0, mdev);
                            break;
                        }  //ExcuSys
                    default: throw new NotImplementedException("이벤트오류");
                }
            }
            else
            {
                beforEventMsg = (int)((bt >> 4) << 4);
                beforEventCh = (int)bt - beforEventMsg;

                switch (beforEventMsg)
                {
                    case 128: mdev.type = "NoteOff"; Event(br.ReadBytes(2), mdev); break; //노트오프 
                    case 144: mdev.type = "NoteOn"; Event(br.ReadBytes(2), mdev); break; //노트온
                    case 160: mdev.type = "NoteAftertouch"; Event(br.ReadBytes(2), mdev); break; //Note Aftertouch
                    case 176: mdev.type = "Controller"; Event(br.ReadBytes(2), mdev); break; //Controller
                    case 192: mdev.type = "Programe"; instType = br.ReadByte(); Event((byte)instType, mdev); break; //악기소리결정 1
                    case 208: mdev.type = "ChannelAftertouch"; Event(br.ReadByte(), mdev); break; //Channel Aftertouch 1
                    case 224: mdev.type = "PitchBend"; Event(br.ReadBytes(2), mdev); break; //Pitch Bend
                    case 240:
                        {
                            mdev.type = "ExcuSys";
                            byte b = 0;
                            while (b != 0xF7)
                            { b = br.ReadByte(); }
                            Event(0, mdev);
                            break;
                        }  //Excu sys
                    default: throw new NotImplementedException("이벤트오류");
                }
            }
        }
        #region Event
        /// <summary>
        /// 이벤트처리
        /// </summary>
        /// <param name="bts"></param>
        /// <param name="mdev"></param>
        private void Event(byte[] bts, MidiEvent mdev)
        {
            mdev.msg = beforEventMsg;
            mdev.ch = beforEventCh;
            mdev.data1 = bts[0];
            mdev.data2 = bts[1];
            GetTerm(mdev);
        }

        /// <summary>
        /// 이벤트처리
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="mdev"></param>
        private void Event(byte bt, MidiEvent mdev)
        {
            mdev.msg = beforEventMsg;
            mdev.ch = beforEventCh;
            mdev.data1 = bt;
            GetTerm(mdev);
        }
        #endregion
        /// <summary>
        /// 채널더하기
        /// </summary>
        private void AddCh()
        {
            foreach (int ch in chList)
            {
                if (beforEventCh == ch)
                {
                    return;
                }
            }
            chList.Add(beforEventCh);

            bool ListAdd = true;
            foreach (int track in ChDic[beforEventCh])
            {
                if (track == trackNum)
                {
                    ListAdd = false;
                }
            }
            if (ListAdd)
            {
                ChDic[beforEventCh].Add(trackNum);
            }
        }
        //같은 채널의 같은 음데이터를 갖는 노트이벤트들의 텀을 구하는 함수
        private void GetTerm(MidiEvent mdevt, bool musicalTrackAnalsis = false, bool addevents = true)
        {
            AddCh();
            if (!musicalTrackAnalsis)
            {
                mdevt.noteEventNum = -1;
            }

            //노트온 이거나 노트오프일때
            if (mdevt.msg == 128 || mdevt.msg == 144)
            {
                if (this.instType == -1 && mdevt.ch != 9)
                {
                    instType = 0;
                }
                if (mdevt.msg == 144 && mdevt.data2 != 0)
                {
                    if (!musicalTrackAnalsis)
                    {
                        mdevt.noteEventNum = noteEventNumBuf;
                        noteEventNumBuf++;
                    }

                    if (mdevt.data1 <= 56)
                    {
                        lowScaleCount++;
                    }
                    else if (mdevt.data1 >= 65)
                    {
                        highScaleCount++;
                    }
                    else
                    {
                        nomalScaleCount++;
                    }
                }

                string key = mdevt.ch.ToString() + "." + mdevt.data1.ToString();

                //노트버프딕셔너리에 해당 채널.음데이터 를 키로 같는 데이터가 있으면
                if (MidiNoteBufDic.ContainsKey(key))
                {
                    if (mdevt.msg == 128 || (mdevt.msg == 144 && mdevt.data2 == 0))
                    {
                        mdevt.noteEventNum = MidiNoteBufDic[key].noteEventNum;
                    }
                    //그 미디이벤트데이터의 텀인자에 현재 미디이벤트데이터의 타임을 더해주고
                    MidiNoteBufDic[key].term = mdevt.time;
                    //이벤트들의 리스트를 뒤에서부터 돌며 같은 채널,음데이터의 값을 갖는 이벤트가 나올때까지 타임들을 더해준다
                    for (int i = events.Count - 1; i >= 0; i--)
                    {
                        if (events[i] is MidiEvent)
                        {
                            if (((MidiEvent)events[i]).ch == mdevt.ch && ((MidiEvent)events[i]).data1 == mdevt.data1)
                            {
                                break;
                            }
                        }
                        MidiNoteBufDic[key].term += ((Event)events[i]).time;
                    }

                    MidiNoteBufDic.Remove(key); //다 더한 후 딕셔너리 상에서 삭제
                }
                MidiNoteBufDic[key] = mdevt; //딕셔너리에 현재 미디이벤트 데이터를 추가
            }
            if (addevents) { events.Add(mdevt); } //이벤트리스트에 현재 미디이벤트 추가
        }

        #region 메타이벤트들
        /// <summary>
        /// 메타이벤트 분석
        /// </summary>
        /// <param name="br"></param>
        /// <param name="mtev"></param>
        /// <returns></returns>
        private bool MetaEventAnalsis(BinaryReader br, MetaEvent mtev)
        {
            int msg = mtev.msg[1] = (int)br.ReadByte();
            int dataLen = mtev.dataLen = (int)br.ReadByte();
            byte[] data = mtev.data = br.ReadBytes(dataLen);

            switch (msg)
            {
                //메타이벤트
                case 0: metaEventInfo.sequenceNum = data; mtev.type = "SequenceNum"; break;
                case 1: metaEventInfo.writer += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "Writer"; break;
                case 2: metaEventInfo.copyright += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "Copyright"; break;
                case 3: this.trackName = new string(ConvertToCharArrbyByteArr(dataLen, data)); metaEventInfo.trackNames[trackNum] = trackName; mtev.type = "TrackName"; break;
                case 4: metaEventInfo.instrumentinfo += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "InstrumentInfo"; break;
                case 5: metaEventInfo.lyric += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "Lyric"; break;
                case 6: metaEventInfo.marker += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "Marker"; break;
                case 7: metaEventInfo.cuePoint += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "CuePoint"; break;
                case 8: metaEventInfo.programname += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "ProgramName"; break;
                case 9: metaEventInfo.devicename += new string(ConvertToCharArrbyByteArr(dataLen, data)); mtev.type = "DeviceName"; break;
                case 32: metaEventInfo.midifreemax = data; mtev.type = "MidiFreemax"; break;
                case 33: metaEventInfo.midiport = data; mtev.type = "MidiPort"; break;
                case 47: MidiNoteBufDic.Clear(); return false; //트랙종료
                case 81: GetTempo(data, mtev); mtev.type = "Tempo"; break;
                case 84: metaEventInfo.smpteOffset = data; mtev.type = "SmpteOffSet"; break;
                case 88: GetTimeSignature(data, mtev); mtev.type = "TimeSignature"; break;
                case 89: metaEventInfo.keySignatureData = data; mtev.type = "KeySignature"; break;
                case 127: metaEventInfo.seqEvent = data; mtev.type = "SeqEvent"; break;
                default: mtev.type = "etc"; break;
            }
            events.Add(mtev);
            return true;
        }
        private void GetTimeSignature(byte[] data, MetaEvent mtev)
        {
            TimeSignature ts = new TimeSignature();
            ts.position = cumulativetime;
            ts.frame = mtev.frame;
            ts.staffPosition = mtev.staffPosition;

            ts.numerator = data[0];
            ts.denominator = (int)Math.Pow(2, (int)data[1]);

            metaEventInfo.timeSignatureList.Add(ts);
        }
        private void GetTempo(byte[] data, MetaEvent mtev)
        {
            Tempo tp = new Tempo();

            tp.position = cumulativetime;
            tp.frame = mtev.frame;
            tp.staffPosition = mtev.staffPosition;

            tp.data = (((int)data[0] * 256) + (int)data[1]) * 256 + (int)data[2];

            metaEventInfo.tempoList.Add(tp);
        }
        private char[] ConvertToCharArrbyByteArr(int dataLen, byte[] data)
        {
            char[] buf = new char[dataLen];
            for (int i = 0; i < dataLen; i++)
            {
                buf[i] = (char)data[i];
            }
            return buf;
        }
        #endregion
        /// <summary>
        /// 델타타임 구하는 메서드
        /// </summary>
        /// <param name="br"></param>
        /// <param name="BeforTime"></param>
        /// <returns></returns>
        private int GetTimeFromDeltaTime(BinaryReader br, int BeforTime = 0)
        {
            int time = (int)br.ReadByte();
            if (time < 128)//128보다 작으면 한자리 수이기때문 한개만 리턴
            {
                return (BeforTime + time);
            }
            else
            {
                //다시
                int timebuf = (time - 128 + BeforTime) * 128;
                return GetTimeFromDeltaTime(br, timebuf);
            }
        }

        #region 시간정보설정
        private void SetTimeInfo(Event ev, int time)
        {
            cumulativetime += time;

            ev.trackNum = trackNum;
            ev.time = time;
            ev.position = cumulativetime;
            ev.staffPosition = SetMidiStaffPosition();
            ev.frame = SetFrame(time);
        }

        private StaffPosition SetMidiStaffPosition()
        {
            StaffPosition sp = new StaffPosition();

            int MeasuerPoint = FindMeasuerPoint();

            int tick = 0;
            int beat = 1;
            int measure = 1;

            for (int i = 0; i < cumulativetime; i++)
            {
                tick++;
                if (tick == TimeDivision)
                {
                    tick = 0;
                    beat++;
                    if (beat == MeasuerPoint)
                    {
                        beat = 1;
                        measure++;
                    }
                }
            }
            sp.measure = measure - 1;
            sp.beat = beat - 1;
            sp.tick = Get32NoteCount(TimeDivision, tick);

            return sp;
        }
        private double Get32NoteCount(int timeDivision, int term)
        {
            double thirtysecondNote = timeDivision / 8;
            double tsNoteCount = (double)term / thirtysecondNote;

            return tsNoteCount; // (int)Math.Round(tsNoteCount);
        }

        private int FindMeasuerPoint()
        {
            int point = 0;
            if (metaEventInfo == null || metaEventInfo.timeSignatureList.Count == 0)
            {
                return 5;
            }
            foreach (TimeSignature ts in metaEventInfo.timeSignatureList)
            {
                if (ts.position <= cumulativetime)
                {
                    double v1 = ts.numerator;
                    double v2 = ts.denominator;
                    point = (int)(v1 / v2 * 4 + 1);
                }
            }
            return point;
        }
        private float SetFrame(int time)
        {
            float tempo = FindTempo();
            if (tempo == 0)
            {
                tempo = 100;
            }

            float frame = 1800 / tempo;
            frame = frame * time;
            frame = frame / TimeDivision;

            cumulativeFrames += frame;

            return cumulativeFrames;
        }
        private float FindTempo()
        {
            float point = 0;
            if (metaEventInfo == null || metaEventInfo.tempoList.Count == 0)
            {
                return 100;
            }

            foreach (Tempo tp in metaEventInfo.tempoList)
            {
                if (tp.position <= cumulativetime)
                {
                    point = tp.GetTempoByData();
                }
            }
            return point;
        }
        #endregion


        #region Get받아오는타입
        private string GetId(BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }
        private int GetLength(BinaryReader br)
        {
            byte[] bt = br.ReadBytes(4);
            int Length = ((((int)bt[0] * 256) + (int)bt[1]) * 256 + (int)bt[2]) * 256 + (int)bt[3];
            return Length;
        }
        #endregion

        #region 업뎃이전
        public string[] ReadEvent(BinaryReader br)
        {
            string[] sttafo = new string[4];


            try
            {
                sttafo[0] = (br.ReadByte()).ToString();
                sttafo[1] = br.ReadByte().ToString();
                sttafo[2] = br.ReadByte().ToString();
                sttafo[3] = br.ReadByte().ToString();
            }
            catch (Exception)
            {

                return null;
            }

            return sttafo;

        }

        public void EndTrack(BinaryWriter bw)
        {
            byte[] end = new byte[4];
            end[0] = 0;
            end[1] = 255;
            end[2] = 47;
            end[3] = 0;
            bw.Write(end);

        }

        public void NoteEvent(List<MusicScale> NoteList, BinaryWriter bw)
        {
            int notecunt = NoteList.Count * 8 + 4    +3;
            bw.Write((byte)((notecunt / 256 / 256 / 256) % 256));
            bw.Write((byte)((notecunt / 256 / 256) % 256));
            bw.Write((byte)((notecunt / 256) % 256));
            bw.Write((byte)(notecunt % 256));

            byte[] arcgi = new byte[3];
            arcgi[0] = 0;
            arcgi[1] = 192;//악기선택 명령어
            arcgi[2] = (byte)instType; //0은 그랜드피아노  24는 기타
            bw.Write(arcgi);
            foreach (MusicScale s in NoteList)
            {
                NoteOn(s, bw);
            }
            EndTrack(bw);
        }
        public void NoteOn(MusicScale ms, BinaryWriter bw)
        {
            byte musical = (byte)ms;
            bw.Write((byte)0x00);
            bw.Write((byte)144);
            bw.Write((byte)musical);
            bw.Write((byte)120); //노트온

            bw.Write((byte)120);
            bw.Write((byte)144);
            bw.Write((byte)musical);
            bw.Write((byte)0); //벨로시티 0

        }
        #endregion





        #region 저장

        /// <summary>
        /// 저장하는 메서드
        /// </summary>
        /// <param name="bw"></param>
        public void SaveMidiFile(BinaryWriter bw)
        {
            byte[] ID = new byte[4];
            ID[0] = (byte)'M';
            ID[1] = (byte)'T';
            ID[2] = (byte)'r';
            ID[3] = (byte)'k';
            bw.Write(ID);

            List<byte> byteList = new List<byte>();

            int beforExcuSysEventtime = 0;
            foreach (Event ev in events)
            {
                if (ev is MetaEvent)
                {
                    MetaEvent me = ev as MetaEvent;

                    List<byte> deltatimeBytes = GetDeltaTimeFromTime(me.time + beforExcuSysEventtime);
                    beforExcuSysEventtime = 0;

                    foreach (byte bt in deltatimeBytes)
                    {
                        byteList.Add(bt);
                    }

                    byteList.Add((byte)me.msg[0]);
                    byteList.Add((byte)me.msg[1]);

                    byteList.Add((byte)me.dataLen);
                   

                    foreach (byte bt in me.data)
                    {
                        byteList.Add(bt);
                    }
                }
                else if (ev is MidiEvent)
                {
                    MidiEvent me = ev as MidiEvent;

                    if (me.msg == 240)
                    {
                        beforExcuSysEventtime += me.time;
                    }
                    else if (me.type == "RunningStatus")
                    {
                        List<byte> deltatimeBytes = GetDeltaTimeFromTime(me.time + beforExcuSysEventtime);
                        beforExcuSysEventtime = 0;

                        foreach (byte bt in deltatimeBytes)
                        {
                            byteList.Add(bt);
                        }

                        byteList.Add((byte)me.data1);
                        byteList.Add((byte)me.data2);
                    }

                    else if (me.msg == 192 || me.msg == 208)
                    {

                        List<byte> deltatimeBytes = GetDeltaTimeFromTime(me.time + beforExcuSysEventtime);
                        beforExcuSysEventtime = 0;


                        foreach (byte bt in deltatimeBytes)
                        {
                            byteList.Add(bt);
                        }
                        byteList.Add((byte)(me.msg + me.ch));
                        byteList.Add((byte)me.data1);
                    }
                    else
                    {
                        List<byte> deltatimeBytes = GetDeltaTimeFromTime(me.time + beforExcuSysEventtime);
                        beforExcuSysEventtime = 0;

                        foreach (byte bt in deltatimeBytes)
                        {
                            byteList.Add(bt);
                        }
                        byteList.Add((byte)(me.msg + me.ch));
                        byteList.Add((byte)me.data1);
                        byteList.Add((byte)me.data2);
                    }
                }
            }

            byteList.Add((byte)0);
            byteList.Add((byte)255);
            byteList.Add((byte)47);
            byteList.Add((byte)0);

            int byteCount = byteList.Count;
            // 0 0 0 0  256 *256 =65536 65536* 256 =1677216
            byte[] trackLen = new byte[4];
            trackLen[0] = (byte)(byteCount / 16777216);
            trackLen[1] = (byte)((byteCount % 16777216) / 65536);
            trackLen[2] = (byte)(((byteCount % 16777216) % 65536) / 256);
            trackLen[3] = (byte)(((byteCount % 16777216) % 65536) % 256);
            bw.Write(trackLen);

            foreach (byte bt in byteList)
            {
                bw.Write(bt);
            }
        }
        private List<byte> GetDeltaTimeFromTime(int time)
        {
            List<byte> deltatimeList = new List<byte>();

            int timebuf = time;

            while (true)
            {
                if ((int)(timebuf / 128) != 0)
                {
                    deltatimeList.Add((byte)(timebuf % 128));
                    timebuf = timebuf / 128;
                }
                else
                {
                    deltatimeList.Add((byte)(timebuf % 128));
                    break;
                }
            }

            deltatimeList.Reverse();

            for (int i = 0; i < deltatimeList.Count - 1; i++)
            {
                deltatimeList[i] += 128;
            }

            return deltatimeList;
        }

        #endregion

        #region 청크데이타필요부분
        public void RemoveMetaEventType(string metaEventType)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].type == metaEventType)
                {
                    if (events[i].time != 0 && events[i + 1] != null)
                    {
                        events[i + 1].time += events[i].time;
                    }
                    events.RemoveAt(i);
                }
            }
        }
        public void AddMetaEvent(string metaEventType, string strData)
        {
            MetaEvent me = new MetaEvent();
            me.type = metaEventType;
            me.trackNum = trackNum;
            me.staffPosition = new StaffPosition();

            me.dataLen = strData.Length;
            me.data = ConvertToByteArrbyString(strData);

            switch (metaEventType)
            {
                case "Writer": metaEventInfo.writer = strData; me.msg[1] = 1; break;
                case "Copyright": metaEventInfo.copyright = strData; me.msg[1] = 2; break;
                case "TrackName": metaEventInfo.trackNames[trackNum] = strData; me.msg[1] = 3; break;
                case "Lyric": metaEventInfo.lyric = strData; me.msg[1] = 5; break;
                //case "ProgramName": metaEventInfor.programname = strData; break;
                //case "DeviceName": metaEventInfor.devicename = strData; break;
                default: throw new NotImplementedException("미구현");
            }

            events.Insert(0, me);
        }
        internal void ChangeNoteScale(int noteNum, MusicScale scale)
        {
            foreach (Event evt in events)
            {
                if (evt is MidiEvent)
                {
                    if (((MidiEvent)evt).noteEventNum == noteNum)
                    {
                        ((MidiEvent)evt).data1 = (int)scale;
                    }
                }
            }

            foreach (Note nt in musicalTrack.notelist)
            {
                if (nt is Harmony)
                {
                    foreach (NomalNote nnt in ((Harmony)nt).nomalNotes)
                    {
                        if (nnt.noteNum == noteNum)
                        {
                            nnt.scale = scale;
                            return;
                        }
                    }
                }
                else if (nt is NomalNote)
                {
                    if (((NomalNote)nt).noteNum == noteNum)
                    {
                        ((NomalNote)nt).scale = scale;
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 박자가 줄어들면 빈 부분에 쉼표가 생기고, 늘어나면 늘어난 만큼 뒤에 있던 음표들의 박자가 사라짐
        /// </summary>
        /// <param name="noteNum"></param>
        /// <param name="beat"></param>
        internal void ChangeNoteBeat(int noteNum, MusicBeat beat) 
        {
            int beforPosition = 0;
            int afterPosition = 0;
            int beatGabTime = 0;

            NoteOffPositionReSet(noteNum, beat, ref beatGabTime, ref beforPosition, ref afterPosition);

            if (beatGabTime > 0)
            {
                influenceEventSet(beforPosition, afterPosition);
            }

            EventsSortByTime();
            EventsTimeInfoReSet();
            SetMusicalTrackMi();
            SetMusicalTrackNote();
        }
        /// <summary>
        /// 박자가 줄어들면 앞 음표들이 땡겨지고, 늘어나면 늘어난 만큼 뒤에 있던 음표들이 미루어짐
        /// </summary>
        /// <param name="noteNum"></param>
        /// <param name="beat"></param>
        /// <param name="afterNotesPull"></param>
        internal void ChangeNoteBeat2(int noteNum, MusicBeat beat, bool afterNotesPull) 
        {
            int beforPosition = 0;
            int afterPosition = 0;
            int beatGabTime = 0;

            NoteOffPositionReSet(noteNum, beat, ref beatGabTime, ref beforPosition, ref afterPosition);

            if (afterNotesPull)
            {
                ChangedNoteAfterNotesReSet(noteNum, beforPosition, beatGabTime);
            }
            else if (beatGabTime > 0)
            {
                ChangedNoteAfterNotesReSet(noteNum, beforPosition, beatGabTime);
            }

            EventsSortByTime();
            EventsTimeInfoReSet();
            SetMusicalTrackMi();
            SetMusicalTrackNote();
        }

        private void ChangedNoteAfterNotesReSet(int noteNum, int beforPosition, int beatGabTime)
        {
            bool nowNoteOnEventsAdd = true;
            List<int> nowNoteOnEventsNoteNum = new List<int>();

            foreach (Event evt in events)
            {
                if (evt is MidiEvent)
                {
                    MidiEvent mdevt = evt as MidiEvent;
                    if (nowNoteOnEventsAdd && mdevt.position >= beforPosition) { nowNoteOnEventsAdd = false; }
                    if (mdevt.msg == 144 && mdevt.data2 != 0)
                    {
                        if (nowNoteOnEventsAdd) { nowNoteOnEventsNoteNum.Add(mdevt.noteEventNum); }
                    }
                    else if (mdevt.msg == 128 || (mdevt.msg == 144 && mdevt.data2 == 0))
                    {
                        nowNoteOnEventsNoteNum.Remove(mdevt.noteEventNum);
                    }
                }

                if (!nowNoteOnEventsAdd)
                {
                    if (evt is MetaEvent)
                    {
                        evt.position = evt.position + beatGabTime;
                    }
                    else if (((MidiEvent)evt).noteEventNum != noteNum)
                    {
                        if (!isBeforNoteOn(nowNoteOnEventsNoteNum, ((MidiEvent)evt).noteEventNum))
                        {
                            evt.position = evt.position + beatGabTime;
                        }
                    }
                }
            }
        }

        private bool isBeforNoteOn(List<int> nowNoteOnEventsNoteNum, int noteNum)
        {
            foreach (int noteOnNum in nowNoteOnEventsNoteNum)
            {
                if (noteOnNum == noteNum)
                {
                    return true;
                }
            }
            return false;
        }

        private void NoteOffPositionReSet(int noteNum, MusicBeat beat, ref int beatGabTime, ref int beforPosition, ref int afterPosition)
        {
            int changedBeatTime = ((int)beat) * (TimeDivision / 8);

            foreach (Event evt in events)
            {
                if (evt is MidiEvent)
                {
                    MidiEvent mdevt = evt as MidiEvent;

                    if (mdevt.noteEventNum == noteNum)
                    {
                        if (mdevt.msg == 144 && mdevt.data2 != 0)
                        {
                            beatGabTime = changedBeatTime - mdevt.term;
                            ((MidiEvent)evt).term = changedBeatTime;
                        }
                        else if (mdevt.msg == 128 || (mdevt.msg == 144 && mdevt.data2 == 0))
                        {
                            beforPosition = evt.position;
                            afterPosition = evt.position = evt.position + beatGabTime;
                        }
                    }
                }
            }
        }

        private void influenceEventSet(int beforPosition, int afterPosition)
        {
            Dictionary<int, Event> deleEventDistinction = new Dictionary<int, Event>();
            List<Event> influEvents = new List<Event>();


            foreach (Event evt in events)
            {
                if (evt.position >= beforPosition && evt.position <= afterPosition)
                {
                    influEvents.Add(evt);
                }
                if (evt.position > afterPosition) { break; }
            }

            foreach (Event evt in influEvents)
            {
                if (evt is MidiEvent)
                {
                    int key = ((MidiEvent)evt).noteEventNum;
                    if (deleEventDistinction.ContainsKey(key))
                    {
                        events.Remove(deleEventDistinction[key]);
                        events.Remove(evt);
                        deleEventDistinction.Remove(key);
                    }
                    else
                    {
                        deleEventDistinction[key] = evt;
                    }
                }
            }

            foreach (KeyValuePair<int, Event> pair in deleEventDistinction)
            {
                if (((MidiEvent)pair.Value).msg == 144 && ((MidiEvent)pair.Value).data2 != 0)
                {
                    int influEventBeforPosition = pair.Value.position;
                    pair.Value.position = afterPosition;
                    ((MidiEvent)pair.Value).term = ((MidiEvent)pair.Value).term - (afterPosition - influEventBeforPosition);
                }
            }
        }

        private void EventsSortByTime()
        {
            events.Sort(new EventComparer());
        }

        private void EventsTimeInfoReSet()
        {
            cumulativeFrames = 0;
            cumulativetime = 0;

            int beforEventTimePosition = 0;

            foreach (Event ev in events)
            {
                SetTimeInfo(ev, ev.position - beforEventTimePosition);
                beforEventTimePosition = ev.position;
            }
        }

        public void ChangeInstType(int InstType)
        {
            this.instType = InstType;
            foreach (Event evt in events)
            {
                if (evt is MidiEvent && ((MidiEvent)evt).msg == 192)
                {
                    ((MidiEvent)evt).data1 = InstType;
                }
            }

            musicalTrack.instType = InstType;
        }

        internal void RemoveNote(int noteNum, bool afterNotesPull)
        {
            int noteOnIndex = 0;
            int noteOffIndex = 0;

            int count = 0;
            foreach (Event evt in events)
            {
                if (evt is MidiEvent)
                {
                    MidiEvent mdevt = evt as MidiEvent;
                    if (mdevt.noteEventNum == noteNum && mdevt.msg == 144 && mdevt.data2 != 0)
                    {
                        noteOnIndex = count;
                    }
                    else if (mdevt.noteEventNum == noteNum && (mdevt.msg == 128 || (mdevt.msg == 144 && mdevt.data2 == 0)))
                    {
                        noteOffIndex = count;
                    }
                }
                count++;
            }

            if (afterNotesPull)
            {
                Dictionary<int, MidiEvent> noteOnBufDic = new Dictionary<int, MidiEvent>();

                for (int i = 0; i < noteOnIndex; i++)
                {
                    if (events[i] is MidiEvent)
                    {
                        MidiEvent mdevt = events[i] as MidiEvent;
                        if (mdevt.msg == 144 && mdevt.data2 != 0)
                        {
                            noteOnBufDic[mdevt.noteEventNum] = mdevt;
                        }
                        else if (mdevt.msg == 128 || (mdevt.msg == 144 && mdevt.data2 == 0))
                        {
                            noteOnBufDic.Remove(mdevt.noteEventNum);
                        }
                    }
                }

                int positionDistinction = events[noteOnIndex + 1].position - events[noteOnIndex].position;

                for (int i = noteOnIndex + 1; i < events.Count - 1; i++)
                {
                    events[i].position -= positionDistinction;

                    if (events[i] is MidiEvent)
                    {
                        MidiEvent mdevt = events[i] as MidiEvent;

                        if (mdevt.msg == 128 || (mdevt.msg == 144 && mdevt.data2 == 0))
                        {
                            foreach (KeyValuePair<int, MidiEvent> pair in noteOnBufDic)
                            {
                                if (mdevt.noteEventNum == pair.Key)
                                {
                                    events[i].position = pair.Value.position + pair.Value.term;
                                }
                            }
                            if (noteOnBufDic.ContainsKey(mdevt.noteEventNum))
                            {
                                noteOnBufDic.Remove(mdevt.noteEventNum);
                            }
                        }
                    }
                }

                events.RemoveAt(noteOnIndex);
                events.RemoveAt(noteOffIndex - 1);

                EventsSortByTime();
                EventsTimeInfoReSet();
            }
            else
            {
                events[noteOnIndex + 1].time += events[noteOnIndex].time;
                events[noteOffIndex + 1].time += events[noteOffIndex].time;

                events.RemoveAt(noteOnIndex);
                events.RemoveAt(noteOffIndex - 1);
            }

            SetMusicalTrackMi();
            SetMusicalTrackNote();
        }
        internal sealed class EventComparer : IComparer<Event>
        {
            public int Compare(Event x, Event y)
            {
                if (x == null || y == null)
                {
                    throw new ApplicationException("Event 가 아닙니다.");
                }

                if (x.position != y.position)
                {
                    return x.position.CompareTo(y.position);
                }

                if (x is MidiEvent && y is MidiEvent)
                {
                    if (((MidiEvent)x).msg == ((MidiEvent)y).msg || ((MidiEvent)x).msg == 192 || ((MidiEvent)y).msg == 192)
                    {
                        return ((MidiEvent)x).data2.CompareTo(((MidiEvent)y).data2);
                    }

                    return ((MidiEvent)x).msg.CompareTo(((MidiEvent)y).msg);
                }
                else if (x is MetaEvent && y is MetaEvent)
                {
                    return x.position.CompareTo(y.position);
                }
                else if (x is MetaEvent && y is MidiEvent)
                {
                    return ((MetaEvent)x).msg[1].CompareTo(((MidiEvent)y).msg);
                }
                else if (x is MidiEvent && y is MetaEvent)
                {
                    return ((MidiEvent)y).msg.CompareTo(((MetaEvent)x).msg[1]);
                }

                return x.position.CompareTo(y.position);
            }
        }
    }

    #endregion
        


}


