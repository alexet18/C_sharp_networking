using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    class Produs
    {
        
        
            String name;
            String owner;
            int starting_price;
            int current_price;
            String state;



            public Produs(String name, String owner, int start, int curr)
            {
                this.name = name;
                this.owner = owner;
                this.starting_price = start;
                this.current_price = curr;
                this.state = "Active";
            } 
        public Produs(String name, String owner, int start, int curr,String state)
            {
                this.name = name;
                this.owner = owner;
                this.starting_price = start;
                this.current_price = curr;
                this.state = state;
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


    public String State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public int Current_price
            {
                get { return this.current_price; }
                set { this.current_price = value; }
            }
       
    }
 }

