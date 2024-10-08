import { AfterContentInit, Component, Input, OnChanges, SimpleChanges, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { CustomRichTextComponent } from 'src/app/shared/components/custom-rich-text/custom-rich-text.component';
import { IconComponent, IconInterface } from '../icon/icon.component';

@Component({
  selector: 'page-header',
  standalone: true,
  imports: [CommonModule, CustomRichTextComponent, IconComponent],
  templateUrl: './page-header.component.html',
  styleUrls: ['./page-header.component.scss'],
})
export class PageHeaderComponent implements AfterContentInit, OnChanges {
  @Input() pageTitle: string = '';
  @Input() preTitle: string = '';
  @Input() customRichTextTypeID: number;
  @Input() customRichTextTypeGeographyID: number;
  @Input() icon: typeof IconInterface;
  @Input() templateAbove: TemplateRef<any>;
  @Input() templateRight: TemplateRef<any>;
  @Input() templateBottomRight: TemplateRef<any>;
  @Input() templateTitleAppend: TemplateRef<any>;

  public pageTitleDisplay: string;

  constructor(private activatedRoute: ActivatedRoute) {}
  ngOnChanges(changes: SimpleChanges): void {
    this.updateTitle();
  }

  ngAfterContentInit() {
    this.updateTitle();
  }

  updateTitle(): void {
    const routeTitle = this.activatedRoute.snapshot.routeConfig.title;

    if (this.pageTitle) {
      // set the title to the input page title first;
      this.pageTitleDisplay = this.pageTitle;
    } else if (routeTitle) {
      this.pageTitleDisplay = routeTitle.toString();
    } else {
      // if not say something else
      this.pageTitleDisplay = 'No title provided and no title on the route for this page.';
    }
  }
}
