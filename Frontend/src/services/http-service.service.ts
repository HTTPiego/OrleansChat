import {Injectable} from "@angular/core";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {catchError, of, throwError} from "rxjs";
import {UserPersonalData, ChatListItem, ChatRoom, UserMessage} from "../types";
import * as signalR from "@microsoft/signalr";

@Injectable()
export class HttpService {
    baseUrl = 'http://localhost:5235'
    constructor(private http: HttpClient) { }

    initializeMasterUser () {
        let URL = this.baseUrl+'/api/Users/master-user'
        return this.http.get<UserPersonalData>(URL)
            .pipe(catchError(this.handleError))
    }

    connectToWebSocket() {
        const socketConn = new signalR.HubConnectionBuilder()
            .withUrl(this.baseUrl + "/chat-hub")
            .build()

        socketConn.start()
            .then((data) => console.log("Connection with socket successfull"))
            .catch((err) => console.log("An error occurred"))

        return socketConn
    }
    getAllUsers() {
        let URL = this.baseUrl+'/api/Users/all'
        return this.http.get<UserPersonalData[]>(URL)
            .pipe(catchError(this.handleError));
    }

    searchUsersByUsername(query) {
        let URL = this.baseUrl+'/api/Users/users-by?search='+query
        return this.http.get<UserPersonalData[]>(URL)
            .pipe(catchError(this.handleError));
    }

    befriendUser(username:string) {
        let URL = this.baseUrl+'/api/ChatRooms/orleans.master/befriend/'+username
        return this.http.get(URL)
            .pipe(catchError(this.handleError));
    }
    getChatsForUser (username: string) {
        let URL = this.baseUrl+'/api/Users/chats-preview-by?username='+username
        return this.http.get<ChatListItem[]>(URL)
            .pipe(catchError(this.handleError));
    }
    getChatRoom (chatRoom: string) {
        let URL = this.baseUrl+'/api/ChatRooms/'+chatRoom
        return this.http.get<ChatRoom>(URL)
            .pipe(catchError(this.handleError));
    }

    sendMessageToChatRoom (message: UserMessage) {
        let URL = this.baseUrl+'/api/Users/send-message'
        return this.http.post<UserMessage>(URL, message)
            .pipe(catchError(this.handleError));
    }

    simulateMassiveUserGeneration() {
        let URL = this.baseUrl+'/api/Simulator/massive-generate-users'
        return this.http.get(URL)
            .pipe(catchError(this.handleError));
    }
    private handleError(error: HttpErrorResponse) {
        return throwError(() => new Error())
    }
}
