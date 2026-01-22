using System;
using System.Collections.Generic;

namespace DatabaseFirstExample.Entities;

public partial class Student
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int Age { get; set; }

    public float? Scholarship { get; set; }
}
