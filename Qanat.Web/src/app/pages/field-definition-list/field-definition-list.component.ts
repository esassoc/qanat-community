import { Component, OnInit, ViewChild } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ColDef } from "ag-grid-community";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AgGridAngular } from "ag-grid-angular";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { CustomRichTextService } from "src/app/shared/generated/api/custom-rich-text.service";
import { CustomRichTextDto } from "src/app/shared/generated/model/custom-rich-text-dto";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";

@Component({
    selector: "qanat-field-definition-list",
    templateUrl: "./field-definition-list.component.html",
    styleUrls: ["./field-definition-list.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent],
})
export class FieldDefinitionListComponent implements OnInit {
    @ViewChild("fieldDefinitionsGrid") fieldDefinitionsGrid: AgGridAngular;

    private currentUser: UserDto;

    public fieldDefinitions: CustomRichTextDto[];
    public richTextTypeID: number = CustomRichTextTypeEnum.LabelsAndDefinitionsList;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    public columnDefs: ColDef[];

    constructor(
        private customRichTextService: CustomRichTextService,
        private authenticationService: AuthenticationService,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;

            this.customRichTextService.fieldDefinitionsGet().subscribe((fieldDefinitions) => {
                this.fieldDefinitions = fieldDefinitions;
            });

            this.createColumnDefs();
        });
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Label", "CustomRichTextType.CustomRichTextTypeDisplayName", "CustomRichTextType.CustomRichTextTypeID", {
                InRouterLink: "./",
                Width: 100,
                Hide: !this.canEdit(),
            }),
            {
                headerName: "Label",
                field: "CustomRichTextType.CustomRichTextTypeDisplayName",
                hide: this.canEdit(),
            },
            { headerName: "Definition", field: "CustomRichTextContent" },
        ];
    }

    private canEdit() {
        return this.authenticationService.hasPermission(this.currentUser, PermissionEnum.FieldDefinitionRights, RightsEnum.Update);
    }
}
