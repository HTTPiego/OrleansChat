import {Component, inject, Input, OnInit} from '@angular/core';
import {FullCenteredFlex} from "../../../custom-directives/full-centered-flex.directive";
import {LucideAngularModule} from "lucide-angular";
import {ChatRoomMessageComponent} from "../chat-room-message/chat-room-message.component";
import {NgClass, NgForOf, NgIf, NgStyle} from "@angular/common";
import {ToastModule} from "primeng/toast";
import {MessageService} from "primeng/api";
import * as signalR from "@microsoft/signalr";
import {HttpService} from "../../../services/http-service.service";
import {ChatroomService} from "../../../services/chatroom.service";
import {ChatRoom, UserMessage} from "../../../types";
import {FormsModule} from "@angular/forms";
import {AvatarComponent} from "../../avatar/avatar.component";

@Component({
  selector: 'chat-room',
  standalone: true,
    imports: [
        FullCenteredFlex,
        LucideAngularModule,
        ChatRoomMessageComponent,
        NgForOf,
        NgClass,
        NgStyle,
        ToastModule,
        NgIf,
        FormsModule,
        AvatarComponent,
    ],
    providers: [MessageService, HttpService],
  templateUrl: './chat-room.component.html',
  styleUrl: './chat-room.component.sass'
})
export class ChatRoomComponent implements OnInit{

    @Input()
    selectedChatId: string = '';

    loading: boolean = true

    chatRoomData: ChatRoom;

    newMessage: string = ''

    chatVisibleName: string
    constructor(private toastService: MessageService, private httpService: HttpService, ) {

    }

    sendMessage() {
        if (this.newMessage != '') {
            let message: UserMessage = {
                authorUsername: 'orleans.master',
                textMessage: this.newMessage,
                timestamp: new Date(),
                chatRoomName: this.selectedChatId
            }
            this.httpService.sendMessageToChatRoom(message).subscribe({
                next: res => {
                    this.newMessage = ''
                    console.log("Message sent!")
                },
                error: err => this.toastService.add({severity: 'error', detail: 'An error occurred when sending the message.'})
            })
        }
    }

    getChatRoomState() {
        this.httpService.getChatRoom(this.selectedChatId).subscribe({
            next: res => {
                this.chatRoomData = {
                    chatName: res.chatName,
                    messages: res.messages.reverse()
                }
                this.chatVisibleName = res.chatName.split('_')[1].split('.').join(' ').toUpperCase()
                this.loading = false
            },
            error: err =>  this.toastService.add({severity:'error', detail:'An error occurred when loading the Chatroom'}),
        })
    }

    ngOnInit() {
        if (this.selectedChatId != '') {
            this.getChatRoomState()

            const socketConn = this.httpService.connectToWebSocket()

            socketConn.on("chatRoomUpdate", (data) => {
                if (this.chatRoomData) {
                    this.chatRoomData.messages.unshift(data)
                }
            })
        }
    }

    protected readonly navigator = navigator;
}
