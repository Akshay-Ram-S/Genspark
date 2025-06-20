import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerItems } from './seller-items';

describe('SellerItems', () => {
  let component: SellerItems;
  let fixture: ComponentFixture<SellerItems>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SellerItems]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SellerItems);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
