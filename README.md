# EduAssign - Assignment & Submission Management System

EduAssign is a role-based Assignment & Submission Management System designed for schools and colleges. The system allows administrators to manage users and academic information, teachers to create and manage assignments and evaluate student submissions, and students to view and submit assignments.

## Project Repositories
* **Frontend Repository:** [https://github.com/Maliha-Akter/eduassign](https://github.com/Maliha-Akter/eduassign)
* **Backend Repository:** [https://github.com/Maliha-Akter/eduassign-backend](https://github.com/Maliha-Akter/eduassign-backend)

The frontend is built with Next.js, React, TypeScript, and Tailwind CSS, while the backend is developed using ASP.NET Core Web API, C#, and MongoDB.

---

## 1. Project Overview
EduAssign is a full-stack web application developed as part of the Assistant Software Engineer Recruitment Project. The application provides separate functionality for three user roles: **Admin**, **Teacher**, and **Student**. The main purpose of the system is to simplify assignment creation, submission, evaluation, and academic management through a centralized platform.

### Main Workflow
* **Teacher:**
  * Creates an assignment.
  * Selects the class/course and subject.
  * Defines the title, description, deadline, and maximum marks.
  * Publishes the assignment or keeps it as a draft.
  * Views student submissions.
  * Gives marks and feedback.
  * Updates the submission status when necessary.
* **Student:**
  * Logs into the system.
  * Views assignments available for their class/course.
  * Views assignment details and deadlines.
  * Submits an answer.
  * Updates the submission before the deadline when allowed.
  * Views submission status, marks, and teacher feedback.
* **Admin:**
  * Manages users.
  * Manages classes/courses and subjects.
  * Assigns teachers to subjects/classes.
  * Views assignments and submissions.
  * Manages application-level settings where necessary.

---

## 2. Main Features
* **Authentication and Authorization:** User registration and login, role-based access control, protected frontend routes, protected backend API endpoints, and Better Auth integration between Next.js and ASP.NET Core API.
* **Admin Features:** Manage users, manage user roles, block/unblock users, manage courses and subjects, assign teachers to classes/subjects, and view assignments and submissions.
* **Teacher Features:** View assigned classes and subjects, create/update/delete assignments, set titles, descriptions, deadlines, and maximum marks, publish assignments or keep as drafts, view student submissions, assign marks, provide feedback, and update submission status.
* **Student Features:** View assignments assigned to their class/course, view details and deadlines, submit answers, update submissions before the deadline, and view submission status, marks, and teacher feedback.
* **API Features:** RESTful ASP.NET Core Web API, role-based authorization, MongoDB integration, request validation, error handling, logging, and OpenAPI documentation.

---

## 3. Technology Stack
* **Frontend:** Next.js, React, TypeScript, Tailwind CSS, Better Auth, Lucide React, React Toastify, REST API integration.
* **Backend:** ASP.NET Core Web API, C#, MongoDB Driver, RESTful API, OpenAPI, Dependency Injection.
* **Database:** MongoDB.
* **Authentication:** Better Auth, session-based authentication.
* **Testing:** .NET testing framework for unit testing important business rules and workflows.
* **Development Tools:** Visual Studio Code, Git, GitHub, Postman.

---

## 4. Project Structure
The project is organized into separate frontend and backend repositories.

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

## Backend Repository Structure (`eduassign-backend/`)

```plaintext
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

```markdown
## 5. Frontend Setup

### Prerequisites
* Node.js
* npm
* Git

### Installation and Running

Clone the frontend repository:
```bash
git clone [https://github.com/Maliha-Akter/eduassign.git](https://github.com/Maliha-Akter/eduassign.git)

```

Move into the project directory:

```bash
cd eduassign

```

Install dependencies:

```bash
npm install

```

Create a `.env.local` file based on the provided `.env.example` and configure the required environment variables.

Run the development server:

```bash
npm run dev

```

The frontend will be available at `http://localhost:3000`.

```

```
