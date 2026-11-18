# E-Commerce Full Stack Project
## Version 1 – Commit 1 & 2 Review Notes

**Purpose:** To understand the implementation, architecture, execution flow, features, and logic completed in the first two backend and frontend commits.

**Technology Stack**
- Backend: C#, ASP.NET Core Web API, .NET 10
- Database: SQL Server Express
- ORM: Entity Framework Core
- Frontend: Angular, TypeScript, HTML, CSS
- API communication: Angular HttpClient
- Frontend styling: Custom CSS
- Architecture: Controller → Service → Repository → DbContext

---

# 1. Overall Project Understanding

The project is an E-Commerce Shopping Application.

It has two main types of users:

**Admin**
- Manages product categories.
- Manages products and stock.
- Manages customer orders.

**Customer**
- Registers and logs in.
- Views product categories and products.
- Searches and filters products.
- Adds products to a cart.
- Places orders.

The application is divided into two independent applications:

1. Backend – responsible for APIs, business logic, validation, and database operations.
2. Frontend – responsible for the user interface and communicating with backend APIs.

The frontend does not directly access the database. It sends HTTP requests to the backend, and the backend processes those requests.

---

# 2. Backend Commit 1 – Database Foundation

## Objective

To create the initial backend project structure, define the database models, configure Entity Framework Core, establish table relationships, and create the database using migrations.

## 2.1 Technologies and concepts used

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server Express
- DbContext
- DbSet
- Entity relationships
- Fluent API
- EF Core migrations

## 2.2 Project architecture

The backend follows this architecture:

**Controller → Service → Repository → DbContext → SQL Server**

Purpose of each layer:

| Layer | Responsibility |
|---|---|
| Controller | Receives HTTP requests and returns HTTP responses. |
| Service | Contains application/business logic. |
| Repository | Handles database-related operations. |
| DbContext | Connects entity models with database tables and manages database operations. |
| SQL Server | Stores application data. |

The architecture separates responsibilities instead of putting all the code inside controllers.

## 2.3 Models created

Seven entity models were created.

| Model | Purpose |
|---|---|
| ApplicationUser | Stores user information, email, password hash, role, and account status. |
| Category | Stores product category information. |
| Product | Stores product details, price, stock, and category reference. |
| Cart | Represents a customer's shopping cart. |
| CartItem | Stores products and quantities added to a cart. |
| Order | Stores customer order details and order status. |
| OrderItem | Stores individual products included in an order. |

**Important:** Models represent the structure of the data. They are not responsible for implementing the application's business logic.

## 2.4 Database tables and relationships

Seven database tables were created:

1. ApplicationUsers
2. Categories
3. Products
4. Carts
5. CartItems
6. Orders
7. OrderItems

Relationships:

| Relationship | Type |
|---|---|
| ApplicationUser – Cart | One-to-One |
| ApplicationUser – Order | One-to-Many |
| Category – Product | One-to-Many |
| Cart – CartItem | One-to-Many |
| Product – CartItem | One-to-Many |
| Order – OrderItem | One-to-Many |
| Product – OrderItem | One-to-Many |

### Example relationship explanation

A category can contain multiple products, but each product belongs to one category.

A cart can contain multiple cart items, but each cart item belongs to one cart.

An order can contain multiple order items, but each order item belongs to one order.

## 2.5 DbContext implementation

`ApplicationDbContext` is the main Entity Framework Core class that manages the application's database interaction.

It contains DbSet properties for the seven models.

Example:

`public DbSet<Product> Products { get; set; }`

This represents the Product entity collection that Entity Framework maps to the Products table.

### Fluent API configurations

The `OnModelCreating()` method was used to configure database rules and relationships.

Important configurations include:

- Unique index on ApplicationUser.Email.
- Maximum lengths for selected user fields.
- Decimal precision for product prices and order amounts.
- One-to-one and one-to-many relationships.
- Delete behaviors for related records.

### Why configure delete behavior?

For example, a category should not be deleted when products are associated with it.

