using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsFormsClipboardWatcher;
namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private static DateTime runTime = DateTime.Now;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(var i in Enum.GetNames(typeof(RegexOptions)))
            {
                if (i == "None") continue;
                ToolStripMenuItem tsmi= new ToolStripMenuItem(i,null, (snd, evta) =>
                  {
                      if (snd is ToolStripMenuItem mi)
                      {
                          mi.Checked = !mi.Checked;
                      }
                  });
                if (i == "Multiline") tsmi.Checked = true;
                regexOptionToolStripMenuItem.DropDownItems.Add(tsmi);

            }
            ClipboardWatcherSetup();
        }
        private void ClipboardWatcherSetup()
        {
            ClipboardWatcher.ClipboardUpdate += (a, b) =>
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts = dt.Subtract(runTime);
                if (ts.TotalSeconds < 1) return;
                runTime = dt;
                if (InvokeRequired)
                {
                    Invoke(new Action(ClipboardEdit));
                }
                ClipboardEdit();

            };
        }

        private void ClipboardEdit()
        {
            if (checkBox1.Checked) return;
            string getText = Clipboard.GetText();
            if (getText == "") return;

            textBox1.Text = getText;

            RegexOptions ro = RegexOptions.None;
            foreach (ToolStripMenuItem i in regexOptionToolStripMenuItem.DropDownItems)
            {
                if (i.Checked) ro |= (RegexOptions)Enum.Parse(typeof(RegexOptions), i.Text);
            }
            try
            {
                Regex reg = new Regex(textBox2.Text,ro);

                string replaceText = reg.Replace(getText, textBox3.Text);
                textBox4.Text = replaceText;
                Clipboard.SetText(replaceText);
                ErrLabel.Text = "";
            }
            catch (System.ArgumentException e)
            {
                ErrLabel.Text = e.Message;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            textBox2.Enabled = textBox3.Enabled = !checkBox1.Checked;
        }
    }
}
