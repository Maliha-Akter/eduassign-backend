# EduAssign - Assignment & Submission Management System

EduAssign is a role-based Assignment & Submission Management System designed for schools and colleges.

The system allows administrators to manage users and academic information, teachers to create and manage assignments and evaluate student submissions, and students to view and submit assignments.

---

## 1. Project Overview

EduAssign is a full-stack web application developed as part of the Assistant Software Engineer Recruitment Project.

The application provides separate functionality for three user roles:
- Admin
- Teacher
- Student

The main purpose of the system is to simplify assignment creation, assignment submission, evaluation, and academic management through a centralized platform.

### Main Workflow

**Teacher:**
1. Creates an assignment.
2. Selects the class/course and subject.
3. Defines the title, description, deadline, and maximum marks.
4. Publishes the assignment or keeps it as a draft.
5. Views student submissions.
6. Gives marks and feedback.
7. Updates the submission status when necessary.

**Student:**
1. Logs into the system.
2. Views assignments available for their class/course.
3. Views assignment details and deadlines.
4. Submits an answer.
5. Updates the submission before the deadline when allowed.
6. Views submission status, marks, and teacher feedback.

**Admin:**
1. Manages users.
2. Manages classes/courses and subjects.
3. Assigns teachers to subjects/classes.
4. Views assignments and submissions.
5. Manages application-level settings where necessary.

---

## 2. Main Features

### Authentication and Authorization
- User login and registration
- Role-based access control (Admin, Teacher, Student roles)
- Protected frontend routes and backend API endpoints
- Session-based authentication using Better Auth
- Secure authentication information passing between frontend and ASP.NET Core API

### Admin Features
- Manage users, roles, and block/unblock capabilities
- Manage classes/courses and subjects
- Assign teachers to classes/subjects
- View all assignments and submissions
- Manage application-level settings

### Teacher Features
- View assigned subjects/classes via the teacher portal
- Create, update, and delete assignments (title, description, deadline, max marks, draft/publish status)
- View student submissions, grade them, provide feedback, and update statuses

### Student Features
- View assigned class/course assignments and deadlines
- Submit answers and update submissions before deadlines
- Track submission status, marks, and teacher feedback

### API Features
- RESTful ASP.NET Core Web API architecture
- Request validation, error handling, and security controls
- MongoDB database integration
- OpenAPI documentation

---

## 3. Technology Stack

- **Frontend:** Next.js, React, TypeScript, Tailwind CSS, Lucide React, React Toastify, Better Auth, REST API integration
- **Backend:** ASP.NET Core Web API, C#, MongoDB Driver, OpenAPI, Dependency Injection
- **Database:** MongoDB
- **Testing:** .NET testing framework (Unit tests for core business rules and workflows)
- **Development Tools:** Visual Studio Code, Git, GitHub, Postman, Swagger/OpenAPI

---

## 4. Project Structure

The repository contains both the frontend and backend applications in a single monorepo structure:

```text
eduassign/
│
├── frontend/
│   │   ├── src/
│   │   │   ├── app/
│   │   │   ├── components/
│   │   │   └── lib/
│   │   ├── public/
│   │   ├── package.json
│   │   └── .env.example
│
├── backend/
│   │   ├── EduAssign.API/
│   │   │   ├── Controllers/
│   │   │   ├── Data/
│   │   │   ├── DTOs/
│   │   │   ├── Models/
│   │   │   ├── Services/
│   │   │   ├── Validators/
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   └── EduAssign.API.csproj
│   │   │
│   │   └── EduAssign.Tests/
│       └── ...
│
├── .gitignore
└── README.md