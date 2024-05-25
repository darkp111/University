using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Models
{
    public class Classroom
    {
        public string ClassroomId { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public int AvailableSeats {  get; set; } = 0;
        public bool Projector { get; set; } = false;
        public bool Whiteboard { get; set; } = false;
        public bool Microphone { get; set; } = false;
        public string Description { get;set; } = string.Empty;
    }
}
