using UnityEngine;

namespace MyProjects.Scripts
{
    public class AnimHash
    {
        public string Name { get; }
        public int Code { get; }

        public AnimHash(string name)
        {
            Name = name;
            Code = Animator.StringToHash(name);
        }

    }
}