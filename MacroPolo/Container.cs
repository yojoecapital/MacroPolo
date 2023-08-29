using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroPolo
{
    /// <summary>
    /// A container that manages char buffers for each window
    /// </summary>
    internal class Container
    {
        private readonly Dictionary<int, Buffer<char>> buffers;
        private readonly int containerCapacity, bufferCapacity;
        private readonly Buffer<char> defaultBuffer;
        private readonly List<int> blacklist;

        public Container(int containerCapacity, int bufferCapacity)
        {
            if (!Macro.Settings.useOneBuffer) buffers = new Dictionary<int, Buffer<char>>(containerCapacity);
            defaultBuffer = new Buffer<char>(bufferCapacity);
            blacklist = new List<int>();
            this.containerCapacity = containerCapacity;
            this.bufferCapacity = bufferCapacity;
        }

        /// <summary>
        /// Returns the buffer for the current window.
        /// If there are too many windows to keep track of or a window is blacklisted, null is returned.
        /// </summary>
        public Buffer<char> Buffer
        {
            get {
                if (!Macro.Settings.useOneBuffer)
                {
                    var process = Window.GetActiveProcess();
                    var id = process?.Id;
                    if (id != null)
                    {
                        if (buffers.ContainsKey(id.Value)) return buffers[id.Value];
                        else if (blacklist.Contains(id.Value)) return null;
                        else if (Macro.Settings.blacklist.Contains(process?.ProcessName))
                        {
                            blacklist.Add(id.Value);
                            return null;
                        }
                        else if (buffers.Count < containerCapacity)
                        {
                            var buffer = new Buffer<char>(bufferCapacity);
                            buffers[id.Value] = buffer;
                            return buffer;
                        }
                    }
                }
                return defaultBuffer;
            }
        }

        public int Clear()
        {
            int count = 1;
            if (!Macro.Settings.useOneBuffer)
            {
                count += buffers.Count;
                buffers.Clear();
            }
            defaultBuffer.Clear();
            return count;
        }
    }
}
