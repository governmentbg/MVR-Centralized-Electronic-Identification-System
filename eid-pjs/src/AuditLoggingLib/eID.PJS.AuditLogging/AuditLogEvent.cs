
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#nullable disable

namespace eID.PJS.AuditLogging
{
    public class AuditLogEvent
    {
        /// <summary>
        /// The type of the event. 
        /// Usually this is the type of action performed e.g. UserLoggedIn, UserRegister, DataSaved and etc.
        /// </summary>
        [JsonProperty("eventType", Order = 6)]
        public string EventType { get; set; }

        /// <summary>
        /// Correlation ID to connect the sequence of events together
        /// </summary>
        [JsonProperty("correlationId", Order = 7)]
        public string CorrelationId { get; set; }

        /// <summary>
        /// Message to the administartors which describes what was logged.
        /// </summary>
        [JsonProperty("message", Order = 8)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the requester user identifier.
        /// </summary>
        /// <value>
        /// The requester user identifier.
        /// </value>
        [JsonProperty("requesterUserId", Order = 9)]
        public string RequesterUserId { get; set; }

        /// <summary>
        /// Gets or sets the requester system identifier.
        /// </summary>
        /// <value>
        /// The requester system identifier.
        /// </value>
        [JsonProperty("requesterSystemId", Order = 10)]
        public string RequesterSystemId { get; set; }

        /// <summary>
        /// Gets or sets the requester system name
        /// </summary>
        [JsonProperty("requesterSystemName", Order = 11)]
        public string RequesterSystemName { get; set; }


        /// <summary>
        /// Gets or sets the target user identifier.
        /// </summary>
        /// <value>
        /// The target user identifier.
        /// </value>
        [JsonProperty("targetUserId", Order = 12)]
        public string TargetUserId { get; set; }



        /// <summary>
        /// Event payload must be JSON serializable. 
        /// </summary>
        [JsonProperty("eventPayload", Order = int.MaxValue)]
        public SortedDictionary<string, object> EventPayload { get; set; }

        [JsonIgnore]
        public SortedDictionary<string, object> EventPayloadEncrypted
        {
            get
            {
                if (EventPayload is null)
                {
                    return null;
                }
                var encryptedPayload = new SortedDictionary<string, object>(EventPayload);
                if (encryptedPayload.ContainsKey(AuditLoggingKeys.Request))
                {
                    encryptedPayload[AuditLoggingKeys.Request] = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(EventPayload[AuditLoggingKeys.Request]));
                }
                foreach (var key in AuditLoggingKeys.GetEncryptablePayloadKeys())
                {
                    if (encryptedPayload.ContainsKey(key))
                    {
                        encryptedPayload[key] = EncryptionHelper.Encrypt(encryptedPayload[key]);
                    }
                }
                return encryptedPayload;
            }
        }


        [JsonIgnore]
        public string RequesterUserIdEncrypted
        {
            get { return EncryptionHelper.Encrypt(RequesterUserId); }
        }

        [JsonIgnore]
        public string TargetUserIdEncrypted
        {
            get { return EncryptionHelper.Encrypt(TargetUserId); }
        }

    }
}
