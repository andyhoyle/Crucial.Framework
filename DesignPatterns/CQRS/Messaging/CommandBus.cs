﻿using System;
using System.Collections.Generic;
using Crucial.DesignPatterns.CQRS.Commands;
using Crucial.DesignPatterns.CQRS.Exceptions;
using Crucial.DesignPatterns.CQRS.Utils;

namespace Crucial.DesignPatterns.CQRS.Messaging
{
    public class CommandBus:ICommandBus
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;

        public CommandBus(ICommandHandlerFactory commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
        }

        public void Send<T>(T command) where T : Command
        {
            var handler = _commandHandlerFactory.GetHandler<T>();
            if (handler != null)
            {
                handler.Execute(command);
            }
            else
            {
                throw new UnregisteredDomainCommandException("no handler registered");
            }
        }        
    }
}
