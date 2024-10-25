using System;
using System.Collections.Generic;

namespace Hutech.Exam.Shared.Models;

public partial class Lop
{
    public int MaLop { get; set; }

    public string? TenLop { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public int? MaKhoa { get; set; }

    public virtual Khoa? MaKhoaNavigation { get; set; }
}
