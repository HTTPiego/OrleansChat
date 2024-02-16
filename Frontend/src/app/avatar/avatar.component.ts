import {Component, Input} from '@angular/core';
import {NgStyle} from "@angular/common";

@Component({
  selector: 'avatar',
  standalone: true,
    imports: [
        NgStyle
    ],
  templateUrl: './avatar.component.html',
  styleUrl: './avatar.component.sass'
})
export class AvatarComponent {
    @Input()
    name: string = 'No Name'

    @Input()
    size: string = '4rem'

    getInitials () {
        let initials = ''
        let words = this.name.split(' ')
        if (words.length > 1)
            initials = `${words[0].charAt(0).toUpperCase()}${words[1].charAt(0).toUpperCase()}`
        else if (words.length == 1)
            initials = `${words[0].charAt(0).toUpperCase()}`
        else
            initials = 'NN'
        return initials
    }
}
