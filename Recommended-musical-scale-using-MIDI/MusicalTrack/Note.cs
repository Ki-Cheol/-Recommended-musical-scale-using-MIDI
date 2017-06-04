using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicalTrackLib
{
    public class Note
    {
        public StaffPosition staffPosition { get; set; }

        protected string GetNoteTime(int tsNoteCount_r)
        {
            if (tsNoteCount_r > 32)
            {
                if (tsNoteCount_r % 32 == 0)
                {
                    return "온*" + (tsNoteCount_r / 32).ToString();
                }
                return "온*" + (tsNoteCount_r / 32).ToString() + "+" + GetNoteTime(tsNoteCount_r % 32);
            }

            switch (tsNoteCount_r)
            {
                case 0: return "";
                case 1: return "32";
                case 2: return "16";
                case 3: return "점16";
                case 4: return "8";
                case 5: return "8+32";
                case 6: return "점8";
                case 7: return "점8+32";
                case 8: return "4";
                case 12: return "점4";
                case 16: return "2";
                case 24: return "점2";
                case 32: return "온";
                default:
                    {
                        if (32 > tsNoteCount_r && 24 < tsNoteCount_r)
                        {
                            return "점2+" + GetNoteTime(tsNoteCount_r - 24);
                        }
                        else if (24 > tsNoteCount_r && 16 < tsNoteCount_r)
                        {
                            return "2+" + GetNoteTime(tsNoteCount_r - 16);
                        }
                        else if (16 > tsNoteCount_r && 12 < tsNoteCount_r)
                        {
                            return "점4+" + GetNoteTime(tsNoteCount_r - 12);
                        }
                        else if (12 > tsNoteCount_r && 8 < tsNoteCount_r)
                        {
                            return "4+" + GetNoteTime(tsNoteCount_r - 8);
                        }
                        else
                        {
                            return "?";
                        }
                    }
            }
        }

            public override string ToString()
        {
            NomalNote no = this as NomalNote;
            if (no != null)
            {
                return no.ToString();
            }
            else if (this is Harmony)
            {
                Harmony ha = this as Harmony;
                ha.ToString();
            }
            else
            {
                RestNote re = this as RestNote;
                re.ToString();
            }
            return null;
        }
    }

}
