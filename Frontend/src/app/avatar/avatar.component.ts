import {Component, Input} from '@angular/core';

@Component({
  selector: 'avatar',
  standalone: true,
  imports: [],
  templateUrl: './avatar.component.html',
  styleUrl: './avatar.component.sass'
})
export class AvatarComponent {
    @Input()
    name: string = 'No Name'

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
