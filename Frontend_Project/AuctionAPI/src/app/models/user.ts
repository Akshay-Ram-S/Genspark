export interface User {
  name: string;
  email: string;
  role: string;
  userId: string;
  sellerId?: string;
  bidderId?: string;
}