import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "../generated/enum/custom-rich-text-type-enum";
import { UserDto } from "../generated/model/user-dto";
import { SystemInfoService } from "../generated/api/system-info.service";
import { SystemInfoDto } from "../generated/model/system-info-dto";
import { Observable } from "rxjs";
import { NgIf, AsyncPipe, DatePipe } from "@angular/common";
import { CustomRichTextComponent } from "../components/custom-rich-text/custom-rich-text.component";

@Component({
    selector: "footer-nav",
    templateUrl: "./footer-nav.component.html",
    styleUrls: ["./footer-nav.component.scss"],
    standalone: true,
    imports: [CustomRichTextComponent, NgIf, AsyncPipe, DatePipe],
})
export class FooterNavComponent implements OnInit {
    public currentYear: number = new Date().getFullYear();
    public footerRichTextID: number = CustomRichTextTypeEnum.Footer;
    public currentUser: UserDto;
    public systemInfo$: Observable<SystemInfoDto>;
    constructor(
        public authenticationService: AuthenticationService,
        private systemInfoService: SystemInfoService
    ) {}

    ngOnInit(): void {
        this.systemInfo$ = this.systemInfoService.getSystemInfo();

        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });
    }
}
