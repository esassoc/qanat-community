import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { FeatureCollection } from "geojson";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root",
})
export class WfsService {
    constructor(private http: HttpClient) {}

    public getParcelByCoordinate(longitude: number, latitude: number, geographyID: number = null): Observable<FeatureCollection> {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        let cqlFilter = `intersects(ParcelGeometry, POINT(${latitude} ${longitude}))`;
        if (geographyID) cqlFilter += ` and GeographyID = ${geographyID}`;
        return this.http.get<FeatureCollection>(url, {
            params: {
                service: "WFS",
                version: "2.0",
                request: "GetFeature",
                outputFormat: "application/json",
                SrsName: "EPSG:4326",
                typeName: "Qanat:AllParcels",
                cql_filter: cqlFilter,
            },
        });
    }

    public getGeoserverWFSLayer(bounds: any, layer: string, cqlFilter: string): Observable<number[]> {
        const cqlFilters = [];

        if (cqlFilter) {
            cqlFilters.push(`${cqlFilter}`);
        }

        if (bounds) {
            const ne = bounds.getNorthEast();
            const sw = bounds.getSouthWest();
            cqlFilters.push(`bbox(ParcelGeometry,${sw.lat},${sw.lng},${ne.lat},${ne.lng})`);
        }

        const cqlFiltersCombined = cqlFilters.join(" and ");

        const url: string = `${environment.geoserverMapServiceUrl}/ows`;
        const wfsParams = new HttpParams()
            .set("responseType", "json")
            .set("service", "wfs")
            .set("version", "2.0")
            .set("request", "GetFeature")
            .set("SrsName", "EPSG:4326")
            .set("typeName", layer)
            .set("outputFormat", "application/json")
            .set("valueReference", "ParcelID")
            .set("cql_filter", cqlFiltersCombined);
        return this.http.post(url, wfsParams).pipe(
            map((rawData: any) => {
                return rawData.features;
            })
        );
    }
}
