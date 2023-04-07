# dragon-core

Requires [Odin](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) from AssetStore.

How to install;

1. Create a folder named "Framework" inside your Asset/ path.
2. Clone or copy the repository to Asset/Framework/dragon-core.

Some parts of documentation included here, but for more complete documentation, see the link below.

[Documentation](https://egehantolunay.atlassian.net/l/cp/ioX1fhNi)

High Level UML Diagram

![highleveluml](https://user-images.githubusercontent.com/52652529/224702615-f3c8eefb-d436-4907-9a8c-8c47cd204e5b.png)

# Intro to Actors

Actors are responsible for executing the primary logic. They associate themselves with IDataContext and IEventContext.

Actors can also have DataField fields, which can be used to extract data from the associated context.

Extending an actor should only be considered if there is concrete evidence that the newly implemented actor will have a significant performance or convenience impact. This process must only be considered during the later stages of the development process when all elements have found their place. This is because actors are designed to be used by their named type rather than by installed bindings. Before extending an actor, it is important to consider implementing any kind of extension by ActorService.

Most of the time, you will be using SimpleActor, which only results in running the listening services. Alternatively, you can use CoreActor, which has a state-oriented approach.

In the case of CoreActor, during initialization, it will first initialize the associated contexts. Then, it will fire up events that can be used for modularity reasons.

```
[Button][ShowIf("ShowInitButton")]
public void InitializeIfNot()
{
    if (_isInitialized) return;

    if (_setSceneReference)
    {
        _sceneInstall.InstalledValue = this;
        _sceneInstall.InstallFor(ContextRegistry.GetContext(gameObject.scene.name).As<IDataContext>());
    }
    _goPool = DataRegistry<IGOInstancePoolRegistry>.GetData(null);
    OnBeforeContextsInitialize();
    _dataContext = _dataContextObject as IDataContext;
    _dataContext.onDestroyContext += OnDestroyDataContext;
    _dataContext.InitializeIfNot();
    _dataContext.SetData(this as IActor);
    OnAfterActorInstalledItself();
    
    _eventContext = _eventContextObject as IEventContext;
    _eventContext.InitializeIfNot();
    OnAfterContextsInitialized();
    _isInitialized = true;
    onInitialize?.Invoke(this);
}
```

At the 16th line, the actor sets the IActor type of data, using itself, for the data context. This installs the Actor as a singleton in the associated context. Subsequently, this can be used to obtain the Actor of this Context if the only thing we have is the context, without requiring any GetComponent operation or casting.

It's important to note that all these public functions must be called from outside. An actor cannot initialize or run itself. It will only assign itself to a group on Awake.

```java
 protected void Awake()
{
    foreach (Key group in _groups)
    {
        if (!DataRegistry<List<IActor>>.ContainsData("Global/"+group.ID))
        {
            DataRegistry<List<IActor>>.SetData(null,new List<IActor>(),group.ID);
        }

        DataRegistry<List<IActor>>.GetData(null, group.ID).Add(this);
    }
}
```

By pulling these group data from other actors and their states, we can obtain a list of actors and initialize them.

Group examples include the Player group, Enemy group, LevelFlow group, CameraHandling group, and others.

However, every process needs a starting point, which is where the FirstSpark script comes in.

The implementation of the Start function inside the FirstSpark MonoBehaviour is as follows:

```java
private IEnumerator Start()
{
    yield return null;
    Application.targetFrameRate = 60;
    _actor.InitializeIfNot();
    _actor.BeginIfNot();
}
```

Typically, the FirstSpark actor will trigger the running of other actors, such as LevelFlow, and LevelFlow will likely trigger the Player, Enemy, and other actors. This approach allows for trackable execution based on the confirmed status of the application.

Additionally, CoreActor has a RunningState, which will be entered when BeginIfNot is called and exited when StopIfNot is called.

For more information on States, please refer to the pages about states.

Additionally, actors can be finished or cancelled. This status is implemented for logic that runs while the actor's job is finished but still has remaining cleanup processes or fade outs. For example, audio or particle effects stopping after one second and smoothly fading out instead of being abruptly cut off.

You can enable this behavior by unticking the \_stopOnFinish boolean. However, now you have to manage stopping the actor on its finish.

# Intro to Contexts

This is a type of structure that defines the scope of an event or data. It does this by associating itself with DataRegistry<T> and EventRegistry classes.

As you can see below, DataRegistry has a \_dictionary with a string key and a T value, which is the type of the object we want to hold

```
public static class DataRegistry<T>
{
    private static readonly Dictionary<string, T> _dictionary = new Dictionary<string, T>();
    public static Dictionary<string, T> Dictionary => _dictionary;
```

The string value is concatenated using the formula below:

```java
_stringBuilder.Append(contextID);
_stringBuilder.Append("/");
_stringBuilder.Append(key);
```

Each context must implement the ContextID string. In most cases, if this is a Unity object, it will be its instance ID.

```java
public interface IContext : ICastable
{
    string ContextID { get; }
    void InitializeIfNot();
    event Action<IContext> onDestroyContext;
}
```

Here is an example of the implementation inside a concrete context:

```java
    [ShowInInspector] [ReadOnly] private string _instanceID;
    public string ContextID => _instanceID;
```

We set the \_instanceID on initialization:

`_instanceID = _isSceneContext ? gameObject.scene.name : GetInstanceID().ToString();`

We use our DataInstallers to install the data to the DataRegistry based on the given context. Here is an example of a method inside a DataInstaller<T>:

```java
public void InstallFor(IDataContext main)
{
    string key = Key ? Key.ID : "";
    if (InstalledValue == null && InstalledValue is not UnityEngine.Object)
    {
        InstalledValue = Activator.CreateInstance<T>();
    }
    DataRegistry<T>.SetData(main,InstalledValue,key);
}
```

These installers are stored inside the DataInstallerGroup.

```java
public abstract class DataInstallerGroup : MonoBehaviour, IDataInstaller
{
    public void InstallFor(IDataContext dataContext)
    {
        foreach (IDataInstaller installer in GetInstallers())
        {
            installer.InstallFor(dataContext);
        }
    }

    protected abstract IEnumerable<IDataInstaller> GetInstallers();
}
```

Now we have to implement the GetInstallers method.

By overriding this class (which is a MonoBehaviour) and assigning it to the installer method field on the concrete data context, we can ensure that our data will be installed properly during initialization.

Here is an example:

```java
public class DIG_String : DataInstallerGroup
{
    [SerializeField] private DataInstaller<string> _string;
    protected override IEnumerable<IDataInstaller> GetInstallers()
    {
        yield return _string;
    }
}
```

In this example, we will be installing a string data. Whether this is a Singleton install or a Key-based install depends on the setup in the editor.

To summarize up to this point:

We have a group of data installers that we have configured to address how a data gets installed. We then slot this group into the concrete data context, which will call the InstallFor method during initialization. This results in the data being installed on the DataRegistry with a string formula.

To retrieve the data, see the states, services, and actor sections.
  
# Intro to Services
  
  Services, like states, are also used for logic running. So, what's the difference?

The difference is that services are designed to be used for the logic that will exist for the lifetime of an Actor.

Because of this, they are also, by design, allowed to install themselves to the context as behaving like data.

They can both hold data and run logic.

For example, take the MovementService.

```java
public class AS_MovementHandler : ActorService
{
    [ShowInInspector]
    [ReadOnly]
    private float _groundDistance;
    [SerializeField] private float _groundCheckDistance = 100f;
    [SerializeField] private float _groundedDistance = 0.3f;
    [SerializeField] private float _gravityTowardsGround = -9.81f;
    [SerializeField] private DataField<D_MovementHandler> _movementSettings;
    [SerializeField] private DataField<D_MovementStatus> _movementStatus;
    [SerializeField] private ConfiguredUpdateField _moveUpdate;

    private CapsuleCollider CapsuleCollider => _movementStatus.Data.CapsuleCollider;
    private float ExternalForceDecayRate => _movementSettings.Data.ExternalForceDecayRate;
    private float ExternalForceContributionFactor => _movementSettings.Data.ExternalForceContributionFactor;
    private float MaxSlopeAngle => _movementSettings.Data.MaxSlopeAngle;
    private LayerMask GroundLayerMask => _movementSettings.Data.GroundLayerMask;
```

…..

```java
public void Move()
{
    CheckGround();
    Vector3 targetVelocity = _movementStatus.Data.DesiredMoveVelocity;

    CalculateExternalForces();

    if (IsOnWalkableGround)
    {
        Vector3 resultingVelocity = Vector3.Lerp(targetVelocity, targetVelocity + _externalForce, ExternalForceContributionFactor);
        Vector3 characterUp = Actor.transform.up;
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(resultingVelocity, characterUp);
        Vector3 reprojected = Quaternion.FromToRotation(characterUp,GroundNormal)*lateralVelocity;

        Rigidbody.velocity = reprojected;
    }
    else
    {
        Vector3 resultingVelocity = Vector3.Lerp(targetVelocity, targetVelocity + _externalForce, ExternalForceContributionFactor);
        
        Rigidbody.velocity = resultingVelocity + Vector3.up * _gravityTowardsGround;
    }

    _movementStatus.Data.CurrentPhysicsVelocity = Rigidbody.velocity;
    _movementStatus.Data.VelocityOutput = Rigidbody.velocity;

    _externalForceThisFrame = Vector3.zero;
}
```

They are pulling data, they have their own data, and they are also an extend-inheritance implementation.

Too powerful.

Yes, but the keyword 'lifetime' is very important. This means that in any of the time or the majority of the time of an actor's lifetime, this service will be used. This is why it's a service.

We could definitely implement this behavior by making a state, but we would have to deal with event configurations and on-state data that makes things just complicated and not as simple or modular.

By making this service, we can register it to the context and get it from states to use or change its behavior and data. Alternatively, it can have its own always running behavior.

The ActorService approach can also be used to move development faster in the early stages without needing to implement states and worry about their configurations.

In later stages, their behaviors can be separated into states or turned into a whole Actors.

They are jacks of all trades and will make things move faster, as long as you keep it to the rule of an actor's lifetime.
  
# Intro to States

  States are the most usable and modular way of implementing any kind of logic.

By their design, they cannot be reached by their implemented type.

Their design is to be override-inheritance.

States can only be entered by giving a scope of actor, as seen below:

```java
public void CheckoutEnter(IActor actor)
{
    if (_isRunning) return;
    _isRunning = true;
    _isFinished = false;
    InsertActor(actor);
    OnGetData();
    OnEnter();
}
```

The InsertActor method will get the DataContext and EventContext and set it to be used by the state.

OnGetData is a virtual method so that you can override it to get your initial data when it's appropriate.

OnEnter is where you implement enter logic.

OnExit is where you implement exit logic.

Here is an example for a state

```
public class State_SetCameraState : MonoActorState
{
    [SerializeField] private DataField<string> _cameraState;
    [SerializeField] private string _aimCameraState = "Shooting";
    [SerializeField] private bool _onExitSetPreviousState = true;
    private string _previousCameraState;
    
    protected override void OnGetData()
    {
        _cameraState.Get(DataContext);
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _previousCameraState = _cameraState.Data;
        _cameraState.Set(DataContext,_aimCameraState);
    }

    protected override void OnExit()
    {
        base.OnExit();
        if (_onExitSetPreviousState)
        {
            _cameraState.Set(DataContext,_previousCameraState);
        }
    }
}
```

In this example, we have a DataField<string> that we will use to pull data from the assigned DataContext provided by the actor that has entered.

We also have on-state or transient data like \_aimCameraState and \_onExitSetPreviousState.

OnGetData, we are retrieving the current value of data by \_cameraState.Get(DataContext).

OnEnter, we are setting it to be used as the previous state of the camera. We then assign the new camera state by using the Set function of DataField<string>.

OnExit, we are returning to the previous state only if a boolean is true.

We could register to an Update, FixedUpdate, or any other event on enter and then unregister on exit. The possibilities with combinations are limitless.

This is how States work. Only they know what they need and what they should listen to. Nothing outside should reach them and change their internal behavior.

For more information on DataFields, check its page.

# Common state types

We have some state types already implemented for certain use cases;

GOMultiState
============

The most common state you will see is GOMultiState.

![](attachments/65960/393335.png)

It will detect every state on the GameObject that it sits on and enter/exit them when it gets entered/exited.

![](attachments/65960/33248.png)

As we can see above, two states underneath GOMultiState will be entered/exited when it gets entered/exited.

State\_Redirect
===============

We also have a state that lets us redirect enter/exit to another state - State\_Redirect.

This is useful when using modular prefabs that have a group of states that need to run exactly in the order they are configured.

![](attachments/65960/360570.png)

Here we can see the combination of GOMultiState and State\_Redirect.

Before running State\_DesiredFacingCalculation, a group of states that sit on MaintainCharacter GameObject will run.

![](attachments/65960/33260.png)

As you can see, there are a bunch of configured states here that will run before `State_DesiredFacingCalculation`, if you remember from above.

So the new execution order will be:

`State_OnSlotAvailableRequestRunActor`, `State_CharacterActorSetupOperations`, and then finally, out of the redirect, we continue with `State_DesiredVelocityCalculation`.

We also have the infamous FSMs;

StateMachineMonoActorState
==========================

For more information about state machine logic, please refer to its dedicated page. Here, we will only discuss how to implement it in an easy way.

```java
public class FSM_Living : StateMachineMonoActorState
{
    private D_Living Living => _living.Data;
    [InfoBox("This state also manages data.")]
    [SerializeField] private DataField<D_Living> _living;
    [SerializeField] private StateField _initialState;
    [SerializeField] private StateField _revive;
    [SerializeField] private StateField _alive;
    [SerializeField] private StateField _dying;
    [SerializeField] private StateField _dead;

    protected override IActorState InitialState => _initialState.State;
```

We start by defining a DataField that we will be using, in this case we are using D\_Living.

Multiple DataFields can be defined based on specific requirements.

Then we define StateFields, which are used to store states configured in the editor. For more information about StateFields, see its page.

After that, we override InitialState to configure it. Conditional approaches can also be used here, but it is generally recommended to support an initial state.

```java
protected override void OnGetData()
{
    base.OnGetData();
    _living.Get(DataContext);
}

protected override void OnEnter()
{
    base.OnEnter();
    _living.Data.onLivingDataChanged += OnLivingDataChanged;
    Evaluate();
}

protected override void OnExit()
{
    base.OnExit();
    _living.Data.onLivingDataChanged -= OnLivingDataChanged;
}

private void OnLivingDataChanged(D_Living obj)
{
    Evaluate();
}
```

We continue fairly simple as you can see.

We register for data change updates of Living data, and when it changes we tell FSM to evaluate transitions.

But we also have to configure transitions.

```java
protected override void OnGetTransitions()
{
    AddDirectTransition(_initialState.State,_alive.State,ConditionToStartAlive);
    AddDirectTransition(_initialState.State,_dead.State,ConditionToStartDead);
    AddDirectTransition(_alive.State,_dying.State,ConditionFromAliveToDying);
    AddDirectTransition(_dying.State,_dead.State,ConditionFromDyingToDead);
    AddDirectTransition(_dead.State,_revive.State,ConditionFromDeadToRevive);
    AddDirectTransition(_revive.State,_alive.State,ConditionFromReviveToAlive);
}

private bool ConditionToStartAlive()
{
    return Living.IsAlive;
}
private bool ConditionToStartDead()
{
    return Living.IsDead;
}
private bool ConditionFromAliveToDying()
{
    bool shouldDie = Living.ShouldDieTrigger;
    if (shouldDie)
    {
        Living.ShouldDieTrigger = false;
        Living.IsDying = true;
    }
    return shouldDie;
}
```

Above, you can see the override implementation of OnGetTransitions().

We use the AddDirectTransition method to add a FROM → TO transition with a Func<bool> that acts as a condition.

You can also use AddAnyTransition to add an ANY → TO transition with a condition.

The difference between any and direct is that any conditions will always be evaluated regardless of the current state, and direct transitions will only be evaluated when the current state is the FROM state.

For fast branching state logic, you can also use:

BranchMonoActorState
====================

```java
public class ActorTagBranchState : BranchMonoActorState
{
    [SerializeField] private TagCheck _tagCheck;
    [SerializeField] private StateField _hasTag;
    [SerializeField] private StateField _noTag;
    
```

Same as FSM we have StateFields \_hasTag and \_noTag;

```java
protected override void OnInitialize()
    {
        _hasTag.InitializeIfNeedsInitialize();
        _noTag.InitializeIfNeedsInitialize();
    }

    protected override IActorState InitialState
    {
        get
        {
            if (_tagCheck.ContainsTag(Actor))
            {
                return _hasTag.State;
            }
            else
            {
                return _noTag.State;
            }
        }   
    }
```

Unlike FSMs, we also have to make sure that we initialize states. This is because, with FSMs, when adding a transition, they automatically get initialized. But with a more free approach like this, we have to do it ourselves.

We override InitialState with a conditional approach too.

The initial state will run when first entered, and this logic will depend on TagCheck's boolean result.

```java
protected override void OnEnter()
    {
        base.OnEnter();
        StaticUpdate.onUpdate += OnUpdate;
    }

    protected override void OnExit()
    {
        StaticUpdate.onUpdate -= OnUpdate;
        base.OnExit();
    }

    private void OnUpdate()
    {
        if (!IsRunning) return;
        if (_tagCheck.ContainsTag(Actor))
        {
            SwitchStateIfNotEntered(_hasTag.State);
        }
        else
        {
            SwitchStateIfNotEntered(_noTag.State);
        }
    }
```

And then we just register to update OnEnter to do our evaluations. We use the protected SwitchStateIfNotEntered method, whose logic is as follows:

```java
if (_currentState == toState) return;
if (_currentState != null)
{
    _currentState.CheckoutExit();
}
_currentState = toState;
if (_currentState != null)
{
    _currentState.CheckoutEnter(Actor);
}
```

As you can see, if our current state is already the one that we have given, it won't process it. For more complex logics, use FSMs instead of this. For even more strictly boolean switches, we can use:

ExplicitBooleanBranchMonoActorState
===================================

```java
public class State_BranchHasHostile : ExplicitBooleanBranchMonoActorState
{
    [SerializeField] private DataField<D_HostileMind> _hostileMind;

    public override bool Boolean => _hostileMind.Data.Hostile != null;
    public override event Action onChangeEvent
    {
        add => _hostileMind.Data.onHostileChanged += (a,b,c) => value.Invoke();
        remove => _hostileMind.Data.onHostileChanged -= (a,b,c) => value.Invoke();
    }

    protected override void OnGetData()
    {
        base.OnGetData();
        _hostileMind.Get(DataContext);
    }
}
```

This is a very simple and templated class.

We only need to override the Boolean that it will check and specify when it will check, and that's about it.
  
# Actor using an Actor

CASS relies on knowing the running status based on data.

Imagine each branch of the state being a table of a control that will affect an aspect of an Actor.

We approach from clearly defined tables. Consider this branch:

![393383](https://user-images.githubusercontent.com/52652529/230637473-0959056d-b81b-4b5f-b1f2-dad67f008eaa.png)

MaintainCharacter is a “table” that deals with character maintenance for the Player actor. When State\_OnSlotAvailableRequestRunActor is called, this Character aspect is handed over to another actor, shown below:

![33303](https://user-images.githubusercontent.com/52652529/230637509-ffd0e5d3-037e-4808-b36c-ec4cd2de6d6c.png)

And when that Actor runs, let's see what happens:

![66027](https://user-images.githubusercontent.com/52652529/230637527-91081cff-4bda-43cd-b20a-007b2bef1823.png)

It has some services. Remember what services do? They manage some sort of logic for the lifetime of an Actor.

![33311](https://user-images.githubusercontent.com/52652529/230637546-540dc259-c47e-460f-9624-962e5857568f.png)

A character actor can have a ragdoll behaviour, so when our Root actor hands over control of its character aspect to this actor, a service under it manages the ragdoll behaviour aspect. Nice!

So, how does this work? Let's take a look at how State\_OnSlotAvailableRequestRunActor behaves.

```java
public class State_OnSlotAvailableRequestRunActor : MonoActorState
{
    [SerializeField] private DataField<AS_ActorRunner> _actorRunner;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private List<SlotOccupationInfo> _slots;
    [SerializeField] private bool _onExitCancelRunning = true;
    [SerializeField] private DataKey _relationKey;
```

If you remember, we can pull services like data (we can actually pull pretty much anything but it's not recommended).

We pull the AS\_ActorRunner service, which handles actor running operations. It consists of slot occupying/overriding logic and also it will bookkeep what is running and what they are.

For more information about its internal operations, check its page.

What we need to focus on here is this one function:

```java
private void TryRunActor()
{
    if (_finishFrame == Time.frameCount) return;
    Debug.Log("Try run " + _prefab.name);
    if (_isActorRunning)
    {
        return;
    }
    Debug.Log("_isActorRunning passed");
    if (!_actorRunner.Data.CheckSlotsEmpty(_slots))
    {
        return;
    }
    Debug.Log("slot check passed");

    if (Actor.IsBeingDestroyed) return;
    ActorRunResult runResult = _actorRunner.Data.RequestRunning(new ActorRunningArgs()
    {
        DoNotParentToUser = false,
        UsageRequestID = "State_OnSlotAvailableApplyLocomotion",
        OccupationInfos = _slots,
        PrefabOrInstance = _prefab,
        RelationKey = _relationKey
    });
    _runningActor = runResult.RunningInstance;
    _isActorRunning = runResult.IsSuccess;
    Debug.Log("run result = " + _isActorRunning);
}
```

At the start, we are trying not to restart an actor in the same frame that it finished because it will result in a looping behavior.

If we are already running an actor, we also prevent re-runs.

We also check ourselves if the slots are empty.

Also, we don’t want to create any new actors when our root one is being destroyed.

And then we call the RequestRunning method inside the AS\_ActorRunner service.

We have a couple of things to set here;

`UsageRequestID = "State_OnSlotAvailableApplyLocomotion",`

This is used for identifying who/why/when an actor is actually being requested. We can implement a logic that can intercept and recognize certain request IDs and perform some specific actions as well.

`OccupationInfos = _slots,`

Which slots will this Actor run operation occupy? You can think of these slots as the aspects we discussed earlier, or as something more unique for your specific situation. Use your best judgment here.

`PrefabOrInstance = _prefab,`

We can either give a poolable prefab, or an instance ready to be used.

This is useful when you want to override certain prefabs within a prefab and then use them without creating new prefabs for each configuration.

`RelationKey = _relationKey`

Remember our DataField addresses? There was this thing called relative. If we provide a relation key, ActorRunner will register this new actor's context with that key to our context.

Now, we can access the character's data from our root object.

```java
protected override void OnExit()
{
    base.OnExit();
    if (_onExitCancelRunning)
    {
        if (_isActorRunning)
        {
            _runningActor.CancelIfNotEnded("State_OnSlotAvailableApplyLocomotion");
            //This should trigger top side OnActorEnded.
        }
    }
    _actorRunner.Data.onUsedActorEnded -= OnActorEnded;
}
```

You must always clean up your table, so on exit you should not let the actor continue running.

Because our root actor is telling that it no longer wants to give you that control.

# An Example

Let’s look at this system by giving an example that is actually implemented

We will look at 3 Actors, these are Root Player actor, Locomotion Runner actor, Grounded Locomotion actor.

![393442](https://user-images.githubusercontent.com/52652529/230637727-44db2750-c224-462f-ae2d-b9b88c95031e.png)

Consider this actor hierarchy of Root player actor

First three things; Services, Data, and Running these are it’s absolute internal components.

![360734](https://user-images.githubusercontent.com/52652529/230637760-8afcbba5-d0ef-46d2-ad43-ca4f7bfdaa95.png)

As you can see data holds large number of DataInstallerGroups, MovementHandler data, MovementStatus data, and many more

Some are installed as “Single” some are assigned with a key like RunningDictionary.

Lets look at its services;

![393454](https://user-images.githubusercontent.com/52652529/230637833-2d05429c-eea5-42f0-b6cd-aa269c0225c0.png)

MovementHandler, ActorRunner, ConfiguredUpdateHandler, bunch of services.

Remember that services can hold both logic and data, and extended behaviours that states can use.

And running which will be containing the running state

![66151](https://user-images.githubusercontent.com/52652529/230637861-bc0cb23a-bcdc-4716-95b2-c03c1543b1b6.png)

As we can see, we are redirecting some states to other gameobjects.

Lets focus on FSMLiving;

![393462](https://user-images.githubusercontent.com/52652529/230637889-883dfa79-521f-45eb-8698-c8e44b20ad31.png)

We have a living fsm here with 5 states, Let’s continue with Alive, which is the most probable place for any locomotion logic.

![66159](https://user-images.githubusercontent.com/52652529/230637914-005832f7-ab96-4312-8353-44253a239ef3.png)

As we can see we are again redirecting here to TryStartLocomotion object. Keep in mind that we are filtering out our control of other parts of this actor as we go deeper in the tree. Let’s move to TryStartLocomotion;

![360744](https://user-images.githubusercontent.com/52652529/230637953-f1a9a0d7-9c88-4441-b4e3-5b3cfadc9eed.png)

Here we go we are handing over our control for locomotion to A\_UA\_LocRun\_Set1 actor prefab.

Our actor when runs, will override-occupy BaseBody slot. Meaning it will remove anything running.

But only when its completely free, so actually it doesn’t matter what slot configuration we have here for this.

As we can see we are pulling ActorRunner service, that is installed to our context by having \[Context, Self\] options. And we are pulling the one that is installed as Single, and not by any key. Remember, we had this script in “Services” GameObject.

Let’s move over to locomotion runner actor;

![360750](https://user-images.githubusercontent.com/52652529/230638006-50186c35-e235-4d9b-bd14-e7188f9c9801.png)

Not every actor will be a huge one, this one is very simple lets look at its Running;

![33445](https://user-images.githubusercontent.com/52652529/230638025-d37a9803-d702-4e23-bb73-169e13f81679.png)

The State\_LocomotionTypeSwitch will use two slotted Airborne and Grounded actor prefabs based on isOnWalkableGround boolean inside

```java
private void UpdateRunningLocomotionType()
{
    if (_currentInstance != null)
    {
        _currentInstance.CancelIfNotEnded("State_LocomotionSwitch");
        _currentInstance = null;
    }
    if (_movementStatus.Data.IsOnWalkableGround)
    {
        var result = _actorRunner.Data.RequestRunning(new ActorRunningArgs()
        {
            OccupationInfos = _slots,
            DoNotParentToUser = false,
            PrefabOrInstance = _groundedLocomotion,
            UsageRequestID = "State_LocomotionSwitch"
        });
        _currentInstance = result.RunningInstance;
    }
    else
    {
        var result = _actorRunner.Data.RequestRunning(new ActorRunningArgs()
        {
            OccupationInfos = _slots,
            DoNotParentToUser = false,
            PrefabOrInstance = _airborneLocomotion,
            UsageRequestID = "State_LocomotionSwitch"
        });
        _currentInstance = result.RunningInstance;
    }
}
```

We first cancel current running, and then start new one as you can see.

Let’s look at something very important,

![66169](https://user-images.githubusercontent.com/52652529/230638061-40fba762-5734-4e9b-9e68-c1b690b39df9.png)

Consider the Single,Context,ROOT and Single,Context,PARENT

Actually both are, in our current hierarchy, pointing to the same context. Which is player root.

Now let's look at Grounded Locomotion,
  
![360756](https://user-images.githubusercontent.com/52652529/230638135-6814f3e6-887c-4623-9bcc-31005d09128a.png)

![66177](https://user-images.githubusercontent.com/52652529/230638090-3f1d7fd4-0718-46f8-a1f1-e7f708342cee.png)

You can see that we installing 2 data to Grounded locomotion actor’s data context.

We use these for controlling animations

Let's look at Running;

![33457](https://user-images.githubusercontent.com/52652529/230638178-b904d667-b50a-4d59-b718-ab0803ce8082.png)

Okay, two states are running here: one is calculating the desired velocity based on input data that sits on our root, which is the player root actor, and the other is controlling the animation based on MovementData on the root and our character actor. Let's focus on that:

![66185](https://user-images.githubusercontent.com/52652529/230638210-d6e75b6e-92ed-47f6-9a45-1e6ff83a8969.png)

Single,Context,Relative ( Root → Stack of \[Character\] )

![360766](https://user-images.githubusercontent.com/52652529/230638233-aef277e2-231b-4f9c-9fa6-196cd575a6c4.png)

So, we go to our root and from there, search for the character. Since this stack contains only one element, we are done searching, and we return that context to be used to get the D\_AnimatedCharacter data, which contains AnimancerComponent and Animator references.

We then use them in this fully modular locomotion actor to handle an aspect of this complete structure. Now, think about having attack abilities, stun disables, knockbacks, etc. Anything is possible.
