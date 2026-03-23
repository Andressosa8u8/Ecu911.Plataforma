export type UserMe = {
  id: string;
  username: string;
  fullName: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  organizationalUnitId?: string | null;
  roles: string[];
  systems: string[];
  currentSystem?: string | null;
};

export type LoginRequest = {
  username: string;
  password: string;
  systemCode?: string;
};

export type LoginResponse = {
  token: string;
  expiration: string;
  user: UserMe;
};