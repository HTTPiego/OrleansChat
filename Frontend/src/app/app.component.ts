import {Component, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {CustomButtonComponent} from "./custom-button/custom-button.component";
import {ChatListItemComponent} from "./chat-view/chat-list-item/chat-list-item.component";
import {NavBarComponent} from "./nav-bar/nav-bar.component";
import {HttpService} from "../services/http-service.service";
import {ToastModule} from "primeng/toast";
import {MessageService} from "primeng/api";

@Component({
  selector: 'app-root',
  standalone: true,
    imports: [CommonModule, RouterOutlet, CustomButtonComponent, ChatListItemComponent, NavBarComponent, ToastModule],
    providers:[HttpService, MessageService],
  templateUrl: './app.component.html',
  styleUrl: './app.component.sass'
})
export class AppComponent implements OnInit{
  title = 'orleans-chat-frontend';

  constructor(private httpService: HttpService, private toastService: MessageService) {
  }

  ngOnInit() {
      // Sets master user to local storage if it is not set
      if (localStorage.getItem('master') == null) {
          this.httpService.initializeMasterUser()
              .subscribe({
                  next: res => {
                      localStorage.setItem('master', JSON.stringify(res))
                      this.toastService.add({severity: 'success', detail: 'Master user successfully initialized'})
                  },
                  error: e => this.toastService.add({severity:'error', detail:'An error occurred when initializing master'}),
              })
      }

  }
}
