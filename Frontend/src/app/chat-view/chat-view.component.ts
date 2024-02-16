import {Component, OnInit} from '@angular/core';
import {ChatsListComponent} from "./chats-list/chats-list.component";
import {ChatRoomComponent} from "./chat-room/chat-room.component";
import {NgIf} from "@angular/common";
import {NgxRerenderModule} from "ngx-rerender";
import {HttpService} from "../../services/http-service.service";

@Component({
  selector: 'chat-view',
  standalone: true,
    imports: [
        ChatsListComponent,
        ChatRoomComponent,
        NgIf,
        NgxRerenderModule
    ],
    providers: [
      HttpService
    ],
  templateUrl: './chat-view.component.html',
  styleUrl: './chat-view.component.sass'
})
export class ChatViewComponent implements OnInit{

    selectedChatId: string = '';

    constructor(private httpService: HttpService) {
    }
    onSelectedChatChange(chatId:string) {
        this.selectedChatId = chatId
    }

    ngOnInit() {

    }
}
