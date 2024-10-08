import { Pipe, PipeTransform } from "@angular/core";
import { GeographyRoleEnum } from "../generated/enum/geography-role-enum";
import { UserDto } from "../generated/model/user-dto";

@Pipe({
    name: "hasGeographyRole",
    standalone: true,
})
export class GeographyRolePipe implements PipeTransform {
    transform(user: UserDto, obj: GeographyRolePipeInput): boolean {
        const geographyUser = user && user.GeographyUser.find((x) => x.GeographyID == obj.GeographyID);
        if (!geographyUser) {
            return false;
        }
        const role = geographyUser.GeographyRoleID;
        return role === obj.GeographyRoleEnum;
    }
}

export interface GeographyRolePipeInput {
    GeographyID: number;
    GeographyRoleEnum: GeographyRoleEnum;
}
