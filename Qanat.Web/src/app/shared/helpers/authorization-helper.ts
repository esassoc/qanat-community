import { FlagEnum } from "../generated/enum/flag-enum";
import { PermissionEnum } from "../generated/enum/permission-enum";
import { UserDto } from "../generated/model/user-dto";
import { RightsEnum } from "../models/enums/rights.enum";

export class AuthorizationHelper {
    public static hasFlag(flagEnum: FlagEnum, user: UserDto): boolean {
        const flagName = FlagEnum[flagEnum];
        return user.Flags[flagName];
    }

    public static hasGeographyFlag(geographyID: number, flagEnum: FlagEnum, user: UserDto): boolean {
        const flagName = FlagEnum[flagEnum];
        const geographyIDAsString = geographyID.toString();
        const hasGeographyRoleFlag = Object.keys(user.GeographyFlags).includes(geographyIDAsString) && user.GeographyFlags[geographyID][flagName];
        return hasGeographyRoleFlag || this.hasFlag(flagEnum, user);
    }

    public static hasRolePermission(permissionEnum: PermissionEnum, rightsEnum: RightsEnum, user: UserDto): boolean {
        const permissionName = PermissionEnum[permissionEnum];

        if (user.Rights[permissionName][rightsEnum]) {
            return true;
        }

        return false;
    }

    public static hasGeographyRolePermission(geographyID: number, permissionEnum: PermissionEnum, rightsEnum: RightsEnum, user: UserDto): boolean {
        const permissionName = PermissionEnum[permissionEnum];

        // todo: why is this a string?
        const geographyIDAsString = geographyID.toString();

        const hasGeographyRolePermission =
            Object.keys(user.GeographyRights).includes(geographyIDAsString) && Object.keys(user.GeographyRights[geographyIDAsString]).includes(permissionName)
                ? user.GeographyRights[geographyIDAsString][permissionName][rightsEnum]
                : false;

        return hasGeographyRolePermission || this.hasRolePermission(permissionEnum, rightsEnum, user);
    }

    public static hasWaterAccountRolePermission(geographyID: number, waterAccountID: number, permissionEnum: PermissionEnum, rightsEnum: RightsEnum, user: UserDto): boolean {
        const permissionName = PermissionEnum[permissionEnum];

        const waterAccountIDAsString = waterAccountID.toString();
        const hasWaterAccountRolePermission =
            Object.keys(user.WaterAccountRights).includes(waterAccountIDAsString) && Object.keys(user.WaterAccountRights[waterAccountIDAsString]).includes(permissionName)
                ? user.WaterAccountRights[waterAccountIDAsString][permissionName][rightsEnum]
                : false;

        return hasWaterAccountRolePermission || this.hasGeographyRolePermission(geographyID, permissionEnum, rightsEnum, user);
    }
}
