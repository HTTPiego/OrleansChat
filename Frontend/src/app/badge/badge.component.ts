import {Component, Input} from '@angular/core';
import {NgClass} from "@angular/common";

@Component({
  selector: 'badge',
  standalone: true,
    imports: [
        NgClass
    ],
  templateUrl: './badge.component.html',
  styleUrl: './badge.component.sass'
})
export class BadgeComponent {
    @Input()
    number: number = 0
}
