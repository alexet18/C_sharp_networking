using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace ServerSide
{
    class Produs
    {
        String name;
        String owner;
        int starting_price;
        int current_price;
        List<User> subscribed_bidders;
        User bid_winner;
        String state;



        public Produs(String name, String owner, int start, int curr)
        {
            this.name = name;
            this.owner = owner;
            this.starting_price = start;
            this.current_price = curr;
            bid_winner = null;
            subscribed_bidders = new List<User>();
            this.state = "Active";
            


        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            
        }

        public String Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
            }
        }

        public String Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        public int Starting_price
        {
            get { return this.starting_price; }
            set { this.starting_price = value; }

        }

        public int Current_price
        {
            get { return this.current_price; }
            set { this.current_price = value; }
        }

        public User BidWinner
        {
            get { return this.bid_winner; }
            set { this.bid_winner = value; }
        }

        public List<User> Subscribers
        {
            get { return this.subscribed_bidders; }
            set { this.subscribed_bidders = value; }
        }

        public String State
        {
            get { return this.state; }
            set { this.state = value; }
        }
    }
}


