import { ComponentFixture, TestBed } from "@angular/core/testing";

import { RasterUploadComponent } from "./raster-upload.component";

describe("RasterUploadComponent", () => {
    let component: RasterUploadComponent;
    let fixture: ComponentFixture<RasterUploadComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [RasterUploadComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(RasterUploadComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
