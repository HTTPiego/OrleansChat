import { Component } from '@angular/core';
import {FullCenteredFlex} from "../../../custom-directives/full-centered-flex.directive";
import {LucideAngularModule} from "lucide-angular";
import {ChatRoomMessageComponent} from "../chat-room-message/chat-room-message.component";
import {NgClass, NgForOf, NgStyle} from "@angular/common";
import {ToastModule} from "primeng/toast";
import {MessageService} from "primeng/api";

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
    ],
    providers: [MessageService],
  templateUrl: './chat-room.component.html',
  styleUrl: './chat-room.component.sass'
})
export class ChatRoomComponent {
    constructor(private toastService: MessageService) {}

    messages: {
        message: string,
        time: string,
        isOwn: boolean
    }[] = [
        {
            message: 'Este es otro mensaje mas cortito, pero de mi usuario',
            time: 'Today 12:20pm',
            isOwn: true
        },
        {
            message: 'Este es otro mensaje mas cortito',
            time: 'Today 12:19pm',
            isOwn: false
        },
        {
            message: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco.',
            time: 'Today 12:18pm',
            isOwn: false
        },
        {
            message: 'Este es un mensaje de prueba. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco.',
            time: 'Today 11:07am',
            isOwn: true
        },
        {
            message: 'Este es otro mensaje mas cortito, pero de mi usuario',
            time: 'Today 10:08am',
            isOwn: true
        },
        {
            message: 'Este es otro mensaje mas cortito',
            time: 'Today 09:13am',
            isOwn: false
        },
        {
            message: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco.',
            time: 'Yesterday 00:20am',
            isOwn: false
        },
        {
            message: 'Este es un mensaje de prueba. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco.',
            time: 'Yesterday 23:35pm',
            isOwn: true
        },
        ]

    sendMessage() {
        console.log('Hi')
        this.toastService.add({severity: 'success', summary: 'New Message', detail: 'The message was successfully sent'})
    }
}
