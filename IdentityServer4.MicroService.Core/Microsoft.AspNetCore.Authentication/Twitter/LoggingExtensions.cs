using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    internal static class LoggingExtensions
    {
        private static Action<ILogger, Exception> _obtainRequestToken;
        private static Action<ILogger, Exception> _obtainAccessToken;
        private static Action<ILogger, Exception> _retrieveUserDetails;

        static LoggingExtensions()
        {
            _obtainRequestToken = LoggerMessage.Define(
                eventId: 1,
                logLevel: LogLevel.Debug,
                formatString: "ObtainRequestToken");
            _obtainAccessToken = LoggerMessage.Define(
                eventId: 2,
                logLevel: LogLevel.Debug,
                formatString: "ObtainAccessToken");
            _retrieveUserDetails = LoggerMessage.Define(
                eventId: 3,
                logLevel: LogLevel.Debug,
                formatString: "RetrieveUserDetails");

        }

        public static void ObtainAccessToken(this ILogger logger)
        {
            _obtainAccessToken(logger, null);
        }

        public static void ObtainRequestToken(this ILogger logger)
        {
            _obtainRequestToken(logger, null);
        }

        public static void RetrieveUserDetails(this ILogger logger)
        {
            _retrieveUserDetails(logger, null);
        }
    }
}
