using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sean.Combat
{
    public class CombatActions : IDisposable
    {
        public InputActionAsset asset { get; }
        private CombatActionMap _combat;
        public CombatActionMap Combat => _combat;

        public CombatActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CombatActions"",
    ""maps"": [
        {
            ""name"": ""Combat"",
            ""id"": ""a1b2c3d4-0001-0001-0001-000000000001"",
            ""actions"": [
                { ""name"": ""ParryUp"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0002-0001-0001-000000000001"" },
                { ""name"": ""ParryDown"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0002-0002-0001-000000000001"" },
                { ""name"": ""ParryLeft"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0002-0003-0001-000000000001"" },
                { ""name"": ""ParryRight"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0002-0004-0001-000000000001"" },
                { ""name"": ""Punch"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0002-0005-0001-000000000001"" },
                { ""name"": ""Dodge"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0002-0006-0001-000000000001"" }
            ],
            ""bindings"": [
                { ""path"": ""<Keyboard>/w"", ""action"": ""ParryUp"" },
                { ""path"": ""<Keyboard>/s"", ""action"": ""ParryDown"" },
                { ""path"": ""<Keyboard>/a"", ""action"": ""ParryLeft"" },
                { ""path"": ""<Keyboard>/d"", ""action"": ""ParryRight"" },
                { ""path"": ""<Keyboard>/upArrow"", ""action"": ""Punch"" },
                { ""path"": ""<Keyboard>/space"", ""action"": ""Dodge"" }
            ]
        }
    ],
    ""controlSchemes"": [
        { ""name"": ""Keyboard"", ""bindingGroup"": ""Keyboard"", ""devices"": [{ ""devicePath"": ""<Keyboard>"" }] }
    ]
}");
            _combat = new CombatActionMap(asset);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public void Enable() => asset.Enable();
        public void Disable() => asset.Disable();

        public class CombatActionMap
        {
            private readonly InputActionMap _map;

            public InputAction ParryUp { get; }
            public InputAction ParryDown { get; }
            public InputAction ParryLeft { get; }
            public InputAction ParryRight { get; }
            public InputAction Punch { get; }
            public InputAction Dodge { get; }

            public CombatActionMap(InputActionAsset asset)
            {
                _map = asset.FindActionMap("Combat", throwIfNotFound: true);
                ParryUp = _map.FindAction("ParryUp", throwIfNotFound: true);
                ParryDown = _map.FindAction("ParryDown", throwIfNotFound: true);
                ParryLeft = _map.FindAction("ParryLeft", throwIfNotFound: true);
                ParryRight = _map.FindAction("ParryRight", throwIfNotFound: true);
                Punch = _map.FindAction("Punch", throwIfNotFound: true);
                Dodge = _map.FindAction("Dodge", throwIfNotFound: true);
            }

            public void Enable() => _map.Enable();
            public void Disable() => _map.Disable();
        }
    }
}
