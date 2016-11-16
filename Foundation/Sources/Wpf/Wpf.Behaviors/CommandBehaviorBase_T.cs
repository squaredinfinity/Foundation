using  xxx.Foundation.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace  xxx.Foundation.Presentation.Behaviors
{
    public class CommandBehaviorBase<T> where T : System.Windows.UIElement
    {
        readonly EventHandler CommandCanExecuteChangedHandler;

        ICommand _command;
        /// <summary>
        /// Corresponding command to be execute and monitored for <see cref="ICommand.CanExecuteChanged"/>
        /// </summary>
        public ICommand Command
        {
            get { return _command; }
            set
            {
                if (this._command != null)
                {
                    this._command.CanExecuteChanged -= this.CommandCanExecuteChangedHandler;
                }

                this._command = value;
                if (this._command != null)
                {
                    this._command.CanExecuteChanged += this.CommandCanExecuteChangedHandler;
                    UpdateEnabledState();
                }
            }
        }

        object _commandStateChangeTrigger;
        public object CommandStateChangeTrigger
        {
            get { return _commandStateChangeTrigger; }
            set
            {
                if (_commandStateChangeTrigger != value)
                {
                    _commandStateChangeTrigger = value;
                    UpdateEnabledState();
                }
            }
        }

        object _commandParameter;
        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter
        {
            get { return this._commandParameter; }
            set
            {
                if (this._commandParameter != value)
                {
                    this._commandParameter = value;
                    this.UpdateEnabledState();
                }
            }
        }

        WeakReference _targetObject;
        /// <summary>
        /// Object to which this behavior is attached.
        /// </summary>
        protected T TargetObject
        {
            get
            {
                return _targetObject.Target as T;
            }
            private set
            {
                _targetObject = new WeakReference(value);
            }
        }

        /// <summary>
        /// Constructor specifying the target object.
        /// </summary>
        /// <param name="targetObject">The target object the behavior is attached to.</param>
        public CommandBehaviorBase(T targetObject)
        {
            this.TargetObject = targetObject;

            // In Silverlight, unlike WPF, this is strictly not necessary since the Command properties
            // in Silverlight do not expect their CanExecuteChanged handlers to be weakly held,
            // but holding on to them in this manner should do no harm.
            this.CommandCanExecuteChangedHandler = new EventHandler(this.CommandCanExecuteChanged);
        }

        /// <summary>
        /// Updates the target object's IsEnabled property based on the commands ability to execute.
        /// </summary>
        protected virtual void UpdateEnabledState()
        {
            if (TargetObject == null)
            {
                this.Command = null;
                this.CommandParameter = null;
            }
            else if (this.Command != null)
            {
                TargetObject.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

        /// <summary>
        /// Executes the command, if it's set, providing the <see cref="CommandParameter"/>
        /// </summary>
        protected virtual void ExecuteCommand()
        {
            if (this.Command != null && this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
        }
    }
}
