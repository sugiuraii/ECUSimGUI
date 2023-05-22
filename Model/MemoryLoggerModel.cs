using System;
using System.Collections.Generic;
using Logging.Memory;
using Microsoft.Extensions.Logging;

namespace SZ2.ECUSimGUI.Model
{
    public class MemoryLoggerModel
    {
        public List<(DateTime time, LogLevel level, string line)> GetTimeLevelAndLog(List<LogLevel> logLevelsToGet)
        {
            var returnVal = new List<(DateTime time, LogLevel level, string line)>();
            foreach(var level in logLevelsToGet)
                foreach(var logcontent in MemoryLogger.GetLogWithTime(level))
                    returnVal.Add((logcontent.time, level, logcontent.line));

            returnVal.Sort((e1, e2) => -DateTime.Compare(e1.time, e2.time));
            return returnVal;
        }
    }
}