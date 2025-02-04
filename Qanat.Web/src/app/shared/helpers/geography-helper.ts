import { GeographyMinimalDto } from "../generated/model/geography-minimal-dto";

export class GeographyHelper {
    public static compareGeography(a: GeographyMinimalDto, b: GeographyMinimalDto) {
        if (a && b) {
            return a.GeographyID === b.GeographyID;
        }

        return false;
    }
}
