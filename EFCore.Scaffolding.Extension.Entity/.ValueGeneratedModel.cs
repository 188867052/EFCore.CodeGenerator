namespace Entities
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1509:Opening braces should not be preceded by blank line", Justification = "<挂起>")]
    public static class IncreaseModel
    {
        public static Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
            { "class.id", "id" },
            { "course.id", "id" },
            { "course_score.id", "id" },
            { "student.id", "id" },
            { "teacher.id", "id" },
            { "teacher_course_mapping.id", "id" },
        };
    }
}
