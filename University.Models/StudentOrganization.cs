using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Models
{
    public class StudentOrganization
    {
        [Key]
        public int OrganizationId {  get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Advisor { get; set; } = string.Empty;
        public string President { get;set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public string MeetingSchedule { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public virtual ICollection<Student>? MemberShip { get; set; } = null;
    }
}
