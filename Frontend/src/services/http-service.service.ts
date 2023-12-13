import {Injectable} from "@angular/core";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {catchError, of, throwError} from "rxjs";
import {UserPersonalData} from "../types";

@Injectable()
export class HttpService {
    baseUrl = 'http://localhost:5235'
    constructor(private http: HttpClient) { }

    initializeMasterUser () {
        let URL = this.baseUrl+'/api/Users/master-user'
        return this.http.get<UserPersonalData>(URL)
            .pipe(catchError(this.handleError))
    }

    getAllUsers() {
        let URL = this.baseUrl+'/api/Users/all'
        return this.http.get<UserPersonalData[]>(URL)
            .pipe(catchError(this.handleError));
    }

    befriendUser(username:string) {
        let URL = this.baseUrl+'/api/Users/orleans.master/befriend/'+username
        return this.http.get(URL)
            .pipe(catchError(this.handleError));
    }
    getChatsForUser (username: string) {
        let URL = this.baseUrl+'/api/Users/'+username+'/chats'
        return this.http.get(URL);
    }
    getChatRoomForUser (chatRoom: string, username: string) {
        let URL = this.baseUrl+'/api/ChatRoom/'+chatRoom+'/'+username
        return this.http.get(URL);
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
