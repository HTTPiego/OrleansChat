import {NgModule} from "@angular/core";
import {MessagesSquare, Cpu, Send, Users, Play, UserPlus, UserCheck, Check, Search, LucideAngularModule} from "lucide-angular";

@NgModule({
    imports: [
        LucideAngularModule.pick({MessagesSquare, Cpu, Send, Users, Play, UserPlus, UserCheck, Check, Search})
    ],
})
export class MyIconsModule {}
