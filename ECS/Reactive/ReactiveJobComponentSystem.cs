using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;

namespace BovineLabs.Toolkit.Reactive
{
    public abstract class ReactiveJobComponentSystem : JobComponentSystem
    {
        private readonly Dictionary<IReactiveAddRemoveGroup, KeyValuePair<ComponentGroup, ComponentGroup>> _addRemoveGroups =
            new Dictionary<IReactiveAddRemoveGroup, KeyValuePair<ComponentGroup, ComponentGroup>>();

        private readonly Dictionary<IReactiveAddRemoveGroup, IReactiveUpdateGroup> _groupMap =
            new Dictionary<IReactiveAddRemoveGroup, IReactiveUpdateGroup>();
        
        protected ComponentGroup GetReactiveAddGroup(params ComponentType[] componentTypes)
        {
            return GetReactiveAddGroup(componentTypes, new ComponentType[0]);
        }

        protected ComponentGroup GetReactiveRemoveGroup(params ComponentType[] componentTypes)
        {
            return GetReactiveRemoveGroup(componentTypes, new ComponentType[0]);
        }

        protected ComponentGroup GetReactiveAddGroup(ComponentType[] componentTypes, ComponentType[] conditionTypes)
        {
            return GetGroups(componentTypes, conditionTypes).Key;
        }

        protected ComponentGroup GetReactiveRemoveGroup(ComponentType[] componentTypes, ComponentType[] conditionTypes)
        {
            return GetGroups(componentTypes, conditionTypes).Value;
        }

        protected ComponentGroup GetReactiveUpdateGroup(ComponentType componentType)
        {
            var addRemove = ReactiveTypeHelper.GetReactiveAddRemoveGroup(new[] {componentType}, new ComponentType[0]);
            
            if (!_groupMap.TryGetValue(addRemove, out var update))
                update = _groupMap[addRemove] = ReactiveTypeHelper.GetReactiveUpdateGroup(World, addRemove);

            return GetComponentGroup(update.Components); 
        }
                
        private KeyValuePair<ComponentGroup, ComponentGroup> GetGroups(ComponentType[] componentTypes, ComponentType[] conditionTypes)
        {
            var group = ReactiveTypeHelper.GetReactiveAddRemoveGroup(componentTypes, conditionTypes);
            if (!_addRemoveGroups.TryGetValue(group, out var groups))
            {
                var addGroup = GetComponentGroup(group.AddComponents);
                var removeGroup = GetComponentGroup(group.RemoveComponents);

            groups = _addRemoveGroups[group] =
                    new KeyValuePair<ComponentGroup, ComponentGroup>(addGroup, removeGroup);
            }

            return groups;
        }
        
        private ReactiveBarrierSystem _barrier;

        protected override void OnCreateManager(int capacity)
        {
            // We need to add our barrier manually as base class injection doesn't work
            _barrier = World.GetOrCreateManager<ReactiveBarrierSystem>();
            var barrierListField = typeof(JobComponentSystem)
                .GetField("m_BarrierList", BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.IsNotNull(barrierListField, "Unity probably renamed m_BarrierList or changed how barriers added.");
            
            var barriers = new List<BarrierSystem>((BarrierSystem[]) barrierListField.GetValue(this)) {_barrier};
            barrierListField.SetValue(this, barriers.ToArray());
        }

        protected sealed override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Assert.IsNotNull(_barrier,
                "_reactiveBarrierSystem is null, make sure you call base.OnCreateManager(int capacity) wasn't called");
            
            inputDeps = OnReactiveUpdate(inputDeps);

            foreach (var groups in _addRemoveGroups)
            {
                var group = groups.Key;

                var addEntities = groups.Value.Key.GetEntityArray();
                var removeEntities = groups.Value.Value.GetEntityArray();
                
                inputDeps = group.CreateAddJob(inputDeps, addEntities, _barrier.CreateCommandBuffer());
                inputDeps = group.CreateRemoveJob(inputDeps, removeEntities, _barrier.CreateCommandBuffer());

                var isUpdateGroup = _groupMap.TryGetValue(group, out var updateGroup);

                if (isUpdateGroup)
                {
                    inputDeps = updateGroup.CreateAddJob(inputDeps, addEntities, _barrier.CreateCommandBuffer());
                    inputDeps = updateGroup.CreateRemoveJob(inputDeps, removeEntities, _barrier.CreateCommandBuffer());
                }
            }
            
            return inputDeps;
        }



        protected abstract JobHandle OnReactiveUpdate(JobHandle inputDeps);

        

        
    }
}