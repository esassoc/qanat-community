using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.Models.DataTransferObjects;
using System;

namespace Qanat.API.Services.Authorization
{
    /// <summary>
    /// Some endpoints have conditional authorization (e.g. a Geography can set a page to be public or not),
    /// this attribute is extended from to to check if the user has permission to access the endpoint.
    /// 
    /// See GeographyAllocationPlansPublicAttribute for an example
    /// </summary>
    public abstract class ConditionalAuthorizationAttribute : Attribute, IAuthorizationAttribute
    {
        public abstract bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext);
        public abstract string InvalidPermissionMessage();
    }


    /// <summary>
    /// Most endpoints require authorization.  Endpoints decorated
    /// with this extended attribute require the user to at the very least be logged in
    /// and then permissions/rights will be checked.
    ///
    /// See RightsCheckerAttribute for usage.
    /// </summary>
    public abstract class BaseAuthorizationAttribute : Attribute, IAuthorizationAttribute
    {
        public abstract bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext);
        public abstract string InvalidPermissionMessage();
    }

    public interface IAuthorizationAttribute
    {
        bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext);
        string InvalidPermissionMessage();
    }   
}
