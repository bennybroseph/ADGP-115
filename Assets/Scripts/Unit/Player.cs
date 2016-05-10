//Unit class used for storing Player and Enemy Data.

namespace Unit
{
    //Public unit class that takes in IStats and IAttack
    public class Unit : IStats, IAttack
    {
        //Private int and string memorable variables
        private int m_Health;
        private int m_Defense;
        private int m_Exp;
        private int m_Lvl;
        private int m_Speed;
        private int m_Mana;
        private string m_Name;
        
        //Default Constructor
        public Unit()
        {

        }
        //Unit class that stores Health, Defense, Exp, Level, Speed, Mana, Name
        public Unit(int a_Health, int a_Defense, int a_Exp,int a_Lvl, int a_Speed, int a_Mana, string a_Name)
        {
            //sets memoorial varial to equal its a
            m_Health = a_Health;
            m_Defense = a_Defense;
            m_Exp = a_Exp;
            m_Lvl = a_Lvl;
            m_Speed = a_Speed;
            m_Mana = a_Mana;
            m_Name = a_Name;
        }

        //Health int property
        public int Health
        {
            get { return m_Health; }
            set { m_Health = value; } 
        }

        //Defense int property
        public int Defense{
            get { return m_Defense; }
            set { m_Defense = value; }
        }
        //Experience int property
        public int Experience
        {
            get { return m_Exp; }
            set { m_Exp = value; }
        }
        //Speed int property
        public int Speed
        {
            get { return m_Speed;}
            set { m_Speed = value; }
        }
        //Level int property
        public int Lvl
        {
            get { return m_Lvl; }
            set { m_Lvl = value; }
        }
       //Mana/Currency int property
        public int Mana
        {
            get { return m_Mana;}
            set { m_Mana = value; }
        }

        //String name property
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        //Function used to detect Fight
        public void Fight()
        {
            //Implement as project goes on.  
        
        }
         
     }

}

