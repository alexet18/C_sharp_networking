using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerSide
{
    public partial class Form1 : Form
    {

        TcpListener ServerListener;
        List<User> clients;
        List<Produs> auctions;

        public Form1()
        {
            InitializeComponent();
            this.listView1.Width = this.Width;
            this.listView1.Height = this.Height;
            this.listView1.Left = 0;
            this.Top = 0;
            listView1.Items.Add("Log:");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Declare and Initialize the IP Adress
            IPAddress ipAd = IPAddress.Parse("192.168.3.108");

            //Declare and Initilize the Port Number;
           int PortNumber = 8888;

            /* Initializes the Listener */
            ServerListener = new TcpListener(ipAd, PortNumber);
            clients = new List<User>();
            auctions = new List<Produs>();

            ServerListener.Start();


            Produs p = new Produs("test", "Ion", 50, 50);
            auctions.Add(p);

            Thread Connections = new Thread(WaitForConnections);
            Connections.Start();
        }

        private void WaitForConnections()
        {
           while(true)
            {
                String exists = "no";
                try
                { exists = "no";
                    TcpClient socket = default(TcpClient);
                    socket = ServerListener.AcceptTcpClient();

                    NetworkStream stream = socket.GetStream();
                    byte[] bytesFrom = new byte[1024];
                    stream.Read(bytesFrom, 0, 1024);
                    String username = System.Text.Encoding.ASCII.GetString(bytesFrom);


                    foreach(User u in clients)
                    {
                        if (u.Username.Substring(0, username.Length) == username)
                        {
                            exists = "yes";
                            User client_denied = new User(username, socket);
                            String notification = "notification$username already exists!$";
                            SendPacket(notification, client_denied);
                            client_denied.Socket.Close();
                        }
                        
                    }

                    if (exists == "no")
                    {
                        User client = new User(username, socket);

                        clients.Add(client);







                        listView1.Invoke(
                            new MethodInvoker(delegate ()
                            {
                                listView1.Items.Add(new ListViewItem(username + " connected!"));

                            }));

                        SendAuctionList(stream);


                        Thread dataReceive = new Thread(() => DataReceive(clients.ElementAt(clients.Count - 1)));
                        dataReceive.Start();
                    }
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
               }
           }
            
        

        private void DataReceive(User user)
        { 
            String[] packet;
            NetworkStream stream = user.Socket.GetStream();
           

            while (true)
            {
                try
                {
                   packet = ReadPacket(stream);

                    DoWorkOnPacket(packet);
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void DoWorkOnPacket(string[] packet)
        {

            switch (packet[0])
            {   
                case "upload_produs":
                    int error = 0;
                        String owner, name;
                    int starting_price, current_price;

                    name = packet[1];


                    foreach(Produs p in auctions)
                    {
                        if(p.Name == name)
                        {
                            foreach(User u in clients)
                            {
                                if(u.Username.Substring(0, p.Owner.Length) == p.Owner)
                                {
                                    String notificaiton = "notification$There already exists an offer for this product!$";
                                    SendPacket(notificaiton, u);
                                }
                            }
                            error = 1;
                        }
                    }
                    if (error == 0)

                    {
                        owner = packet[2];
                        starting_price = Int32.Parse(packet[3]);
                        current_price = Int32.Parse(packet[4]);

                        Produs p = new Produs(name, owner, starting_price, current_price);

                        auctions.Add(p);

                        Task.Delay(new TimeSpan(0, 1, 0)).ContinueWith(o => { Expire(p); });


                        refresh_users_auctions();

                        foreach (User u in clients)
                        {
                            if (u.Username != p.Owner)
                            {
                                String notification = "notification$A new offer" + p.Name + " by " + p.Owner + " with starting price of " + p.Starting_price + " has been listed!!$";
                                SendPacket(notification, u);
                            }
                        }


                    }

                    break;
                case "bid":
                    foreach(Produs pr in auctions)
                    {
                        if(pr.Name == packet[1])
                        {
                            if (pr.Current_price >= Int32.Parse(packet[2]))
                            {
                                foreach (User u in clients)
                                {   if ((u.Username.Substring(0,packet[3].Length)) == packet[3])
                                    {
                                        String notification = "notification$Error: Your bid cannot be lower or equal to the current bid!$";
                                        SendPacket(notification, u);
                                    }
                                }
                            }
                            else
                            {
                                pr.Current_price = Int32.Parse(packet[2]);

                                foreach (User u in clients)
                                {
                                    if (u.Username.Substring(0, packet[3].Length) == packet[3]) 
                                    {
                                        pr.BidWinner = u;
                                        if (!pr.Subscribers.Contains(u)) ;
                                        pr.Subscribers.Add(u);
                                    }
                                }
                                
                                foreach(User u in pr.Subscribers)
                                {
                                    if(u.Username.Substring(0,pr.Owner.Length) == pr.Owner)
                                    {
                                        String notification = ("notification$Somebody has placed a higher bid on" + pr.Name + "!$");
                                        SendPacket(notification, u);
                                    }
                                    if (u != pr.BidWinner)
                                    {
                                        String notification = ("notification$Somebody has placed a higher bid on" + pr.Name + "!$");
                                        SendPacket(notification, u);
                                        
                                        
                                    }
                                }
                            }
                            
                        }

                    }
                    refresh_users_auctions();
                    break;
            }
        }

        private void Expire(Produs p)
        {
            p.State = "Expired";
            String notification = "notification$" + p.Name + " expired!$";
            foreach(User u in clients)
            {
                SendPacket(notification, u);
            }
            refresh_users_auctions();
        }

        private void refresh_users_auctions()
        {
            foreach(User u in clients)
            {
                SendAuctionList(u.Socket.GetStream());
            }
        }

         

        private String[] ReadPacket(NetworkStream stream)
        {
            String[] packet;

            byte[] bytesFrom = new byte[1024];

            stream.Read(bytesFrom, 0, 1024);

            packet = System.Text.Encoding.ASCII.GetString(bytesFrom).Split('$');

            return packet;


        }

        private void SendAuctionList(NetworkStream stream)
        {
            String packet = "update_produs$"+auctions.Count().ToString()+"$";
            foreach(Produs p in auctions)
            {
                
                
                    packet += p.Name + "$" + p.Owner + "$" + p.Starting_price.ToString() + "$" + p.Current_price.ToString() + "$"+p.State+"$";
                
               

            }

            byte[] sendBytes = new byte[1024];
            sendBytes = Encoding.ASCII.GetBytes(packet);
            stream.Write(sendBytes, 0, sendBytes.Length);
        }

        private void SendPacket(string packet,User u)
        {
            byte[] sendByte = new byte[1024];
            sendByte = Encoding.ASCII.GetBytes(packet);

            NetworkStream stream = u.Socket.GetStream();

            stream.Write(sendByte, 0, sendByte.Length);

        }

       

    }
}
