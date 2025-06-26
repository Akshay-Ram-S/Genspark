import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowBids } from './show-bids';

describe('ShowBids', () => {
  let component: ShowBids;
  let fixture: ComponentFixture<ShowBids>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShowBids]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShowBids);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
