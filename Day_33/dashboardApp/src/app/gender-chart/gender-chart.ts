import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CanvasJSAngularChartsModule } from '@canvasjs/angular-charts';

@Component({
  selector: 'app-gender-chart',
  standalone: true,
  imports: [CommonModule, CanvasJSAngularChartsModule],
  templateUrl: './gender-chart.html',
  styleUrl: './gender-chart.css'
})

export class GenderChartComponent implements OnChanges {
  @Input() users: any[] = [];

  genderOptions: any;

  ngOnChanges() {
    if (this.users?.length) {
      this.updateGenderPercentage();
    }
    console.log(this.users)
  }

  updateGenderPercentage() {
    const total = this.users.length;
    const maleCount = this.users.filter(user => user.gender === 'male').length;
    const malePercentage = parseFloat(((maleCount * 100) / total).toFixed(2));
    const femalePercentage = parseFloat((100 - malePercentage).toFixed(2));

    this.genderOptions = {
      theme: "dark2",
      exportEnabled: true,
      title: { text: "Gender" },
      data: [{
        type: "pie",
        indexLabel: "{name}: {y}%",
        dataPoints: [
          { name: "Male", y: malePercentage },
          { name: "Female", y: femalePercentage }
        ]
      }]
    };
  }
}