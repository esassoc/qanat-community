import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "../generated/enum/custom-rich-text-type-enum";
import { UserDto } from "../generated/model/user-dto";
import { SystemInfoDto } from "../generated/model/system-info-dto";
import { Observable } from "rxjs";
import { NgIf, AsyncPipe, DatePipe } from "@angular/common";
import { CustomRichTextComponent } from "../components/custom-rich-text/custom-rich-text.component";
import { PublicService } from "../generated/api/public.service";
import { IconComponent } from "../components/icon/icon.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: "footer-nav",
    templateUrl: "./footer-nav.component.html",
    styleUrls: ["./footer-nav.component.scss"],
    standalone: true,
    imports: [CustomRichTextComponent, NgIf, AsyncPipe, DatePipe, IconComponent, RouterLink],
})
export class FooterNavComponent implements OnInit {
    public currentYear: number = new Date().getFullYear();
    public footerRichTextID: number = CustomRichTextTypeEnum.Footer;
    public currentUser: UserDto;
    public systemInfo$: Observable<SystemInfoDto>;
    constructor(
        public authenticationService: AuthenticationService,
        private publicService: PublicService
    ) {}

    ngOnInit(): void {
        this.systemInfo$ = this.publicService.publicSystemInfoGet();

        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });
    }
}
