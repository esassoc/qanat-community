using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API;
using Qanat.API.Services.Authorization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using VerifyMSTest;
using System;
using Newtonsoft.Json;
using Qanat.Models.DataTransferObjects.Geography;
using VerifyTests;
using Qanat.Models.Security;

namespace Qanat.Tests.API.Permissions
{
    /// <summary>
    ///     We have 3 role levels right now:
    ///         General
    ///           -> Geography 
    ///             -> WaterAccount.
    ///     All Rights and Flags should be defined at the General role level. At the more specific levels, they are sparsely populated depending on what we want to override.
    /// </summary>
    
    // todo: rename class
    [TestClass]
    [UsesVerify]
    public partial class WithRightsTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            VerifierSettings.UseUtf8NoBom();
            VerifierSettings.ScrubLinesWithReplace(line => line.Replace("\r\n", "\n"));
        }

        private const int MIUGSAGeographyID = 1;
        private const int MSGSAGeographyID = 6;

        [TestMethod]
        //[Description("Each controller action should have exactly one feature on it. Less means it's not secure, more means that there is confusion over which feature wins out.")]
        public void EachControllerActionShouldHaveAtLeastOneRight()
        {
            var controllerTypes = GetControllerTypes();
            var controllerHttpMethods = controllerTypes.SelectMany(type => type.GetMethods())
                .Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute)));

            var actionMethodsWithoutRightsAttrs = controllerHttpMethods.Where(m =>
                !m.GetCustomAttributes(typeof(BaseAuthorizationAttribute)).Any() &&
                !m.GetCustomAttributes(typeof(ConditionalAuthorizationAttribute)).Any() &&
                !m.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any() &&
                !m.GetCustomAttributes(typeof(AuthorizeAttribute)).Any()).ToList();

            Assert.IsTrue(!actionMethodsWithoutRightsAttrs.Any(),
                $"All Actions need at least one Right, Found {actionMethodsWithoutRightsAttrs.Count()} actions without at least one Right : " +
                string.Join(", ", actionMethodsWithoutRightsAttrs.Select(c => $"{c.ReflectedType!.FullName}.{c.Name}()")));
        }

        /// <summary>
        /// Allow Anonymous endpoints should be prefixed with '/public'.
        /// We want this so that we can more easily inform Angular which
        /// endpoints it doesn't need to send authentication info to.
        /// Without this, we would have to decorate every anonymous
        /// endpoint in the protected resource maps in the app.module
        /// </summary>
        [TestMethod]
        public void AllowAnonymousEndpointsShouldBeInPublicController()
        {
            var controllerTypes = GetControllerTypes();
            var controllerHttpMethods = controllerTypes.Where(x => x.Name != "PublicController").SelectMany(type => type.GetMethods())
                .Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute)));

            var actionMethodsWithAllowAnonymousAttribute = controllerHttpMethods.Where(m =>  m.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any());

            var matchingEndpoints = actionMethodsWithAllowAnonymousAttribute.Select(m => (HttpMethodAttribute) m.GetCustomAttributes(typeof(HttpMethodAttribute)).Single()).ToList().Where(x => x.Name != "GetSystemInfo" && !x.Template!.StartsWith("public")).ToList();
            Assert.IsNotNull(matchingEndpoints);
            Assert.IsTrue(!matchingEndpoints.Any(),
                $"All Actions that are AllowAnonymous are required to be prefixed with \"public/\", Found {matchingEndpoints.Count()} actions: " +
                string.Join(", ", matchingEndpoints.Select(c => $"\"{c.Template}\"")));
        }

        [TestMethod]
        //[Description("Each controller action should have exactly one feature on it. Less means it's not secure, more means that there is confusion over which feature wins out.")]
        public void EachControllerShouldHaveRightsChecker()
        {
            var controllerTypes = GetControllerTypes();
            var controllersWithoutRightsChecker = controllerTypes
                .Where(m => !m.GetCustomAttributes(typeof(RightsCheckerAttribute)).Any()).ToList();

            var assertMessage = "All Controllers need the RightsChecker: " + string.Join(", ", controllersWithoutRightsChecker.Select(c => c.FullName));
            Assert.IsTrue(controllersWithoutRightsChecker.All(x => x.Name == "PublicController"), assertMessage);
        }

        [TestMethod]
        public async Task TestBaseRolePermissions()
        {
            var controllerTypes = GetControllerTypes();

            var hierarchyContext = CreateTestHierarchyContext();
            var geographyRoles = new Dictionary<int, GeographyRole> { { MIUGSAGeographyID, GeographyRole.Normal } };
            var waterAccountRoles = new Dictionary<int, WaterAccountRole> {  };
            var testSystemAdminDto = CreateTestUserDto(Role.SystemAdmin, geographyRoles, waterAccountRoles);
            var testNormalDto = CreateTestUserDto(Role.Normal, geographyRoles, waterAccountRoles);
            var testPendingLoginDto = CreateTestUserDto(Role.PendingLogin, geographyRoles, waterAccountRoles);
            var testNoAccessDto = CreateTestUserDto(Role.NoAccess, geographyRoles, waterAccountRoles);

            var controllerActionPermissionResults = new List<ControllerActionPermissionResultForBaseRole>();
            foreach (var controllerType in controllerTypes)
            {
                var controllerMethodsToConsider = controllerType.GetMethods().Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute))).OrderBy(x => x.Name);
                foreach (var methodInfo in controllerMethodsToConsider)
                {
                    var controllerActionName = $"{controllerType.Name}.{methodInfo.Name}";
                    var withPermissionAttribute = methodInfo.GetCustomAttributes<WithRolePermissionAttribute>().SingleOrDefault();

                    var httpMethodAttribute = methodInfo.CustomAttributes.First(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute));
                    if (withPermissionAttribute != null)
                    {
                        var systemAdmin = withPermissionAttribute.HasPermission(testSystemAdminDto, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var normal = withPermissionAttribute.HasPermission(testNormalDto,CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var pendingLogin = withPermissionAttribute.HasPermission(testPendingLoginDto,CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var noAccess = withPermissionAttribute.HasPermission(testNoAccessDto, CreateTestAuthorizationFilterContext(), hierarchyContext);

                        var controllerActionPermissionResult = new ControllerActionPermissionResultForBaseRole(controllerActionName, systemAdmin, normal, pendingLogin, noAccess, httpMethodAttribute.AttributeType.Name);
                        controllerActionPermissionResults.Add(controllerActionPermissionResult);
                    }
                }
            }

            var result = "HttpVerb\tController.ActionName\tSystemAdmin\tNormal\tPendingLogin\tNoAccess\n" + string.Join("\n", controllerActionPermissionResults);
            await Verifier.Verify(result);
        }

        [TestMethod]
        public async Task TestWaterAccountRolePermissions()
        {
            var controllerTypes = GetControllerTypes();

            var hierarchyContext = CreateTestHierarchyContext();
            var geographyRoles = new Dictionary<int, GeographyRole> { { MIUGSAGeographyID, GeographyRole.Normal } };
            var testOwnerDto = CreateTestUserDto(Role.Normal, geographyRoles, new Dictionary<int, WaterAccountRole>{ { 1, WaterAccountRole.WaterAccountHolder } } );
            var testViewerDto = CreateTestUserDto(Role.Normal, geographyRoles, new Dictionary<int, WaterAccountRole>{ { 1, WaterAccountRole.WaterAccountViewer } } );
            var testNotPartOfWaterAccountDto = CreateTestUserDto(Role.Normal, new Dictionary<int, GeographyRole>(), new Dictionary<int, WaterAccountRole> { });

            var controllerActionPermissionResults = new List<ControllerActionPermissionResultForWaterAccountRole>();
            foreach (var controllerType in controllerTypes)
            {
                var controllerMethods = controllerType.GetMethods().Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute))).OrderBy(x => x.Name);
                foreach (var methodInfo in controllerMethods)
                {
                    var controllerActionName = $"{controllerType.Name}.{methodInfo.Name}";
                    var withPermissionAttribute = methodInfo.GetCustomAttributes<WithWaterAccountRolePermission>().SingleOrDefault();
                    var httpMethodAttribute = methodInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute));
                    if (withPermissionAttribute != null)
                    {
                        var waterAccountOwner = withPermissionAttribute.HasPermission(testOwnerDto, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var waterAccountViewer = withPermissionAttribute.HasPermission(testViewerDto, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var waterAccountNoPermission = withPermissionAttribute.HasPermission(testNotPartOfWaterAccountDto, CreateTestAuthorizationFilterContext(), hierarchyContext);

                        var controllerActionPermissionResult =new ControllerActionPermissionResultForWaterAccountRole(controllerActionName, waterAccountOwner, waterAccountViewer, waterAccountNoPermission, httpMethodAttribute!.AttributeType.Name);
                        controllerActionPermissionResults.Add(controllerActionPermissionResult);
                    }
                }
            }

            var result = "HttpVerb\tController.ActionName\tWaterAccountOwner\tWaterAccountViewer\tNoWaterAccountPermissions\n" + string.Join("\n", controllerActionPermissionResults);
            await Verifier.Verify(result);
        }

        [TestMethod]
        public async Task TestGeographyRolePermissionsForMIUGSA()
        {
            await TestGeographyRoleImpl(MIUGSAGeographyID);
        }

        [TestMethod]
        public async Task TestGeographyRolePermissionsForMSGSA()
        {
            await TestGeographyRoleImpl(6);
        }

        private static async Task TestGeographyRoleImpl(int geographyID)
        {
            var controllerTypes = GetControllerTypes();

            var hierarchyContext = CreateTestHierarchyContext();
            var testNormal = CreateTestUserDto(Role.Normal, new Dictionary<int, GeographyRole> { { geographyID, GeographyRole.Normal } }, null);
            var testWaterManager = CreateTestUserDto(Role.Normal, new Dictionary<int, GeographyRole> { { geographyID, GeographyRole.WaterManager } }, null);

            var controllerActionPermissionResults = new List<ControllerActionPermissionResultForGeographyRole>();
            foreach (var controllerType in controllerTypes)
            {
                var controllerMethods = controllerType.GetMethods().Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute))).OrderBy(x => x.Name);
                foreach (var methodInfo in controllerMethods)
                {
                    var controllerActionName = $"{controllerType.Name}.{methodInfo.Name}";
                    var withPermissionAttribute = methodInfo.GetCustomAttributes<WithGeographyRolePermission>().SingleOrDefault();

                    var httpMethodAttribute = methodInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute));
                    if (withPermissionAttribute != null)
                    {
                        var normal = withPermissionAttribute.HasPermission(testNormal, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var waterManager = withPermissionAttribute.HasPermission(testWaterManager, CreateTestAuthorizationFilterContext(), hierarchyContext);

                        var controllerActionPermissionResult = new ControllerActionPermissionResultForGeographyRole(controllerActionName, normal, waterManager, httpMethodAttribute!.AttributeType.Name);
                        controllerActionPermissionResults.Add(controllerActionPermissionResult);
                    }
                }
            }

            var result = "HttpVerb\tController.ActionName\tWaterManager\tNormal\n" + string.Join("\n", controllerActionPermissionResults);
            await Verifier.Verify(result);
        }

        [TestMethod]
        public async Task NoMoreThanOneWithPermissionsAttributePerControllerAction()
        {
            var controllerTypes = GetControllerTypes();

            var controllerHttpMethods = controllerTypes.SelectMany(type => type.GetMethods())
                .Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute)));

            var actionsWithMoreThanOneBaseAuthorizationAttribute = controllerHttpMethods.Where(m => m.GetCustomAttributes(typeof(BaseAuthorizationAttribute)).Count(g => g.GetType() != typeof(WithRoleFlagAttribute) && g.GetType() != typeof(WithGeographyRoleFlagAttribute)) > 1);
            var result = string.Join("\r\n", actionsWithMoreThanOneBaseAuthorizationAttribute.Select(c => $"{c.ReflectedType!.FullName}.{c.Name}()"));
            await Verifier.Verify(result);
        }

        [TestMethod]
        public async Task NoMoreThanOneHttpVerbPerControllerAction()
        {
            var controllerTypes = GetControllerTypes();

            var failingControllerMethods = controllerTypes.SelectMany(type => type.GetMethods())
                .Where(m => m.IsPublic && m.CustomAttributes.Count(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute)) > 1);

            var result = string.Join("\r\n", failingControllerMethods.Select(c => $"{c.ReflectedType!.FullName}.{c.Name}()"));
            await Verifier.Verify(result);
        }

        [TestMethod]
        public async Task TestBaseFlagPermissions()
        {
            var controllerTypes = GetControllerTypes();
            var hierarchyContext = CreateTestHierarchyContext();
            var geographyRoles = new Dictionary<int, GeographyRole> { { MIUGSAGeographyID, GeographyRole.Normal } };
            var waterAccountRoles = new Dictionary<int, WaterAccountRole> { };
            var testSystemAdminDto = CreateTestUserDto(Role.SystemAdmin, geographyRoles, waterAccountRoles);
            var testNormalDto = CreateTestUserDto(Role.Normal, geographyRoles, waterAccountRoles);
            var testPendingLoginDto = CreateTestUserDto(Role.PendingLogin, geographyRoles, waterAccountRoles);
            var testNoAccessDto = CreateTestUserDto(Role.NoAccess, geographyRoles, waterAccountRoles);

            var controllerActionPermissionResults = new List<ControllerActionPermissionResultForBaseRole>();
            foreach (var controllerType in controllerTypes)
            {
                var controllerMethods = controllerType.GetMethods().Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute))).OrderBy(x => x.Name);
                foreach (var methodInfo in controllerMethods)
                {
                    var controllerActionName = $"{controllerType.Name}.{methodInfo.Name}";
                    var withRoleFlagAttribute = methodInfo.GetCustomAttributes<WithRoleFlagAttribute>().SingleOrDefault();

                    var httpMethodAttribute = methodInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute));
                    if (withRoleFlagAttribute != null)
                    {
                        var systemAdmin = withRoleFlagAttribute.HasPermission(testSystemAdminDto, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var normal = withRoleFlagAttribute.HasPermission(testNormalDto, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var pendingLogin = withRoleFlagAttribute.HasPermission(testPendingLoginDto, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var noAccess = withRoleFlagAttribute.HasPermission(testNoAccessDto, CreateTestAuthorizationFilterContext(), hierarchyContext);

                        var controllerActionPermissionResult = new ControllerActionPermissionResultForBaseRole(controllerActionName, systemAdmin, normal, pendingLogin, noAccess, httpMethodAttribute!.AttributeType.Name);
                        controllerActionPermissionResults.Add(controllerActionPermissionResult);
                    }
                }
            }

            var result = "HttpVerb\tController.ActionName\tSystemAdmin\tNormal\tPendingLogin\tNoAccess\n" + string.Join("\n", controllerActionPermissionResults);
            await Verifier.Verify(result);
        }

        [TestMethod]
        public async Task TestGeographyRoleFlagPermissionsForMIUGSA()
        {
            await TestGeographyRoleFlagImpl(MIUGSAGeographyID);
        }

        [TestMethod]
        public async Task TestGeographyRoleFlagPermissionsForMSGSA()
        {
            await TestGeographyRoleFlagImpl(MSGSAGeographyID);
        }

        private static async Task TestGeographyRoleFlagImpl(int geographyID)
        {
            var controllerTypes = GetControllerTypes();
            var hierarchyContext = CreateTestHierarchyContext();
            var testNormal = CreateTestUserDto(Role.Normal, new Dictionary<int, GeographyRole> { { geographyID, GeographyRole.Normal } }, null);
            var testWaterManager = CreateTestUserDto(Role.Normal, new Dictionary<int, GeographyRole> { { geographyID, GeographyRole.WaterManager } }, null);

            var controllerActionPermissionResults = new List<ControllerActionPermissionResultForGeographyRole>();
            foreach (var controllerType in controllerTypes)
            {
                var controllerMethods = controllerType.GetMethods().Where(m => m.IsPublic && m.CustomAttributes.Any(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute))).OrderBy(x => x.Name);
                foreach (var methodInfo in controllerMethods)
                {
                    var controllerActionName = $"{controllerType.Name}.{methodInfo.Name}";
                    var withPermissionAttribute = methodInfo.GetCustomAttributes<WithGeographyRoleFlagAttribute>().SingleOrDefault();

                    var httpMethodAttribute = methodInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.BaseType == typeof(HttpMethodAttribute));
                    if (withPermissionAttribute != null)
                    {
                        var normal = withPermissionAttribute.HasPermission(testNormal, CreateTestAuthorizationFilterContext(), hierarchyContext);
                        var waterManager = withPermissionAttribute.HasPermission(testWaterManager, CreateTestAuthorizationFilterContext(), hierarchyContext);

                        var controllerActionPermissionResult = new ControllerActionPermissionResultForGeographyRole(controllerActionName, normal, waterManager, httpMethodAttribute!.AttributeType.Name);
                        controllerActionPermissionResults.Add(controllerActionPermissionResult);
                    }
                }
            }

            var result = "HttpVerb\tController.ActionName\tWaterManager\tNormal\n" + string.Join("\n", controllerActionPermissionResults);
            await Verifier.Verify(result);
        }

        private static List<Type> GetControllerTypes()
        {
            var asm = Assembly.GetAssembly(typeof(Program));
            return asm!.GetTypes().Where(type => type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract).OrderBy(x => x.Name).ToList();
        }

        private static UserDto CreateTestUserDto(Role role, Dictionary<int, GeographyRole> geographyRoles, Dictionary<int, WaterAccountRole> waterAccountRoles)
        {
            var user = new UserDto()
            {
                RoleID = role.RoleID,
                RoleDisplayName = role.RoleDisplayName,
                Rights = JsonConvert.DeserializeObject<Dictionary<string, Rights>>(role.Rights),
                Flags = JsonConvert.DeserializeObject<Dictionary<string, bool>>(role.Flags),
                GeographyRights = geographyRoles?.ToDictionary(x => x.Key, x => x.Value.AsGeographyRights()),
                WaterAccountRights = waterAccountRoles?.ToDictionary(x => x.Key, x => x.Value.AsWaterAccountRights()),
                GeographyFlags = geographyRoles?.ToDictionary(x => x.Key, x => x.Value.AsGeographyFlags())
            };

            return user;
        }

        private static AuthorizationFilterContext CreateTestAuthorizationFilterContext()
        {
            return new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
        }

        private static HierarchyContext CreateTestHierarchyContext()
        {
            var geographyDto = new GeographyDto(){GeographyID = MIUGSAGeographyID };
            var waterAccountDto = new WaterAccountDto(){WaterAccountID = 1};
            return new HierarchyContext(MIUGSAGeographyID, geographyDto, 1, waterAccountDto, 2, new WellRegistrationMinimalDto(), 3, new ParcelMinimalDto());
        }
    }

    public class ControllerActionPermissionResultForBaseRole
    {
        public ControllerActionPermissionResultForBaseRole(string controllerActionName, bool systemAdmin, bool normal,
            bool pendingLogin, bool noAccess, string httpVerb)
        {
            ControllerActionName = controllerActionName;
            SystemAdmin = systemAdmin;
            NoAccess = noAccess;
            Normal = normal;
            PendingLogin = pendingLogin;
            HttpVerb = httpVerb.Replace("Attribute", "").Replace("Http", "");
        }

        public string ControllerActionName { get; set; }
        public bool SystemAdmin { get; set; }
        public bool NoAccess { get; set; }
        public bool Normal { get; set; }
        public bool PendingLogin { get; set; }
        public string HttpVerb { get; set; }

        public override string ToString()
        {
            return $"[{HttpVerb}]\t{ControllerActionName}\t{SystemAdmin} \t {Normal} \t {PendingLogin} \t {NoAccess}";
        }
    }

    public class ControllerActionPermissionResultForWaterAccountRole
    {
        public ControllerActionPermissionResultForWaterAccountRole(string controllerActionName, bool waterAccountOwner,
            bool waterAccountViewer, bool waterAccountNoPermission, string httpVerb)
        {
            ControllerActionName = controllerActionName;
            WaterAccountOwner = waterAccountOwner;
            WaterAccountViewer = waterAccountViewer;
            WaterAccountNoPermission = waterAccountNoPermission;
            HttpVerb = httpVerb.Replace("Attribute", "").Replace("Http", "");
        }

        public string ControllerActionName { get; set; }
        public bool WaterAccountOwner { get; set; }
        public bool WaterAccountViewer { get; set; }
        public bool WaterAccountNoPermission { get; set; }
        public string HttpVerb { get; set; }

        public override string ToString()
        {
            return $"[{HttpVerb}]\t{ControllerActionName}\t{(WaterAccountOwner)}\t{(WaterAccountViewer)}\t{(WaterAccountNoPermission)}";
        }
    }

    public class ControllerActionPermissionResultForGeographyRole
    {
        public ControllerActionPermissionResultForGeographyRole(string controllerActionName, bool normal, bool waterManager, string httpVerb)
        {
            ControllerActionName = controllerActionName;
            Normal = normal;
            WaterManager = waterManager;
            HttpVerb = httpVerb.Replace("Attribute", "").Replace("Http", "");
        }

        public string ControllerActionName { get; set; }
        public bool Normal { get; set; }
        public bool WaterManager { get; set; }
        public string HttpVerb { get; set; }

        public override string ToString()
        {
            return $"[{HttpVerb}]\t{ControllerActionName}\t{(WaterManager)}\t{(Normal)}";
        }
    }
}