# E-Commerce Project – Quick Revision Document Version 2
## Backend Commit 3, Frontend Commit 3, Backend Commit 4 and Frontend Commit 4

---

# 1. Project Overview

This phase of the E-Commerce project implements two major modules:

1. **Product Management and Product Display**
2. **Customer Registration and Authentication**

The application follows a layered architecture in the backend and Angular standalone component architecture in the frontend.

### Technologies Used

| Backend | Frontend |
|---|---|
| C# | Angular |
| .NET 10 Web API | TypeScript |
| Entity Framework Core | HTML |
| SQL Server | CSS |
| Repository Pattern | Angular Services |
| Service Layer | HttpClient |
| DTOs | RxJS Observables |
| JWT Authentication | Template-driven Forms |

The frontend communicates with the backend through HTTP APIs. The backend communicates with SQL Server using Entity Framework Core.

---

# 2. Backend Commit 3 – Product CRUD, Validation, Search and Pagination

## 2.1 Objective

The objective was to implement Product APIs that allow the application to:

- Create products.
- Retrieve all active products.
- Retrieve an individual product.
- Update product information.
- Deactivate products.
- Search products by name.
- Filter products by category.
- Retrieve products using pagination.

The existing Product model and database table were created in Backend Commit 1. Therefore, no new migration was required.

## 2.2 Backend Architecture

The Product module follows this execution flow:

**Controller → Service → Repository → DbContext → SQL Server**

### Controller

File: `Controllers/ProductsController.cs`

Responsibilities:

- Receives HTTP requests.
- Reads route and query parameters.
- Accepts DTOs.
- Calls the Product Service.
- Returns appropriate HTTP responses.

Example endpoints:

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/products` | Retrieve products |
| GET | `/api/products/{id}` | Retrieve one product |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Deactivate product |

### Service

File: `Services/ProductService.cs`

The service contains the business logic.

Important responsibilities:

- Validate whether the selected category exists and is active.
- Trim product names and descriptions.
- Set default values when creating products.
- Apply search and pagination parameters.
- Map database entities into response DTOs.
- Deactivate products instead of permanently deleting them.

### Repository

File: `Repositories/ProductRepository.cs`

The repository handles database operations through `ApplicationDbContext`.

Important Entity Framework methods:

- `Include()` – loads related Category information.
- `AsNoTracking()` – retrieves data without tracking changes when only reading.
- `Where()` – filters products.
- `AnyAsync()` – checks whether a category exists.
- `CountAsync()` – counts matching products.
- `OrderBy()` – sorts products.
- `Skip()` and `Take()` – implement pagination.
- `ToListAsync()` – executes the query and returns the results.
- `SaveChangesAsync()` – saves changes to the database.

---

## 2.3 DTOs

DTO means Data Transfer Object.

DTOs define the information accepted or returned by the API.

We created:

1. `CreateProductDto`
2. `UpdateProductDto`
3. `ProductResponseDto`
4. `ProductListResponseDto`

### Why do we use DTOs?

We should not directly expose our database entity for every API operation.

DTOs allow us to:

- Control which properties users can submit.
- Apply input validation.
- Control the response structure.
- Avoid exposing unnecessary database information.

### Validation Attributes

| Attribute | Purpose |
|---|---|
| `[Required]` | Property must have a value |
| `[StringLength]` | Restricts text length |
| `[Range]` | Restricts numeric values |

Examples:

- Product name is required.
- Price must be greater than zero.
- Stock quantity cannot be negative.
- Category ID must be a valid positive number.

Because the controller uses `[ApiController]`, invalid model data automatically produces a validation response before the action method executes.

---

## 2.4 Product Creation Flow

Example request:

`POST /api/products`

```json
{
  "productName": "Samsung Galaxy S25",
  "description": "Samsung smartphone with 256GB storage",
  "price": 79999,
  "stockQuantity": 15,
  "imageUrl": "https://example.com/samsung.jpg",
  "categoryId": 1
}
```

Execution flow:

1. The client sends product information.
2. `ProductsController` receives the request.
3. ASP.NET Core validates the DTO.
4. The controller calls `ProductService.CreateAsync()`.
5. The service checks whether the category exists and is active.
6. The service creates a Product entity.
7. It assigns default values:
   - `IsActive = true`
   - `CreatedAt = DateTime.UtcNow`
8. The repository adds the entity to the DbContext.
9. `SaveChangesAsync()` saves it to SQL Server.
10. The service returns a Product Response DTO.
11. The controller responds with `201 Created`.

---

## 2.5 Search and Category Filtering

Example:

`GET /api/products?searchTerm=Samsung&categoryId=1`

The controller receives query parameters and passes them to the service.

The repository builds a query:

- First, retrieve active products.
- If a search term exists, filter by product name.
- If a category ID exists, filter by that category.
- Count the matching products.
- Sort the results.
- Return the requested page.

Search is performed using:

`ProductName.Contains(searchTerm)`

Category filtering uses the product's `CategoryId`.

Both filters can be applied together.

---

## 2.6 Pagination Logic

Pagination prevents the API from returning every product at once.

We use:

- `pageNumber`
- `pageSize`

Example:

`GET /api/products?pageNumber=1&pageSize=5`

The repository uses:

```csharp
Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
```

Example:

If page number is 2 and page size is 5:

`Skip(5).Take(5)`

The first five products are skipped, and the next five products are returned.

The service calculates:

```csharp
TotalPages = (int)Math.Ceiling(
    TotalCount / (double)pageSize
);
```

The response includes:

- Products
- TotalCount
- PageNumber
- PageSize
- TotalPages

The service also ensures that page number is at least 1 and page size is between 1 and 50.

---

## 2.7 Soft Delete

Instead of permanently deleting a product, we set:

```csharp
product.IsActive = false;
```

This is called soft deletion or deactivation.

Why?

Products may already be referenced by CartItems or OrderItems. Keeping the record protects historical order information and database relationships.

Inactive products are excluded from normal product retrieval.

---

# 3. Frontend Commit 3 – Product List, Product Card, Product Details, Search and Filtering

## 3.1 Objective

The objective was to connect Angular with the Product APIs and allow customers to:

- View products.
- View product cards.
- Search products.
- Filter by category.
- Navigate between product pages.
- View individual product details.

## 3.2 Angular Structure

Important files:

```text
src/app/
│
├── core/
│   └── services/
│       ├── category.service.ts
│       └── product.service.ts
│
├── models/
│   ├── category.ts
│   └── product.ts
│
├── features/
│   └── products/
│       ├── product-list/
│       └── product-details/
│
└── shared/
    └── components/
        └── product-card/
