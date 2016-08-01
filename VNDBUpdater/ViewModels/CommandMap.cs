using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;


/// <summary>
/// Allows using command collection.
/// See https://blogs.msdn.microsoft.com/morgan/2010/06/24/simplifying-commands-in-mvvm-and-wpf/ for details.
/// </summary>
namespace VNDBUpdater.ViewModels
{
    /// <summary>
    /// A map that exposes commands in a WPF binding friendly manner
    /// </summary>
    [TypeDescriptionProvider(typeof(CommandMapDescriptionProvider))]
    public class CommandMap
    {
        private Dictionary<string, ICommand> _commands;

        public void AddCommand(string commandName, Action<object> executeMethod)
        {
            Commands[commandName] = new DelegateCommand(executeMethod);
        }

        public void AddCommand(string commandName, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            Commands[commandName] = new DelegateCommand(executeMethod, canExecuteMethod);
        }

        public void RemoveCommand(string commandName)
        {
            Commands.Remove(commandName);
        }

        protected Dictionary<string, ICommand> Commands
        {
            get
            {
                if (null == _commands)
                    _commands = new Dictionary<string, ICommand>();

                return _commands;
            }
        }
        
        /// <summary>
        /// Implements ICommand in a delegate friendly way
        /// </summary>
        private class DelegateCommand : ICommand
        {
            private Predicate<object> _canExecuteMethod;
            private Action<object> _executeMethod;

            public DelegateCommand(Action<object> executeMethod) : this(executeMethod, null) { }

            public DelegateCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
            {
                if (null == executeMethod)
                    throw new ArgumentNullException("executeMethod");

                _executeMethod = executeMethod;
                _canExecuteMethod = canExecuteMethod;
            }

            public bool CanExecute(object parameter)
            {
                return (null == _canExecuteMethod) ? true : _canExecuteMethod(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                _executeMethod(parameter);
            }
        }


        /// <summary>
        /// Expose the dictionary entries of a CommandMap as properties
        /// </summary>
        private class CommandMapDescriptionProvider : TypeDescriptionProvider
        {
            public CommandMapDescriptionProvider()
                : this(TypeDescriptor.GetProvider(typeof(CommandMap)))
            {
            }

            public CommandMapDescriptionProvider(TypeDescriptionProvider parent)
                : base(parent)
            {
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return new CommandMapDescriptor(base.GetTypeDescriptor(objectType, instance), instance as CommandMap);
            }
        }

        /// <summary>
        /// This class is responsible for providing custom properties to WPF - in this instance
        /// allowing you to bind to commands by name
        /// </summary>
        private class CommandMapDescriptor : CustomTypeDescriptor
        {
            private CommandMap _map;

            public CommandMapDescriptor(ICustomTypeDescriptor descriptor, CommandMap map)
                : base(descriptor)
            {
                _map = map;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                var props = new PropertyDescriptor[_map.Commands.Count];

                int pos = 0;

                foreach (KeyValuePair<string, ICommand> command in _map.Commands)
                    props[pos++] = new CommandPropertyDescriptor(command);

                return new PropertyDescriptorCollection(props);
            }
        }

        /// <summary>
        /// A property descriptor which exposes an ICommand instance
        /// </summary>
        private class CommandPropertyDescriptor : PropertyDescriptor
        {
            private ICommand _command;

            public CommandPropertyDescriptor(KeyValuePair<string, ICommand> command)
                : base(command.Key, null)
            {
                _command = command.Value;
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { throw new NotImplementedException(); }
            }

            public override object GetValue(object component)
            {
                var map = component as CommandMap;

                if (null == map)
                    throw new ArgumentException("component is not a CommandMap instance", "component");

                return map.Commands[this.Name];
            }

            public override Type PropertyType
            {
                get { return typeof(ICommand); }
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                throw new NotImplementedException();
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }            
        }
    }
}