This helps protect related database records from accidental deletion.

## 2.6 Database connection

The SQL Server connection string was configured in `appsettings.json`.

The backend uses SQL Server Express with the database name `EcommerceDB`.

`Program.cs` registers `ApplicationDbContext` using `AddDbContext()` and `UseSqlServer()`.

## 2.7 EF Core migrations

Commands used:

```powershell
Add-Migration InitialCreate
Update-Database
```

**Add-Migration:** Generates migration files based on the entity models and database configuration.

**Update-Database:** Applies the migration to SQL Server and creates the required database tables.

### Execution flow

1. Create entity model classes.
2. Configure relationships and constraints in DbContext.
3. Register DbContext in Program.cs.
4. Configure the SQL Server connection string.
5. Generate a migration.
6. Apply the migration.
7. Verify the database and tables in SQL Server.

**Commit 1 outcome:** The initial database foundation was successfully created and verified.

---

# 3. Frontend Commit 1 – Angular Application Setup

## Objective

To create the Angular application, establish the initial component structure, configure routing, and build the basic customer and admin layouts.

## 3.1 Technologies and concepts used

- Angular
- TypeScript
- Standalone components
- HTML
- CSS
- Angular Router
- Input decorator
- RouterLink
- RouterOutlet
- Angular control flow using `@if`

No Bootstrap or other CSS framework was used. Styling was implemented using custom CSS.

## 3.2 Angular project structure

The initial frontend structure includes:

- Shared components – Navbar and Footer.
- Customer layout – Customer-facing application layout.
- Admin layout – Admin-facing application layout.
- Application routes – Defines navigation between pages.
- Application configuration – Registers application-level providers.

## 3.3 Standalone components

The application uses Angular standalone components.

A standalone component can directly declare the Angular features and other components it needs through its `imports` array.

This avoids requiring an NgModule for every component.

## 3.4 Navbar component

The Navbar is a reusable component.

It accepts an `isAdmin` input property.

Example:

```typescript
@Input() isAdmin = false;
```

This allows the parent layout to tell the Navbar whether it is being displayed in the Admin or Customer area.

Example:

```html
<app-navbar [isAdmin]="true"></app-navbar>
```

The Navbar uses Angular's `@if` to display different navigation links based on the value.

## 3.5 Angular routing

Angular Router is used to navigate between application views without manually reloading the entire application.

Initial routes:

- `/` – Customer layout.
- `/admin` – Admin layout.

### RouterOutlet

`<router-outlet>` is the location where Angular displays the component matching the current route.

### RouterLink

`routerLink` is used to navigate to another route when the user clicks a link.

### Execution flow

1. The browser opens the Angular application.
2. Angular loads the root component.
3. The root component contains `router-outlet`.
4. Angular checks the current URL.
5. The Router loads the matching layout.
6. The layout displays the Navbar, page content, and Footer.

**Commit 1 outcome:** The initial Angular application structure, reusable layouts, navigation, and custom styling were created.

---

# 4. Backend Commit 2 – Category CRUD APIs

## Objective

To implement category management functionality using the Controller-Service-Repository architecture.

CRUD means:

- Create
- Read
- Update
- Delete

## 4.1 Features implemented

| Operation | HTTP Method | Endpoint |
|---|---|---|
| Get all categories | GET | `/api/categories` |
| Create category | POST | `/api/categories` |
| Update category | PUT | `/api/categories/{id}` |
| Delete category | DELETE | `/api/categories/{id}` |

## 4.2 DTOs

DTO means Data Transfer Object.

DTOs are used to transfer data between the client and backend without directly exposing the database entity.

Three DTOs were created:

**CreateCategoryDto**
- CategoryName
- Description

**UpdateCategoryDto**
- CategoryName
- Description

**CategoryResponseDto**
- CategoryId
- CategoryName
- Description
- IsActive
- CreatedAt

### Why use DTOs?

The frontend does not need to send every property present in the database model.

For example, when creating a category, the frontend sends only the category name and description.

