using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    public partial class ListBoxProxy : ListBox
    {
        private readonly object m_lockTimer = new object();
        private Timer m_Timer;
        private DateTime m_EndTime;

        public ListBoxProxy()
        {
            InitializeComponent();
        }

        public override void ResetBackColor()
        {
                createTimeInterval(100);
                this.BackColor = Color.DarkCyan;
        }

        private void createTimeInterval(int i_MiliSecsToWait)
        {
            lock (m_lockTimer)
            {
                m_Timer = new Timer();
                m_EndTime = DateTime.Now.AddSeconds(3);
                m_Timer.Interval = i_MiliSecsToWait;
                m_Timer.Tick += OnTimedEvent;
                m_Timer.Start();
            }
        }

        private void switchColor()
        {
            if (this.BackColor == Color.DarkCyan)
            {
                this.BackColor = Color.LightBlue;
            }
            else
            {
                this.BackColor = Color.DarkCyan;
            }
        }

        private void OnTimedEvent(object source, EventArgs e)
        {
            lock (m_lockTimer)
            {
                switchColor();
                if (DateTime.Now >= m_EndTime)
                {
                    m_Timer.Stop();
                    base.ResetBackColor();
                }
            }
        }
    }
}
