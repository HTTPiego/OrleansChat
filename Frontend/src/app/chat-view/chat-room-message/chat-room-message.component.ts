import {Component, Input} from '@angular/core';
import {NgClass} from "@angular/common";
import {UserMessage} from "../../../types";
import moment from "moment";

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
    message: UserMessage
    protected readonly moment = moment;
}
