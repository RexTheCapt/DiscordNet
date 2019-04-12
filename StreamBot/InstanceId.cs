#region usings

using System;

#endregion

namespace StreamBot
{
    public class InstanceId
    {
        private int _id;

        public InstanceId()
        {
            NewId();
        }

        public string NewId()
        {
            Random rdm = new Random();
            _id = rdm.Next(int.MinValue, int.MaxValue);
            return Id();
        }

        public string Id()
        {
            return _id.ToString("x8");
        }
    }
}