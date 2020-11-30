using System;
using System.Drawing;
using System.IO.Ports;
using System.Collections.Generic;

namespace ombCom
{
    public class OMB
    {
        private SerialPort port;
        private int brightness = 127;
        
        public event EventHandler DumpReceived;

        #region Aliases
        public static readonly string STXT = "~t#";
        public static readonly string ETXT = "#t~";
        public static readonly string BTDN = "#b~";
        public static readonly string BTUP = "~b#";
        public static readonly string KEYC = "~k#";
        #endregion

        #region Commands
        public static readonly string CMD_RGB = "rgb";
        public static readonly string CMD_HUE = "hue";
        public static readonly string CMD_EFFECT = "eff";
        public static readonly string CMD_BRIGHT = "brg";
        public static readonly string CMD_CMD = "cmd";
        public static readonly string CMD_LOAD = "loa";
        public static readonly string CMD_SAVE = "sav";
        public static readonly string CMD_CLEAR = "clr";
        public static readonly string CMD_DUMP = "dmp";
        public static readonly string CMD_TEMP = "tmp";
        public static readonly string CMD_RESET = "rst";
        #endregion

        public List<Button> Buttons = new List<Button>();

        public OMB(string _port)
        {
            port = new SerialPort
            {
                NewLine = "\n",
                PortName = _port,
                RtsEnable = true
            };
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public void Connect()
        {
            if(port.PortName != "")
            {
                try
                {
                    port.Open();
                    RequestDump();
                }
                catch
                {

                }
            }
        }

        #region Commands
        /// <summary>
        /// Sends all button settings to port
        /// </summary>
        /// <param name="btn"></param>
        public void SendButton(Button btn)
        {
            SetStringCmd(btn.ButtonNumber, btn.Command);
            SetColor(btn.ButtonNumber, btn.LedColor);
            SetEffect(btn.ButtonNumber, btn.LedEffect);
        }

        /// <summary>
        /// Sends a command string to given button number
        /// </summary>
        /// <param name="_button"></param>
        /// <param name="_command"></param>
        public void SetStringCmd(int _button, string _command)
        {
            if (!port.IsOpen) return;
            string send = CMD_CMD;
            send += _button.ToString("D3");
            send += _command;
            port.WriteLine(send);
        }

        /// <summary>
        /// Sets the color for given button number to given rgb value
        /// </summary>
        /// <param name="_button"></param>
        /// <param name="_r"></param>
        /// <param name="_g"></param>
        /// <param name="_b"></param>
        public void SetColor(int _button, int _r, int _g, int _b)
        {
            if (!port.IsOpen) return;
            string send = CMD_RGB;
            send += _button.ToString("D3");
            send += _r.ToString("D3");
            send += _g.ToString("D3");
            send += _b.ToString("D3");
            port.WriteLine(send);
        }
        /// <summary>
        /// Sets the color for given button number to given color value
        /// </summary>
        /// <param name="_button"></param>
        /// <param name="_col"></param>
        public void SetColor(int _button, Color _col)
        {
            if (!port.IsOpen) return;
            string send = CMD_RGB;
            send += _button.ToString("D3");
            send += _col.R.ToString("D3");
            send += _col.G.ToString("D3");
            send += _col.B.ToString("D3");
            port.WriteLine(send);
        }

        /// <summary>
        /// Sets the Effect of given led to given effect
        /// </summary>
        /// <param name="_button"></param>
        /// <param name="_effect"></param>
        public void SetEffect(int _button, Button.effect _effect)
        {
            if (!port.IsOpen) return;
            string send = CMD_EFFECT;
            send += _button.ToString("D3");
            send += ((int)_effect).ToString("D3");
            port.WriteLine(send);
        }

        /// <summary>
        /// Sets brightness
        /// </summary>
        public void SetBrightness()
        {
            if (!port.IsOpen) return;
            string send = CMD_BRIGHT;
            send += brightness.ToString("D3");
            port.WriteLine(send);
        }

        /// <summary>
        /// Saves current config to eeprom
        /// </summary>
        public void SaveConfig()
        {
            if (!port.IsOpen) return;
            string send = CMD_SAVE;
            port.WriteLine(send);
        }

        /// <summary>
        /// Loads config from eeprom
        /// </summary>
        public void LoadConfig()
        {
            if (!port.IsOpen) return;
            port.WriteLine(CMD_LOAD);
        }

        /// <summary>
        /// Erases saved config from eeprom
        /// </summary>
        public void ClearConfig()
        {
            if (!port.IsOpen) return;
            string send = CMD_CLEAR;
            port.WriteLine(send);
        }

        /// <summary>
        /// Resets the omb, killing the serial connection
        /// </summary>
        public void ResetOMB()
        {
            if (!port.IsOpen) return;
            string send = CMD_RESET;
            port.WriteLine(send);
            port.Close();
        }

        /// <summary>
        /// Gets Button settings from OMB
        /// </summary>
        public void RequestDump()
        {
            if (!port.IsOpen) return;
            port.WriteLine(CMD_DUMP);
        }

        public void SendSystemInfo(int _cpuTemp, int _cpuLoad, int _gpuTemp, int _gpuLoad)
        {
            if (!port.IsOpen) return;
            string send = CMD_TEMP;
            send += _cpuTemp.ToString("D3");
            send += _cpuLoad.ToString("D3");
            send += _gpuTemp.ToString("D3");
            send += _gpuLoad.ToString("D3");
            port.WriteLine(send);
        }

        #endregion

        #region Callbacks
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();
            Buttons.Clear();
            foreach (string line in data.Split("\r\n"))
            {
                if (line != "")
                {
                    Button btn = new Button(line);
                    Buttons.Add(btn);
                }
            }
            DumpReceived?.Invoke(this, null);
        }
        #endregion

        #region Statics
        public static string BtDown(int keycode)
        {
            string ret = BTDN;  // Add ButtonDown Command
            ret += KEYC;        // Add Keycode Command
            ret += keycode.ToString("D3");
            return ret;
        }
        public static string BtDown(char key)
        {
            string ret = BTDN;  // Add ButtonDown Command
            ret += key; 
            return ret;
        }

        public static string BtUp(int keycode)
        {
            string ret = BTUP;  // Add ButtonUp Command
            ret += KEYC;        // Add Keycode Command
            ret += keycode.ToString("D3");
            return ret;
        }
        public static string BtUp(char key)
        {
            string ret = BTUP;  // Add ButtonUp Command
            ret += key;
            return ret;
        }
        #endregion
    }
}
