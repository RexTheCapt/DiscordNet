using System.Diagnostics;
using System.Reflection;

namespace DiscordNet
{
    public static class BotInfo
    {
        public static string Prefix
        {
            get => BotUser.Default.Prefix;
            set
            {
                BotUser.Default.Prefix = value;
                BotUser.Default.Save();
            }
        }

        public static readonly InstanceId InstanceId = new InstanceId();

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
    }
}