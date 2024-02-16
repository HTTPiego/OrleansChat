import { Component, Input } from '@angular/core';
import {NgClass} from "@angular/common";
import {BadgeComponent} from "../../badge/badge.component";
import {ChatListItem} from "../../../types";
import {AvatarComponent} from "../../avatar/avatar.component";
import moment from "moment/moment";

@Component({
  selector: 'chat-list-item',
  standalone: true,
    imports: [
        NgClass,
        BadgeComponent,
        AvatarComponent
    ],
  templateUrl: './chat-list-item.component.html',
  styleUrl: './chat-list-item.component.sass'
})
export class ChatListItemComponent {
    @Input()
    chatItem: ChatListItem

    @Input()
    isSelected:boolean
    protected readonly moment = moment;
}
