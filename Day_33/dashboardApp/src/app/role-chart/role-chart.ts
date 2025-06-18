import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CanvasJSAngularChartsModule } from '@canvasjs/angular-charts';

@Component({
  selector: 'app-role-chart',
  standalone: true,
  imports: [CommonModule, CanvasJSAngularChartsModule],
  templateUrl: './role-chart.html',
  styleUrl: './role-chart.css' 
})

export class RoleChartComponent implements OnChanges {
  @Input() users: any[] = [];

  roleOptions: any;

  ngOnChanges() {
    if (this.users?.length) {
      this.updateRolePercentage();
    }
    console.log(this.users)
  }

  updateRolePercentage() {
    const total = this.users.length;
    const userCount = this.users.filter(user => user.role === 'user').length;
    const adminCount = this.users.filter(user => user.role === 'admin').length;
    const moderatorCount = this.users.filter(user => user.role === 'moderator').length;

    const userPercentage = parseFloat(((userCount / total) * 100).toFixed(2));
    const adminPercentage = parseFloat(((adminCount / total) * 100).toFixed(2));
    const moderatorPercentage = parseFloat(((moderatorCount / total) * 100).toFixed(2));

    this.roleOptions = {
      theme: "dark2",
      exportEnabled: true,
      title: { text: "Roles" },
      data: [{
        type: "doughnut",
        indexLabel: "{name}: {y}%",
        dataPoints: [
          { name: "User", y: userPercentage },
          { name: "Admin", y: adminPercentage },
          { name: "Moderator", y: moderatorPercentage }
        ]
      }]
    };
  }
}