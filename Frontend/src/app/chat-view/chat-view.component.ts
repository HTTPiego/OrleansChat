import { Component } from '@angular/core';
import {ChatsListComponent} from "./chats-list/chats-list.component";
import {ChatRoomComponent} from "./chat-room/chat-room.component";

@Component({
  selector: 'chat-view',
  standalone: true,
    imports: [
        ChatsListComponent,
        ChatRoomComponent
    ],
  templateUrl: './chat-view.component.html',
  styleUrl: './chat-view.component.sass'
})
export class ChatViewComponent {

}
