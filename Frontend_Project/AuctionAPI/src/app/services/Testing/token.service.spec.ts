import { TokenService } from '../token.service';
import { jwtDecode } from 'jwt-decode';

describe('TokenService', () => {
  let service: TokenService;

  beforeEach(() => {
    service = new TokenService();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should decode token when present', () => {
    const payload = { Id: 'user123', role: 'Seller' };
    const base64Payload = btoa(JSON.stringify(payload));
    const fakeToken = `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.${base64Payload}.signature`;

    localStorage.setItem('token', fakeToken);

    const result = service.getDecodedToken();
    expect(result).toEqual(jasmine.objectContaining(payload));
  });

  it('should return null if no token', () => {
    expect(service.getDecodedToken()).toBeNull();
  });

  it('should return user Id from token', () => {
    const mockPayload = { Id: 'abc123' };
    spyOn(service, 'getDecodedToken').and.returnValue(mockPayload);

    expect(service.getUserId()).toBe('abc123');
  });

  it('should return null if Id is missing', () => {
    spyOn(service, 'getDecodedToken').and.returnValue({});
    expect(service.getUserId()).toBeNull();
  });

  it('should return role from token', () => {
    const mockPayload = { role: 'Bidder' };
    spyOn(service, 'getDecodedToken').and.returnValue(mockPayload);

    expect(service.getRole()).toBe('Bidder');
  });

  it('should return null if role is missing', () => {
    spyOn(service, 'getDecodedToken').and.returnValue({});
    expect(service.getRole()).toBeNull();
  });
});
