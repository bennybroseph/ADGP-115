using Define;
using Interfaces;
using Library;
using Units.Skills;

namespace Units
{
    public class GoblinMage : Enemy
    {
        protected override void Start()
        {

            base.Start();

            Publisher.self.Broadcast(Event.UpgradeSkill, this, 0);
        }
    }
}
