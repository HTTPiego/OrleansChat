import {EventEmitter, Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";

@Injectable()
export class ChatroomService {

    public selectedChatRoom: BehaviorSubject<string> = new BehaviorSubject<string>('')
}
