﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

using ombCom;

namespace OMBHostDebug
{
    public partial class Form1 : Form
    {
        public OMB omb;

        private bool update = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void bt_connect_Click(object sender, EventArgs e)
        {
            omb = new OMB(comboBox1.Text);
            omb.DumpReceived += DumpReceivedHandler;
            omb.Connect();
            omb.LoadConfig();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach(string port in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(port);
            }
        }

        private void DumpReceivedHandler(object sender, EventArgs e)
        {
            update = true;
        }

        private void ti_ui_Tick(object sender, EventArgs e)
        {
            if(update)
            {
                listBox1.Items.Clear();
                foreach (ombCom.Button btn in omb.Buttons)
                {
                    listBox1.Items.Add("Button " + (btn.ButtonNumber + 1).ToString("D2"));
                }
                update = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.Clear();
                listBox2.Items.Add(omb.Buttons[listBox1.SelectedIndex].ButtonNumber);
                listBox2.Items.Add(omb.Buttons[listBox1.SelectedIndex].Command);
                listBox2.Items.Add(omb.Buttons[listBox1.SelectedIndex].LedColor.R);
                listBox2.Items.Add(omb.Buttons[listBox1.SelectedIndex].LedColor.G);
                listBox2.Items.Add(omb.Buttons[listBox1.SelectedIndex].LedColor.B);
                listBox2.Items.Add(omb.Buttons[listBox1.SelectedIndex].LedEffect);
            }
            catch
            {
                //Nothing
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    omb.SetColor(listBox1.SelectedIndex, colorDialog1.Color);
                }
                catch
                {

                }
            }
        }
    }
}
