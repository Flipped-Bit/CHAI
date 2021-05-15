using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing a <see cref="User"/>.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the <see cref="ClientId"/> that uniquely identifies this <see cref="User" /> instance.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds that the <see cref="Token"/> of this <see cref="User" /> instance will <see cref="ExpiresIn"/>.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Username"/> of this <see cref="User" /> instance.
        /// </summary>
        [JsonPropertyName("login")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Scopes"/> of this <see cref="User" /> instance.
        /// </summary>
        [JsonPropertyName("scopes")]
        public List<string> Scopes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Token"/> of this <see cref="User" /> instance.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserId"/> of this <see cref="User" /> instance.
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}
