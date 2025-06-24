import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LiveBid } from './live-bid';

describe('LiveBid', () => {
  let component: LiveBid;
  let fixture: ComponentFixture<LiveBid>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LiveBid]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LiveBid);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
