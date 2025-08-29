import { Component, EventEmitter, inject, Input, OnDestroy, Output } from "@angular/core";
import { FileResourceSimpleDto } from "../../generated/model/models";
import { Subscription } from "rxjs";
import { FileResourceService } from "../../generated/api/file-resource.service";
import { DatePipe } from "@angular/common";
import saveAs from "file-saver";
import { IconComponent } from "../icon/icon.component";
import { FileUploadModalComponent, IFileResourceUpload } from "./file-upload-modal/file-upload-modal.component";
import { FileDescriptionUpdateModalComponent } from "./file-description-update-modal/file-description-update-modal.component";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "file-resource-list",
    imports: [DatePipe, IconComponent],
    templateUrl: "./file-resource-list.component.html",
    styleUrl: "./file-resource-list.component.scss",
})
export class FileResourceListComponent implements OnDestroy {
    private fileResourceService: FileResourceService = inject(FileResourceService);
    private dialogService: DialogService = inject(DialogService);
    private subscriptions: Subscription[] = [];

    @Input() title: string = "Documents";
    @Input() fileResources: IHaveFileResource[];
    @Input() allowEditing: boolean = true;

    @Output() fileResourceUploaded = new EventEmitter<IFileResourceUpload>();
    @Output() fileResourceUpdated = new EventEmitter<IHaveFileResource>();
    @Output() fileResourceDeleted = new EventEmitter<IHaveFileResource>();

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => {
            if (subscription.unsubscribe) {
                subscription.unsubscribe();
            }
        });
    }

    public openFileUploadModal(): void {
        const dialogRef = this.dialogService.open(FileUploadModalComponent, {
            data: {},
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result: IFileResourceUpload) => {
            if (result) {
                this.fileResourceUploaded.emit(result);
            }
        });
    }

    public downloadFileResource(fileResource: FileResourceSimpleDto) {
        let downloadFileSubscription = this.fileResourceService.downloadFileResourceFileResource(fileResource.FileResourceGUID).subscribe((response) => {
            saveAs(response, `${fileResource.OriginalBaseFilename}.${fileResource.OriginalFileExtension}`);
        });

        this.subscriptions.push(downloadFileSubscription);
    }

    public openFileDescriptionUpdateModal(fileResource: IHaveFileResource): void {
        const dialogRef = this.dialogService.open(FileDescriptionUpdateModalComponent, {
            data: { FileResource: fileResource },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.fileResourceUploaded.emit(result);
            }
        });
    }

    public deleteFileResource(fileResource: IHaveFileResource) {
        this.fileResourceDeleted.emit(fileResource);
    }
}

export interface IHaveFileResource {
    FileResource?: FileResourceSimpleDto;
    FileDescription?: string;
}