The backend sets properties such as `IsActive` and `CreatedAt`.

The response DTO contains the information needed by the frontend.

## 4.3 Validation

Data annotations were used in the request DTOs.

Examples:

- `[Required]` – CategoryName must be provided.
- `[StringLength(100)]` – CategoryName cannot exceed 100 characters.
- `[StringLength(500)]` – Description cannot exceed 500 characters.

`[ApiController]` enables automatic model validation responses for invalid request data.

## 4.4 Repository layer

`ICategoryRepository` defines the database operations required for category management.

`CategoryRepository` implements those operations using `ApplicationDbContext`.

Operations include:

- Get all categories.
- Get category by ID.
- Add a category.
- Update a category.
- Delete a category.
- Check whether a category has associated products.
- Save database changes.

### AsNoTracking()

`AsNoTracking()` is used when retrieving categories for read-only operations.

It tells Entity Framework Core not to track the returned entities for changes.

This is useful when the retrieved data will only be displayed.

### AnyAsync()

`AnyAsync()` checks whether at least one matching record exists.

It is used to check whether products are associated with a category before allowing deletion.

## 4.5 Service layer

`ICategoryService` defines the category-related application operations.

`CategoryService` implements the application logic.

Its responsibilities include:

- Creating categories.
- Retrieving categories.
- Updating categories.
- Deleting categories.
- Mapping entity properties to response DTO properties.
- Checking whether a category can be deleted.

### Create logic

When a category is created:

1. Receive the CreateCategoryDto.
2. Trim unnecessary spaces from the category name.
3. Create a Category entity.
4. Set `IsActive` to true.
5. Set `CreatedAt` to the current UTC date and time.
6. Save the category through the repository.
7. Return the response DTO.

### Update logic

1. Receive the category ID and UpdateCategoryDto.
2. Retrieve the category.
3. If the category does not exist, return a not-found response.
4. Update the category name and description.
5. Save the changes.
6. Return the updated category response.

### Delete logic

1. Retrieve the category.
2. Check whether the category exists.
3. Check whether products are associated with it.
4. If products exist, prevent deletion.
5. Otherwise, delete the category and save changes.

**Important business rule:** A category containing products cannot be deleted.

## 4.6 Controller layer

`CategoriesController` exposes the category APIs.

It receives HTTP requests and calls the corresponding service methods.

The controller does not directly perform database operations.

HTTP responses include:

- `200 OK` – Request completed successfully.
- `201 Created` – Category created successfully.
- `204 No Content` – Category deleted successfully.
- `404 Not Found` – Requested category does not exist.
- `409 Conflict` – Category cannot be deleted because products are associated with it.

## 4.7 Dependency Injection

The repository and service were registered in `Program.cs`:

```csharp
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
```

`AddScoped()` creates one instance per HTTP request scope.

Dependency Injection allows the controller to receive the required service instead of creating service objects manually.

### Complete backend execution flow

**Example: Admin creates a category**

1. Admin sends a POST request to `/api/categories`.
2. `CategoriesController` receives the request.
3. The request data is validated.
4. Controller calls `ICategoryService`.
5. `CategoryService` creates the Category entity and applies the required logic.
6. Service calls `ICategoryRepository`.
7. Repository uses `ApplicationDbContext`.
8. Entity Framework Core saves the data in SQL Server.
9. The response is returned to the controller.
10. Controller returns `201 Created`.

---

# 5. Frontend Commit 2 – Category Listing and Management

## Objective

To connect the Angular application to the Category APIs and display category information in the customer and admin sections.

## 5.1 Features

**Customer**
- View the available categories.

**Admin**
- View categories.
- Add a category.
- Edit a category.
- Delete a category.

## 5.2 Angular model interfaces

Two TypeScript interfaces were created:

**Category**

Represents the category data received from the backend.

Properties include:
- categoryId
- categoryName
- description
- isActive
- createdAt

**CategoryRequest**

Represents the data sent to the backend when creating or updating a category.

