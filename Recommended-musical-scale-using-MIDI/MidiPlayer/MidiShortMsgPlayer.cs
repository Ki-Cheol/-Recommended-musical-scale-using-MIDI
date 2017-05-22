using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MidiPlayLib
{
    public class MidiShortMsgPlayer
    {
        [DllImport("winmm.dll")]
        static extern uint midiOutOpen(ref IntPtr lphMidiOut, uint uDeviceID,
            IntPtr dwCallback, IntPtr dwInstance, uint dwFlags);

        [DllImport("winmm.dll")]
        protected static extern int midiOutShortMsg(IntPtr handle, uint message);

        [DllImport("winmm.dll")]
        static extern uint midiOutClose(IntPtr hMidiOut);

        static IntPtr hmidi = IntPtr.Zero;

        /// <summary>
        /// WINMM.DLL 미디파일열기
        /// </summary>
        static public void MidiOpen()
        {
            midiOutOpen(ref hmidi, 0, IntPtr.Zero, IntPtr.Zero, 0);
            
        }
        /// <summary>
        /// MIDIEVENT전달
        /// </summary>
        /// <param name="GetWinmmMsg"></param>
        static public void SendMidiShortMsg(int GetWinmmMsg)
        {
            if (hmidi == IntPtr.Zero)
            {
                midiOutOpen(ref hmidi, 0, IntPtr.Zero, IntPtr.Zero, 0);
            }
            midiOutShortMsg(hmidi, (uint)GetWinmmMsg);
        }


        static public void StopMusic()
        {
            midiOutClose(hmidi);
            hmidi = IntPtr.Zero;
        }
    }
}
