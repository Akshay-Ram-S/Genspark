export interface User {
  name: string;
  email: string;
  role: string;
  userId: string;
  status: string;
  sellerId?: string;
  bidderId?: string;
}