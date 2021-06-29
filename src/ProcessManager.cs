using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace CHAI
{
    /// <summary>
    /// Class for interacting with <see cref="Process"/>es.
    /// </summary>
    public class ProcessManager
    {
        /// <summary>
        /// Method for finding <see cref="Process"/> by Window title.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger{ProcessManager}"/>.</param>
        /// <param name="windowTitle">Window title of <see cref="Process"/>.</param>
        /// <returns>Name of <see cref="Process"/> if found.</returns>
        public static string FindProcess(ILogger logger, string windowTitle)
        {
            Process process = Process.GetProcesses()
                .Where(proc => proc.MainWindowTitle.Contains(windowTitle))
                .FirstOrDefault();

            if (process != null)
            {
                return process.ProcessName;
            }
            else
            {
                logger.LogError($"{windowTitle} is not running...");
                return string.Empty;
            }
        }

        /// <summary>
        /// Method for checking if selected <see cref="Process"/> is running.
        /// </summary>
        /// <param name="processName"><see cref="Process"/> to check.</param>
        /// <returns>A <see cref="bool"/> value indicating whether <see cref="Process"/> is running.</returns>
        public static bool ProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        /// <summary>
        /// Method for sending Key presses to a <see cref="Process"/>.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger{ProcessManager}"/>.</param>
        /// <param name="processName">Name of recipient <see cref="Process"/>.</param>
        /// <param name="keyValue">Fullname of key to send to <see cref="Process"/>.</param>
        /// <param name="key">Key to send to the <see cref="Process"/>.</param>
        public static void SendKeyPress(ILogger logger, string processName, string keyValue, int key)
        {
            if (ProcessRunning(processName))
            {
                logger.LogInformation($"Sending {keyValue} to {processName} as {key}");
                var result = SendMessage(Process.GetProcessesByName(processName)[0].MainWindowHandle, 0x0104, key, null);
                logger.LogInformation($"{result}");
            }
            else
            {
                logger.LogError($"{processName} not running...");
            }
        }

        /// <summary>
        /// Method for sending Key presses to a recipient window.
        /// </summary>
        /// <param name="hWnd">Handle to recipient window.</param>
        /// <param name="uMsg">Message type(eg WM_SYSKEYDOWN is 0x0104).</param>
        /// <param name="wParam">Key to send to window.</param>
        /// <param name="lParam">Unused parameter.</param>
        /// <returns>Success as an <see cref="int"/>.</returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
    }
}
