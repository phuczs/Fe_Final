using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class UpdateMyProfileRequest
    {
        public string? DisplayName { get; set; }

        public string? StaffStudentId { get; set; }

        public string? Sex { get; set; }

        public string? MobilePhone { get; set; }
    }

}
