using Godot;
using System;

namespace Managers
{
    public abstract class ManagerBase : Node2D
    {
        public GameManager GameManager { get; protected set; }
        public abstract void InitialiseManager(GameManager gameManager);
    }
}
