using System.Collections.Generic;
using System.Linq;

namespace JahnStarGames.Langpipe
{
    public class PipelineVerbose
    {
        private string id;
        private List<VerboseLog> loglist = new();
        private bool debugHighVerbose;
        public PipelineVerbose(bool debugHighVerbose = false)
        {
            id =  System.Guid.NewGuid().ToString()[..5];
            this.debugHighVerbose = debugHighVerbose;
        }
        public void Log(string message, bool highVerbose = false)
        {
            if (debugHighVerbose) 
            {
                if (!highVerbose || loglist.Count == 0) loglist.Add(new VerboseLog(GetPrefix(), message));
                else loglist.Last().Log(message);
            }
            else if (!highVerbose) loglist.Add(new VerboseLog(GetPrefix(), message));
        }
        public void LogError(string message)
        {
            loglist.Add(new VerboseLog(GetPrefix(), message, true));
        }
        public string GetPrefix() => id + "-" + (loglist.Count + 1);
    }
    internal class VerboseLog
    {
        private string prefix;
        private string log;
        private List<string> tree = new();
        public VerboseLog(string prefix, string log, bool error = false)
        {
            if (!error) Debug(prefix + ": " + log);
            else Debug(prefix + ": " + log, true);
            //
            this.prefix =  prefix;
            this.log = log;
        }
        public void Log(string message)
        {
            int index = tree.Count + 1;
            message = prefix + "." + index + ": " + message;
            tree.Add(message);
            Debug(message);
        }
        public void LogError(string message)
        {
            int index = tree.Count + 1;
            message = prefix + "." + index + ": " + message;
            tree.Add(message);
            Debug(message, true);
        }
        private void Debug(string message, bool error = false)
        {
            #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            if (!error) UnityEngine.Debug.Log(message);
            else UnityEngine.Debug.LogError(message);
            #else
            if (!error) System.Console.WriteLine(message);
            else System.Console.Error.WriteLine(message);
            #endif
        }
    }
}