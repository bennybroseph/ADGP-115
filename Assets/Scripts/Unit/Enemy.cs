
namespace Unit
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : IStats, IAttackable
    {
        //Private int and string memorable variables
        private int m_Health;
        private int m_Defense;
        private int m_Exp;  //Total experience each monster drops
        private int m_Lvl; //wont be displayed for Enemy
        private int m_Speed;
        private int m_Mana;
        private string m_Name;

        //Default Constructor for Enemy class
        public Enemy()
        {

        }

        //Enemy class that stores Health, Defense, Exp, Level, Speed, Mana, Name
        public Enemy(int a_Health, int a_Defense, int a_Exp, int a_Lvl, int a_Speed, int a_Mana, string a_Name)
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
        //Public int health property
        public int health
        {
            get { return m_Health;}
            set { m_Health = value; }
        }
        //Public int Defense property
        public int defense
        {
            get { return m_Defense;}
            set { m_Defense = value; }
        }
        //Public int Experience property
        public int experience
        {
            get { return m_Exp; }
            set { m_Exp = value; }
        }
        //Public int Level property
        public int level
        {
            get { return m_Lvl;}
            set { m_Lvl = value; }
        }
        //Public int Speed property
        public int speed
        {
            get { return m_Speed;}
            set { m_Speed = value; }
        }
        //Public int Mana property
        public int mana
        {
            get { return m_Mana;}
            set { m_Mana = value; }
        }
        //Public string Name property
        public string name
        {
            get { return m_Name;}
            set { m_Name = value; }
        }
        //Enemy Fight Function in herited from IAttackable
        public void Fight()
        {
            
        }
    }

}
