import {Component, Input} from '@angular/core';
import {NgClass} from "@angular/common";

@Component({
  selector: 'chat-room-message',
  standalone: true,
    imports: [
        NgClass
    ],
  templateUrl: './chat-room-message.component.html',
  styleUrl: './chat-room-message.component.sass'
})
export class ChatRoomMessageComponent {
    @Input()
    message: {
        message: string,
        time: string,
        isOwn: boolean
    }
}
