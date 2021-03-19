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

namespace ClientSide
{
    public partial class Form1 : Form
    {

        TcpClient tcpclnt;
        String username;
        List<Produs> my_offers;
        List<Produs> produse;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tcpclnt = new TcpClient();


            produse = new List<Produs>();
            my_offers = new List<Produs>();
            Produs p;




            IPAddress ipAd = IPAddress.Parse("192.168.3.108");


            int PortNumber = 8888;


            username dialog = new username();
            dialog.ShowDialog();

            if (dialog.DialogResult == DialogResult.OK)
            {
                label3.Text = "Username: " + dialog.tb_username.Text;
                username = dialog.tb_username.Text;
                try
                {
                    tcpclnt.Connect(ipAd, PortNumber);

                    NetworkStream stream = tcpclnt.GetStream();
                    byte[] sendBytes;
                    sendBytes = Encoding.ASCII.GetBytes(username);
                    stream.Write(sendBytes, 0, sendBytes.Length);


                    Thread ReceiveData = new Thread(DataReceive);
                    ReceiveData.Start();

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

            }






        }

        private void DataReceive()
        {
            NetworkStream stream = tcpclnt.GetStream();
            String[] packet;

            while (true)
            {
                try
                {
                    packet = ReadPacket(stream);

                    DoWorkOnPacket(packet);

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        private void DoWorkOnPacket(string[] packet)
        {
            switch (packet[0])
            {
                case "update_produs":



                    produse.Clear();
                    my_offers.Clear();

                    String owner, name,state;
                    int starting_price, current_price, parser = 2;

                    int x = Int32.Parse(packet[1]);

                    for (int j = 1; j <= x; j++)
                    {

                        name = packet[parser];
                        parser++;
                        owner = packet[parser];
                        parser++;
                        starting_price = Int32.Parse(packet[parser]);
                        parser++;
                        current_price = Int32.Parse(packet[parser]);
                        parser++;
                        state = packet[parser];
                        parser++;
                        Produs p = new Produs(name, owner, starting_price, current_price,state);
                        
                       



                       
                        if (owner == username)
                        {
                            my_offers.Add(p);
                        }
                        else
                        {
                            produse.Add(p);
                        }


                    }

                   

                    dataGridView1.Invoke(
                       new MethodInvoker(delegate ()
                       {
                           dataGridView1.ClearSelection();
                           dataGridView1.DataSource = null;
                           dataGridView1.DataSource = produse;
                           dataGridView1.ClearSelection();
                           foreach(DataGridViewRow r in  dataGridView1.Rows)
                           {
                               if (produse.ElementAt(r.Index).State == "Active")
                               {
                                   r.DefaultCellStyle.ForeColor = Color.Green;
                               }
                               else
                                   r.DefaultCellStyle.ForeColor = Color.Red;
                           }

                       }));

                    dataGridView2.Invoke(
                          new MethodInvoker(delegate ()
                          {
                              dataGridView2.ClearSelection();
                              dataGridView2.DataSource = null;
                              dataGridView2.DataSource = my_offers;
                              dataGridView2.ClearSelection();
                              

                          }));




                    break;

                case "notification":
                    MessageBox.Show(packet[1]);
                    break;

            }



        }

        private string[] ReadPacket(NetworkStream stream)
        {
            String[] packet;

            byte[] bytesFrom = new byte[1024];

            stream.Read(bytesFrom, 0, 1024);

            packet = System.Text.Encoding.ASCII.GetString(bytesFrom).Split('$');

            return packet;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            AddAuction dialog = new AddAuction();
            dialog.ShowDialog();

            if (dialog.DialogResult == DialogResult.OK)
            {
                String packet = "upload_produs$" + dialog.tb_name.Text + "$" + username + "$" + dialog.tb_startingprice.Text + "$" + dialog.tb_startingprice.Text + "$";

                SendPacket(packet);
            }
        }

        private void SendPacket(string packet)
        {
            byte[] sendByte = new byte[1024];
            sendByte = Encoding.ASCII.GetBytes(packet);

            NetworkStream stream = tcpclnt.GetStream();

            stream.Write(sendByte, 0, sendByte.Length);

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                
                Produs p = produse.ElementAt(dataGridView1.CurrentCell.RowIndex);

                if (p.State == "Expired")
                {
                    MessageBox.Show("This offer expired!");
                }
                else
                {
                    BidOnOffer dialog = new BidOnOffer(p.Current_price);
                    dialog.ShowDialog();
                    if (dialog.DialogResult == DialogResult.OK)
                    {

                        String packet = "bid$" + p.Name + "$" + dialog.tb_bid.Text + "$" + username + "$";
                        p = null;
                        SendPacket(packet);

                    }
                }

            }
            catch
            {
                dataGridView1.ClearSelection();
            }
            }
        }

        

     

    }

