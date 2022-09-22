using System;
using System.Collections.Generic;
using System.Text;

namespace PnP.Core.Model.Security
{
    /// <summary>
    /// Properties that can be set when updating a new Sharing Link
    /// </summary>
    public class GrantAccessOptions
    {
        /// <summary>
        /// People to add to the sharing link
        /// </summary>
        public List<IDriveRecipient> Recipients { get; set; }

        /// <summary>
        /// If the link is an "existing access" link, specifies roles to be granted to the users. Otherwise must match the role of the link.
        /// </summary>
        public List<PermissionRole> Roles { get; set; }
    }
}
