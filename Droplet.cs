using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDoser
{
    class Droplet
    {
        public string IP { get; set; }
        public string Password { get; set; }

        public Droplet(string textBoxLine, string password)
        {
            Password = password;
            IP = textBoxLine.Split( new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}
