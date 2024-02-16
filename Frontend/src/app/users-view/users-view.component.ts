import {Component, OnInit} from '@angular/core';
import {FriendCard, UserPersonalData} from "../../types";
import {UserCardComponent} from "./user-card/user-card.component";
import {NgForOf, NgIf} from "@angular/common";
import {LucideAngularModule} from "lucide-angular";
import {HttpService} from "../../services/http-service.service";
import {MessageService} from "primeng/api";
import {ToastModule} from "primeng/toast";
import {findIndex} from "rxjs";
import {FormsModule} from "@angular/forms";

@Component({
  selector: 'users-view',
  standalone: true,
    imports: [
        UserCardComponent,
        NgForOf,
        LucideAngularModule,
        ToastModule,
        FormsModule,
        NgIf
    ],
    providers: [HttpService, MessageService],
  templateUrl: './users-view.component.html',
  styleUrl: './users-view.component.sass'
})
export class UsersViewComponent implements OnInit{

    searchText: string = ''

    users: FriendCard[] = []

    constructor(private httpService: HttpService, private toastService: MessageService) {
    }

    ngOnInit() {
        this.getAllUsers()
    }

    searchUsers() {
        if (this.searchText != '') {
            this.httpService.searchUsersByUsername(this.searchText).subscribe({
                next: res => {
                    this.users = []
                    this.setUsers(res)
                },
                error: err => this.toastService.add({severity:'error', detail:'An error occurred when searching users'}),
            })
        }
        else {
            this.users = []
            this.getAllUsers()
        }
    }

    getAllUsers() {
        this.httpService.getAllUsers().subscribe({
            next: res => this.setUsers(res),
            error: err => this.toastService.add({severity:'error', detail:'An error occurred when fetching users'}),
        })
    }
    setUsers (usersList:UserPersonalData[]) {
        let storedMaster = this.getMaster()
        console.log(storedMaster.friends)
        usersList.forEach((user) => {
            if (user.username != "orleans.master")
                this.users.push({...user, befriended: storedMaster.friends.includes(user.username)})
        })
    }

    befriendUser (username:string) {
        console.log("clicked")
        this.httpService.befriendUser(username).subscribe({
            next: res => {
                this.addFriend(username)
                this.toastService.add({severity:'success', detail:'You have befriended '+username+'!'})
            },
            error: err => this.toastService.add({severity:'error', detail:'An error occurred when befriending '+username}),
        })
    }

    addFriend(username:string) {
        let master = this.getMaster()
        if (master)
            master.friends.push(username)
        this.storeMaster(master)

        let index = this.users.findIndex(user => user.username == username)

        if (index != -1)
            this.users[index].befriended = true
    }

    getMaster() {
        let master = null
        let storedMaster = sessionStorage.getItem("master")
        if (storedMaster)
            master = JSON.parse(storedMaster)
        return master
    }

    storeMaster(master:any) {
        sessionStorage.setItem('master', JSON.stringify(master))
    }
}
