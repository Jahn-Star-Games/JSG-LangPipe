// fill the script
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JahnStarGames.Langpipe
{
    public interface IBaseMemory: IEnumerable<Message>
    {
        private readonly static string cachePath;
        public string key { get; }
        public List<Message> messages { get; }
        public PipelineVerbose verbose { get; set; }
        void Add(Message item);
        bool LoadFromCache();
        bool SaveToCache();
    }
    public class BaseMemoryException : Exception
    {
        public BaseMemoryException(string message) : base(message) { }
    }
}
