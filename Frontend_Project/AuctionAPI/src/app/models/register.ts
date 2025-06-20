export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  pan: string;
  aadhar: string;
  role: 'bidder' | 'seller';
}
