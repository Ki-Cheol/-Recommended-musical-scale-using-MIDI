using System;
using System.Collections.Generic;
using System.IO;
public enum NoteEvent
{
    NoteOn=0x90
}

namespace MidiChunkDataLib
{
    public class MidiHeaderChunkData
    {

        public string MidiChunkID { get; private set; }
        public int Length { get; private set; }//헤더의 싸이즈 항상 6을 뜻함
        public int format { get; private set; }//포멧타입(0~2)
        public int trackcount { get; private set; } //트랙 수
        public int timedivision { get; private set; } //timebae 정보 (4분음표의 타임)
     
        public MidiHeaderChunkData(int trackCount)
        {
            Length = 4;
            format = 1;
            trackcount = trackCount;
            timedivision = 192;
            MidiTrackChunkData.metaEventInfo = new MidiMetaEventInfo(trackcount);
        }
        public MidiHeaderChunkData(BinaryReader br)
        {
            MidiChunkID = GetId(br);
            if (MidiChunkID != "MThd")
            {
                throw new NotFiniteNumberException("잘못된 헤더");
            }
            Length = GetLength(br);
            format = GetFormat(br);
            trackcount = GetTrackCount(br);
            timedivision = GetTimebase(br);
            MidiTrackChunkData.metaEventInfo = new MidiMetaEventInfo(trackcount);


        }
        public void TrackCountIncrease()
        {
            trackcount++;
        }

        public void TrackCountDecrease()
        {
            trackcount--;
        }
        private string GetId(BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }
        private int GetLength(BinaryReader br)
        {
            return (int)br.ReadChars(4)[3];
        }
        private int GetFormat(BinaryReader br)
        {
            return (int)br.ReadChars(2)[1];
        }
        private int GetTrackCount(BinaryReader br)
        {
            byte[] bts = br.ReadBytes(2);
            return bts[0] * 256 + bts[1];
        }
        private int GetTimebase(BinaryReader br)
        {
            byte[] bts = br.ReadBytes(2);
            return bts[0] * 256 + bts[1];
        }


        public void SaveMidiFile(BinaryWriter bw)
        {
            byte[] headerByte = new byte[14];
            headerByte[0] = (byte)'M';
            headerByte[1] = (byte)'T';
            headerByte[2] = (byte)'h';
            headerByte[3] = (byte)'d';

            headerByte[4] = (byte)0;
            headerByte[5] = (byte)0;
            headerByte[6] = (byte)0;
            headerByte[7] = (byte)6;

            headerByte[8] = (byte)0;
            headerByte[9] = (byte)format;

            headerByte[10] = (byte)(trackcount / 256);
            headerByte[11] = (byte)(trackcount % 256);

            headerByte[12] = (byte)(timedivision / 256);
            headerByte[13] = (byte)(timedivision % 256);
            bw.Write(headerByte);
        }

    
}