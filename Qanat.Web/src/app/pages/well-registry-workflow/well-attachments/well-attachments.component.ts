import { ChangeDetectorRef, Component, ComponentRef, TemplateRef, ViewContainerRef, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared//models/alert";
import { FileResourceSimpleDto } from "src/app/shared/generated/model/file-resource-simple-dto";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { UserDto, WellRegistrationFileResourceDto, WellRegistrationFileResourceUpdateDto } from "src/app/shared/generated/model/models";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { saveAs } from "file-saver";
import { WellRegistrationFileResourceService } from "src/app/shared/generated/api/well-registration-file-resource.service";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { NgIf, NgFor, DatePipe } from "@angular/common";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { FormsModule } from "@angular/forms";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";

@Component({
    selector: "well-attachments",
    templateUrl: "./well-attachments.component.html",
    styleUrls: ["./well-attachments.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, WorkflowBodyComponent, FormsModule, ButtonComponent, NgIf, NgFor, DatePipe],
})
export class WellAttachmentsComponent implements OnInit {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryAttachments;
    private currentUser: UserDto;
    private wellID: number;
    public wellRegistrationFileResources: WellRegistrationFileResourceDto[];

    public fileUpload: File;
    public fileDescription: string;

    private fileUploadElement: HTMLElement;
    public fileUploadElementID = "file-upload";

    public isLoadingSubmit = false;

    private updateModalComponent: ComponentRef<ModalComponent>;
    public wellRegistrationFileResourceToUpdate: WellRegistrationFileResourceDto;
    public model: WellRegistrationFileResourceUpdateDto;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private wellRegistrationService: WellRegistrationService,
        private wellRegistrationFileResourceService: WellRegistrationFileResourceService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private fileResourceService: FileResourceService,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;

            const id = this.route.snapshot.paramMap.get(routeParams.wellRegistrationID);
            if (id) {
                this.wellID = parseInt(id);

                this.wellRegistrationService.wellRegistrationsWellRegistrationIDFileResourcesGet(this.wellID).subscribe((wellRegistrationFileResources) => {
                    this.wellRegistrationFileResources = wellRegistrationFileResources;
                });
            }

            this.model = new WellRegistrationFileResourceUpdateDto();
            this.cdr.detectChanges();
        });
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public continue() {
        this.router.navigate(["../submit"], { relativeTo: this.route });
    }

    public openFileResourceLink(fileResource: FileResourceSimpleDto) {
        this.fileResourceService.fileResourcesFileResourceGuidAsStringGet(fileResource.FileResourceGUID).subscribe((response) => {
            saveAs(response, `${fileResource.OriginalBaseFilename}.${fileResource.OriginalFileExtension}`);
        });
    }

    public onClickFileUpload() {
        if (!this.fileUploadElement) {
            this.fileUploadElement = document.getElementById(this.fileUploadElementID);
        }

        this.fileUploadElement.click();
    }

    public onFileUploadChange(event: any) {
        if (!event.target.files || !event.target.files.length) {
            this.fileUpload = null;
        }

        const [file] = event.target.files;
        this.fileUpload = event.target.files.item(0);
    }

    public createWellRegistrationFileResource() {
        this.alertService.clearAlerts();

        if (!this.fileUpload) {
            this.alertService.pushAlert(new Alert("Please select a file to upload.", AlertContext.Danger));
            return;
        }

        this.isLoadingSubmit = true;
        this.wellRegistrationFileResourceService.wellRegistrationsWellRegistrationIDFileResourcesPost(this.wellID, this.wellID, this.fileUpload, this.fileDescription).subscribe({
            next: (wellRegistrationFileResources) => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("File successfully uploaded", AlertContext.Success));

                this.wellRegistrationFileResources = wellRegistrationFileResources;

                this.fileUpload = null;
                this.fileDescription = "";
                this.wellRegistryProgressService.updateProgress(this.wellID);
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            },
        });
    }

    public deleteWellRegistrationFileResource(wellRegistrationFileResource: WellRegistrationFileResourceDto, index: number) {
        this.alertService.clearAlerts();

        const message = `You are about to delete <b>${wellRegistrationFileResource.FileResource.OriginalBaseFilename}</b> as an attachment. Are you sure you wish to proceed?`;
        this.confirmService
            .confirm({ title: "Delete Attachment", message: message, buttonTextYes: "Delete", buttonClassYes: "btn-danger", buttonTextNo: "Cancel" })
            .then((confirmed) => {
                if (confirmed) {
                    this.wellRegistrationFileResourceService
                        .wellRegistrationsWellRegistrationIDFileResourcesWellRegistrationFileResourceIDDelete(
                            wellRegistrationFileResource.WellRegistrationID,
                            wellRegistrationFileResource.WellRegistrationFileResourceID
                        )
                        .subscribe((x) => {
                            this.alertService.pushAlert(new Alert("File successfully deleted", AlertContext.Success));

                            this.wellRegistrationFileResources.splice(index, 1);
                        });
                }
            });
    }

    open(template: TemplateRef<any>, wellRegistrationFileResource): void {
        this.wellRegistrationFileResourceToUpdate = wellRegistrationFileResource;
        this.model.FileDescription = wellRegistrationFileResource.FileDescription;
        this.model.WellRegistrationFileResourceID = wellRegistrationFileResource.WellRegistrationFileResourceID;

        this.updateModalComponent = this.modalService.open(template, this.viewContainerRef);
    }

    close(): void {
        if (!this.updateModalComponent) return;
        this.modalService.close(this.updateModalComponent);
    }

    public updateWellRegistrationFileResource() {
        this.close();
        this.alertService.clearAlerts();

        this.wellRegistrationFileResourceService
            .wellRegistrationsWellRegistrationIDFileResourcesWellRegistrationFileResourceIDPut(
                this.wellID,
                this.wellRegistrationFileResourceToUpdate.WellRegistrationFileResourceID,
                this.model
            )
            .subscribe(() => {
                this.wellRegistrationFileResourceToUpdate.FileDescription = this.model.FileDescription;

                this.wellRegistrationFileResourceToUpdate = null;
                this.model.FileDescription = "";

                this.alertService.pushAlert(new Alert("File description successfully updated", AlertContext.Success));
            });
    }
}
