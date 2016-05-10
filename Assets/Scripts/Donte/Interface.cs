//Interface

namespace Donte
{
    public interface IControl
    {
        //Can be used to check if Wave is 
    }

    public interface IAttack
    {
        //Function for combat
        void Fight();
    }

    public interface IStats
    {
        //Health property
        int Health { get; set; }
        //Mana(currency) property
        int Mana { get; set; }
        //Speed property
        int Speed { get; set; }
        //Defense property
        int Defense { get; set; }
        //Experience property
        int Experience { get; set; }
        //Level property
        int Lvl { get; set; }
        //String Name property
        string Name { get; set; }
    }
}
