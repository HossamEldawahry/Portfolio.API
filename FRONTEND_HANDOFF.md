# Frontend Handoff - Portfolio API

هذا الملف هو مرجع كامل للفرونت عشان يبني المشروع من الصفر على الـ API الحالي.

## 1) Base Info

- **Base URL**: `https://<your-api-domain>`
- **API Version (preferred)**: `api/v1/...`
- يوجد بعض مسارات legacy (مثل `GetAll`, `Create`) للحفاظ على التوافق.
- كل الـ responses بصيغة JSON ما عدا upload endpoints (`multipart/form-data`).

## 2) Authentication Flow (Admin)

### 2.1 Login
- `POST /api/v1/auth/login`
- Body:
```json
{
  "username": "admin",
  "password": "ChangeMe123!"
}
```
- Success `200`:
```json
{
  "accessToken": "jwt_here",
  "refreshToken": "refresh_token_here",
  "tokenType": "Bearer",
  "expiresAtUtc": "2026-04-09T12:00:00Z"
}
```

### 2.2 Refresh (Rotate)
- `POST /api/v1/auth/refresh`
- Body:
```json
{
  "refreshToken": "refresh_token_here"
}
```
- يرجع access + refresh جديدين.
- لازم الفرونت يستبدل الـ refresh القديم بالجديد فورًا.

### 2.3 Logout
- `POST /api/v1/auth/logout`
- Body:
```json
{
  "refreshToken": "refresh_token_here"
}
```
- Success: `204 No Content`

### 2.4 Authorization Header
- أي endpoint Admin يحتاج:
- `Authorization: Bearer <accessToken>`

## 3) Data Models (Frontend Types)

```ts
export interface Project {
  id: number;
  title: string;
  description: string;
  imageUrl?: string | null;
  gitHubUrl?: string | null;
  demoUrl?: string | null;
  isFeatured: boolean;
}

export interface Skill {
  id: number;
  name: string;
  level: number; // 0..5
}

export interface Profile {
  id: number;
  name: string;
  title: string;
  email: string;
  bio: string;
  imageUrl?: string | null;
  linkedInUrl?: string | null;
  gitHubUrl?: string | null;
}

export interface Message {
  id: number;
  name: string;
  email: string;
  subject: string;
  content: string;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PortfolioStats {
  projectsCount: number;
  skillsCount: number;
  messagesCount: number;
}
```

## 4) Public Endpoints (no auth)

### Projects
- `GET /api/v1/projects`
- `GET /api/v1/projects/{id}`
- `GET /api/v1/projects/paged?page=1&pageSize=5`
- `GET /api/v1/projects/count`
- `GET /api/v1/projects/featured?take=6`

### Skills
- `GET /api/v1/skills`
- `GET /api/v1/skills/{id}`
- `GET /api/v1/skills/paged?page=1&pageSize=10`
- `GET /api/v1/skills/count`

### Profile
- `GET /api/v1/profile/current`
- `GET /api/v1/profile/{id}`

### Stats
- `GET /api/v1/stats`

### Contact (Public with Rate Limit)
- `POST /api/v1/messages`
- Body:
```json
{
  "name": "Client Name",
  "email": "client@example.com",
  "subject": "Project inquiry",
  "content": "I want to build a web app..."
}
```
- Rate limit على endpoint ده (`429` لو تعدى الحد).

## 5) Admin Endpoints (JWT required)

### Projects (multipart/form-data)
- `POST /api/v1/projects`
- `PUT /api/v1/projects/{id}`
- `DELETE /api/v1/projects/{id}`

`FormData` fields:
- `title` (required, max 200)
- `description` (required, max 8000)
- `image` (optional file)
- `gitHubUrl` (optional)
- `demoUrl` (optional)
- `isFeatured` (boolean)

### Skills (application/json)
- `POST /api/v1/skills`
- `PUT /api/v1/skills/{id}`
- `DELETE /api/v1/skills/{id}`

Body:
```json
{
  "name": "Angular",
  "level": 4
}
```

### Profile (multipart/form-data)
- `POST /api/v1/profile`
- `PUT /api/v1/profile/{id}`

`FormData` fields:
- `name` (required)
- `title` (required)
- `email` (required)
- `bio` (required)
- `image` (optional file)
- `linkedInUrl` (optional)
- `gitHubUrl` (optional)

### Messages Admin
- `GET /api/v1/messages`
- `GET /api/v1/messages/{id}`
- `GET /api/v1/messages/paged?page=1&pageSize=10`
- `GET /api/v1/messages/count`
- `PUT /api/v1/messages/{id}`
- `DELETE /api/v1/messages/{id}`

## 6) Status Codes to handle in UI

- `200` success read
- `201` created
- `204` no content (update/delete/logout)
- `400` validation error
- `401` auth failed / token invalid
- `404` not found
- `429` too many requests (contact form)
- `500` server error

## 7) Frontend Auth Strategy (recommended)

- خزّن:
  - `accessToken` في memory state (أفضل)
  - `refreshToken` في secure storage (حسب سياسة الأمان عندكم)
- Axios/fetch interceptor:
  - يضيف `Authorization: Bearer ...`
  - عند `401`: جرب `POST /api/v1/auth/refresh` مرة واحدة
  - لو refresh نجح: أعد الطلب الأصلي
  - لو فشل: اعمل logout محلي وارجع لشاشة login

## 8) Screens to Implement

- Public:
  - Home (Profile + Skills + Featured Projects + Stats)
  - Projects list/details
  - Contact form
- Admin:
  - Login
  - Dashboard stats
  - Manage Projects (CRUD + featured toggle)
  - Manage Skills (CRUD)
  - Manage Profile (create/update)
  - Manage Messages (list/read/delete/update status في الواجهة)

## 9) Ready-to-use Frontend TODO

- [ ] Create `apiClient` with base URL and interceptors
- [ ] Implement `authService` (login/refresh/logout)
- [ ] Implement `projectsService`, `skillsService`, `profileService`, `messagesService`, `statsService`
- [ ] Add route guards for admin pages
- [ ] Build admin forms (`FormData` for project/profile)
- [ ] Add error toaster handling for `400/401/429`

## 10) Notes

- Swagger متاح في dev وفيه authorize + examples جاهزة لمسارات auth.
- استخدم دائمًا `api/v1` في الفرونت الجديد.
- لو عندكم فرونت قديم، مسارات legacy ما زالت موجودة مؤقتًا.
