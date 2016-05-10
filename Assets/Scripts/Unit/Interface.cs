//Interface

namespace Unit
{
    public interface IControlable
    {
        //Can be used to check if Wave is 
    }

    public interface IAttackable
    {
        //Function for combat
        void Fight();
    }

    public interface IStats
    {
        //Health property
        int health { get; set; }
        //Mana(currency) property
        int mana { get; set; }
        //Speed property
        int speed { get; set; }
        //Defense property
        int defense { get; set; }
        //Experience property
        int experience { get; set; }
        //Level property
        int level { get; set; }
        //String Name property
        string name { get; set; }
    }
}
