using UnityEngine;
using System.Collections;
using Units.Skills;

namespace Interfaces
{
        public interface IParentable
        {
            Transform transform { get; }
            GameObject gameObject { get; }
        }

        public interface IChildable<TParentType> where TParentType : IParentable
        {
            TParentType parent { get; set; }
        }

        public interface ICastable<TParentType> : IChildable<TParentType> where TParentType : IParentable
        {
            float currentLifetime { get; }
            float maxLifetime { get; set; }

            SkillData skillData { get; set; }
        }
}
