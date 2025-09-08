import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, shareReplay, switchMap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "geography-support",
    templateUrl: "./geography-support.component.html",
    styleUrls: ["./geography-support.component.scss"],
    imports: [PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, CustomRichTextComponent, AsyncPipe]
})
export class GeographySupportComponent implements OnInit {
    public geography$: Observable<GeographyDto>;
    public customRichTextTypeID = CustomRichTextTypeEnum.GeographySupport;

    constructor(
        private publicService: PublicService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                return this.publicService.getGeographyByNamePublic(params[routeParams.geographyName]);
            }),
            shareReplay()
        );
    }

    public setRouteQueryParams(geographyName: string) {
        this.router.navigate(["../../../request-support"], { relativeTo: this.route, queryParams: { GeographyName: geographyName } });
    }
}
