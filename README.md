```markdown
# EduAssign - Assignment & Submission Management System

Welcome to EduAssign! This is a role-based Assignment & Submission Management System designed for schools and colleges. The system streamlines the academic workflow, allowing administrators to manage users, teachers to create and evaluate assignments, and students to seamlessly view and submit their work.

## Project Repositories
* **Frontend Repository:** [https://github.com/Maliha-Akter/eduassign](https://github.com/Maliha-Akter/eduassign)
* **Backend Repository:** [https://github.com/Maliha-Akter/eduassign-backend](https://github.com/Maliha-Akter/eduassign-backend)

The frontend is built with Next.js, React, TypeScript, and Tailwind CSS. The backend is developed using ASP.NET Core Web API, C#, and MongoDB.

---

## 1. Project Overview
EduAssign is a full-stack web application developed as part of the Assistant Software Engineer Recruitment Project. The application provides dedicated interfaces and functionalities for three distinct user roles: **Admin**, **Teacher**, and **Student**. 

### Main Workflow
* **Teacher:**
  * Creates an assignment (selecting the relevant class/course and subject).
  * Defines the title, description, deadline, and maximum marks.
  * Publishes the assignment or saves it as a draft.
  * Views student submissions, assigns marks, provides feedback, and updates submission statuses.
* **Student:**
  * Views assignments available for their specific class/course.
  * Checks assignment details and deadlines.
  * Submits their work and can update the submission before the deadline (if permitted).
  * Tracks submission status, grades, and teacher feedback.
* **Admin:**
  * Manages user accounts and roles.
  * Manages classes, courses, and subjects.
  * Assigns teachers to their respective subjects/classes.
  * Oversees platform activity by viewing all assignments and submissions.

---

## 2. Main Features
* **Authentication and Authorization:** Secure registration and login, role-based access control, protected routes (frontend), and protected API endpoints (backend) using Better Auth integrated with Next.js and ASP.NET Core.
* **Admin Features:** Comprehensive user management (including role updates and account blocking/unblocking), course/subject management, and teacher assignments.
* **Teacher Features:** Dashboard to view assigned classes, full CRUD capabilities for assignments, grading tools, and submission status tracking.
* **Student Features:** Personalized dashboard for assigned coursework, submission handling, and real-time visibility into grades and feedback.
* **API Features:** RESTful ASP.NET Core Web API with strict role-based authorization, MongoDB integration, robust request validation, centralized error handling, and OpenAPI/Swagger documentation.

---

## 3. Technology Stack
* **Frontend:** Next.js, React, TypeScript, Tailwind CSS, Better Auth, Lucide React, React Toastify, REST API integration.
* **Backend:** ASP.NET Core Web API, C#, MongoDB Driver, RESTful API, OpenAPI, Dependency Injection.
* **Database:** MongoDB.
* **Authentication:** Better Auth (session-based authentication).
* **Testing:** .NET testing framework for validating core business rules and workflows.
* **Development Tools:** Visual Studio Code, Git, GitHub, Postman.

---

## 4. Project Structure
The project is divided into two main repositories to cleanly separate the client and server environments.

### Frontend Repository Structure (`eduassign/`)
```text
eduassign/
├── src/
│   ├── app/
│   │   ├── dashboard/
│   │   ├── login/
│   │   ├── register/
│   │   └── ...
│   ├── components/
│   └── lib/
├── public/
├── package.json
├── .env.example
└── README.md

```

### Backend Repository Structure (`eduassign-backend/`)

```text
eduassign-backend/
├── EduAssign.API/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── Validators/
│   ├── Program.cs
│   ├── appsettings.json
│   └── EduAssign.API.csproj
├── EduAssign.Tests/
│   └── ...
└── README.md

