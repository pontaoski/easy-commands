﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyCommands
{
    /// <summary>
    /// Command delegate for command with sub-commands
    /// </summary>
    class CommandGroupDelegate<TSender> : CommandDelegate<TSender>
    {
        //TODO: change this to a CommandDelegate to support sub-sub-sub-...-commands?
        private Dictionary<string, BaseCommandDelegate<TSender>> subcommands = new Dictionary<string, BaseCommandDelegate<TSender>>();

        public CommandGroupDelegate(TextOptions options, ArgumentParser parser, string name) : base(options, parser, name) { }

        public override void Invoke(TSender sender, IEnumerable<string> args)
        {
            if(args.Count() == 0)
            {
                //TODO: proper list
                throw new CommandParsingException(string.Format(textOptions.ShowSubcommands, Name));
            }
            string subcommand = args.First();
            if(!subcommands.ContainsKey(subcommand))
            {
                throw new CommandParsingException(
                    string.Format(textOptions.CommandNotFound, $"{Name} {subcommand}") + "\n"
                    + string.Format(textOptions.ShowSubcommands, Name));
            }
            subcommands[subcommand].Invoke(sender, args.ToList().GetRange(1, args.Count() - 1));
        }

        public void AddSubcommand(BaseCommandDelegate<TSender> command, string[] names)
        {
            foreach(string name in names)
            {
                if(subcommands.ContainsKey(name))
                {
                    throw new CommandRegistrationException($"Failed to register command \"{Name} {name}\" because it is a duplicate.");
                }
                subcommands[name] = command;
            }
        }

        public void AddSubcommand(MethodInfo command, string[] names)
        {
            AddSubcommand(new BaseCommandDelegate<TSender>(textOptions, parser, names[0], command), names);
        }
    }
}
