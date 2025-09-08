import { Component, inject, OnInit } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { GeographyContext } from "src/app/shared/components/water-account/modals/create-water-account/create-water-account.component";
import { MeterReadingCSVService } from "src/app/shared/generated/api/meter-reading-csv.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "meter-reading-upload-modal",
    imports: [ButtonComponent],
    templateUrl: "./meter-reading-upload-modal.component.html",
    styleUrl: "./meter-reading-upload-modal.component.scss",
})
export class MeterReadingUploadModalComponent implements OnInit {
    public ref: DialogRef<GeographyContext, boolean> = inject(DialogRef);

    public fileUpload: File;
    public fileUploadHeaders: string[];
    public fileUploadElementID = "file-upload";
    public fileUploadElement: HTMLInputElement;

    public isLoadingSubmit: boolean = false;

    public constructor(
        private meterDataCSVService: MeterReadingCSVService,
        private alertService: AlertService
    ) {}

    public ngOnInit(): void {}

    public onClickFileUpload() {
        if (!this.fileUploadElement) {
            this.fileUploadElement = <HTMLInputElement>document.getElementById(this.fileUploadElementID);
        }

        this.fileUploadElement.click();
    }

    public onFileUploadChange(event: any) {
        if (!event.target.files || !event.target.files.length) {
            this.fileUpload = null;
            event.target.value = null;
        }

        const [file] = event.target.files;
        this.fileUpload = event.target.files.item(0);
    }

    save(): void {
        this.isLoadingSubmit = true;

        this.meterDataCSVService.uploadCSVMeterReadingCSV(this.ref.data.GeographyID, this.fileUpload).subscribe(
            (result) => {
                this.isLoadingSubmit = false;
                this.ref.close(true);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Issue uploading CSV", AlertContext.Danger));
                this.isLoadingSubmit = false;
                this.ref.close(false);
            }
        );
    }

    close(): void {
        this.ref.close(false);
    }
}
