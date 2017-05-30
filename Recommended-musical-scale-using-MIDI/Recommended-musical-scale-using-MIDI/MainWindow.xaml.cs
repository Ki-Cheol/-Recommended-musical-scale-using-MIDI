using Microsoft.Win32;
using MidiChunkDataLib;
using MidiPlayLib;
using MusicalTrackLib;
using System;
using System.Collections.Generic;
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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Note> scalebase;
        public MainWindow()
        {
            InitializeComponent();
          scalebase = keyname.muscialNote;
            CB_InstrumentInit();
            CB_BeatInit();
            //초기 4분음표를 주기 위하여
            keyname.Key_Beat = (MusicBeat)Enum.Parse(typeof(MusicBeat), CB_Beat.Text);

        }
       private void CB_BeatInit()
       {
            foreach(string s  in Enum.GetNames(typeof(MusicBeat)))
            {
                CB_Beat.Items.Add(s);
            }
            CB_Beat.SelectedItem = CB_Beat.Items[5];
        }
        private void CB_InstrumentInit()
        {
            foreach (string name in Enum.GetNames(typeof(Instrument)))
            {
                CB_Instrument.Items.Add(name);
            }
            CB_Instrument.SelectedItem = CB_Instrument.Items[0];
        }
        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "*.mid|*.mid|*.*|*.*";
            if (sf.ShowDialog() !=true)
            {
                throw new NotFiniteNumberException("잘못된 세이브파일입니다.");
            }
            MusicalTrack ms = new MusicalTrack(sf.FileName);
            ms.instType = (int)(Instrument)Enum.Parse(typeof(Instrument) ,CB_Instrument.Text);
           
           MusicInfo msi= ms.musicInfo;
            // stafftype 
            msi.staffType = 0;

            MessageBox.Show(msi.key.ToString()+msi.staffType.ToString()+msi.tempoList.ToString()+msi.timeSignatureList.ToString());

            foreach (Note note in scalebase)
            {
                if (note is NomalNote)
                {
                    ms.AddNote(note);
                }
                else if(note is Harmony)
                {
                    throw new KeyNotFoundException("미구현");
                }

            }
            
            MidiChunkData mc = new MidiChunkData(ms,sf.FileName);
           
            mc.SaveMidiFile(sf.FileName);

        }

        private void BT_ChangeInstrument_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[4];

            data[0] = 0xC0;//change instrument, channel 0

            CB_Instrument.Items[CB_Instrument.SelectedIndex].ToString();
            data[1] = (byte)(Instrument)Enum.Parse(typeof(Instrument), CB_Instrument.Text);

            uint msg = BitConverter.ToUInt32(data, 0);

            MidiShortMsgPlayer.SendMidiShortMsg((int)msg);
            keyname.Focus();


        }

        private void BT_Beat_Click(object sender, RoutedEventArgs e)
        {
            keyname.Key_Beat = (MusicBeat)Enum.Parse(typeof(MusicBeat),CB_Beat.Text);
            keyname.Focus();
        }

        private void BT_Lead_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.mid|*.mid|*.*|*.*";
            if (ofd.ShowDialog() != true)
            {
                return;
            }
           
            ReadMidi midiread = new ReadMidi(ofd.FileName);
           
            midiread.Show();
        }

        private void BT_OpenSerial_Click(object sender, RoutedEventArgs e)
        {
            SerialPortConnect sc = new SerialPortConnect();
            sc.ShowDialog();
        }
    }
}
