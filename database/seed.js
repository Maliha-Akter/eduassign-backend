// EduAssign Database Seed
// Database: eduassign
//
// Collections included:
// - user
// - Courses
// - TeacherAssignments
// - assignments
// - submissions
//
// Better Auth collections such as:
// - account
// - jwks
// - verification
// are NOT seeded here because they contain authentication/system data
// and are managed by Better Auth.

use("eduassign");

// ======================================================
// FIXED IDs
// ======================================================

const adminId = ObjectId("6a7ac5a2a5c1462ce3510cb1");
const teacherId = ObjectId("6a7ac5a2a5c1462ce3510cb2");
const studentId = ObjectId("6a7bdbad27e199a9598ede02");

const libertyCourseId = ObjectId("6a7da4dd181a2e4518abadf6");
const databaseCourseId = ObjectId("6a7da5c5245d31c41e26af42");

const teacherAssignmentId = ObjectId("6a7f3717e64452b459416514");

const assignmentId = ObjectId("6a7ac6abe6560f21b29ed089");

const gradedSubmissionId = ObjectId("6a7c672dfc54483778d55afe");
const submittedSubmissionId = ObjectId("6a7c6869fc54483778d55b00");


// ======================================================
// 1. USERS
// ======================================================

// Admin
db.user.updateOne(
    { _id: adminId },
    {
        $set: {
            name: "Demo Admin",
            email: "admin@eduassign.com",
            emailVerified: false,
            image: "",
            createdAt: new Date(),
            updatedAt: new Date(),
            role: "admin",
            isBlocked: false
        }
    },
    { upsert: true }
);


// Teacher
db.user.updateOne(
    { _id: teacherId },
    {
        $set: {
            name: "akhi akhi",
            email: "teacher@eduassign.com",
            emailVerified: false,
            image: "",
            createdAt: new Date(),
            updatedAt: new Date(),
            role: "teacher",
            isBlocked: false,
            primarySubject: "Mathematics",
            qualification: "Bachelor's"
        }
    },
    { upsert: true }
);


// Student
db.user.updateOne(
    { _id: studentId },
    {
        $set: {
            name: "Halima",
            email: "student@eduassign.com",
            emailVerified: false,
            image: "",
            createdAt: new Date(),
            updatedAt: new Date(),
            role: "student",
            isBlocked: false,
            class: "10"
        }
    },
    { upsert: true }
);


// ======================================================
// 2. COURSES
// ======================================================

db.Courses.updateOne(
    { _id: libertyCourseId },
    {
        $set: {
            name: "Liberty",
            code: "LIB-2109"
        }
    },
    { upsert: true }
);


db.Courses.updateOne(
    { _id: databaseCourseId },
    {
        $set: {
            name: "Database",
            code: "Data-2309"
        }
    },
    { upsert: true }
);


// ======================================================
// 3. TEACHER ASSIGNMENTS
// ======================================================

db.TeacherAssignments.updateOne(
    { _id: teacherAssignmentId },
    {
        $set: {
            TeacherId: teacherId.toString(),
            AssignedClass: "Class-10",
            Section: "Section-C",
            CreatedAt: new Date(),
            PrimarySubject: "Mathematics"
        }
    },
    { upsert: true }
);


// ======================================================
// 4. ASSIGNMENTS
// ======================================================

db.assignments.updateOne(
    { _id: assignmentId },
    {
        $set: {
            TeacherId: teacherId.toString(),
            ClassId: "class_10",
            SubjectId: "sub_math",
            Title: "Finance",
            Description: "Do all check up",
            Deadline: new Date("2026-12-30T23:59:59.000Z"),
            MaximumMarks: 100,
            Status: "Published",
            CreatedAt: new Date(),
            UpdatedAt: new Date()
        }
    },
    { upsert: true }
);


// ======================================================
// 5. SUBMISSIONS
// ======================================================

// Graded submission
db.submissions.updateOne(
    { _id: gradedSubmissionId },
    {
        $set: {
            AssignmentId: assignmentId.toString(),
            StudentId: studentId.toString(),
            Answer: "opk",
            Marks: 78,
            Feedback: "good Masha-Allah",
            Status: "Graded",
            SubmittedAt: new Date(),
            UpdatedAt: new Date()
        }
    },
    { upsert: true }
);


// Submitted but not graded
db.submissions.updateOne(
    { _id: submittedSubmissionId },
    {
        $set: {
            AssignmentId: assignmentId.toString(),
            StudentId: studentId.toString(),
            Answer: "er",
            Marks: null,
            Feedback: null,
            Status: "Submitted",
            SubmittedAt: new Date(),
            UpdatedAt: new Date()
        }
    },
    { upsert: true }
);


// ======================================================
// DONE
// ======================================================

print("");
print("==============================================");
print("EduAssign database seed completed successfully");
print("==============================================");

print("Database: eduassign");
print("Users: " + db.user.countDocuments());
print("Courses: " + db.Courses.countDocuments());
print("TeacherAssignments: " + db.TeacherAssignments.countDocuments());
print("Assignments: " + db.assignments.countDocuments());
print("Submissions: " + db.submissions.countDocuments());

print("");
print("Better Auth collections such as account, jwks,");
print("and verification are intentionally not seeded.");
print("==============================================");