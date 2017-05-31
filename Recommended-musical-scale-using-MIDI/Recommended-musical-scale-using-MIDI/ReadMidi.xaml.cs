using MidiChunkDataLib;
using MusicalTrackLib;
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

namespace Recommended_musical_scale_using_MIDI
{
    /// <summary>
    /// ReadMidi.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReadMidi : Window
    {
        public string FileName { get; private set; }
        MidiHeaderChunkData midiheader;
        List<MidiTrackChunkData> miditrack;
        public ReadMidi(string filename)
        {
            InitializeComponent();
            MidiChunkData midichunk = new MidiChunkData(filename);
           
            this.midiheader = midichunk.header ;
            
            miditrack = midichunk.trackList;
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
         
            FileName = FileName.Remove(FileName.LastIndexOf('.'));

            for (bool check = false; check != true; check = false)
            {
                int cnt = FileName.IndexOf('\\');
                if (cnt >= 0)
                {
                    FileName = FileName.Substring(cnt + 1);
                }
                else
                {
                    check = true;
                    break;
                }
            }//파일이름 거르기
           
           
            foreach (MidiTrackChunkData mc in miditrack)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = FileName;
                string[] eventss = new string[mc.events.Count];
               // eventss[0] = mc.TrackID;
               // eventss[1] = mc.trackSize.ToString();
                int i = 0;
                foreach (Event ev in mc.events)
                {

                    MidiEvent mr = ev as MidiEvent;
                    if (mr != null)
                    {
                        

                        eventss[i] = string.Format("MIDI type [{0}] time [{1}] data [{2}] data [{3}] ", ev.type, ev.time, mr.data1, mr.data2);


                    }
                    else
                    {
                        MetaEvent Meta = ev as MetaEvent;
                        char[] arr = new char[Meta.dataLen];
                        arr.Initialize();
                        for(int datacnt =0; datacnt<Meta.dataLen;datacnt++)
                        {
                            arr[datacnt] = (char)Meta.data[datacnt];
                        }
                        if(Meta.type== "TrackName")
                        {
                            item.Header = new string(arr);
                        }
                        eventss[i] = string.Format("MATA type{0}  META길이{1} META데이터{2}",Meta.type, Meta.dataLen, new string(arr));

                    }
                    i++;
                }
               
                item.ItemsSource = eventss;


                var tree = sender as TreeView;
                tree.Items.Add(item);
                TEST.Text = string.Empty;
                foreach (Note note in  mc.musicalTrack.notelist)
                {
                    if(note is NomalNote)
                    {
                        NomalNote nmnote = note as NomalNote;
                        TEST.Text += (int)nmnote.beat+"\n";
                        TEST.Text += (int)nmnote.scale + "\n";
                    }
                }

            }
            
        }
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

    }
}