Properties:
- categoryName
- description

### Why use interfaces?

Interfaces define the expected structure of objects in TypeScript.

They help ensure that the frontend works with the expected data properties.

## 5.3 Category service

`CategoryService` is responsible for communicating with the backend Category APIs.

It uses:

- `HttpClient`
- `Observable`
- GET
- POST
- PUT
- DELETE

Methods:

| Method | Purpose |
|---|---|
| getCategories() | Retrieves all categories. |
| createCategory() | Sends a category creation request. |
| updateCategory() | Sends an update request. |
| deleteCategory() | Sends a delete request. |

### Why use a service?

The HTTP communication logic is kept in one reusable service instead of writing API calls separately in each component.

Both the customer category list and admin category management component can use the same service.

## 5.4 HttpClient and Observable

Angular `HttpClient` sends HTTP requests to the backend.

The methods return Observables.

An Observable represents a stream of asynchronous data.

For example:

```typescript
this.categoryService.getCategories().subscribe({
  next: (response) => {
    this.categories = response;
  },
  error: (error) => {
    this.errorMessage = 'Unable to load categories.';
  }
});
```

**subscribe()** is used to receive the asynchronous response.

- `next` runs when the request succeeds.
- `error` runs when the request fails.

The component updates its data based on the response.

## 5.5 Lifecycle hook – ngOnInit()

The CategoryList component implements `OnInit`.

`ngOnInit()` is called by Angular after the component has been initialized.

It calls `loadCategories()` to retrieve category data when the component is loaded.

### Customer category listing flow

1. Customer opens `/categories`.
2. Angular Router loads CategoryList.
3. `ngOnInit()` executes.
4. `loadCategories()` calls CategoryService.
5. CategoryService sends a GET request.
6. Backend retrieves categories from SQL Server.
7. Backend returns the category response.
8. Angular receives the response through `subscribe()`.
9. The component stores the result in `categories`.
10. Angular displays the categories.

## 5.6 Admin category management

The CategoryManagement component handles category creation, editing, listing, and deletion.

It uses Angular FormsModule and `ngModel` for two-way data binding.

Example:

```html
<input [(ngModel)]="categoryName">
```

When the input value changes, the component property is updated. When the property changes, the input displays the updated value.

### Important component properties

- `categories` – Stores the category list.
- `categoryName` – Stores the category name entered in the form.
- `description` – Stores the description.
- `selectedCategoryId` – Identifies whether the form is editing an existing category.
- `isLoading` – Tracks the category-list loading state.
- `isSaving` – Tracks the save operation state.
- `errorMessage` – Stores error messages.
- `successMessage` – Stores success messages.

### Add category flow

1. Admin enters the category name and description.
2. Admin clicks Save.
3. Component calls `createCategory()`.
4. CategoryService sends a POST request.
5. Backend validates and saves the category.
6. Backend returns the response.
7. Component displays the result and refreshes the category list.

### Edit category flow

1. Admin clicks Edit.
2. The selected category details are loaded into the form.
3. `selectedCategoryId` stores the category ID.
4. Admin modifies the values.
5. Component calls `updateCategory()`.
6. CategoryService sends a PUT request with the ID.
7. Backend updates the category.
8. Angular refreshes the displayed category list.

### Delete category flow

1. Admin clicks Delete.
2. The component asks for confirmation.
3. If confirmed, it calls `deleteCategory()`.
4. CategoryService sends a DELETE request.
5. Backend checks whether products are associated with the category.
6. If products exist, backend returns a conflict response.
7. Otherwise, the category is deleted.
8. Angular refreshes the category list.

## 5.7 Angular features used in templates

**@if**

Used for conditional display.

Examples:
- Show loading message.
- Show error message.
- Show category list only when data exists.
- Display different form states.

**@for**

Used to iterate through the category array and display multiple categories.

**RouterLink**

Used to navigate to category pages.

**FormsModule and ngModel**

Used to capture category form values.

## 5.8 Development proxy

