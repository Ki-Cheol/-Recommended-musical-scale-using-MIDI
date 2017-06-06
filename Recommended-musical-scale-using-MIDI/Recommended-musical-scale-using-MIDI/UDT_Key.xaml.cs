using MidiPlayLib;
using MusicalTrackLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Recommended_musical_scale_using_MIDI
{
    /// <summary>
    /// UDT_Key.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class UDT_Key : UserControl
    {
     
        bool dodown = false;
        bool doshapdown = false;
        bool Redown = false;
        bool reshapdown = false;
        bool midown = false;
        bool fadown = false;
        bool fashapdown = false;
        bool soldown = false;
        bool solshapdown = false;
        bool ladown = false;
        bool lashapdown = false;
        bool sidown = false;
        bool do6down = false;
       public int octavecount { get; set; }
        static int inst;

        #region 초창기 노트베이스
        public List<MusicScale> ScaleBase
        {
            get
            {
                return scalebase;
            }
        }
        private List<MusicScale> scalebase = new List<MusicScale>();
        #endregion


        SerialPort serial;
        public void SetSial(SerialPort _port)
        {
            serial = _port;
            serial.DataReceived += Serial_DataReceived;

        }

        public MusicBeat Key_Beat { get; set; }
        public List<Note> muscialNote = new List<Note>();



        string g_sRecvData = String.Empty;
        delegate void SetTextCallBack(String text);


        public UDT_Key()
        {
            octavecount = 5;
            InitializeComponent();
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;
            MidiShortMsgPlayer.MidiOpen();
            
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                g_sRecvData = serial.ReadExisting();
                if ((g_sRecvData != string.Empty)) // && (g_sRecvData.Contains('\n')))
                {
                    SetText(g_sRecvData);
                }
            }
            catch (TimeoutException)
            {
                g_sRecvData = string.Empty;
            }

        }
        private void BT_SrialTest_Click(object sender, RoutedEventArgs e)
        {

            if (serial.IsOpen)
            {
               
            }
            else
            {
               
            }
        }


        private void SetText(string text)
        {
            if (TB_NoteList.Dispatcher.CheckAccess())
            {
                //TB_NoteList.Text+=text;
                SeriaL_keyGen(text);
            }
            else
            {
                SetTextCallBack d = new SetTextCallBack(SetText);
                TB_NoteList.Dispatcher.Invoke(d, new object[] { text });
            }
        }
        public void SeriaL_keyGen(string s)
        {
            
            int msgch = 144;
            int velo = 120;
            int buf2 = velo << 16;
                switch (s)
            {
                case "Z":
                    {


                        int pitch = octavecount * 12 + 0;
                        int buf1 = pitch << 8;

                             MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Do5 + " Term(" + Key_Beat + ")";

                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Do5));
                        
                       
                    }
                    break;
                case "S":
                    {


                        int pitch = octavecount * 12 + 1;
                        int buf1 = pitch << 8;

                             MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.DoSharp5 + " Term(" + Key_Beat + ")";

                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.DoSharp5));
                        
                    }
                    break;
                case "X":
                    {

                        int pitch = octavecount * 12 + 2;
                        int buf1 = pitch << 8;
                      
                             MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Re5 + " Term(" + Key_Beat + ")";

                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Re5));
                        
                    }
                    break;
                case "D":
                    {


                        int pitch = octavecount * 12 + 3;
                        int buf1 = pitch << 8;

                        MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.ReSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.ReSharp5));
                        
                    }
                    break;
                case "C":
                    {

                        int pitch = octavecount * 12 + 4;
                        int buf1 = pitch << 8;

                            MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Mi5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Mi5));
                        
                    }
                    break;
                case "V":
                    {

                        int pitch = octavecount * 12 + 5;
                        int buf1 = pitch << 8;

                             MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Fa5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Fa5));
                        
                    }
                    break;
                case "G":
                    {


                        int pitch = octavecount * 12 + 6;
                        int buf1 = pitch << 8;

                       
                            fashapdown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.FaSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.FaSharp5));
                        
                    }
                    break;
                case "B":
                    {

                        int pitch = octavecount * 12 + 7;
                        int buf1 = pitch << 8;
                        
                            soldown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Sol5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Sol5));
                        
                    }
                    break;
                case "H":
                    {


                        int pitch = octavecount * 12 + 8;
                        int buf1 = pitch << 8;

                        
                             MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.SolSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.SolSharp5));
                       
                    }
                    break;
                case "N":
                    {

                        int pitch = octavecount * 12 + 9;
                        int buf1 = pitch << 8;
                        MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.La5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.La5));
                        
                    }
                    break;
                case "J":
                    {


                        int pitch = octavecount * 12 + 10;
                        int buf1 = pitch << 8;

                        MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                            TB_NoteList.Text += MusicScale.LaSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.LaSharp5));
                        
                    }
                    break;
                case "M":
                    {

                        int pitch = octavecount * 12 + 11;
                        int buf1 = pitch << 8;
                        MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                            TB_NoteList.Text += MusicScale.Si5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Si5));
                        
                    }
                    break;
                case ",":
                    {

                        int pitch = (octavecount + 1) * 12;
                        int buf1 = pitch << 8;
                         MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Do6 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Do6));
                        
                    } break;
                case "-":
                    {
                        if((--inst == -1))
                        {
                            inst = 0;
                            break;
                        }
                        InstSetting(inst);
                        serial.WriteLine(inst.ToString());

                    }
                    break;
                case "+":
                    {
                        if (++inst == 100)
                        {
                            inst = 100;
                            break;
                            
                        }
                        InstSetting(inst);
                        serial.WriteLine(inst.ToString());
                    }
                    break;
                case "`":
                    {
                       
               MidiShortMsgPlayer.StopMusic();
                        inst = 0;
                        InstSetting(inst);
                    }
                    break;
                default: break;
            }

        }
        public UDT_Key(List<MusicScale> scalebase)
        {
            InitializeComponent();
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;
            MidiShortMsgPlayer.MidiOpen();
            this.scalebase = scalebase;
            
           
        }
        public void InstSetting(int cnt)
        {
            byte[] data = new byte[4];

            data[0] = 0xC0;//change instrument, channel 0
            Instrument ar;
            ar = (Instrument)cnt;


            data[1] = (byte)ar;

            uint msg = BitConverter.ToUInt32(data, 0);

            MidiShortMsgPlayer.SendMidiShortMsg((int)msg);
        }
        public string GetNoteList()
        {
            return TB_NoteList.Text;
        }
        public void SetNoteListBox(string notelist)
        {
            TB_NoteList.Text = notelist;
        }
        #region 키보드조작
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            int msgch = 128;
            int velo = 120;
            int buf2 = velo << 16;
            switch (e.Key)
            {

                case Key.Q:
                    {


                        int pitch = octavecount * 12 + 0;
                        int buf1 = pitch << 8;

                        if (dodown)
                        {
                            dodown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                        }
                    }
                    break;
                case Key.D2:
                    {


                        int pitch = octavecount * 12 + 1;
                        int buf1 = pitch << 8;

                        if (doshapdown)
                        {
                            doshapdown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                        }
                    }
                    break;
                case Key.W:
                    {

                        int pitch = octavecount * 12 + 2;
                        int buf1 = pitch << 8;
                        if (Redown)
                        {
                            Redown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                        }
                    }
                    break;
                case Key.D3:
                    {


                        int pitch = octavecount * 12 + 3;
                        int buf1 = pitch << 8;

                        if (reshapdown)
                        {
                            reshapdown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                        }
                    }
                    break;
                case Key.E:
                    {

                        int pitch = octavecount * 12 + 4;
                        int buf1 = pitch << 8;
                        if (midown)
                        {
                            midown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.R:
                    {

                        int pitch = octavecount * 12 + 5;
                        int buf1 = pitch << 8;
                        if (fadown)
                        {
                            fadown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.D5:
                    {


                        int pitch = octavecount * 12 + 6;
                        int buf1 = pitch << 8;

                        if (fashapdown)
                        {
                            fashapdown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.T:
                    {

                        int pitch = octavecount * 12 + 7;
                        int buf1 = pitch << 8;
                        if (soldown)
                        {
                            soldown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.D6:
                    {

                        int octavecount = 5;
                        int pitch = octavecount * 12 + 8;
                        int buf1 = pitch << 8;

                        if (solshapdown)
                        {
                            solshapdown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.Y:
                    {

                        int pitch = octavecount * 12 + 9;
                        int buf1 = pitch << 8;
                        if (ladown)
                        {
                            ladown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.D7:
                    {


                        int pitch = octavecount * 12 + 10;
                        int buf1 = pitch << 8;

                        if (lashapdown)
                        {
                            lashapdown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.U:
                    {

                        int pitch = octavecount * 12 + 11;
                        int buf1 = pitch << 8;
                        if (sidown)
                        {
                            sidown = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                case Key.I:
                    {

                        int pitch = (octavecount + 1) * 12;
                        int buf1 = pitch << 8;
                        if (do6down)
                        {
                            do6down = false; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                        }
                    }
                    break;
                default: break;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            int msgch = 144;
            int velo = 120;
            int buf2 = velo << 16;
            switch (e.Key)
            {

                case Key.Q:
                    {


                        int pitch = octavecount * 12 + 0;
                        int buf1 = pitch << 8;

                        if (!dodown)
                        {
                            dodown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Do5 + " Term(" + Key_Beat + ")";
                            
                            muscialNote.Add(new NomalNote(Key_Beat,MusicScale.Do5));
                        }
                    }
                    break;
                case Key.D2:
                    {


                        int pitch = octavecount * 12 + 1;
                        int buf1 = pitch << 8;

                        if (!doshapdown)
                        {
                            doshapdown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.DoSharp5 + " Term(" + Key_Beat + ")";
                            
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.DoSharp5));
                        }
                    }
                    break;
                case Key.W:
                    {

                        int pitch = octavecount * 12 + 2;
                        int buf1 = pitch << 8;
                        if (!Redown)
                        {
                            Redown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Re5 + " Term(" + Key_Beat + ")";
                        
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Re5));
                        }
                    }
                    break;
                case Key.D3:
                    {


                        int pitch = octavecount * 12 + 3;
                        int buf1 = pitch << 8;

                        if (!reshapdown)
                        {
                            reshapdown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.ReSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.ReSharp5));
                        }
                    }
                    break;
                case Key.E:
                    {

                        int pitch = octavecount * 12 + 4;
                        int buf1 = pitch << 8;
                        if (!midown)
                        {
                            midown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Mi5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Mi5));
                        }
                    }
                    break;
                case Key.R:
                    {

                        int pitch = octavecount * 12 + 5;
                        int buf1 = pitch << 8;
                        if (!fadown)
                        {
                            fadown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Fa5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Fa5));
                        }
                    }
                    break;
                case Key.D5:
                    {


                        int pitch = octavecount * 12 + 6;
                        int buf1 = pitch << 8;

                        if (!fashapdown)
                        {
                            fashapdown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.FaSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.FaSharp5));
                        }
                    }
                    break;
                case Key.T:
                    {

                        int pitch = octavecount * 12 + 7;
                        int buf1 = pitch << 8;
                        if (!soldown)
                        {
                            soldown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Sol5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Sol5));
                        }
                    }
                    break;
                case Key.D6:
                    {


                        int pitch = octavecount * 12 + 8;
                        int buf1 = pitch << 8;

                        if (!solshapdown)
                        {
                            solshapdown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.SolSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.SolSharp5));
                        }
                    }
                    break;
                case Key.Y:
                    {

                        int pitch = octavecount * 12 + 9;
                        int buf1 = pitch << 8;
                        if (!ladown)
                        {
                            ladown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.La5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.La5));
                        }
                    }
                    break;
                case Key.D7:
                    {


                        int pitch = octavecount * 12 + 10;
                        int buf1 = pitch << 8;

                        if (!lashapdown)
                        {
                            lashapdown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                            TB_NoteList.Text += MusicScale.LaSharp5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.LaSharp5));
                        }
                    }
                    break;
                case Key.U:
                    {

                        int pitch = octavecount * 12 + 11;
                        int buf1 = pitch << 8;
                        if (!sidown)
                        {
                            sidown = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);

                            TB_NoteList.Text += MusicScale.Si5 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Si5));
                        }
                    }
                    break;
                case Key.I:
                    {

                        int pitch = (octavecount + 1) * 12;
                        int buf1 = pitch << 8;
                        if (!do6down)
                        {
                            do6down = true; MidiShortMsgPlayer.SendMidiShortMsg(buf1 + buf2 + msgch);
                            TB_NoteList.Text += MusicScale.Do6 + " Term(" + Key_Beat + ")";
                            muscialNote.Add(new NomalNote(Key_Beat, MusicScale.Do6));
                        }
                    }
                    break;
                default: break;
            }
        }
        #endregion

        
    }
}
