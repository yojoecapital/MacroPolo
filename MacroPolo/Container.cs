using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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

        public Container(int containerCapacity, int bufferCapacity)
        {
            if (!Macro.Settings.useOneBuffer) buffers = new Dictionary<int, Buffer<char>>(containerCapacity);
            defaultBuffer = new Buffer<char>(containerCapacity);
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
                    var window = Window.GetActiveWindowId();
                    if (window != null)
                    {
                        if (buffers.ContainsKey(window.Value)) return buffers[window.Value];
                        else if (buffers.Count < containerCapacity)
                        {
                            var buffer = new Buffer<char>(bufferCapacity);
                            buffers[window.Value] = buffer;
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
