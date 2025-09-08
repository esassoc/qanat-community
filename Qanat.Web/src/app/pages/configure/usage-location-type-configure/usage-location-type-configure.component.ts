import { CdkDropList, CdkDrag, CdkDragHandle, CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";
import { AsyncPipe, JsonPipe } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { tap, switchMap, Observable, Subscription, of, BehaviorSubject, combineLatest, filter, take } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UsageLocationTypeDto, UsageLocationTypeUpsertDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "usage-location-type-configure",
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        LoadingDirective,
        ModelNameTagComponent,
        AlertDisplayComponent,
        CdkDropList,
        CdkDrag,
        CdkDragHandle,
        FormsModule,
        ButtonLoadingDirective,
        RouterModule,
    ],
    templateUrl: "./usage-location-type-configure.component.html",
    styleUrl: "./usage-location-type-configure.component.scss",
})
export class UsageLocationTypeConfigureComponent implements OnInit, OnDestroy {
    public geographyID: number;
    public usageLocationTypes$: Observable<UsageLocationTypeDto[]>;
    public updateUsageLocationTypesBehaviorSubject: BehaviorSubject<void> = new BehaviorSubject(null);
    public updateUsageLocationTypeTrigger$: Observable<void> = this.updateUsageLocationTypesBehaviorSubject.asObservable();

    public workingUsageLocationTypes: UsageLocationTypeDto[];
    public originalUsageLocationTypes: UsageLocationTypeDto[];
    public isLoading: boolean = true;
    public isLoadingSubmit: boolean = false;

    public newUsageLocationTypeName: string;
    public newUsageLocationTypeColor: string = "#000000"; // Default color for new usage location types
    public newUsageLocationTypeCanBeRemoteSensed: boolean = false;
    public newUsageLocationTypeIsIncludedInUsageCalculation: boolean = false;
    public newUsageLocationTypeDefinition: string;

    public richTextTypeID: number = CustomRichTextTypeEnum.UsageLocationTypeEdit;
    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    public subscriptions: Subscription[] = [];