```

---

## 5. Frontend Setup

### Prerequisites

* Node.js
* npm
* Git

### Installation and Running

1. Clone the frontend repository:
```bash
git clone [https://github.com/Maliha-Akter/eduassign.git](https://github.com/Maliha-Akter/eduassign.git)

```


2. Move into the project directory:
```bash
cd eduassign

```


3. Install the required dependencies:
```bash
npm install

```


4. Create a `.env.local` file based on the provided `.env.example` and configure your local environment variables.
5. Start the development server:
```bash
npm run dev

```


The frontend will be available at `http://localhost:3000`.

---

## 6. Backend Setup

### Prerequisites

* .NET SDK
* MongoDB
* Git

### Installation and Running

1. Clone the backend repository:
```bash
git clone [https://github.com/Maliha-Akter/eduassign-backend.git](https://github.com/Maliha-Akter/eduassign-backend.git)

```


2. Move into the backend directory:
```bash
cd eduassign-backend

```


3. Restore the .NET dependencies:
```bash
dotnet restore

```


4. Verify your MongoDB connection string (`mongodb://localhost:27017`) and database name (`eduassign`) in the backend environment configuration (`appsettings.json`).
5. Run the backend API:
```bash
dotnet run

```



---

## 7. Database Setup

EduAssign uses MongoDB for its flexible, document-based data model.

### Main Collections

* `user`
* `account`
* `Courses`
* `TeacherAssignments`
* `assignments`
* `submissions`

*Note: Better Auth authentication-related collections (sessions, verifications, etc.) are generated automatically.*

### Database Initialization & Seed Data

EduAssign uses MongoDB as its database. A database seed script is included in the project to provide sample data for local evaluation.

The seed script can be used to initialize the required collections and sample data for testing the Admin, Teacher, and Student workflows.

Before running the application, execute the seed script according to the instructions provided in the backend repository.

---

## 8. Database Collections and Relationships

| Collection | Purpose |
| --- | --- |
| **user** | Stores Admin, Teacher, and Student profile data. |
| **account** | Manages authentication and account credentials. |
| **Courses** | Contains the catalog of available courses/subjects. |
| **TeacherAssignments** | Maps teachers to their respective classes/subjects. |
| **assignments** | Stores all coursework created by teachers. |
| **submissions** | Tracks student answers, teacher marks, feedback, and grading status. |

Relationships between these collections are maintained using identifiers like `TeacherId`, `StudentId`, `AssignmentId`, and `Course`/`SubjectId`.

---

## 9. Demo Credentials

To help you quickly evaluate the system, here are working login credentials for all three roles:

| Role | Email | Password |
| --- | --- | --- |
| **Admin** | `maliha1M@admin.com` | `maliha1M@admin.com` |
| **Teacher** | `hania@gmail.com` | `Pa$$w0rd!` |
| **Student** | `hali@gmail.com` | `haluHALU123` |

---

## 10. Environment Configuration

The project includes `.env.example` files to make local configuration straightforward. You can use these as templates to set up your environment.

**Required Configuration Examples:**

* MongoDB connection string
* Database name (`eduassign`)
* Authentication configuration & secrets
* Frontend URL and Backend API URL

---

## 11. Running Tests

The backend includes a dedicated test suite that verifies core business rules, authorization logic, and submission workflows.

1. Navigate to the backend repository:
```bash
cd eduassign-backend

```


2. Execute the tests:
```bash
dotnet test

```


*Tests are located in the `EduAssign.Tests/` directory.*

---

## 12. API Documentation

The ASP.NET Core Web API is fully documented using OpenAPI/Swagger. When running the backend in development mode, you can access the Swagger UI endpoint to inspect, explore, and test the RESTful endpoints for assignments, submissions, user management, and authentication.

---

## 13. Authentication and Authorization

EduAssign leverages Better Auth for robust session-based authentication across all roles. The ASP.NET Core backend strictly enforces role-based authorization by validating session data and verifying role claims before granting access to protected API endpoints.

---

## 14. Assumptions & Design Choices

* **Database:** MongoDB was selected because its document-oriented structure aligns perfectly with the hierarchical nature of courses, assignments, and submissions.
* **Roles:** The architecture strictly isolates functionality between Admins, Teachers, and Students to ensure data security and a clean UX.
* **Workflows:** Teachers maintain full control over the assignments in their mapped subjects, while students are restricted to interacting only with their assigned coursework.
* **State Management:** Assignments utilize states (Draft vs. Published) to give teachers flexibility, and submission states are dynamically updated during the evaluation process.

---

## 15. Known Limitations

* The current setup assumes a local instance of MongoDB is running (unless a remote URI like MongoDB Atlas is explicitly configured in the settings).
* Deployment to production would require standardizing production environment variables and securing the database configuration.
* Advanced features like real-time push notifications or microservice messaging queues are currently outside the scope of this implementation.
* Automated testing is focused on validating critical business logic and core workflows rather than comprehensive end-to-end UI testing.

---

## 16. Quick Start (Local Evaluation Setup)

1. **Start MongoDB:** Ensure your local MongoDB service is running (or have your cloud URI ready).
2. **Start Backend:**
```bash
cd eduassign-backend
dotnet restore
dotnet run

```


3. **Start Frontend:**
```bash
cd eduassign
npm install
npm run dev

```


4. **Explore:** Open `http://localhost:3000` in your browser and use the demo credentials provided in Section 9 to explore the application!

---

## 17. Submission Checklist

* [x] Frontend repository link provided
* [x] Backend repository link provided
* [x] Complete project code included
* [x] Database setup instructions and seed/sample data included
* [x] Demo credentials provided for Admin, Teacher, and Student roles
* [x] `.env.example` included for easy setup
* [x] README includes a comprehensive overview, tech stack, setup instructions, assumptions, and limitations
* [x] Role-based authorization implemented and unit tests included

```

```