```

### Product Model

File: `models/product.ts`

The Product interface represents one product.

The ProductListResponse interface represents the paginated API response.

These interfaces provide TypeScript type information and help us work with structured API data.

---

## 3.3 Product Service

File: `core/services/product.service.ts`

The Product Service communicates with the backend.

It contains:

- `getProducts()`
- `getProductById()`

It uses Angular's `HttpClient`.

Example:

```typescript
return this.http.get<ProductListResponse>(
  this.apiUrl,
  { params }
);
```

The method returns an Observable.

An Observable represents an asynchronous stream of data. The component subscribes to it to receive the API response.

### Why use a service?

We keep HTTP communication separate from the component.

This improves code organization and avoids writing the same API logic in multiple components.

---

## 3.4 Product List Component

File: `features/products/product-list/product-list.ts`

The Product List component displays products and handles:

- Loading products.
- Loading categories.
- Search.
- Category filtering.
- Pagination.
- Navigation to product details.

### `ngOnInit()`

This lifecycle hook runs when Angular initializes the component.

We call:

```typescript
ngOnInit(): void {
  this.loadCategories();
  this.loadProducts();
}
```

Therefore, categories and products are requested when the page opens.

### Search Flow

1. User enters a product name.
2. The search button is clicked or Enter is pressed.
3. `searchProducts()` resets the page number to 1.
4. It calls `loadProducts()`.
5. Product Service sends the search term to the backend.
6. Backend filters the products.
7. Angular receives the response.
8. The product list updates.

### Category Filter Flow

1. User selects a category.
2. `ngModelChange` calls `filterByCategory()`.
3. Page number resets to 1.
4. Product Service sends the selected category ID.
5. Backend filters products.
6. Angular displays the filtered result.

### Pagination Flow

The Previous and Next buttons change the page number and call `loadProducts()` again.

The backend returns only the requested page.

---

## 3.5 Product Card Component – Input and Output

File: `shared/components/product-card/product-card.ts`

This component demonstrates communication between a parent component and a child component.

### `@Input()`

```typescript
@Input() product!: Product;
```

The Product List is the parent component.

The Product Card is the child component.

The parent sends one product object to the child.

In the HTML:

```html
<app-product-card
  [product]="product">
