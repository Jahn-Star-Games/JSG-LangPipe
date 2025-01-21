// fill the script
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace JahnStarGames.Langpipe
{
    public class BufferMemory : IBaseMemory
    {
        private static string cachePath;
        public string key { get; set; }
        public List<Message> messages { get; set; } = new();
        public PipelineVerbose verbose { get; set; }
        
        public BufferMemory(string key, List<Message> transferMessages = null, PipelineVerbose verbose = null)
        {
            this.key = key;
            if (transferMessages != null) this.messages = transferMessages;
            this.verbose = verbose ?? new();
            
            cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Langpipe", "MemoryCache");
            #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            cachePath = Path.Combine(UnityEngine.Application.persistentDataPath, "Langpipe", "MemoryCache");
            #endif
        }

        public virtual void Add(Message value)
        {
            messages.Add(value);
        }

        // TODO: Create a abstract class for IBaseMemory and implement the LoadFromCache and SaveToCache methods
        public bool LoadFromCache()
        {
            if (!Directory.Exists(cachePath) || !File.Exists(cachePath + key + ".cache")) return false;
            string json = File.ReadAllText(cachePath + key + ".cache");
            messages = JsonConvert.DeserializeObject<List<Message>>(json);
            return true;
        }

        public bool SaveToCache()
        {
            try 
            {
                if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);
                string json = JsonConvert.SerializeObject(messages);
                File.WriteAllText(cachePath + key + ".cache", json);
            }
            catch (Exception e)
            {
                verbose?.LogError("Error: " + e.Message);
                return false;
            }
            return true;
        }

        public IEnumerator<Message> GetEnumerator() => messages.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}