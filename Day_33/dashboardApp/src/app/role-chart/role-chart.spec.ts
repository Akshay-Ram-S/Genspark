import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleChart } from './role-chart';

describe('RoleChart', () => {
  let component: RoleChart;
  let fixture: ComponentFixture<RoleChart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleChart]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoleChart);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
