import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';

@Component({
  selector: 'water-dashboard-nav',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent],
  templateUrl: './water-dashboard-nav.component.html',
  styleUrl: './water-dashboard-nav.component.scss'
})
export class WaterDashboardNavComponent {
}
