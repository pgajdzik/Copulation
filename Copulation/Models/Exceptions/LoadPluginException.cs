using System;

namespace Copulation.Models.Exceptions
{
    public class LoadPluginException : Exception
    {
        private string p;

        public LoadPluginException(Exception innerException)
            : base(string.Empty, innerException)
        {
        }

        public LoadPluginException(string message)
            : base(message)
        {
        }
    }
}