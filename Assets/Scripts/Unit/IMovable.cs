//////////////////
//   IMovable   //
//////////////////

using UnityEngine;

namespace Unit
{
    /// <summary>
    /// Guarantees that the object can move using velocity
    /// </summary>
    public interface IMovable
    {
        Transform transform { get; }
        Vector3 totalVelocity { get; set; }

        /// <summary> The speed and direction at which the object is moving. </summary>
        Vector3 velocity { get; set; }

        /// <summary> The speed at which the object is moving. </summary>
        float speed { get; set; }
    }

    public static class Movable
    {
        public static void Brake(IMovable a_Movable)
        {
            if (Mathf.Abs(a_Movable.velocity.x) <= 0.01f &&
                Mathf.Abs(a_Movable.velocity.z) <= 0.01f)
            {
                a_Movable.velocity = Vector3.zero;
            }
            else
            {
                if (a_Movable.velocity.x > 0.0f)
                {
                    a_Movable.velocity -= new Vector3(
                        20f * Time.deltaTime,
                        0.0f,
                        0.0f);
                    if (a_Movable.velocity.x < 0.0f)
                        a_Movable.velocity = new Vector3(0.0f, a_Movable.velocity.y, a_Movable.velocity.z);
                }
                if (a_Movable.velocity.x < 0.0f)
                {
                    a_Movable.velocity += new Vector3(
                        20f * Time.deltaTime,
                        0.0f,
                        0.0f);
                    if (a_Movable.velocity.x > 0.0f)
                        a_Movable.velocity = new Vector3(0.0f, a_Movable.velocity.y, a_Movable.velocity.z);
                }

                if (a_Movable.velocity.z > 0.0f)
                {
                    a_Movable.velocity -= new Vector3(
                        0.0f,
                        0.0f,
                        20f * Time.deltaTime);
                    if (a_Movable.velocity.z < 0.0f)
                        a_Movable.velocity = new Vector3(a_Movable.velocity.x, a_Movable.velocity.y, 0.0f);
                }
                if (a_Movable.velocity.z < 0.0f)
                {
                    a_Movable.velocity += new Vector3(
                        0.0f,
                        0.0f,
                        20f * Time.deltaTime);
                    if (a_Movable.velocity.z > 0.0f)
                        a_Movable.velocity = new Vector3(a_Movable.velocity.x, a_Movable.velocity.y, 0.0f);
                }
            }
        }
    }
}