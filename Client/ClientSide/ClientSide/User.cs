using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    class User
    {
        String username;
        TcpClient socket;


        public User(string s, TcpClient t)
        {
            this.username = s;
            this.socket = t;
        }

        public TcpClient Socket
        {
            get { return this.socket; }
            set { this.socket = value; }
        }

        public String Username
        {
            get { return this.username; }
            set { this.username = value; }
        }


    }
}
