import { AfterViewInit, Directive, ElementRef, Input, OnChanges, SecurityContext, SimpleChanges } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";

@Directive({
    selector: "[highlight]",
    standalone: true,
})
export class HighlightDirective implements OnChanges, AfterViewInit {
    @Input("highlight") searchTerm: string;
    @Input() caseSensitive = false;
    @Input() customClasses = "";
    private isReady: boolean = false;

    constructor(
        private el: ElementRef,
        private sanitizer: DomSanitizer
    ) {}

    ngAfterViewInit(): void {
        this.isReady = true;
        this.walkElementAndHighlight(this.el.nativeElement);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (this.isReady) {
            this.walkElementAndHighlight(this.el.nativeElement);
        }
    }

    walkElementAndHighlight(rootElement: HTMLElement) {
        const elementsToReplace: ElementToReplace[] = [];
        this.removeMarks(rootElement);
        if (this.searchTerm === "") {
            return;
        }
        const treeWalker = document.createTreeWalker(rootElement, NodeFilter.SHOW_TEXT);
        while (treeWalker.nextNode()) {
            const current = treeWalker.currentNode;
            const replaceContent = this.replace(treeWalker.currentNode);
            if (replaceContent) {
                const elementToReplace = { current, replace: this.replace(treeWalker.currentNode) } as ElementToReplace;
                elementsToReplace.push(elementToReplace);
            }
        }

        elementsToReplace.forEach((element) => {
            element.current.parentElement.innerHTML = element.replace;
        });
    }

    replace(current: Node) {
        const regex = new RegExp(this.searchTerm, this.caseSensitive ? "gm" : "gim");
        const found = current.textContent.match(regex);
        if (!found) {
            return false;
        }
        const newText = current.textContent.replace(regex, (match: string) => {
            return `<mark class="highlight ${this.customClasses}">${match}</mark>`;
        });

        const sanitizedHtml = this.sanitizer.sanitize(SecurityContext.HTML, newText);
        return sanitizedHtml;
    }

    removeMarks(node: HTMLElement) {
        const regex = new RegExp(
            // eslint-disable-next-line no-useless-escape
            '<mark class="highlight .*?">(.*?)</mark>',
            this.caseSensitive ? "gm" : "gim"
        );
        const cleanText = node.innerHTML.replace(regex, function (match, capturedText) {
            return `${capturedText}`;
        });
        node.innerHTML = cleanText;
    }
}

interface ElementToReplace {
    current: Node;
    replace: string;
}
