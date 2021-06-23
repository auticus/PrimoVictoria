using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimoVictoria.Utilities
{
    /// <summary>
    /// A class that holds static resources shared by multiple objects throughout the solution
    /// </summary>
    public static class StaticResources
    {
        public const string MESH_DECORATOR_TAG = "UnitMeshDecorator";
        public const string SELECT_BUTTON = "Input1"; //the name of the control set in bindings
        public const string EXECUTE_BUTTON = "Input2";
        public const string WHEEL_LEFT = "WheelUnitLeft";
        public const string WHEEL_RIGHT = "WheelUnitRight";
        public const string MOVE_UNIT_UP_DOWN = "MoveUnitUpDown";
        public const string MOVE_UNIT_RIGHT_LEFT = "MoveUnitRightLeft";
        public const int TERRAIN_LAYER = 8;
        public const int MINIATURES_LAYER = 9;

        public const string PRELOAD_SCENE = "Preload";
        public const string SANDBOX_SCENE = "Sandbox";
    }
}
