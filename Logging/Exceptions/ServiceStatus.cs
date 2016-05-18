using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Crucial.Framework.Logging.Exceptions
{
    #region Status Object Class
    /// <summary>
    /// Public class to return input status
    /// </summary>
    [Serializable]
    [DataContract]
    public class ServiceStatus
    {
        #region Public properties.
        /// <summary>
        /// Get/Set property for accessing Status Code
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }
        /// <summary>
        /// Get/Set property for accessing Status Message
        /// </summary>
        [DataMember]
        public string StatusMessage { get; set; }
        /// <summary>
        /// Get/Set property for accessing Status Message
        /// </summary>
        [DataMember]
        public string ReasonPhrase { get; set; }

        #endregion
    }

    #endregion
}
