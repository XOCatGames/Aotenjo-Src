using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class RegistrationManager
    {
        private readonly Dictionary<Type, Dictionary<string, IRegisterable>> registry = new();
        
        public void Register(IRegisterable registerable, Type type)
        {
            registry[type][registerable.GetRegName()] = registerable;
        }
        
        public IRegisterable CreateNewInstance(string regName, Type type)
        {
            IRegisterable registerable = registry[type].GetValueOrDefault(regName);
            if (registerable == null)
            {
                throw new ArgumentException($"No registered instance found for {regName} of type {type.Name}");
            }
            // Create a new instance of the registerable
            IRegisterable newInstance = (IRegisterable)Activator.CreateInstance(type);
            return newInstance;
        }
    }
}