import { Component, inject } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { LoaderService } from '../../../services/loader.service';

@Component({
  selector: 'app-loader',
  imports: [MatProgressBarModule],
  templateUrl: './loader.component.html'
})
export class AppLoaderComponent {
  loader = inject(LoaderService);
}
