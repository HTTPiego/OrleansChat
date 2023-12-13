import {Directive, ElementRef, OnInit, Renderer2} from "@angular/core";

@Directive({
    standalone: true,
    selector: '[fullCenteredFlex]'
})
export class FullCenteredFlex implements OnInit {
    constructor(private element: ElementRef, private renderer: Renderer2) {
    }
    ngOnInit() {
        this.renderer.setStyle(this.element.nativeElement,'display', 'flex')
        this.renderer.setStyle(this.element.nativeElement,'justify-content', 'center')
        this.renderer.setStyle(this.element.nativeElement,'align-items', 'center')
    }
}
