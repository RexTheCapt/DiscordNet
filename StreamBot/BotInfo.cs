#region usings

using System;
using System.Diagnostics;
using System.Reflection;
using DiscordNet;

#endregion

namespace StreamBot
{
    public static class BotInfo
    {
        public static readonly InstanceId InstanceId = new InstanceId();

        public static DateTime StartDateTime = DateTime.Now;

        public static string Prefix
        {
            get => BotUser.Default.Prefix;
            set
            {
                BotUser.Default.Prefix = value;
                BotUser.Default.Save();
            }
        }

        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;
                return version;
            }
        }

        public static bool BotIsPaused = false;
    }
}