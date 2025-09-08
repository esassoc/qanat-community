using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System;
using System.Linq;

namespace Qanat.API.Services.Authorization
{
    public static class PermissionChecker
    {
        public static bool HasFlag(UserDto userDto, FlagEnum flag, bool right)
        {
            if (userDto == null)
            {
                return false;
            }

            var value = false;
            var flagName = Enum.GetName(typeof(FlagEnum), flag);

            if (!string.IsNullOrEmpty(flagName) && userDto.Flags.ContainsKey(flagName))
            {
                value = userDto.Flags[flagName];
            }

            return value == right;
        }

        public static bool HasGeographyFlag(UserDto userDto, FlagEnum flag, bool right,
            HierarchyContext hierarchyContext, bool forAnyGeography = false)
        {
            if (userDto == null)
            {
                return false;
            }

            var value = false;
            var flagName = Enum.GetName(typeof(FlagEnum), flag);

            if (!string.IsNullOrEmpty(flagName) && userDto.Flags.ContainsKey(flagName))
            {
                // for specific geography
                var geographyID = hierarchyContext.GeographyDisplayDto?.GeographyID;
                if (geographyID.HasValue && userDto.GeographyFlags.ContainsKey(geographyID.Value)
                                         && userDto.GeographyFlags[geographyID.Value]
                                             .TryGetValue(flagName, out var geographyFlag))
                {
                    value = geographyFlag;
                }
                // for any geography
                if (forAnyGeography && userDto.GeographyFlags.Values.Any(x => x[flagName]))
                {
                    value = true;
                }
            }

            return value == right;
        }

        public static bool HasPermission(UserDto userDto, PermissionEnum permission, RightsEnum rights)
        {
            if (userDto == null)
            {
                return false;
            }

            short mask = 0;
            var permissionName = Enum.GetName(typeof(PermissionEnum), permission);

            if (!string.IsNullOrEmpty(permissionName))
            {
                if (userDto.Rights.TryGetValue(permissionName, out var right))
                {
                    mask = right;
                }
            }

            var rightEnum = (RightsEnum)mask;
            return rightEnum.HasFlag(rights);
        }

        public static bool HasGeographyPermission(UserDto userDto, PermissionEnum permission, RightsEnum rights, HierarchyContext hierarchyContext)
        {
            if (userDto == null)
            {
                return false;
            }

            short mask = 0;
            var permissionName = Enum.GetName(typeof(PermissionEnum), permission);

            bool hasRights = false;


            if (!string.IsNullOrEmpty(permissionName))
            {
                var geographyID = hierarchyContext.GeographyDisplayDto?.GeographyID;
                if (geographyID.HasValue && userDto.GeographyRights.ContainsKey(geographyID.Value)
                    && userDto.GeographyRights[geographyID.Value]
                        .TryGetValue(permissionName, out var geographyRight))
                {
                    mask = geographyRight;
                    var rightEnum = (RightsEnum)mask;
                    if (rightEnum.HasFlag(rights)) hasRights = true;
                }
            }

            return hasRights;
        }

        public static bool HasWaterAccountPermission(UserDto userDto, PermissionEnum permission, RightsEnum rights, HierarchyContext hierarchyContext)
        {
            if (userDto == null)
            {
                return false;
            }

            short mask = 0;
            var permissionName = Enum.GetName(typeof(PermissionEnum), permission);

            bool hasRights = false;


            if (!string.IsNullOrEmpty(permissionName))
            {
                var waterAccountID = hierarchyContext.WaterAccountDisplayDto?.WaterAccountID;
                if (waterAccountID.HasValue && userDto.WaterAccountRights.ContainsKey(waterAccountID.Value)
                    && userDto.WaterAccountRights[waterAccountID.Value]
                        .TryGetValue(permissionName, out var waterAccountRight))
                {
                    mask = waterAccountRight;
                    var rightEnum = (RightsEnum)mask;
                    if (rightEnum.HasFlag(rights)) hasRights = true;

                }
            }

            return hasRights;
        }

        public static bool HasScenarioPlannerPermission(UserDto user, PermissionEnum permission, RightsEnum rights)
        {
            if (user == null)
            {
                return false;
            }

            short mask = 0;
            var permissionName = Enum.GetName(typeof(PermissionEnum), permission);

            if (!string.IsNullOrEmpty(permissionName))
            {
                if (user.ScenarioPlannerRights.TryGetValue(permissionName, out var right))
                {
                    mask = right;
                }
            }

            var rightEnum = (RightsEnum)mask;
            return rightEnum.HasFlag(rights);
        }
    }
}