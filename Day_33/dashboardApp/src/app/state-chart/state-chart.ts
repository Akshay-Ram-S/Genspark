import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CanvasJSAngularChartsModule } from '@canvasjs/angular-charts';

@Component({
  selector: 'app-state-chart',
  standalone: true,
  imports: [CommonModule, CanvasJSAngularChartsModule],
  templateUrl: './state-chart.html',
  styleUrl: './state-chart.css'
})


export class StateChartComponent implements OnChanges {
  @Input() users: any[] = [];

  readonly validStates = new Set([
    'Alabama', 'Arkansas', 'Colorado', 'Connecticut', 'Delaware',
    'Florida', 'Hawaii', 'Idaho', 'Illinois', 'Iowa',
    'Kansas', 'Louisiana', 'Maine', 'Maryland', 'Massachusetts',
    'Minnesota', 'Mississippi', 'Missouri', 'Montana', 'Nebraska',
    'Nevada', 'New Hampshire', 'New Jersey', 'New Mexico', 'New York',
    'North Carolina', 'North Dakota', 'Ohio', 'Oregon', 'Pennsylvania',
    'Rhode Island', 'South Dakota', 'Tennessee', 'Texas', 'Utah',
    'West Virginia', 'Wisconsin', 'Wyoming'
  ]);

  stateOptions: any;

  ngOnChanges() {
    if (this.users?.length) {
      this.updateStateChart();
    }
    console.log(this.users)
  }

  updateStateChart() {
    const stateMap: { [key: string]: number } = {};

    this.users.forEach(user => {
      let state = user.address?.state?.trim();
      if (!state || !this.validStates.has(state)) {
        state = 'Unknown';
      }
      stateMap[state] = (stateMap[state] || 0) + 1;
    });

    const dataPoints = Object.entries(stateMap).map(([label, y]) => ({ label, y }));

    this.stateOptions = {
      animationEnabled: true,
      theme: 'light2',
      title: { text: 'Users by State' },
      axisY: { title: 'User Count' },
      axisX: { labelAngle: -45 },
      data: [{
        type: 'column',
        dataPoints
      }]
    };
  }
}
