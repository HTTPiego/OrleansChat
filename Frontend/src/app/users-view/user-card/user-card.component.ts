import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FriendCard} from "../../../types";
import {NgIf, NgSwitch, NgSwitchCase, NgSwitchDefault} from "@angular/common";
import {LucideAngularModule} from "lucide-angular";
import {FullCenteredFlex} from "../../../custom-directives/full-centered-flex.directive";
import {min} from "rxjs";

@Component({
  selector: 'user-card',
  standalone: true,
    imports: [
        NgIf,
        LucideAngularModule,
        FullCenteredFlex,
        NgSwitch,
        NgSwitchCase,
        NgSwitchDefault
    ],
  templateUrl: './user-card.component.html',
  styleUrl: './user-card.component.sass'
})
export class UserCardComponent {

    @Input()
    user: FriendCard

    imagePath: string = this.getRandomImage()

    getRandomImage() {
        let imagePaths = [
            './assets/user-profile-man-1.png',
            './assets/user-profile-man-2.png',
            './assets/user-profile-woman-1.png',
            './assets/user-profile-woman-2.png',
        ]

        return imagePaths[Math.floor(Math.random() * 4)]
    }

    @Output()
    clickedBefriend = new EventEmitter<string>()

    onClickBefriend(username:string) {
        this.clickedBefriend.emit(username)
    }
}
