using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using bSearch.Common;
using bSearch.Common.Logging;
using libbSearch;

namespace libbSearch.EncodingDetection.Caching
{
    /// <summary>
    /// Provides the ability to cache file encodings and save/load them to disk.  
    /// Each performance setting is a separate cache file.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class EncodingCache
    {
        private Dictionary<string, EncodingCacheItem> cache = null;
        private LinkedList<string> lruList = null;
        private EncodingOptions.Performance currentPerformance = EncodingOptions.Performance.Default;
        private static EncodingCache instance;
        private int capacity = 150000;

        /// <summary>
        /// Retrieves the current instance of the EncodingCache.
        /// </summary>

        public static EncodingCache Instance
        {
            get
            {
                if (EncodingCache.instance == null)
                {
                    EncodingCache.instance = new EncodingCache();
                }
                return EncodingCache.instance;
            }
        }

        /// <summary>
        /// Creates a new instance of this class (private, use Instance property to access).
        /// </summary>

        private EncodingCache()
        {
            cache = new Dictionary<string, EncodingCacheItem>(capacity);
            lruList = new LinkedList<string>();
        }

        /// <summary>
        /// Determines if given key is in cache.
        /// </summary>
        /// <param name="key">Key used for caching</param>
        /// <returns>true if found, false otherwise</returns>

        public bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }

        /// <summary>
        /// Gets a current item from the cache, null if not found.
        /// </summary>
        /// <param name="key">Key to item</param>
        /// <returns>EncodingCacheItem object if found, null otherwise</returns>

        public EncodingCacheItem GetItem(string key)
        {
            if (cache.ContainsKey(key))
                return cache[key];

            return null;
        }

        /// <summary>
        /// Removes an EncodingCacheItem from the cache for the given key.
        /// </summary>
        /// <param name="key">Unique key</param>

        public void RemoveItem(string key)
        {
            if (cache.ContainsKey(key))
            {
                // Remove it from current position
                lruList.Remove(key);
            }
        }

        /// <summary>
        /// Sets an EncodingCacheItem in the cache for the given key.
        /// </summary>
        /// <param name="key">Unique key</param>
        /// <param name="item">EncodingCacheItem to add</param>

        public void SetItem(string key, EncodingCacheItem item)
        {
            if (cache.ContainsKey(key))
            {
                // update item incase of change
                cache[key] = item;

                // Remove it from current position
                lruList.Remove(key);

                // Add it again, this will result in it being placed on top
                lruList.AddLast(key);
            }
            else
            {
                cache.Add(key, item);

                if (cache.Count > capacity)
                {
                    // over capacity so remove least used item and key
                    cache.Remove(lruList.First.Value);
                    lruList.RemoveFirst();
                }

                lruList.AddLast(key);
            }
        }

        /// <summary>
        /// Saves the current cache to disk with the given performanc setting.
        /// </summary>
        /// <param name="performanceSetting">Performance setting that should match the cache contents.</param>

        public void Save(EncodingOptions.Performance performanceSetting)
        {
            string path = GetFilePath(performanceSetting);
            LogClient.Instance.Logger.Info("Saving encoding cache [{0} items] to disk at {1}", cache.Count, path);

            try
            {
                FileInfo fInfo = new FileInfo(path);

                if (!fInfo.Directory.Exists)
                {
                    fInfo.Directory.Create();
                }

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (var deflate = new DeflateStream(fs, CompressionMode.Compress))
                    {
                        // Construct a BinaryFormatter and use it to serialize the data to the stream.
                        BinaryFormatter formatter = new BinaryFormatter();

                        formatter.Serialize(deflate, cache);
                    }
                }
            }
            catch (SerializationException e)
            {
                LogClient.Instance.Logger.Error("Serialization error: {0}", LogClient.GetAllExceptions(e));
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Saving generic error: {0}", LogClient.GetAllExceptions(ex));
            }
        }

        /// <summary>
        /// Loads a cache from disk for the given performance setting.
        /// </summary>
        /// <param name="performanceSetting">Desired performance setting cache to load</param>

        public void Load(EncodingOptions.Performance performanceSetting)
        {
            if (cache != null && cache.Count > 0 && performanceSetting == currentPerformance)
            {
                LogClient.Instance.Logger.Info("Encoding cache already loaded [{0} items]", cache.Count);
                return;
            }

            string path = GetFilePath(performanceSetting);
            LogClient.Instance.Logger.Info("Loading encoding cache from disk at {0}", path);

            try
            {
                Clear();

                if (File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        using (var deflate = new DeflateStream(fs, CompressionMode.Decompress))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();

                            // Deserialize the cache from the file and  
                            // assign the reference to the local variable.
                            if (deflate.CanSeek)
                            {
                                deflate.Seek(0, SeekOrigin.Begin);
                            }
                            cache = (Dictionary<string, EncodingCacheItem>)formatter.Deserialize(deflate);

                            // load up lrulist with all keys so they are insync
                            foreach (var key in cache.Keys)
                            {
                                lruList.AddLast(key);
                            }

                            // successful, so set now
                            currentPerformance = performanceSetting;

                            LogClient.Instance.Logger.Info("Encoding cache loaded successfully with {0} items", cache.Count);
                        }
                    }
                }
            }
            catch (SerializationException e)
            {
                LogClient.Instance.Logger.Error("Deserialization error: {0}", LogClient.GetAllExceptions(e));
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Loading generic error: {0}", LogClient.GetAllExceptions(ex));
            }
        }

        /// <summary>
        /// Clears the current cache.
        /// </summary>
        /// <param name="deletePhysical">determines if all physical cache files should be deleted</param>

        public void Clear(bool deletePhysical = false)
        {
            cache.Clear();
            lruList.Clear();

            if (deletePhysical)
            {
                LogClient.Instance.Logger.Info("Deleting the physical encoding cache files");

                // delete all cache files by going through all values of enumeration
                Array values = Enum.GetValues(typeof(EncodingOptions.Performance));
                foreach (var val in values)
                {
                    FileInfo fInfo = null;
                    try
                    {
                        fInfo = new FileInfo(GetFilePath((EncodingOptions.Performance)val));
                        if (fInfo.Exists)
                        {
                            fInfo.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogClient.Instance.Logger.Error("Error deleting cache file {0}: {1}", fInfo != null ? fInfo.FullName : "[unknown]", LogClient.GetAllExceptions(ex));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the file path for the given performance setting.
        /// </summary>
        /// <param name="performanceSetting">Desired performance setting</param>
        /// <returns></returns>

        private static string GetFilePath(EncodingOptions.Performance performanceSetting)
        {
            return Path.Combine(ApplicationPaths.CacheDirectory, string.Format("encodings_{0}.cache", performanceSetting));
        }
    }
}
