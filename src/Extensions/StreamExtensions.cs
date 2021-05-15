using System.IO;

namespace CHAI.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Stream"/>s.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the content of the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Input <see cref="Stream"/>.</param>
        /// <returns>Returns a <see cref="string"/> with the content of the input <see cref="Stream"/>.</returns>
        public static string ReadToEnd(this Stream stream)
        {
            try
            {
                stream.Position = 0;
            }
            catch
            {
            }

            using (StreamReader sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
