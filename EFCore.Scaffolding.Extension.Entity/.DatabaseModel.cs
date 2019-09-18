namespace Entities
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1509:Opening braces should not be preceded by blank line", Justification = "<挂起>")]
    public static class DatabaseModel
    {
        public static Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
            { "Student.PrimaryKey", "id" },
            { "Student", "student" },
            { "Student.Name", "name" },
            { "Student.Sex", "sex" },
            { "Student.Id", "id" },
            { "Student.ClassId", "class_id" },
            { "Student.CreateTime", "create_time" },
            { "Student.UpdateTime", "update_time" },
            { "Student.Address", "address" },
            { "Student.Mobile", "mobile" },

            { "Class.PrimaryKey", "id" },
            { "Class", "class" },
            { "Class.Id", "id" },
            { "Class.Name", "name" },
            { "Class.HeadTeacherId", "head_teacher_id" },
            { "Class.CreateTime", "create_time" },
            { "Class.UpdateTime", "update_time" },
            { "Class.Grade", "grade" },
            { "Class.Location", "location" },

            { "Course.PrimaryKey", "id" },
            { "Course", "course" },
            { "Course.Id", "id" },
            { "Course.Name", "name" },
            { "Course.TeacherId", "teacher_id" },
            { "Course.CreateTime", "create_time" },
            { "Course.UpdateTime", "update_time" },

            { "Teacher.PrimaryKey", "id" },
            { "Teacher", "teacher" },
            { "Teacher.Id", "id" },
            { "Teacher.Name", "name" },
            { "Teacher.Sex", "sex" },
            { "Teacher.CreateTime", "create_time" },
            { "Teacher.UpdateTime", "update_time" },

            { "CourseScore.PrimaryKey", "id" },
            { "CourseScore", "course_score" },
            { "CourseScore.Id", "id" },
            { "CourseScore.Score", "score" },
            { "CourseScore.StudentId", "student_id" },
            { "CourseScore.CourseId", "course_id" },
            { "CourseScore.CreateTime", "create_time" },
            { "CourseScore.UpdateTime", "update_time" },

            { "TeacherCourseMapping.PrimaryKey", "id" },
            { "TeacherCourseMapping", "teacher_course_mapping" },
            { "TeacherCourseMapping.Id", "id" },
            { "TeacherCourseMapping.CourseId", "course_id" },
            { "TeacherCourseMapping.TeacherId", "teacher_id" },
            { "TeacherCourseMapping.CreateTime", "create_time" },
            { "TeacherCourseMapping.UpdateTime", "update_time" },

            { "Log.PrimaryKey", "identifier" },
            { "Log", "log" },
            { "Log.Identifier", "identifier" },
            { "Log.Message", "message" },
            { "Log.CreateTime", "create_time" },
        };
    }
}
