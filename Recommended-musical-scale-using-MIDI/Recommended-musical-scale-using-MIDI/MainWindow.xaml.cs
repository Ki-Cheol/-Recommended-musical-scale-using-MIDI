using Microsoft.Win32;
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
        List<MusicalTrackLib.MusicScale> scalebase;
        public MainWindow()
        {
            InitializeComponent();
          scalebase = keyname.ScaleBase;
            CB_InstrumentInit();

        }
        private void CB_InstrumentInit()
        {
            foreach (string name in Enum.GetNames(typeof(Instrument)))
            {
                CB_Instrument.Items.Add(name);
            }
        }
        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "*.mid|*.mid|*.*|*.*";
            if (sf.ShowDialog() !=true)
            {
                throw new NotFiniteNumberException("잘못된 세이브파일입니다.");
            }
            
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
    }
}