</app-product-card>
```

### `@Output()` and EventEmitter

```typescript
@Output() viewDetails = new EventEmitter<number>();
```

The child component sends the selected product ID back to its parent.

When the customer clicks View Details:

```typescript
onViewDetails(): void {
  this.viewDetails.emit(this.product.productId);
}
```

The parent listens for this event:

```html
<app-product-card
  [product]="product"
  (viewDetails)="viewProductDetails($event)">
</app-product-card>
```

The `$event` contains the emitted product ID.

The parent then navigates to the details page.

This demonstrates parent-to-child and child-to-parent communication.

---

## 3.6 Product Details Component

File: `features/products/product-details/product-details.ts`

The details page receives the product ID through the route.

Example route:

`/products/1`

We retrieve the ID using:

```typescript
const id = Number(
  this.route.snapshot.paramMap.get('id')
);
```

`ActivatedRoute` provides access to route information.

The component then calls:

```typescript
this.productService.getProductById(id)
```

The backend returns the product details, and Angular displays them.

If the product does not exist, an error message is displayed.

---

## 3.7 Angular Concepts Used

| Concept | Purpose |
|---|---|
| Interfaces | Define product data structures |
| Services | Separate API communication |
| Dependency Injection | Inject services into components |
| HttpClient | Send HTTP requests |
| Observable | Handle asynchronous responses |
| `subscribe()` | Receive API results |
| `ngOnInit()` | Initialize component data |
| `@Input()` | Pass data to child component |
| `@Output()` | Send events to parent |
| EventEmitter | Emit selected product ID |
| `ngModel` | Two-way data binding |
| `@if` | Conditional display |
| `@for` | Loop through products/categories |
| CurrencyPipe | Format prices |
| ActivatedRoute | Read route parameters |
| Router | Navigate between pages |

---

# 4. Backend Commit 4 – Registration, Login and JWT Authentication

## 4.1 Objective

The objective was to implement customer registration and login.

We introduced JWT authentication so that the backend can identify users through tokens.

The existing ApplicationUser model and ApplicationUsers table were reused. No migration was required.

## 4.2 Authentication Architecture

The flow follows:

**AuthController → AuthService → AuthRepository → ApplicationDbContext → SQL Server**

The service also uses:

- `IPasswordHasher<ApplicationUser>`
- JWT token generation
- Configuration values from `appsettings.json` and User Secrets

### DTOs Created

1. `RegisterDto`
2. `LoginDto`
3. `LoginResponseDto`

These DTOs control the request and response data.

---

## 4.3 Customer Registration Flow

Endpoint:

`POST /api/auth/register`

Example request:

```json
{
  "fullName": "Test Customer",
  "email": "customer@gmail.com",
  "password": "Customer@123"
}
```

Execution flow:

1. The client submits registration information.
2. AuthController receives the request.
3. The controller calls `AuthService.RegisterAsync()`.
4. The service trims the email and converts it to lowercase.
5. AuthRepository checks whether the email already exists.
6. If it exists, registration is rejected.
7. Otherwise, a new ApplicationUser object is created.
8. The service assigns the Customer role.
9. The password is hashed.
10. The repository saves the user.
11. The controller returns `201 Created`.

### Why do we hash passwords?

We should never store plain-text passwords in a database.

We use ASP.NET Core's `PasswordHasher<ApplicationUser>`.

The registration logic uses:

```csharp
_passwordHasher.HashPassword(user, dto.Password);
```

The generated hash is stored in `PasswordHash`.

During login, we verify the entered password against the stored hash.

We do not decrypt or retrieve the original password.

---

## 4.4 Login Flow

Endpoint:

`POST /api/auth/login`

Example request:

```json
{
  "email": "customer@gmail.com",
  "password": "Customer@123"
}
```

Execution flow:

1. AuthController receives the login request.
2. AuthService searches for the user by email.
3. If the user does not exist, login fails.
4. If the account is inactive, login fails.
5. PasswordHasher verifies the entered password.
6. If verification fails, the API returns 401 Unauthorized.
7. If verification succeeds, the service generates a JWT.
8. The controller returns the token and success message.

The same generic error message is used for an unknown email and an incorrect password, rather than revealing which part was wrong.

---

## 4.5 JWT Authentication

JWT stands for JSON Web Token.

It is a digitally signed token that carries information about the authenticated user.

The token contains claims such as:

- User ID
- Full Name
- Email
- Role
- Token ID
- Expiration time

### Claims

Claims are pieces of information about the authenticated user.

We create claims using:

```csharp
new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
new Claim(ClaimTypes.Name, user.FullName)
new Claim(ClaimTypes.Email, user.Email)
new Claim(ClaimTypes.Role, user.Role)
```

The role claim will be used for role-based authorization in the next backend commit.

### JWT Configuration

We configure:

- Issuer
- Audience
- ExpiryMinutes
- Signing Key

The signing key is stored in User Secrets for local development rather than committed to GitHub.

### Token Generation

The service creates a symmetric signing key and uses HMAC SHA-256.

The JWT is generated using:

```csharp
new JwtSecurityToken(...)
```

It is converted into a string using:

```csharp
new JwtSecurityTokenHandler().WriteToken(token);
```

The API returns this token to the client.

### Token Validation

In `Program.cs`, JWT Bearer authentication is configured to validate:

- Issuer
- Audience
- Expiration
- Signing key

`AddAuthentication()` registers the authentication scheme.

`AddJwtBearer()` configures how incoming bearer tokens are validated.

The middleware order is important:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Authentication identifies the user. Authorization checks whether that user has permission to access a resource.

At this stage, the authentication infrastructure is configured, but the Product and Category controllers are not yet protected with `[Authorize]`.

---

## 4.6 HTTP Status Codes Used

| Status | Meaning | Example |
|---|---|---|
| 201 | Created | Registration successful |
| 200 | OK | Login successful |
| 401 | Unauthorized | Invalid login credentials |
| 409 | Conflict | Duplicate email |
| 400 | Bad Request | Invalid request data |

---

# 5. Frontend Commit 4 – Angular Registration, Login and Auth Service

## 5.1 Objective

The objective was to connect Angular with the backend authentication APIs.

We implemented:

- Registration form.
- Login form.
- Auth Service.
- JWT token storage.
- Form validation.
- Success and error messages.
- Navigation after successful operations.

## 5.2 Authentication Models

File: `models/auth.ts`

We created interfaces:

- `RegisterRequest`
- `LoginRequest`
- `LoginResponse`
- `AuthMessageResponse`

These represent the request and response structures used by the backend APIs.

## 5.3 Auth Service

File: `core/services/auth.service.ts`

The Auth Service contains:

| Method | Purpose |
|---|---|
| `register()` | Sends registration details |
| `login()` | Sends login details |
| `getToken()` | Reads stored JWT |
| `isLoggedIn()` | Checks whether a token exists |
| `logout()` | Removes stored JWT |

### Token Storage

After successful login:

```typescript
localStorage.setItem(this.tokenKey, response.token);
```

We use browser `localStorage` to retain the token across page refreshes.

The token key is:

`ecommerceToken`

### Why use `tap()`?

The login request returns an Observable.

We use RxJS `tap()` to store the token when the response is successful, without changing the response data.

If the HTTP request fails, the token is not stored.

---

## 5.4 Login Component

File: `features/auth/login/login.ts`

The component contains:

- Login form data.
- Loading state.
- Error message.
- Success message.
- Login submission method.

### Login Execution Flow

1. Customer enters email and password.
2. Angular validates the form.
3. The customer submits the form.
4. `onLogin()` is executed.
5. The component calls `AuthService.login()`.
6. The service sends an HTTP POST request.
7. The backend verifies credentials and generates JWT.
8. Angular receives the response.
9. The Auth Service stores the token using `tap()`.
10. The component navigates to `/products`.

If the backend returns an error, Angular displays the error message.

---

## 5.5 Register Component

File: `features/auth/register/register.ts`

The registration component collects:

- Full Name
- Email
- Password

Execution flow:

1. Customer enters registration information.
2. Angular validates the form.
3. The component calls `AuthService.register()`.
4. The service sends a POST request.
5. Backend validates the request and checks for duplicate email.
6. If registration succeeds, Angular navigates to `/login`.
7. If registration fails, an error message is displayed.

Registration does not automatically log the user in. The customer must log in separately.

---

## 5.6 Template-driven Forms

We use Angular's `FormsModule`.

Important features:

### `ngModel`

Used for two-way data binding.

Example:

```html
<input
  name="email"
  [(ngModel)]="loginData.email"
  required
  email>
