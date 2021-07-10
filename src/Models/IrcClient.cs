using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Sockets;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing an <see cref="IrcClient"/>.
    /// </summary>
    public class IrcClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IrcClient"/> class.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger"/>.</param>
        /// <param name="ip">The IP address for the IRC connection.</param>
        /// <param name="port">The port for the IRC connection.</param>
        /// <param name="userName">Username of <see cref="User"/> to connect as.</param>
        /// <param name="password">OAuth token to connect with.</param>
        /// <param name="channel">The channel to connect to.</param>
        public IrcClient(ILogger logger, string ip, int port, string userName, string password, string channel)
        {
            Logger = logger;
            try
            {
                TcpClient = new TcpClient(ip, port);
                InputStream = new StreamReader(TcpClient.GetStream());
                OutputStream = new StreamWriter(TcpClient.GetStream());

                OutputStream.WriteLine("PASS " + password);
                OutputStream.WriteLine("NICK " + userName);
                OutputStream.WriteLine("USER " + userName + " 8 * :" + userName);
                OutputStream.WriteLine("CAP REQ :twitch.tv/tags");
                OutputStream.WriteLine("JOIN #" + channel);
                OutputStream.Flush();
                Logger.LogInformation("IRC Client Initialised successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Gets the injected <see cref="ILogger{IrcClient}"/>.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the <see cref="InputStream"/> of the <see cref="TcpClient"/>.
        /// </summary>
        public StreamReader InputStream { get; private set; }

        /// <summary>
        /// Gets the <see cref="OutputStream"/> of the <see cref="TcpClient"/>.
        /// </summary>
        public StreamWriter OutputStream { get; private set; }

        /// <summary>
        /// Gets the <see cref="TcpClient"/>.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Method for writing messages to an IRC connection.
        /// </summary>
        /// <param name="message">Message for the IRC connection.</param>
        public void SendMessage(string message)
        {
            try
            {
                OutputStream.WriteLine(message);
                OutputStream.Flush();
                if (message == "PING irc.twitch.tv")
                {
                    Logger.LogInformation("PING");
                }
                else
                {
                    Logger.LogInformation($"Message: \"{message}\" sent successfully");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error sending message: \"{message}\" {ex.Message}");
            }
        }

        /// <summary>
        /// Method for reading messages from an IRC connection.
        /// </summary>
        /// <returns>Message from IRC connection.</returns>
        public string ReadMessage()
        {
            try
            {
                return InputStream.ReadLine();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error receiving message: " + ex.Message);
                return "Error receiving message: " + ex.Message;
            }
        }
    }
}
