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

        public Container(int containerCapacity, int bufferCapacity)
        {
            buffers = new Dictionary<int, Buffer<char>>(containerCapacity);
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
                return null;
            }
        }

        public int Clear()
        {
            var count = Buffer.Count;
            Buffer.Clear();
            return count;
        }
    }
}
