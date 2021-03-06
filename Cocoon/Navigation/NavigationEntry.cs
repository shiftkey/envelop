﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Helpers;

namespace Cocoon.Navigation
{
    internal class NavigationEntry
    {
        // *** Fields ***

        private readonly string pageName;
        private object state;

        // *** Constructors ***

        public NavigationEntry(string pageName, object arguments)
        {
            this.pageName = pageName;
            this.Arguments = arguments;
        }

        public NavigationEntry(string pageName, byte[] argumentsData, byte[] stateData)
        {
            this.pageName = pageName;
            this.ArgumentsData = argumentsData;
            this.StateData = stateData;
        }

        // *** Properties ***

        public object Arguments
        {
            get;
            private set;
        }

        public byte[] ArgumentsData
        {
            get;
            private set;
        }

        public IViewLifetimeContext ViewLifetimeContext
        {
            get;
            set;
        }

        public string PageName
        {
            get
            {
                return pageName;
            }
        }

        public object State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                StateData = null;
            }
        }

        public byte[] StateData
        {
            get;
            private set;
        }

        // *** Methods ***

        public void DeserializeData(Type argumentsType, Type stateType)
        {
            if (Arguments == null && ArgumentsData != null)
                Arguments = SerializationHelper.DeserializeFromArray(ArgumentsData, argumentsType);

            if (State == null && StateData != null)
                state = SerializationHelper.DeserializeFromArray(StateData, stateType);
        }

        public void SerializeData(Type argumentsType, Type stateType)
        {
            if (ArgumentsData == null && Arguments != null)
                ArgumentsData = SerializationHelper.SerializeToArray(Arguments, argumentsType);

            if (StateData == null && State != null)
                StateData = SerializationHelper.SerializeToArray(State, stateType);
        }

        public void DisposeCachedItems()
        {
            if (ViewLifetimeContext != null)
            {
                ViewLifetimeContext.Dispose();
                ViewLifetimeContext = null;
            }
        }
    }
}
