using System.Collections;

namespace libbSearch.Plugin
{
    /// <summary>
    /// Used to store PluginWrapper modules in a type-specific collection.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class PluginCollection
    {
        private ArrayList __List = null;

        /// <summary>
        /// Initializes a new instance of the PluginCollection class.
        /// </summary>

        public PluginCollection()
        {
            __List = new ArrayList();
        }

        /// <summary>
        /// Initializes a new instance of the PluginCollection class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list is initially capable of storing.</param>

        public PluginCollection(int capacity)
        {
            __List = new ArrayList(capacity);
        }

        /// <summary>
        /// Frees any resources for this class.
        /// </summary>
        ~PluginCollection()
        {
            __List.Clear();
            __List = null;
        }

        /// <summary>
        /// Adds a PluginWrapper to the end of the collection.
        /// </summary>
        /// <param name="plugin">The PluginWrapper to add to the end of the collection.</param>
        /// <returns>Position that the PluginWrapper was inserted.</returns>

        public int Add(PluginWrapper plugin)
        {
            return __List.Add(plugin);
        }

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>

        public int Count
        {
            get { return __List.Count; }
        }

        /// <summary>
        /// Removes the first occurrence of the given PluginWrapper.
        /// </summary>
        /// <param name="plugin">PluginWrapper object to remove.</param>

        public void Remove(PluginWrapper plugin)
        {
            __List.Remove(plugin);
        }

        /// <summary>
        /// Removes the occurrence of the given PluginWrapper at the given index.
        /// </summary>
        /// <param name="index">The zero-based index of the PluginWrapper to remove.</param>

        public void RemoveAt(int index)
        {
            __List.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>

        public PluginWrapper this[int index]
        {
            get { return (PluginWrapper)__List[index]; }
            set { __List[index] = value; }
        }

        /// <summary>
        /// Inserts a PluginWrapper at the given index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="plugin">The PluginWrapper to insert.</param>

        public void Insert(int index, PluginWrapper plugin)
        {
            __List.Insert(index, plugin);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>

        public void Clear()
        {
            __List.Clear();
        }

        /// <summary>
        /// Determines whether a PluginWrapper exists in the collection. 
        /// </summary>
        /// <param name="plugin">The PluginWrapper to locate in the collection.</param>
        /// <returns></returns>

        public bool Contains(PluginWrapper plugin)
        {
            return __List.Contains(plugin);
        }
    }
}
