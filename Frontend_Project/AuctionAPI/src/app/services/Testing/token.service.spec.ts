import { TestBed } from '@angular/core/testing';
import { TokenService, JwtWrapperService } from '../token.service';

describe('TokenService', () => {
  let service: TokenService;
  let jwtWrapperSpy: jasmine.SpyObj<JwtWrapperService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('JwtWrapperService', ['decode']);

    TestBed.configureTestingModule({
      providers: [
        TokenService,
        { provide: JwtWrapperService, useValue: spy }
      ]
    });

    service = TestBed.inject(TokenService);
    jwtWrapperSpy = TestBed.inject(JwtWrapperService) as jasmine.SpyObj<JwtWrapperService>;
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should decode token when present', () => {
    const fakeToken = 'fake.token';
    const decodedPayload = { Id: 'user123', role: 'Seller' };
    localStorage.setItem('token', fakeToken);
    jwtWrapperSpy.decode.and.returnValue(decodedPayload);

    const result = service.getDecodedToken();
    expect(jwtWrapperSpy.decode).toHaveBeenCalledWith(fakeToken);
    expect(result).toEqual(decodedPayload);
  });

  it('should return null if no token', () => {
    expect(service.getDecodedToken()).toBeNull();
  });

  it('should return user Id from token', () => {
    localStorage.setItem('token', 'x');
    jwtWrapperSpy.decode.and.returnValue({ Id: 'abc123' });

    expect(service.getUserId()).toBe('abc123');
  });

  it('should return null if Id is missing', () => {
    localStorage.setItem('token', 'x');
    jwtWrapperSpy.decode.and.returnValue({});

    expect(service.getUserId()).toBeNull();
  });

  it('should return role from token', () => {
    localStorage.setItem('token', 'x');
    jwtWrapperSpy.decode.and.returnValue({ role: 'Bidder' });

    expect(service.getRole()).toBe('Bidder');
  });

  it('should return null if role is missing', () => {
    localStorage.setItem('token', 'x');
    jwtWrapperSpy.decode.and.returnValue({});

    expect(service.getRole()).toBeNull();
  });
});
