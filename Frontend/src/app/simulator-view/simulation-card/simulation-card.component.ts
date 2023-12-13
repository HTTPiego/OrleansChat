import {Component, Input} from '@angular/core';
import {LucideAngularModule} from "lucide-angular";
import {HttpService} from "../../../services/http-service.service";
import {MessageService} from "primeng/api";

@Component({
  selector: 'simulation-card',
  standalone: true,
    imports: [
        LucideAngularModule
    ],
    providers: [HttpService],
  templateUrl: './simulation-card.component.html',
  styleUrl: './simulation-card.component.sass'
})
export class SimulationCardComponent {

    constructor(private httpService: HttpService, private toastService: MessageService) {
    }

    @Input()
    OnSimulate: () => void;

    @Input() cardName: string

    @Input() description: string

    @Input() imagePath: string

}
