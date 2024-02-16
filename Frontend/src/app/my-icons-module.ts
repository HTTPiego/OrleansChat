import {NgModule} from "@angular/core";
import {MessagesSquare, Cpu, Send, Users, Play, UserPlus, UserCheck, Check, Search, BarChartBig, LucideAngularModule} from "lucide-angular";

@NgModule({
    imports: [
        LucideAngularModule.pick({MessagesSquare, Cpu, Send, Users, Play, UserPlus, UserCheck, Check, Search, BarChartBig})
    ],
})
export class MyIconsModule {}
