using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using MyMusic;
namespace MIDICREACT
{
    /// <summary>
    /// ReadMidi.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReadMidi : Window
    {

        MidiHeaderChunkData midiheader;
        MidiTrackChunkData miditrack;
        BinaryReader MidifileBr;
        public string FileName { get; private set; }
        public ReadMidi()
        {
            InitializeComponent();
            TB_Header.Text = "잘못된 접근입니다.";
        }
        public ReadMidi(MidiTrackChunkData midtrack,MidiHeaderChunkData header,string filename)
        {
            InitializeComponent();
            this.midiheader = header;
            miditrack = midtrack;
            FileName = filename;
            TB_Header.Text = midiheader.MidiChunkID;
            TB_Length.Text = midiheader.Length.ToString();
            TB_Fomat.Text = midiheader.format.ToString();
            TB_NoteLength.Text = midiheader.timedivision.ToString();
            TB_TrackCunt.Text = midiheader.trackcount.ToString();
        }

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Create a TreeViewItem.
            TreeViewItem item = new TreeViewItem();
            FileName = FileName.Remove(FileName.LastIndexOf('.'));
           
            for(bool check =false;check !=true ;check=false)
            {
              int cnt=  FileName.IndexOf('\\');
                if (cnt>=0)
                {
                    FileName = FileName.Substring(cnt+1);
                }
                else
                {
                    check = true;
                    break;
                }
            }//파일이름 거르기
            item.Header = FileName;
            string[] eventss = new string[miditrack.events.Count+2];
            eventss[0] = miditrack.TrackID;
            eventss[1] = miditrack.trackSize.ToString();
            int i = 0;
            foreach(Event ev in miditrack.events)
            {
               
                MidiEvent mr = ev as MidiEvent;
                if(mr!=null)
                {
                    if (mr.data2 == 0)
                    {
                       
                        mr.type = "NoteOff";
                    }
                    if (mr.msg == 192)
                    {
                        mr.type = "setIns";
                    }

                    eventss[i] = string.Format("MIDI 메시지 [{0}] time [{1}] data [{2}] data [{3}] ", ev.type, ev.time,mr.data1,mr.data2);


                }
                else
                {
                    eventss[i] = string.Format("MATA 메시지{0} 델타타임 {1}", ev.type, ev.time);

                }
                 i++;
            }
            item.ItemsSource = eventss;


          var tree = sender as TreeView;
            tree.Items.Add(item);
        }

        /// <summary>
        /// 아이탬눌럿을시 반응
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tree.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                var item = tree.SelectedItem as TreeViewItem;
                this.Title = "Selected header: " + item.Header.ToString();
            }
            else if (tree.SelectedItem is string)
            {
                // ... Handle a string.
                this.Title = "Selected: " + tree.SelectedItem.ToString();
            }
        }

        //public ReadMidi(BinaryReader br,string FileName)
        //{
        //    MidifileBr = br;
        //    this.FileName = FileName;
        //    InitializeComponent();
        //    midiheader = new MidiHeaderChunkData(br);
        //   TB_Header.Text=  midiheader.MidiChunkID;
        //    TB_Length.Text = midiheader.Length.ToString();
        //    TB_Fomat.Text = midiheader.format.ToString();
        //    TB_NoteLength.Text = midiheader.timedivision.ToString();
        //    TB_TrackCunt.Text = midiheader.trackcount.ToString();
           
            
        //}
    }
}
