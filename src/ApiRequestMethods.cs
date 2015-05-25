using System.ComponentModel;

namespace Transloadit
{
    public class ApiRequestMethods
    {
        #region Public enums

        /// <summary>
        /// Available request methods
        /// </summary>
        public enum RequestMethod
        {
            /// <summary>
            /// Used for GET requests
            /// </summary>
            Get,

            /// <summary>
            /// Used for POST requests
            /// </summary>
            Post,

            /// <summary>
            /// Used for PUT requests
            /// </summary>
            Put,

            /// <summary>
            /// Used for DELETE requests
            /// </summary>
            Delete,

            /// <summary>
            /// Used for PATCH requests
            /// </summary>
            Patch
        }

        #endregion
    }
}
