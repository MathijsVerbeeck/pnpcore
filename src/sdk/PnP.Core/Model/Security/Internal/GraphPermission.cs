using PnP.Core.Model.SharePoint;
using PnP.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PnP.Core.Model.Security
{
    [GraphType]
    internal sealed class GraphPermission : BaseDataModel<IGraphPermission>, IGraphPermission
    {
        #region Constructor
        public GraphPermission(PnPContext context, IDataModelParent parent)
        {
            PnPContext = context;
            Parent = parent;
        }
        #endregion

        #region Properties

        public string Id { get => GetValue<string>(); set => SetValue(value); }

        public ISharePointIdentitySet GrantedToV2 { get => GetModelValue<ISharePointIdentitySet>(); set => SetModelValue(value); }

        public List<ISharePointIdentitySet> GrantedToIdentitiesV2 { get => GetValue<List<ISharePointIdentitySet>>(); set => SetModelValue(value); }

        public ISharingInvitation Invitation { get => GetModelValue<ISharingInvitation>(); set => SetModelValue(value); }

        public ISharingLink Link { get => GetModelValue<ISharingLink>(); set => SetModelValue(value); }

        public List<PermissionRole> Roles { get => GetValue<List<PermissionRole>>(); set => SetModelValue(value); }

        public string ShareId { get => GetValue<string>(); set => SetValue(value); }

        public DateTime ExpirationDateTime { get => GetValue<DateTime>(); set => SetValue(value); }

        public bool HasPassword { get => GetValue<bool>(); set => SetValue(value); }

        [KeyProperty(nameof(Id))]
        public override object Key { get => Id; set => Id = value.ToString(); }
        #endregion

        #region Methods

        public async Task GrantPermissionsAsync(GrantAccessOptions grantAccessOptions)
        {
            dynamic body = new ExpandoObject();
            body.roles = grantAccessOptions.Roles.Select(y => y.ToString()).ToList();
            body.recipients = grantAccessOptions.Recipients;

            var apiCall = new ApiCall($"shares/{Link.WebUrl}/permission/grant", ApiType.GraphBeta, jsonBody: JsonSerializer.Serialize(body, typeof(ExpandoObject), PnPConstants.JsonSerializer_IgnoreNullValues));

            var response = await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {

            }
            else
            {
                throw new Exception("Error occured during creation");
            }
        }

        public void GrantPermissions(GrantAccessOptions options)
        {
            GrantPermissionsAsync(options).GetAwaiter().GetResult();
        }

        public async Task DeletePermissionAsync()
        {
            if (Parent.GetType() == typeof(File))
            {
                var parent = (IFile)Parent;
                var apiCall = new ApiCall($"sites/{PnPContext.Site.Id}/drives/{parent.VroomDriveID}/items/{parent.VroomItemID}/permissions/{Id}", ApiType.GraphBeta);
                var response = await RawRequestAsync(apiCall, HttpMethod.Delete).ConfigureAwait(false);
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("Error occured");
                }
            }
            else if (Parent.GetType() == typeof(Folder))
            {
                var parent = (IFolder)Parent;
                var (driveId, driveItemId) = await (parent as Folder).GetGraphIdsAsync().ConfigureAwait(false);

                var apiCall = new ApiCall($"sites/{PnPContext.Site.Id}/drives/{driveId}/items/{driveItemId}/permissions/{Id}", ApiType.GraphBeta);
                var response = await RawRequestAsync(apiCall, HttpMethod.Delete).ConfigureAwait(false);
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("Error occured");
                }
            }
            else if (Parent.GetType() == typeof(ListItem))
            {
                throw new ClientException(ErrorType.Unsupported, "Deleting list item permissions is not supported");

                // For now this is not yet supported via Graph APIs
                //var parent = (IListItem)Parent;
                //var listId = await (parent as ListItem).GetListIdAsync().ConfigureAwait(false);

                //var apiCall = new ApiCall($"sites/{PnPContext.Site.Id}/lists/{listId}/items/{(parent as ListItem).Id}/permissions/{Id}", ApiType.GraphBeta);
                //var response = await RawRequestAsync(apiCall, HttpMethod.Delete).ConfigureAwait(false);
                //if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                //{
                //    throw new Exception("Error occured");
                //}
            }

        }

        public void DeletePermission()
        {
            DeletePermissionAsync().GetAwaiter().GetResult();
        }

        #endregion
    }
}
