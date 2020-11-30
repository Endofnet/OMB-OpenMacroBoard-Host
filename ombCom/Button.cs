using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ombCom
{
    public class Button
    {
        public enum effect
        {
            off,
            solid,
            toggle,
            fade,
            invFade,
            onPush,
            onIdle,
            remote
        }

        public Color LedColor { get; set; } = Color.White;
        public effect LedEffect { get; set; } = effect.solid;
        public int ButtonNumber { get; set; } = 0;
        public string Command { get; set; } = "";  

        public Button(int _num, Color _col, effect _eff)
        {
            ButtonNumber = _num;
            LedColor = _col;
            LedEffect = _eff;
        }

        public Button(string dumpLine)
        {
            // Split into segments
            string[] segments = dumpLine.Split(":");
            if (segments.Length >= 4)
            {
                ButtonNumber = int.Parse(segments[0]);              // First part is the Number
                LedColor = Color.FromArgb(int.Parse(segments[1]));  // Second part is the Color
                LedEffect = (effect)int.Parse(segments[2]);         // Third part is the effect
                Command = "";
                for(int i = 3; i < segments.Length; i++)            // Last parts are command, that can be more if commadn contains ":"
                {
                    Command += segments[i];
                }
            }
        }
    }
}
