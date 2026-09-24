using System;

namespace Aotenjo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SubscribeToEventAttribute : Attribute
    {
    }
}