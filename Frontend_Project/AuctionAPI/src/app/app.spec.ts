import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { of } from 'rxjs';

describe('App', () => {
  const mockActivatedRoute = {
    snapshot: {
      paramMap: {
        get: (key: string) => 'mockId'
      },
      url: [{ path: 'profile' }, { path: 'seller' }]
    }
  };

  const routerSpy = {
    events: of(new NavigationEnd(0, '/', '/')),
    navigate: jasmine.createSpy('navigate'),
    createUrlTree: jasmine.createSpy('createUrlTree').and.returnValue({}),
    serializeUrl: jasmine.createSpy('serializeUrl').and.callFake((url) => url)
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App, HttpClientTestingModule],
      providers: [
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render navbar component', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-navbar-component')).toBeTruthy();
  });
});
