using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicalTrackLib
{
    public enum MusicScale
    {
        Do0, DoSharp0, Re0, ReSharp0, Mi0, Fa0, FaSharp0, Sol0, SolSharp0, La0, LaSharp0, Si0,
        Do1, DoSharp1, Re1, ReSharp1, Mi1, Fa1, FaSharp1, Sol1, SolSharp1, La1, LaSharp1, Si1,
        Do2, DoSharp2, Re2, ReSharp2, Mi2, Fa2, FaSharp2, Sol2, SolSharp2, La2, LaSharp2, Si2,
        Do3, DoSharp3, Re3, ReSharp3, Mi3, Fa3, FaSharp3, Sol3, SolSharp3, La3, LaSharp3, Si3,
        Do4, DoSharp4, Re4, ReSharp4, Mi4, Fa4, FaSharp4, Sol4, SolSharp4, La4, LaSharp4, Si4,
        Do5, DoSharp5, Re5, ReSharp5, Mi5, Fa5, FaSharp5, Sol5, SolSharp5, La5, LaSharp5, Si5,        //기본
        Do6, DoSharp6, Re6, ReSharp6, Mi6, Fa6, FaSharp6, Sol6, SolSharp6, La6, LaSharp6, Si6,
        Do7, DoSharp7, Re7, ReSharp7, Mi7, Fa7, FaSharp7, Sol7, SolSharp7, La7, LaSharp7, Si7,
        Do8, DoSharp8, Re8, ReSharp8, Mi8, Fa8, FaSharp8, Sol8, SolSharp8, La8, LaSharp8, Si8,
        Do9, DoSharp9, Re9, ReSharp9, Mi9, Fa9, FaSharp9, Sol9, SolSharp9, La9, LaSharp9, Si9,
        Do10, DoSharp10, Re10, ReSharp10, Mi10, Fa10, FaSharp10, Sol10
    }
    public enum Instrument
    {
        //0~8 피아노
        GrandPiano, AcousticGrand, BrightAcoustic, ElectricGrand, Honky_Tonk, Electric_Piano1, Electric_Piano2, Harpsichord, Clavinet = 8,
        //9~16 chromatic 
        elesta = 9, Glockenspiel, Music_Box, Vibraphone, Marimba, Xylophone, Tublar_Bells = 15, Dulcimer = 16,
        //17~24 ORGAN 오르간
        Drawber_Organ = 17, Percussive_Organ, Rock_Organ, Church_Organ = 20, Reed_Organ, Accordion, Harmonica, Tango_Accordian,
        //25-32 기타
        Acoustic_Guitar_nylon = 25, Acoustic_Guitar_steel = 26, Electoric_Guitar_jazz, Electoric_Guitar_clean, Electoric_Guitar_muted, Overdriven_Guitar, Guitar_Harmonics = 32,
        //33~40 베이스
        Acoustic_Bass = 33, Electoric_Bass_finger, Electoric_Bass_pick, Fietless_Base, Slap_Bass1, Slap_Bass2, Synth_Bass1, Synth_Bass2,
        //41-48 줄치는거 바이올린류
        Violin = 41, Viola, Cello, Contrabass, Tremolo_Strings, Pizzicato_sTRINGS, Orchestral_Strings, Timpani,
        //49-56 ensemble
        String_Ensemble1, String_Ensemble2, SynthStrings1, SynthStrings2, ChoirAshs, Voice_Oohs, Synth_Voice, Orchestra_Hit,
        //57~64BRASS
        Trympet, Trombone, Tuba, Muted_Trumpet, French_Horn, Brass_Section, SynthBrass1, SynthBrass2,
        //65~72Reed 
        Soprano_Sax, Alto_Sax, Tenor_Sax, Baritone_Sax, Oboe, English_Horn, Bassoon, Clarinet,
        //73~80 PIPE
        Piccolo, Flute, Recorder, Pan_Flute, Blown_Bottle, Skakuhachi, Whistle, Ocarina,
        //81-88 systhlead
        Lead1_square, Lead2_sawtooth, Lead3_calliope, Lead4_chiff, Lead5_charang, Lead6_voice, Lead17_fifths, Lead18_basepluslead,
        //89-9

    }


    struct DB_Note
    {
        int Scale;
        int MuscialBeat;
        int Notecnt;
    }
    public enum MusicBeat
    {
        note32 = 1, note16, note16dot, note8, note8dot = 6, note4 = 8, note4dot = 12, note2 = 16,
        note2dot = 24, onenote = 32
    }


    public struct StaffPosition //(악보에서의 위치)
    {
        public int measure; //마디 0~
        public int beat;    //박자 0~3
        public double tick;  //32분박자 0~7
    }

    public struct Tempo
    {
        public int position;
        public float frame;
        public StaffPosition staffPosition;

        public int data;    // (60000000(1분의 마이크로초) / tempo)

        /// <summary>
        /// 템포가져오기
        /// </summary>
        /// <returns></returns>
        public float GetTempoByData()
        {
            return (float)60000000 / (float)data;
        }
        /// <summary>
        /// 템포변경
        /// </summary>
        /// <param name="tempo">변경할 탬포</param>
        public void SetDataByTempo(float tempo)
        {
            this.data = (int)(60000000 / tempo);
        }

        public override string ToString()
        {
            return "dt:" + position.ToString() + " data:" + data.ToString();
        }
    }

    public struct TimeSignature
    {
        public int position;
        public float frame;
        public StaffPosition staffPosition;

        public int numerator;   //분자 0
        public int denominator; //분모 1

        public override string ToString()
        {
            return numerator.ToString() + "/" + denominator.ToString();
        }
    }
    public enum _KeyVal
    {
        Q = 81, W = 87, E = 69, R = 82
    }
}
