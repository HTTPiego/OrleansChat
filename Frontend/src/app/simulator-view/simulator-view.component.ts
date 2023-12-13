import { Component } from '@angular/core';
import {ChatRoomComponent} from "../chat-view/chat-room/chat-room.component";
import {ChatsListComponent} from "../chat-view/chats-list/chats-list.component";
import {SimulationCardComponent} from "./simulation-card/simulation-card.component";
import {FullCenteredFlex} from "../../custom-directives/full-centered-flex.directive";
import {HttpService} from "../../services/http-service.service";
import {MessageService} from "primeng/api";
import {ToastModule} from "primeng/toast";

@Component({
  selector: 'simulator-view',
  standalone: true,
    imports: [
        ChatRoomComponent,
        ChatsListComponent,
        SimulationCardComponent,
        FullCenteredFlex,
        ToastModule
    ],
    providers:[HttpService, MessageService],
  templateUrl: './simulator-view.component.html',
  styleUrl: './simulator-view.component.sass'
})
export class SimulatorViewComponent {

    protected readonly console = console;

    constructor(private httpService: HttpService, private toastService: MessageService) {
    }

    massiveGenerateUsers() {
        this.httpService.simulateMassiveUserGeneration().subscribe({
            next: res => this.toastService.add({severity:'success', detail:'20 users successfully generated!'}),
            error: err => this.toastService.add({severity:'error', detail:'An error occurred while generating the users'}),
        })
    }

}
