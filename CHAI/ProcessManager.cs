using System.Diagnostics;

namespace CHAI
{
    /// <summary>
    /// Manager for Interacting with <see cref="Process"/>es.
    /// </summary>
    public static class ProcessManager
    {
        /// <summary>
        /// Method for checking if selected <see cref="Process"/> is running.
        /// </summary>
        /// <param name="processName"><see cref="Process"/> to check.</param>
        /// <returns>A boolean value indicating whether <see cref="Process"/> is running.</returns>
        public static bool ProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
}
