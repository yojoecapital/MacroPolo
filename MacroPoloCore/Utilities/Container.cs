using MacroPoloCore.Managers;
using System.Collections.Generic;
using System.Diagnostics;

namespace MacroPoloCore.Utilities
{
    /// <summary>
    /// A container that manages char buffers for each window
    /// </summary>
    internal class Container
    {
        private readonly MacroPoloFileManager fileManager;
        private readonly Dictionary<int, Buffer<char>> buffers;
        private readonly int containerCapacity, bufferCapacity;
        private readonly Buffer<char> defaultBuffer;
        private readonly List<int> blacklist;

        public Container(MacroPoloFileManager fileManager, int containerCapacity, int bufferCapacity)
        {
            this.fileManager = fileManager;
            if (!fileManager.Settings.useOneBuffer) buffers = new Dictionary<int, Buffer<char>>(containerCapacity);
            defaultBuffer = new Buffer<char>(bufferCapacity);
            blacklist = new List<int>();
            this.containerCapacity = containerCapacity;
            this.bufferCapacity = bufferCapacity;
        }

        public IEnumerable<string> GetBufferNames()
        {
            if (!fileManager.Settings.useOneBuffer)
            {
                foreach (var id in buffers.Keys)
                {
                    Process process;
                    try 
                    {
                        process = Process.GetProcessById(id);
                    }
                    catch
                    {
                        continue;
                    }
                    yield return process?.ProcessName;
                }
            }
            yield return "defaultBuffer";
        }

        /// <summary>
        /// Returns the buffer for the current window.
        /// If there are too many windows to keep track of or a window is blacklisted, null is returned.
        /// </summary>
        public Buffer<char> Buffer
        {
            get {
                if (!fileManager.Settings.useOneBuffer)
                {
                    var process = WindowManager.GetActiveProcess();
                    var id = process?.Id;
                    if (id != null)
                    {
                        if (buffers.ContainsKey(id.Value)) return buffers[id.Value];
                        else if (blacklist.Contains(id.Value)) return null;
                        else if (fileManager.Settings.blacklist.Contains(process?.ProcessName))
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
            if (!fileManager.Settings.useOneBuffer)
            {
                count += buffers.Count;
                buffers.Clear();
            }
            defaultBuffer.Clear();
            return count;
        }
    }
}
