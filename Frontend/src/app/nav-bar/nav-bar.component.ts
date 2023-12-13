import {Component, NgModule} from '@angular/core';
import {RouterLink, RouterLinkActive} from "@angular/router";
import {LucideAngularModule} from "lucide-angular";
import {MyIconsModule} from "../my-icons-module";

@Component({
  selector: 'app-nav-bar',
  standalone: true,
    imports: [
        RouterLink,
        MyIconsModule,
        LucideAngularModule,
        RouterLinkActive,
    ],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.sass'
})
export class NavBarComponent {

}