A proxy configuration was introduced for local Angular-to-ASP.NET API communication.

Angular uses `/api` as the API base URL, and the proxy forwards requests to the backend HTTPS address.

This avoids requiring a separate CORS configuration for this local development setup.

The backend must be running, and the proxy target must use the correct backend port.

---

# 6. Complete Category Feature – End-to-End Flow

The category feature connects both applications.

```text
ADMIN
  |
  v
Angular Admin Category Management
  |
  v
Category Component
  |
  v
Category Service
  |
  v
HttpClient
  |
  v
Backend CategoriesController
  |
  v
CategoryService
  |
  v
CategoryRepository
  |
  v
ApplicationDbContext
  |
  v
SQL Server
```

The response follows the reverse direction:

```text
SQL Server
  |
  v
ApplicationDbContext
  |
  v
Repository
  |
  v
Service
  |
  v
Controller
  |
  v
HTTP Response
  |
  v
Angular HttpClient
  |
  v
Observable subscribe()
  |
  v
Component updates its data
  |
  v
Angular updates the UI
```

The same CategoryService is reused by both the customer category listing and admin category management components.

---

# 7. Important Concepts to Revise

| Concept | What to remember |
|---|---|
| Web API | Allows the frontend and backend to communicate using HTTP. |
| Entity | A C# class representing database data. |
| DbContext | Manages database interaction through EF Core. |
| DbSet | Represents an entity collection mapped to a database table. |
| Migration | Records database schema changes. |
| DTO | Defines the data transferred between client and backend. |
| Repository | Handles database operations. |
| Service | Contains application/business logic. |
| Controller | Receives HTTP requests and returns responses. |
| Dependency Injection | Provides required objects to classes. |
| AddScoped | Registers a service with a lifetime scoped to an HTTP request. |
| HttpClient | Sends HTTP requests from Angular. |
| Observable | Represents asynchronous data that can be subscribed to. |
| subscribe | Receives Observable success or error notifications. |
| ngOnInit | Angular lifecycle hook called after component initialization. |
| ngModel | Provides two-way data binding. |
| @if | Conditional rendering in Angular templates. |
| @for | Iteration in Angular templates. |
| RouterLink | Navigates between Angular routes. |
| RouterOutlet | Displays the component matching the active route. |

---

# 8. Questions Your Teacher May Ask

## Backend Commit 1 – Database

**Q1. What is Entity Framework Core?**

EF Core is an Object Relational Mapper (ORM). It allows us to work with database records using C# classes and objects instead of writing every database operation manually in SQL.

**Q2. What is DbContext?**

DbContext is the main class responsible for managing the connection between entity classes and the database. It tracks entities and performs database operations.

**Q3. What is DbSet?**

DbSet represents a collection of entities that EF Core maps to a database table.

**Q4. What is a migration?**

A migration records changes to the database schema based on the entity models and configurations.

**Q5. What is the difference between a model and a DTO?**

A model represents the application's entity/data structure. A DTO defines the data transferred between the frontend and backend.

**Q6. Why do we use Fluent API?**

Fluent API allows us to configure database relationships, constraints, indexes, and column properties in the DbContext.

**Q7. What is a one-to-many relationship?**

One record in a parent table can be related to multiple records in another table. For example, one category can contain multiple products.

**Q8. Why is Email unique?**

To prevent multiple user accounts from being created with the same email address.

## Backend Commit 2 – Category APIs

**Q9. Explain the Controller-Service-Repository architecture.**

Controller handles HTTP requests. Service contains business logic. Repository handles database operations. DbContext communicates with the database.

**Q10. Why not write all the logic inside the controller?**

Separating responsibilities makes the application easier to maintain, test, and reuse.

**Q11. Why do we use DTOs?**

To control the data sent to and received from the API and avoid exposing the database entity directly.

**Q12. What is the purpose of AsNoTracking()?**

It prevents EF Core from tracking an entity retrieved for a read-only operation.

**Q13. What is AnyAsync()?**