```

When the customer types, Angular updates `loginData.email`.

When the model changes, the input reflects the new value.

### `ngForm`

Example:

```html
<form #loginForm="ngForm">
```

This gives access to the form's validation state.

### Validation

We use:

- `required`
- `email`
- `minlength`
- `maxlength`

Example:

```html
[disabled]="loginForm.invalid || isLoading"
```

The submit button is disabled if the form is invalid or a request is already being processed.

### `ngSubmit`

The form calls `onLogin()` or `onRegister()` when submitted.

---

# 6. Complete End-to-End Execution Flow

## 6.1 Product Listing

1. Customer opens `/products`.
2. Angular Router loads ProductList.
3. `ngOnInit()` runs.
4. ProductList calls ProductService.
5. ProductService sends GET `/api/products`.
6. ProductsController receives the request.
7. ProductService processes the business logic.
8. ProductRepository queries SQL Server.
9. Product data is returned as a paginated response.
10. Angular receives the response.
11. `@for` displays the products.
12. Each product is passed to ProductCard through `@Input()`.

## 6.2 Product Details

1. Customer clicks View Details.
2. ProductCard emits the product ID through `EventEmitter`.
3. ProductList receives the ID.
4. Angular Router navigates to `/products/{id}`.
5. ProductDetails reads the ID using ActivatedRoute.
6. ProductService sends GET `/api/products/{id}`.
7. Backend retrieves the product.
8. Angular displays its details.

## 6.3 Customer Registration

1. Customer opens `/register`.
2. Customer enters details.
3. Angular validates the form.
4. AuthService sends POST `/api/auth/register`.
5. AuthController receives the request.
6. AuthService checks for duplicate email.
7. PasswordHasher hashes the password.
8. AuthRepository saves the customer.
9. Backend returns 201.
10. Angular navigates to `/login`.

## 6.4 Customer Login

1. Customer opens `/login`.
2. Customer enters email and password.
3. Angular validates the form.
4. AuthService sends POST `/api/auth/login`.
5. AuthController calls AuthService.
6. AuthService retrieves the user.
7. PasswordHasher verifies the password.
8. JWT is generated.
9. Backend returns the token.
10. Angular AuthService stores the token in localStorage.
11. Angular navigates to `/products`.

---

# 7. Important Teacher Review Questions and Answers

### Q1. Why do we use Controller-Service-Repository architecture?

It separates responsibilities.

- Controller handles HTTP requests and responses.
- Service contains business logic.
- Repository handles database operations.
- DbContext communicates with the database.

This makes the application easier to maintain and test.

### Q2. What is the difference between an Entity and a DTO?

An Entity represents the database structure.

A DTO represents the data transferred between the client and server.

We use DTOs to control API input and output.

### Q3. Why do we use `AsNoTracking()`?

It tells Entity Framework Core not to track retrieved entities.

This is useful for read-only operations because it avoids unnecessary change-tracking overhead.

### Q4. Why do we use `Include()`?

`Include()` loads related entity information.

For products, we use it to retrieve Category information along with the Product.

### Q5. What is pagination?

Pagination divides a large result set into smaller pages.

We use `Skip()` and `Take()` to retrieve only the requested page.

### Q6. Why do we use soft deletion?

Products may be referenced by cart and order records.

Soft deletion preserves the record while marking it inactive.

### Q7. What is the difference between `@Input()` and `@Output()`?

`@Input()` sends data from parent to child.

`@Output()` sends an event from child to parent.

### Q8. What is EventEmitter?

EventEmitter allows a child component to emit an event that its parent can listen to.

We use it to send the selected product ID from ProductCard to ProductList.

### Q9. What is an Observable?

An Observable represents an asynchronous stream of data.

Angular HttpClient returns Observables, and components subscribe to receive the results.

### Q10. What is Dependency Injection?

Dependency Injection is a design technique where required objects are provided to a class instead of the class creating them itself.

Angular injects services into components. ASP.NET Core injects repositories and services into backend classes.

### Q11. What is JWT?

JWT stands for JSON Web Token.

It is a signed token containing claims about an authenticated user. The backend validates the token when it is presented in an authenticated request.

### Q12. Why do we hash passwords?

Hashing protects passwords from being stored as plain text.

We use ASP.NET Core PasswordHasher to hash and verify passwords.

### Q13. What is the difference between hashing and encryption?

Hashing is designed to be one-way. We verify a password against its stored hash.

Encryption is designed to be reversible when the correct decryption key is available.

### Q14. What are JWT claims?

Claims are information about the user stored inside the token.

In our project, claims include User ID, Full Name, Email and Role.

### Q15. What is the difference between authentication and authorization?

Authentication identifies who the user is.

Authorization checks what the authenticated user is allowed to do.

### Q16. Why do we use User Secrets?

User Secrets keep sensitive development configuration, such as the JWT signing key, outside committed application configuration files.

### Q17. Why is the Admin role not accepted during registration?

If users could choose their own role, they could register themselves as Admin.

Therefore, the backend assigns the Customer role directly. Admin creation will be handled through controlled seeding.

### Q18. Why is the JWT stored in localStorage?

It allows the frontend to retrieve the token after a page refresh.

However, localStorage is accessible to JavaScript, so an XSS vulnerability can expose the token. Production applications should carefully consider token storage and browser security protections.

### Q19. Why does registration navigate to Login instead of directly logging in?

Our backend registration endpoint only creates the customer account and returns a success message. It does not generate a JWT. Therefore, the customer must log in separately.

### Q20. Does storing a JWT automatically protect the APIs?

No. Storing the token alone does not secure API endpoints.

The frontend must attach the token to protected requests, and the backend must validate it and apply authorization rules. These steps are handled in the later authorization and integration work.

---

# 8. Important Files to Remember

| Module | Important Files |
|---|---|
| Product Backend | `ProductsController.cs` |
| Product Business Logic | `ProductService.cs` |
| Product Database Access | `ProductRepository.cs` |
| Product Request/Response | Product DTO files |
| Product Frontend API | `product.service.ts` |
| Product List | `product-list.ts` |
| Product Card | `product-card.ts` |
| Product Details | `product-details.ts` |
| Authentication Backend | `AuthController.cs` |
| Authentication Logic | `AuthService.cs` |
| Authentication Database Access | `AuthRepository.cs` |
| Authentication Frontend | `auth.service.ts` |
| Login | `login.ts` |
| Registration | `register.ts` |
| JWT Configuration | `Program.cs` and User Secrets |

---

# 9. Current Project Progress

| Commit | Module | Status |
|---|---|---|
| Backend 1 | Database foundation and relationships | Completed |
| Frontend 1 | Angular setup and layouts | Completed |
| Backend 2 | Category CRUD | Completed |
| Frontend 2 | Category list and management | Completed |
| Backend 3 | Product CRUD, search and pagination | Completed |
| Frontend 3 | Product listing, details and filtering | Completed |
| Backend 4 | Registration, login and JWT | Completed |
| Frontend 4 | Login, registration and token storage | Completed |

## Next Commit

**Backend Commit 5 – Admin Authorization and Stock Management**

Planned work:

- Create the Admin account through controlled seeding.
- Implement role-based authorization.
- Protect Category and Product management APIs.
- Allow only Admin users to perform management operations.
- Implement stock management.

After that, Frontend Commit 5 will connect the Admin product and category management screens to the protected APIs.

---

# 10. How to Explain These Four Commits to Your Teacher

You can explain the work in this order:

**First – Product Backend:**

"I implemented the Product module using Controller-Service-Repository architecture. The API supports CRUD operations, search by product name, category filtering and pagination. I used DTOs for request validation and response formatting. I also implemented soft deletion to preserve product records referenced by cart and order tables."

**Second – Product Frontend:**

"I connected the Angular frontend to the Product APIs using HttpClient and a Product Service. I created separate Product List, Product Card and Product Details components. I used Input and Output decorators for parent-child communication, ActivatedRoute for product IDs, and pagination and filtering to display products."

**Third – Authentication Backend:**

"I implemented customer registration and login. The backend checks duplicate emails, hashes passwords using ASP.NET Core PasswordHasher, verifies passwords during login and generates JWT tokens. The token includes claims such as user ID, email and role. JWT validation is configured through authentication middleware."

**Fourth – Authentication Frontend:**

"I created Login and Registration components using Angular template-driven forms. I used ngModel for two-way data binding and built-in form validation. The Auth Service communicates with the backend. After successful login, RxJS tap stores the JWT in localStorage, and the application navigates to the Products page."

**Overall:**

"These commits establish the product browsing and customer authentication foundation. The next phase will implement Admin authorization, stock management, cart and order processing."

---

**Revision tip:** Practise explaining the four end-to-end flows without reading the document: Product Creation, Product Listing, Customer Registration and Customer Login. If you can explain which component, service, controller, repository and database operation runs at each stage, you will be well prepared for a technical review.