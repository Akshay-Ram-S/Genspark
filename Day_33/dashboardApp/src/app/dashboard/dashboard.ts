import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../services/user.service';
import { CanvasJSAngularChartsModule } from '@canvasjs/angular-charts';
import { debounceTime } from 'rxjs';
import { GenderChartComponent } from '../gender-chart/gender-chart';
import { RoleChartComponent } from '../role-chart/role-chart';
import { StateChartComponent } from '../state-chart/state-chart';

@Component({
  selector: 'app-dashboard',
  imports: [ReactiveFormsModule, GenderChartComponent, RoleChartComponent,StateChartComponent],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})


export class DashboardComponent implements OnInit {
  allUsers: any[] = [];
  filteredUsers: any[] = [];

  filterForm = new FormGroup({
    gender: new FormControl(''),
    role: new FormControl(''),
    state: new FormControl('')
  });

  constructor(private userService: UserService) {}

  readonly states = new Set([
    'Alabama', 'Arkansas', 'Colorado', 'Connecticut', 'Delaware',
    'Florida', 'Hawaii', 'Idaho', 'Illinois', 'Iowa',
    'Kansas', 'Louisiana', 'Maine', 'Maryland', 'Massachusetts',
    'Minnesota', 'Mississippi', 'Missouri', 'Montana', 'Nebraska',
    'Nevada', 'New Hampshire', 'New Jersey', 'New Mexico', 'New York',
    'North Carolina', 'North Dakota', 'Ohio', 'Oregon', 'Pennsylvania',
    'Rhode Island', 'South Dakota', 'Tennessee', 'Texas', 'Utah',
    'West Virginia', 'Wisconsin', 'Wyoming'
  ]);

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (res) => {
        this.allUsers = res.users;
        this.filteredUsers = [...this.allUsers];
      },
      error: (err) => {
        console.error('Error fetching users:', err);
      }
    });

    this.filterForm.valueChanges
      .pipe(debounceTime(300))
      .subscribe(() => this.applyFilters());
  }

  applyFilters() {
    const { gender, role, state } = this.filterForm.value;

    this.filteredUsers = this.allUsers.filter(user =>
      (!gender || user.gender === gender) &&
      (!role || user.role === role) &&
      (!state || user.address.state === state)
    );
  }

  clearFilters() {
    this.filterForm.reset();
    this.filteredUsers = [...this.allUsers];
  }
}