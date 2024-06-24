//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Programmer/Scripts/InputActions/L2PlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @L2PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @L2PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""L2PlayerInput"",
    ""maps"": [
        {
            ""name"": ""CharacterControls"",
            ""id"": ""ccc18e45-36fe-455a-83fd-66c206aa17d0"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""19d8d1ed-bd5b-4bb5-8e88-009604abbba4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""a51d8339-d162-43c5-8c38-0d56baf71f4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""fc33a397-13fc-4d00-b405-3d82558623be"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""086c5a76-8d23-4ffc-80c2-b0bc027fcf3a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Skill1"",
                    ""type"": ""Button"",
                    ""id"": ""49e05646-9f30-4f0e-91cf-3dd9cc0bd634"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""ffd148a0-5d62-4a1c-947f-33a08d99b0ed"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""32b09564-684c-4b93-abe6-3c8b77a0b1c7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cbf49ad6-4d92-4426-908f-e52d2eb94ad0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7e4a6f4e-575d-4b65-b1ae-5c9d5f923aec"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""94cd84ed-1ac1-4644-9379-c362abb9e9f4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7cd25570-5f45-43d8-95f6-d84049ef1d78"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba2a5a78-b06e-45ff-adc8-f487b236b6d7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""548e6e4a-1bf5-4fbc-8058-1bd46e7d4210"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52ccd377-88d5-451e-af9f-208240475777"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d50b2bb0-3441-4b5f-9af9-9930f8831455"",
                    ""path"": ""<Joystick>/{Hatswitch}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d85f241-4ad5-4967-93bf-874d5c8fbaf3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ShortcutKey"",
            ""id"": ""cda95a47-e31d-4e0c-bfa5-cb76064bea69"",
            ""actions"": [
                {
                    ""name"": ""GetPuppet"",
                    ""type"": ""Button"",
                    ""id"": ""e6c27cfb-e420-41c0-a6be-2ef2ac43c7bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fc5218ad-fd09-47a4-8ac4-4e54e3f7d96c"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GetPuppet"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""AfterSplitScreenShortCut"",
            ""id"": ""cf5a46ed-d126-4bad-b930-968feab59afd"",
            ""actions"": [
                {
                    ""name"": ""SplitScreen1"",
                    ""type"": ""Button"",
                    ""id"": ""a7db3ec2-a2d7-4a4b-aff3-4f3bf21d3bfe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SplitScreen2"",
                    ""type"": ""Button"",
                    ""id"": ""1bdddd62-30f0-4aec-bb34-9c6f35922be4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SplitScreen3"",
                    ""type"": ""Button"",
                    ""id"": ""59957eef-ec5a-48a6-a3e7-2eccc35e8d82"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""de1bf0a0-c6a7-4017-a24b-dfc4c48d9747"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SplitScreen1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6bb77002-bffd-48a4-b066-4eb51c6f4cf6"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SplitScreen2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff3567fd-6038-4424-a6a8-4b1d8b899d44"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SplitScreen3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Interaction"",
            ""id"": ""c5996b6a-72c6-4332-80c2-7c8c2152738d"",
            ""actions"": [
                {
                    ""name"": ""interact"",
                    ""type"": ""Button"",
                    ""id"": ""66bb4374-b9b8-44bd-8612-4791b497953f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""974a8b28-152e-4d15-96f5-f3063ac76674"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Always"",
            ""id"": ""d1bfd32c-f6dd-44e1-8d8a-878ea01eaf16"",
            ""actions"": [
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""5969fa1f-f016-4486-87fd-053d3aed1690"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e896c8cc-f3de-4142-9270-176748072d07"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""LockView"",
            ""id"": ""130d1183-2d1a-4d5e-b8db-3b636aa54e40"",
            ""actions"": [
                {
                    ""name"": ""Lock"",
                    ""type"": ""Button"",
                    ""id"": ""27a19c0d-983c-4d2f-a175-10b99d339d5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""GiveUpBackPanel"",
                    ""type"": ""Button"",
                    ""id"": ""3be353d6-7868-4dae-8158-5fe49f7d6306"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""59757b19-db2c-410f-ab39-e8ff26b595c2"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Lock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6aab37db-6a5f-40f1-81bb-09e61a2a5295"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GiveUpBackPanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CharacterControls
        m_CharacterControls = asset.FindActionMap("CharacterControls", throwIfNotFound: true);
        m_CharacterControls_Move = m_CharacterControls.FindAction("Move", throwIfNotFound: true);
        m_CharacterControls_Run = m_CharacterControls.FindAction("Run", throwIfNotFound: true);
        m_CharacterControls_Jump = m_CharacterControls.FindAction("Jump", throwIfNotFound: true);
        m_CharacterControls_Look = m_CharacterControls.FindAction("Look", throwIfNotFound: true);
        m_CharacterControls_Skill1 = m_CharacterControls.FindAction("Skill1", throwIfNotFound: true);
        // ShortcutKey
        m_ShortcutKey = asset.FindActionMap("ShortcutKey", throwIfNotFound: true);
        m_ShortcutKey_GetPuppet = m_ShortcutKey.FindAction("GetPuppet", throwIfNotFound: true);
        // AfterSplitScreenShortCut
        m_AfterSplitScreenShortCut = asset.FindActionMap("AfterSplitScreenShortCut", throwIfNotFound: true);
        m_AfterSplitScreenShortCut_SplitScreen1 = m_AfterSplitScreenShortCut.FindAction("SplitScreen1", throwIfNotFound: true);
        m_AfterSplitScreenShortCut_SplitScreen2 = m_AfterSplitScreenShortCut.FindAction("SplitScreen2", throwIfNotFound: true);
        m_AfterSplitScreenShortCut_SplitScreen3 = m_AfterSplitScreenShortCut.FindAction("SplitScreen3", throwIfNotFound: true);
        // Interaction
        m_Interaction = asset.FindActionMap("Interaction", throwIfNotFound: true);
        m_Interaction_interact = m_Interaction.FindAction("interact", throwIfNotFound: true);
        // Always
        m_Always = asset.FindActionMap("Always", throwIfNotFound: true);
        m_Always_Exit = m_Always.FindAction("Exit", throwIfNotFound: true);
        // LockView
        m_LockView = asset.FindActionMap("LockView", throwIfNotFound: true);
        m_LockView_Lock = m_LockView.FindAction("Lock", throwIfNotFound: true);
        m_LockView_GiveUpBackPanel = m_LockView.FindAction("GiveUpBackPanel", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // CharacterControls
    private readonly InputActionMap m_CharacterControls;
    private List<ICharacterControlsActions> m_CharacterControlsActionsCallbackInterfaces = new List<ICharacterControlsActions>();
    private readonly InputAction m_CharacterControls_Move;
    private readonly InputAction m_CharacterControls_Run;
    private readonly InputAction m_CharacterControls_Jump;
    private readonly InputAction m_CharacterControls_Look;
    private readonly InputAction m_CharacterControls_Skill1;
    public struct CharacterControlsActions
    {
        private @L2PlayerInput m_Wrapper;
        public CharacterControlsActions(@L2PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_CharacterControls_Move;
        public InputAction @Run => m_Wrapper.m_CharacterControls_Run;
        public InputAction @Jump => m_Wrapper.m_CharacterControls_Jump;
        public InputAction @Look => m_Wrapper.m_CharacterControls_Look;
        public InputAction @Skill1 => m_Wrapper.m_CharacterControls_Skill1;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControlsActions set) { return set.Get(); }
        public void AddCallbacks(ICharacterControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_CharacterControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CharacterControlsActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @Skill1.started += instance.OnSkill1;
            @Skill1.performed += instance.OnSkill1;
            @Skill1.canceled += instance.OnSkill1;
        }

        private void UnregisterCallbacks(ICharacterControlsActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @Skill1.started -= instance.OnSkill1;
            @Skill1.performed -= instance.OnSkill1;
            @Skill1.canceled -= instance.OnSkill1;
        }

        public void RemoveCallbacks(ICharacterControlsActions instance)
        {
            if (m_Wrapper.m_CharacterControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICharacterControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_CharacterControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CharacterControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CharacterControlsActions @CharacterControls => new CharacterControlsActions(this);

    // ShortcutKey
    private readonly InputActionMap m_ShortcutKey;
    private List<IShortcutKeyActions> m_ShortcutKeyActionsCallbackInterfaces = new List<IShortcutKeyActions>();
    private readonly InputAction m_ShortcutKey_GetPuppet;
    public struct ShortcutKeyActions
    {
        private @L2PlayerInput m_Wrapper;
        public ShortcutKeyActions(@L2PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @GetPuppet => m_Wrapper.m_ShortcutKey_GetPuppet;
        public InputActionMap Get() { return m_Wrapper.m_ShortcutKey; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ShortcutKeyActions set) { return set.Get(); }
        public void AddCallbacks(IShortcutKeyActions instance)
        {
            if (instance == null || m_Wrapper.m_ShortcutKeyActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ShortcutKeyActionsCallbackInterfaces.Add(instance);
            @GetPuppet.started += instance.OnGetPuppet;
            @GetPuppet.performed += instance.OnGetPuppet;
            @GetPuppet.canceled += instance.OnGetPuppet;
        }

        private void UnregisterCallbacks(IShortcutKeyActions instance)
        {
            @GetPuppet.started -= instance.OnGetPuppet;
            @GetPuppet.performed -= instance.OnGetPuppet;
            @GetPuppet.canceled -= instance.OnGetPuppet;
        }

        public void RemoveCallbacks(IShortcutKeyActions instance)
        {
            if (m_Wrapper.m_ShortcutKeyActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IShortcutKeyActions instance)
        {
            foreach (var item in m_Wrapper.m_ShortcutKeyActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ShortcutKeyActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ShortcutKeyActions @ShortcutKey => new ShortcutKeyActions(this);

    // AfterSplitScreenShortCut
    private readonly InputActionMap m_AfterSplitScreenShortCut;
    private List<IAfterSplitScreenShortCutActions> m_AfterSplitScreenShortCutActionsCallbackInterfaces = new List<IAfterSplitScreenShortCutActions>();
    private readonly InputAction m_AfterSplitScreenShortCut_SplitScreen1;
    private readonly InputAction m_AfterSplitScreenShortCut_SplitScreen2;
    private readonly InputAction m_AfterSplitScreenShortCut_SplitScreen3;
    public struct AfterSplitScreenShortCutActions
    {
        private @L2PlayerInput m_Wrapper;
        public AfterSplitScreenShortCutActions(@L2PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @SplitScreen1 => m_Wrapper.m_AfterSplitScreenShortCut_SplitScreen1;
        public InputAction @SplitScreen2 => m_Wrapper.m_AfterSplitScreenShortCut_SplitScreen2;
        public InputAction @SplitScreen3 => m_Wrapper.m_AfterSplitScreenShortCut_SplitScreen3;
        public InputActionMap Get() { return m_Wrapper.m_AfterSplitScreenShortCut; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AfterSplitScreenShortCutActions set) { return set.Get(); }
        public void AddCallbacks(IAfterSplitScreenShortCutActions instance)
        {
            if (instance == null || m_Wrapper.m_AfterSplitScreenShortCutActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_AfterSplitScreenShortCutActionsCallbackInterfaces.Add(instance);
            @SplitScreen1.started += instance.OnSplitScreen1;
            @SplitScreen1.performed += instance.OnSplitScreen1;
            @SplitScreen1.canceled += instance.OnSplitScreen1;
            @SplitScreen2.started += instance.OnSplitScreen2;
            @SplitScreen2.performed += instance.OnSplitScreen2;
            @SplitScreen2.canceled += instance.OnSplitScreen2;
            @SplitScreen3.started += instance.OnSplitScreen3;
            @SplitScreen3.performed += instance.OnSplitScreen3;
            @SplitScreen3.canceled += instance.OnSplitScreen3;
        }

        private void UnregisterCallbacks(IAfterSplitScreenShortCutActions instance)
        {
            @SplitScreen1.started -= instance.OnSplitScreen1;
            @SplitScreen1.performed -= instance.OnSplitScreen1;
            @SplitScreen1.canceled -= instance.OnSplitScreen1;
            @SplitScreen2.started -= instance.OnSplitScreen2;
            @SplitScreen2.performed -= instance.OnSplitScreen2;
            @SplitScreen2.canceled -= instance.OnSplitScreen2;
            @SplitScreen3.started -= instance.OnSplitScreen3;
            @SplitScreen3.performed -= instance.OnSplitScreen3;
            @SplitScreen3.canceled -= instance.OnSplitScreen3;
        }

        public void RemoveCallbacks(IAfterSplitScreenShortCutActions instance)
        {
            if (m_Wrapper.m_AfterSplitScreenShortCutActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IAfterSplitScreenShortCutActions instance)
        {
            foreach (var item in m_Wrapper.m_AfterSplitScreenShortCutActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_AfterSplitScreenShortCutActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public AfterSplitScreenShortCutActions @AfterSplitScreenShortCut => new AfterSplitScreenShortCutActions(this);

    // Interaction
    private readonly InputActionMap m_Interaction;
    private List<IInteractionActions> m_InteractionActionsCallbackInterfaces = new List<IInteractionActions>();
    private readonly InputAction m_Interaction_interact;
    public struct InteractionActions
    {
        private @L2PlayerInput m_Wrapper;
        public InteractionActions(@L2PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @interact => m_Wrapper.m_Interaction_interact;
        public InputActionMap Get() { return m_Wrapper.m_Interaction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InteractionActions set) { return set.Get(); }
        public void AddCallbacks(IInteractionActions instance)
        {
            if (instance == null || m_Wrapper.m_InteractionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InteractionActionsCallbackInterfaces.Add(instance);
            @interact.started += instance.OnInteract;
            @interact.performed += instance.OnInteract;
            @interact.canceled += instance.OnInteract;
        }

        private void UnregisterCallbacks(IInteractionActions instance)
        {
            @interact.started -= instance.OnInteract;
            @interact.performed -= instance.OnInteract;
            @interact.canceled -= instance.OnInteract;
        }

        public void RemoveCallbacks(IInteractionActions instance)
        {
            if (m_Wrapper.m_InteractionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInteractionActions instance)
        {
            foreach (var item in m_Wrapper.m_InteractionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InteractionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InteractionActions @Interaction => new InteractionActions(this);

    // Always
    private readonly InputActionMap m_Always;
    private List<IAlwaysActions> m_AlwaysActionsCallbackInterfaces = new List<IAlwaysActions>();
    private readonly InputAction m_Always_Exit;
    public struct AlwaysActions
    {
        private @L2PlayerInput m_Wrapper;
        public AlwaysActions(@L2PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Exit => m_Wrapper.m_Always_Exit;
        public InputActionMap Get() { return m_Wrapper.m_Always; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AlwaysActions set) { return set.Get(); }
        public void AddCallbacks(IAlwaysActions instance)
        {
            if (instance == null || m_Wrapper.m_AlwaysActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_AlwaysActionsCallbackInterfaces.Add(instance);
            @Exit.started += instance.OnExit;
            @Exit.performed += instance.OnExit;
            @Exit.canceled += instance.OnExit;
        }

        private void UnregisterCallbacks(IAlwaysActions instance)
        {
            @Exit.started -= instance.OnExit;
            @Exit.performed -= instance.OnExit;
            @Exit.canceled -= instance.OnExit;
        }

        public void RemoveCallbacks(IAlwaysActions instance)
        {
            if (m_Wrapper.m_AlwaysActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IAlwaysActions instance)
        {
            foreach (var item in m_Wrapper.m_AlwaysActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_AlwaysActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public AlwaysActions @Always => new AlwaysActions(this);

    // LockView
    private readonly InputActionMap m_LockView;
    private List<ILockViewActions> m_LockViewActionsCallbackInterfaces = new List<ILockViewActions>();
    private readonly InputAction m_LockView_Lock;
    private readonly InputAction m_LockView_GiveUpBackPanel;
    public struct LockViewActions
    {
        private @L2PlayerInput m_Wrapper;
        public LockViewActions(@L2PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Lock => m_Wrapper.m_LockView_Lock;
        public InputAction @GiveUpBackPanel => m_Wrapper.m_LockView_GiveUpBackPanel;
        public InputActionMap Get() { return m_Wrapper.m_LockView; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(LockViewActions set) { return set.Get(); }
        public void AddCallbacks(ILockViewActions instance)
        {
            if (instance == null || m_Wrapper.m_LockViewActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_LockViewActionsCallbackInterfaces.Add(instance);
            @Lock.started += instance.OnLock;
            @Lock.performed += instance.OnLock;
            @Lock.canceled += instance.OnLock;
            @GiveUpBackPanel.started += instance.OnGiveUpBackPanel;
            @GiveUpBackPanel.performed += instance.OnGiveUpBackPanel;
            @GiveUpBackPanel.canceled += instance.OnGiveUpBackPanel;
        }

        private void UnregisterCallbacks(ILockViewActions instance)
        {
            @Lock.started -= instance.OnLock;
            @Lock.performed -= instance.OnLock;
            @Lock.canceled -= instance.OnLock;
            @GiveUpBackPanel.started -= instance.OnGiveUpBackPanel;
            @GiveUpBackPanel.performed -= instance.OnGiveUpBackPanel;
            @GiveUpBackPanel.canceled -= instance.OnGiveUpBackPanel;
        }

        public void RemoveCallbacks(ILockViewActions instance)
        {
            if (m_Wrapper.m_LockViewActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ILockViewActions instance)
        {
            foreach (var item in m_Wrapper.m_LockViewActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_LockViewActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public LockViewActions @LockView => new LockViewActions(this);
    public interface ICharacterControlsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnSkill1(InputAction.CallbackContext context);
    }
    public interface IShortcutKeyActions
    {
        void OnGetPuppet(InputAction.CallbackContext context);
    }
    public interface IAfterSplitScreenShortCutActions
    {
        void OnSplitScreen1(InputAction.CallbackContext context);
        void OnSplitScreen2(InputAction.CallbackContext context);
        void OnSplitScreen3(InputAction.CallbackContext context);
    }
    public interface IInteractionActions
    {
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface IAlwaysActions
    {
        void OnExit(InputAction.CallbackContext context);
    }
    public interface ILockViewActions
    {
        void OnLock(InputAction.CallbackContext context);
        void OnGiveUpBackPanel(InputAction.CallbackContext context);
    }
}