It asynchronously checks whether at least one record satisfies a condition.

**Q14. Why should a category not be deleted if it contains products?**

Because deleting the category could affect products associated with it. The application prevents this deletion to preserve data consistency.

**Q15. What is Dependency Injection?**

Dependency Injection is a technique in which required objects are provided to a class instead of the class creating those objects manually.

**Q16. What is the difference between 404 and 409?**

404 means the requested resource was not found. 409 means the request conflicts with the current state of the resource.

## Frontend Commit 1 – Angular

**Q17. What is a standalone component?**

A standalone component can be used without declaring it inside an NgModule.

**Q18. What is the purpose of Input?**

Input allows a parent component to pass data to a child component.

**Q19. What is RouterOutlet?**

It is the placeholder where Angular displays the component matching the current route.

**Q20. What is RouterLink?**

It is used to navigate between Angular routes.

## Frontend Commit 2 – Angular and API Integration

**Q21. Why do we create Angular services?**

To reuse application logic and keep HTTP communication separate from UI components.

**Q22. What is HttpClient?**

HttpClient is Angular's service for sending HTTP requests to backend APIs.

**Q23. What is an Observable?**

An Observable represents asynchronous data that can be received over time.

**Q24. What is subscribe()?**

It is used to listen to an Observable and handle its emitted data, errors, and completion.

**Q25. What is ngOnInit()?**

It is a lifecycle hook that executes after Angular initializes a component. We use it to load categories when the category page opens.

**Q26. What is two-way data binding?**

It keeps the component property and the form input value synchronized. In Angular, `[(ngModel)]` is used for this.

**Q27. What is the difference between @if and @for?**

`@if` conditionally displays content. `@for` repeats content for each item in a collection.

**Q28. How does Angular communicate with ASP.NET Core?**

Angular uses HttpClient to send HTTP requests to backend endpoints. The backend processes the requests and returns HTTP responses, which Angular handles through Observables.

---

# 9. Current Commit Status

| Commit | Application | Work |
|---|---|---|
| Commit 1 | Backend | Database foundation, models, relationships, migration |
| Commit 1 | Frontend | Angular setup, layouts, navbar, routing |
| Commit 2 | Backend | Category CRUD APIs, DTOs, repository, service, controller |
| Commit 2 | Frontend | Category listing, admin category management, API integration |

**Verification status**

- Backend Commit 1: Confirmed successful.
- Frontend Commit 1: Confirmed successful.
- Backend Commit 2: Implementation provided; final verification pending confirmation.
- Frontend Commit 2: Implementation provided; final end-to-end verification pending confirmation.

---

# 10. How to Explain These Two Commits to Your Teacher

You can explain the work in this order:

1. First, I created the backend database foundation using Entity Framework Core. I created seven models, configured their relationships in ApplicationDbContext, and generated the database using migrations.
2. Next, I created the Angular application using standalone components. I created reusable Navbar and Footer components, separate Admin and Customer layouts, and configured Angular routing.
3. In Backend Commit 2, I implemented Category CRUD APIs using Controller-Service-Repository architecture. I used DTOs for request and response data, validation attributes for input validation, and business logic to prevent deleting categories that contain products.
4. In Frontend Commit 2, I connected Angular to the Category APIs using HttpClient and Observables. Customers can view categories, while Admins can add, edit, and delete categories.
5. Finally, I connected the frontend and backend so that category operations go through Angular components, services, backend controllers, service and repository layers, and SQL Server.

**Important points to understand before the review:**

- Be able to draw and explain the complete backend architecture.
- Know the purpose of all seven database tables.
- Explain the relationships between Category and Product, Cart and CartItem, and Order and OrderItem.
- Explain why DTOs are used.
- Explain why category deletion is restricted when products exist.
- Explain how Angular HttpClient communicates with the backend.
- Be able to explain the complete Add Category and Delete Category execution flows.
- Understand the difference between an Observable and its subscription.

**End of Version 1 – Covers Backend and Frontend Commits 1 and 2.**