import { Routes } from '@angular/router';
import { ChatViewComponent} from "./chat-view/chat-view.component";
import {SimulatorViewComponent} from "./simulator-view/simulator-view.component";
import {UsersViewComponent} from "./users-view/users-view.component";

export const routes: Routes = [
    //{ path: ''},
    { path: 'chat', component: ChatViewComponent},
    { path: 'simulator', component: SimulatorViewComponent},
    { path: 'users', component: UsersViewComponent},
];
