export interface ResponseToken {
  token: string;
  expiration: number;
  isFirstLogin: boolean;
  role: string;
  hasPasswordExpired: boolean;
}
