import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ChatListItemComponent} from "../chat-list-item/chat-list-item.component";
import {NgClass, NgForOf, NgIf} from "@angular/common";
import {FullCenteredFlex} from "../../../custom-directives/full-centered-flex.directive";
import {ChatListItem} from "../../../types";
import {HttpService} from "../../../services/http-service.service";
import {ChatroomService} from "../../../services/chatroom.service";

@Component({
  selector: 'chats-list',
  standalone: true,
    imports: [
        ChatListItemComponent,
        NgForOf,
        NgClass,
        FullCenteredFlex,
        NgIf
    ],
    providers: [
        HttpService, ChatroomService
    ],
  templateUrl: './chats-list.component.html',
  styleUrl: './chats-list.component.sass'
})
export class ChatsListComponent implements OnInit{

    @Output()
    selectedChatChanged: EventEmitter<string> = new EventEmitter<string>()

    selectedChatsOption: 'GROUPS'|'SINGLE' = 'SINGLE'
    chatsPreviews: ChatListItem[] = []

    constructor(private httpService: HttpService, private chatService: ChatroomService) {
    }

    ngOnInit() {
        this.fetchChats()
    }

    fetchChats () {
        this.httpService.getChatsForUser('orleans.master')
            .subscribe({
                next: res => this.chatsPreviews = res,
                error: err => console.log(err)
            })
    }
    onSelectedChatsChange (option: 'GROUPS'|'SINGLE') {
        this.selectedChatsOption = option
    }

    onSelectChat (chatId: string) {
        this.selectedChat = chatId
        this.selectedChatChanged.emit(this.selectedChat);
        this.chatService.selectedChatRoom.next(this.selectedChat)
    }

    selectedChat: string = ''
}
