#region usings

using System;
using System.Windows.Forms;
using DiscordNet;

#endregion

namespace DiscordNetGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            BotUser.Default.Token = textBox1.Text;

            new DiscordBot().MainAsync().GetAwaiter().GetResult();
        }
    }
}