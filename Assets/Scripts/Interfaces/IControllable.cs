using Library;
using Units;
using Units.Controller;
using UnityEngine;

namespace Interfaces
{

    public enum MovementState
    {
        Init,
        Idle,
        Walking,
        Running,
    }

    public enum ControllerType
    {
        User,
        Fortress,
        GoblinMage,
        Goblin
    }

    /// <summary>
    ///  Ensures the object can move using input or other stimulus 
    /// </summary>
    public interface IControllable : IMovable, IParentable
    {
        /// <summary> Which directions the object is moving in. </summary>
        Moving isMoving { get; set; }
        bool canMoveWithInput { get; set; }

        FiniteStateMachine<MovementState> movementFSM { get; }

        ControllerType controllerType { get; set; }
        IController controller { get; set; }
        NavMeshAgent navMashAgent { get; }
        GameObject following { get; set; }
    }


}