    public constructor(
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private usageLocationTypeService: UsageLocationTypeService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.usageLocationTypes$ = combineLatest({ params: this.route.params, _: this.updateUsageLocationTypeTrigger$ }).pipe(
            filter(({ params }) => params[routeParams.geographyName]),
            tap(() => {
                this.isLoading = true;
            }),
            switchMap(({ params }) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
            }),
            switchMap((geography) => {
                return this.usageLocationTypeService.listUsageLocationType(geography.GeographyID);
            }),
            tap((usageLocationTypes) => {
                this.isLoading = false;
                this.originalUsageLocationTypes = usageLocationTypes;
                this.workingUsageLocationTypes = usageLocationTypes.map((type) => {
                    return { ...type }; // Create a shallow copy to avoid mutating the original
                });
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => {
            subscription.unsubscribe();
        });
    }

    canExit(): boolean {
        var canExit = JSON.stringify(this.originalUsageLocationTypes) === JSON.stringify(this.workingUsageLocationTypes);
        return canExit;
    }

    onDefaultChanged(event: any, usageLocationType: UsageLocationTypeDto, usageLocationTypes: UsageLocationTypeDto[]): void {
        if (event) {
            // Set all other usage location types to not default
            usageLocationTypes.forEach((type) => {
                if (type.UsageLocationTypeID !== usageLocationType.UsageLocationTypeID) {
                    type.IsDefault = false;
                }
            });
        }
    }

    dropUsageLocation($event: CdkDragDrop<any, any, any>, usageLocationTypes: UsageLocationTypeDto[]): void {
        moveItemInArray(usageLocationTypes, $event.previousIndex, $event.currentIndex);

        // Update the sort order based on the new order
        usageLocationTypes.forEach((type, index) => {
            type.SortOrder = index + 1; // Sort order starts at 1
        });

        this.updateUsageLocationTypes(usageLocationTypes);
    }

    addUsageLocationType(usageLocationTypes: UsageLocationTypeDto[]): void {
        this.isLoadingSubmit = true;
        let newUsageLocationType: UsageLocationTypeUpsertDto = {
            Name: this.newUsageLocationTypeName,
            Definition: this.newUsageLocationTypeDefinition,
            CanBeRemoteSensed: this.newUsageLocationTypeCanBeRemoteSensed,
            IsIncludedInUsageCalculation: this.newUsageLocationTypeIsIncludedInUsageCalculation,
            IsDefault: false,
            ColorHex: this.newUsageLocationTypeColor,
            SortOrder: usageLocationTypes.length + 1, // Set sort order to the next available number
        };

        let createSubscription = this.usageLocationTypeService.createUsageLocationType(this.geographyID, newUsageLocationType).subscribe({
            next: (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Usage Location Type successfully created.", AlertContext.Success));
                this.isLoadingSubmit = false;
                this.originalUsageLocationTypes = result;
                this.updateUsageLocationTypesBehaviorSubject.next();

                // Reset the form fields
                this.newUsageLocationTypeName = "";
                this.newUsageLocationTypeColor = "#000000"; // Reset to default color
                this.newUsageLocationTypeCanBeRemoteSensed = false;
                this.newUsageLocationTypeIsIncludedInUsageCalculation = false;
                this.newUsageLocationTypeDefinition = "";

                setTimeout(() => {
                    this.alertService.clearAlerts();
                }, 5 * 1000); // Clear alerts after 5 seconds
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });

        this.subscriptions.push(createSubscription);
    }

    updateUsageLocationTypes(usageLocationTypes: UsageLocationTypeDto[], resetAddUsageLocationTypeForm: boolean = false): void {
        this.isLoadingSubmit = true;
        let usageLocationTypeUpsertDtos: UsageLocationTypeUpsertDto[];

        usageLocationTypeUpsertDtos = usageLocationTypes.map((type) => {
            return {
                UsageLocationTypeID: type.UsageLocationTypeID,
                Name: type.Name,
                Definition: type.Definition,
                CanBeRemoteSensed: type.CanBeRemoteSensed,
                IsIncludedInUsageCalculation: type.IsIncludedInUsageCalculation,
                ColorHex: type.ColorHex,
                SortOrder: type.SortOrder,
                IsDefault: type.IsDefault,
            } as UsageLocationTypeUpsertDto;
        });

        let updateSubscription = this.usageLocationTypeService.updateUsageLocationType(this.geographyID, usageLocationTypeUpsertDtos).subscribe({
            next: (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Usage Location Types successfully saved.", AlertContext.Success));
                this.isLoadingSubmit = false;
                this.originalUsageLocationTypes = result;
                this.updateUsageLocationTypesBehaviorSubject.next();

                if (resetAddUsageLocationTypeForm) {
                    this.newUsageLocationTypeName = "";
                    this.newUsageLocationTypeColor = "#000000"; // Reset to default color
                }

                setTimeout(() => {
                    this.alertService.clearAlerts();
                }, 5 * 1000); // Clear alerts after 5 seconds
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });

        this.subscriptions.push(updateSubscription);
    }

    deleteUsageLocationType(usageLocationType: UsageLocationTypeDto): void {
        const options = {
            title: "Confirm: Delete Usage Location Type",
            message: `Are you sure you want to delete ${usageLocationType.Name}?`,
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;

        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                let deleteSubscription = this.usageLocationTypeService.deleteUsageLocationType(this.geographyID, usageLocationType.UsageLocationTypeID).subscribe({
                    next: () => {
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert(`Usage Location Type ${usageLocationType.Name} successfully deleted.`, AlertContext.Success));

                        // Remove the deleted usage location type from the original list
                        this.originalUsageLocationTypes = this.originalUsageLocationTypes.filter((type) => type.UsageLocationTypeID !== usageLocationType.UsageLocationTypeID);
                        for (let i = 0; i < this.originalUsageLocationTypes.length; i++) {
                            this.originalUsageLocationTypes[i].SortOrder = i + 1; // Reorder the remaining types
                        }

                        this.updateUsageLocationTypesBehaviorSubject.next();

                        setTimeout(() => {
                            this.alertService.clearAlerts();
                        }, 5 * 1000); // Clear alerts after 5 seconds
                    },
                    error: () => {
                        this.alertService.clearAlerts();
                    },
                });

                this.subscriptions.push(deleteSubscription);
            }
        });
    }
}
