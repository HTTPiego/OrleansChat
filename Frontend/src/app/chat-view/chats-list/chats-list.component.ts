import {Component, Input} from '@angular/core';
import {ChatListItemComponent} from "../chat-list-item/chat-list-item.component";
import {NgClass, NgForOf, NgIf} from "@angular/common";
import {FullCenteredFlex} from "../../../custom-directives/full-centered-flex.directive";
import {ChatListItem} from "../../../types";
import {HttpService} from "../../../services/http-service.service";

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
        HttpService
    ],
  templateUrl: './chats-list.component.html',
  styleUrl: './chats-list.component.sass'
})
export class ChatsListComponent {

    selectedChatsOption: 'GROUPS'|'SINGLE' = 'SINGLE'

    constructor(private httpService: HttpService) {
    }

    fetchChats () {
        this.httpService.getChatsForUser('')
            .subscribe((data) => {

            })
    }
    onSelectedChatsChange (option: 'GROUPS'|'SINGLE') {
        this.selectedChatsOption = option
    }

    onSelectChat (chatId: string) {
        this.selectedChat = chatId
    }

    chats: ChatListItem[] = [
        {
            chatId: 'fran_bruno',
            chatName: "Bruno Gini",
            isGroup: false,
            lastSenderUsername: "franco.suelgaray",
            lastSenderName: "Franco Suelgaray",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "1 min ago",
            pendingMessages:0
        },
        {
            chatId:'fran_pepe',
            chatName: "Pepe Rodriguez",
            isGroup: false,
            lastSenderUsername: 'pepe.rodriguez',
            lastSenderName: "Pepe Rodriguez",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "2 hours ago",
            pendingMessages:5
        },
        {
            chatId:'fran_danna',
            chatName: "Danna Paola",
            isGroup: false,
            lastSenderUsername: 'danna.paola',
            lastSenderName: "Danna Paola",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "15 mins ago",
            pendingMessages:0
        },
        {
            chatId: 'fran_gini',
            chatName: "Bruno Gini",
            isGroup: false,
            lastSenderUsername: "franco.suelgaray",
            lastSenderName: "Franco Suelgaray",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "1 min ago",
            pendingMessages:0
        },
        {
            chatId:'fran_mon',
            chatName: "Pepe Rodriguez",
            isGroup: false,
            lastSenderUsername: 'pepe.rodriguez',
            lastSenderName: "Pepe Rodriguez",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "2 hours ago",
            pendingMessages:10
        },
        {
            chatId:'fran_gonza',
            chatName: "Danna Paola",
            isGroup: false,
            lastSenderUsername: 'danna.paola',
            lastSenderName: "Danna Paola",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "15 mins ago",
            pendingMessages:2
        },
        {
            chatId: 'fran_brun',
            chatName: "Bruno Gini",
            isGroup: false,
            lastSenderUsername: "franco.suelgaray",
            lastSenderName: "Franco Suelgaray",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "1 min ago",
            pendingMessages:0
        },
        {
            chatId:'fran_pep',
            chatName: "Pepe Rodriguez",
            isGroup: false,
            lastSenderUsername: 'pepe.rodriguez',
            lastSenderName: "Pepe Rodriguez",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "2 hours ago",
            pendingMessages:0
        },
        {
            chatId:'fran_dann',
            chatName: "Danna Paola",
            isGroup: false,
            lastSenderUsername: 'danna.paola',
            lastSenderName: "Danna Paola",
            lastMessage: "This is the last message sent to Fran. Es muy largo por lo que deberia aplicarse elipsis",
            lastMessageTime: "15 mins ago",
            pendingMessages:21
        },
    ]

    selectedChat: string = 'fran_bruno'
}
