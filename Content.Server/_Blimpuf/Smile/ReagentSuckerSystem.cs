using System.Linq;
using System.Numerics;
using Content.Server.Actions;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Gravity;
using Content.Server.Popups;
using Content.Shared._Blimpuf.Smile;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Projectiles;
using Content.Shared.Vapor;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.Smile;

    public sealed partial class ReagentSuckerSystem : EntitySystem
    {
        [Dependency] private readonly ActionsSystem _actionsSystem = default!;
        [Dependency] private readonly PuddleSystem _puddleSystem = default!;
        [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
        [Dependency] private readonly EntityLookupSystem _lookupSystem = default!;
        [Dependency] private readonly TransformSystem _transformSystem = default!;
        [Dependency] private PopupSystem _popup = default!;
        [Dependency] private readonly SharedTransformSystem _transform = default!;
        [Dependency] private readonly VaporSystem _vapor = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly ContainerSystem _container = default!;
        [Dependency] private readonly GravitySystem _gravity = default!;
        [Dependency] private readonly PhysicsSystem _physics = default!;
        [Dependency] private readonly IPrototypeManager _proto = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
        [Dependency] private readonly SharedProjectileSystem _projectile = default!;

        private float _gridImpulseMultiplier;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ReagentSuckerComponent, SuckUpLiquid>(OnSuckUpLiquid);
            SubscribeLocalEvent<ReagentSuckerComponent, EmptyLiquid>(OnEmptyLiquid);
            SubscribeLocalEvent<ReagentSuckerComponent, SprayLiquid>(OnSprayLiquid);
            SubscribeLocalEvent<ReagentSuckerComponent, MapInitEvent>(OnMapInit);
            SubscribeLocalEvent<ReagentSuckerComponent, SuckUpLiquidDoAfterEvent>(OnDoAfterSuckUp);
            SubscribeLocalEvent<ReagentSuckerComponent, EmptyLiquidDoAfterEvent>(OnDoAfterEmpty);
            Subs.CVar(_cfg, CCVars.GridImpulseMultiplier, UpdateGridMassMultiplier, true);
        }
        private void UpdateGridMassMultiplier(float value) => _gridImpulseMultiplier = value;

        private void OnMapInit(EntityUid uid, ReagentSuckerComponent component, MapInitEvent args)
        {
            _actionsSystem.AddAction(uid, ref component.SuckUpLiquidEntity, component.SuckUpLiquid);
            _actionsSystem.AddAction(uid, ref component.EmptyLiquidEntity, component.EmptyLiquid);
            _actionsSystem.AddAction(uid, ref component.SprayLiquidEntity, component.SprayLiquid);
        }
        private void OnSuckUpLiquid(EntityUid uid, ReagentSuckerComponent component, SuckUpLiquid args)
        {
            var @event = new SuckUpLiquidDoAfterEvent();
            // time it takes to activate ability: TimeSpan.FromSeconds(X) X = number of seconds
            var doAfter = new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(6), @event, uid)
            {
                BreakOnDamage = true,
                BreakOnMove = true
            };
            var mapCoords = _transformSystem.GetMapCoordinates(uid);
            var puddles = _lookupSystem.GetEntitiesInRange<PuddleComponent>(mapCoords, component.Radius);
            if (!puddles.Any())
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-suck-liquids-failure-message-user"), uid, uid);
                return;
            }
            if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var tankSolution))
                return;
            var tank = tankSolution.Value.Comp.Solution;
            var availableSpace = tank.MaxVolume - tank.Volume;
            if (availableSpace <= FixedPoint2.Zero)
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-suck-liquids-full-message-user"), uid, uid);
                return; // Your storage tank is totally full
            }
            _doAfter.TryStartDoAfter(doAfter);
        }
        private void OnDoAfterSuckUp(EntityUid uid, ReagentSuckerComponent component, SuckUpLiquidDoAfterEvent args)
        {
            if (args.Cancelled || args.Handled)
                return;

            if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var tankSolution))
                return;
            var tank = tankSolution.Value.Comp.Solution;
            // 2. Find all puddles within the defined range
            var mapCoords = _transformSystem.GetMapCoordinates(uid);
            var puddles = _lookupSystem.GetEntitiesInRange<PuddleComponent>(mapCoords, component.Radius);
            if (!puddles.Any())
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-suck-liquids-failure-message-user"), uid, uid);
            }
            bool suckedAnything = false;
            foreach (var puddle in puddles)
            {
                if (!_solutionSystem.TryGetSolution(puddle.Owner, "puddle", out var puddleSolution))
                    continue;
                var puddleSol = puddleSolution.Value.Comp.Solution;
                // Determine how much space is left in the entity's tank
                var availableSpace = tank.MaxVolume - tank.Volume;
                if (availableSpace <= FixedPoint2.Zero)
                {
                    _popup.PopupEntity(Loc.GetString("smile-popup-suck-liquids-full-message-user"), uid, uid);
                    break; // Your storage tank is totally full
                }

                // Take liquid from puddle and add to tank
                var transferAmount = FixedPoint2.Min(availableSpace, puddleSol.Volume);
                var split = _solutionSystem.SplitSolution(puddleSolution.Value, transferAmount);

                // 6. Inject fluid into your internal tank entity Smile
                _solutionSystem.TryAddSolution(tankSolution.Value, split);
                suckedAnything = true;
            }

            if (suckedAnything)
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-suck-liquids-message-others", ("entity", uid)), uid, Filter.PvsExcept(uid), true);
                _popup.PopupEntity(Loc.GetString("smile-popup-suck-liquids-message-user"), uid, uid);
                args.Handled = true;
            }
        }

        private void OnEmptyLiquid(EntityUid uid, ReagentSuckerComponent component, EmptyLiquid args)
        {
            var @event = new EmptyLiquidDoAfterEvent();
            // time it takes to activate ability: TimeSpan.FromSeconds(X) X = number of seconds
            var doAfter = new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(6), @event, uid)
            {
                BreakOnDamage = true,
                BreakOnMove = true
            };
            if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var tankSolution))
                return;
            var tank = tankSolution.Value.Comp.Solution; // get the solution
            if (tank.Volume <= FixedPoint2.Zero)
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-empty-liquids-failure-message-user"), uid, uid);
                return; // nothing to empty
            }
            _doAfter.TryStartDoAfter(doAfter);
        }

        private void OnDoAfterEmpty(EntityUid uid, ReagentSuckerComponent component, EmptyLiquidDoAfterEvent args)
        {
            if (args.Cancelled || args.Handled)
                return;
            var emptiedAnything = false;
            // get the internal tank.
            if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var tankSolution))
                return;
            var tank = tankSolution.Value.Comp.Solution; // get the solution
            if (tank.Volume <= FixedPoint2.Zero)
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-empty-liquids-failure-message-user"), uid, uid);
                return; // nothing to empty
            }
            // get coords for underneath the entity.
            var coordinates = Transform(uid).Coordinates;
            // get all fluid from tank
            var allFluid = _solutionSystem.SplitSolution(tankSolution.Value, tank.Volume);
            // spawn puddle at coords
            _puddleSystem.TrySpillAt(coordinates, allFluid, out _);
            emptiedAnything = true;
            if (emptiedAnything)
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-empty-liquids-message-others", ("entity", uid)), uid, Filter.PvsExcept(uid), true);
                _popup.PopupEntity(Loc.GetString("smile-popup-empty-liquids-message-user"), uid, uid);
                args.Handled = true;
            }
        }

        private void OnSprayLiquid(EntityUid uid, ReagentSuckerComponent component, SprayLiquid args)
        {
            var sprayedAnything = false;
            if (args.Handled)
                return;
            if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var tankSolution))
                return;
            var tank = tankSolution.Value.Comp.Solution; // get the solution
            if (tank.Volume <= FixedPoint2.Zero)
            {
                _popup.PopupEntity(Loc.GetString("smile-popup-spray-liquids-failure-message-user"), uid, uid);
                args.Handled = true;
                return; // nothing to empty
            }
            var xformQuery = GetEntityQuery<TransformComponent>();
            var sprayerXform = xformQuery.GetComponent(uid);
            var sprayerMapPos = _transform.GetMapCoordinates(sprayerXform);

            var xform = Transform(uid);
            var throwing = xform.LocalRotation.ToWorldVec() * component.SprayDistance;
            var direction = xform.Coordinates.Offset(throwing);
            var clickMapPos = _transform.ToMapCoordinates(direction);

            var diffPos = clickMapPos.Position - sprayerMapPos.Position;
            if (diffPos == Vector2.Zero || diffPos == Vector2Helpers.NaN)
                return;

            var diffNorm = diffPos.Normalized();
            var diffLength = diffPos.Length();

            if (diffLength > component.SprayDistance)
            {
                diffLength = component.SprayDistance;
            }

            var diffAngle = diffNorm.ToAngle();

            var amount = Math.Max(Math.Min((tank.Volume / component.TransferAmount).Int(), component.VaporAmount), 1);
            var spread = component.VaporSpread / amount;
            for (var i = 0; i < amount; i++)
            {
                var rotation = new Angle(diffAngle + Angle.FromDegrees(spread * i) - Angle.FromDegrees(spread * (amount - 1) / 2));
                var directionVector = rotation.ToVec();

                // Drain the fluid resource per shot
                var adjustedSolutionAmount = component.TransferAmount / component.VaporAmount;
                var consumed = _solutionSystem.SplitSolution(tankSolution.Value, adjustedSolutionAmount);
                if (consumed.Volume <= FixedPoint2.Zero)
                    break;

                var bullet = Spawn("BulletWaterShot", sprayerMapPos);
                var bulletXform = xformQuery.GetComponent(bullet);

                // Offsets the sprite rotation if the base artwork points up instead of right
                var spriteCorrection = Angle.FromDegrees(90);
                _transform.SetWorldRotation(bulletXform, rotation + spriteCorrection);

                if (_solutionSystem.TryGetSolution(bullet, "vapor", out var bulletSolution))
                {
                    _solutionSystem.TryAddSolution(bulletSolution.Value, consumed);
                }

                // Set the color of the bullet to match the exact liquid solution color
                if (TryComp<AppearanceComponent>(bullet, out var appearance))
                {
                    var liquidColor = tank.GetColor(_proto).WithAlpha(1f);

                    // Updates the visual layer using your prototype's VaporVisuals data
                    _appearance.SetData(bullet, VaporVisuals.Color, liquidColor, appearance);
                    _appearance.SetData(bullet, VaporVisuals.State, true, appearance);
                }

                if (TryComp<PhysicsComponent>(bullet, out var bulletPhysics))
                {
                    _physics.SetLinearVelocity(bullet, Vector2.Zero, body: bulletPhysics);
                    _physics.ApplyLinearImpulse(bullet, directionVector * component.SprayVelocity, body: bulletPhysics);
                }

                // Set shooter data so the bullet knows who to ignore (don't shoot yourself)
                if (TryComp<ProjectileComponent>(bullet, out var projComp))
                {
                    _projectile.SetShooter(bullet, projComp, uid);
                }

                var thingGettingPushed = uid;
                if (_container.TryGetOuterContainer(uid, sprayerXform, out var container))
                    thingGettingPushed = container.Owner;

                if (TryComp<PhysicsComponent>(thingGettingPushed, out var body))
                {
                    var impulseDir = rotation.ToVec();
                    if (_gravity.IsWeightless(thingGettingPushed))
                    {
                        _physics.ApplyLinearImpulse(thingGettingPushed, -impulseDir * component.PushbackAmount, body: body);
                    }
                    else
                    {
                        var userTransform = Transform(thingGettingPushed);
                        if (userTransform.GridUid == userTransform.ParentUid)
                        {
                            _physics.ApplyLinearImpulse(userTransform.GridUid.Value, -impulseDir * _gridImpulseMultiplier * component.PushbackAmount, userTransform.LocalPosition);
                        }
                    }
                }
            }
            sprayedAnything = true;

            if(sprayedAnything)
            {
                _audio.PlayPvs(component.SpraySound, uid, component.SpraySound.Params.WithVariation(0.125f));
                args.Handled = true;
            }
        }
    }
