using Bloodthirst.Core.BISD.CodeGeneration;
using Bloodthirst.System.CommandSystem;
namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class CommandExecuteCodeGenerator<TGenerator> : CommandInstant<CommandExecuteCodeGenerator<TGenerator>, bool> where TGenerator : ICodeGenerator , new()
    {
        private readonly BISDInfoContainer infoContainer;
        private readonly bool lazyGeneration;
        private readonly TGenerator generator;

        public CommandExecuteCodeGenerator(BISDInfoContainer infoContainer, bool lazyGeneration)
        {
            this.infoContainer = infoContainer;
            this.lazyGeneration = lazyGeneration;
            generator = new TGenerator();
        }

        protected override bool GetResult()
        {
            if (!lazyGeneration
               ||
               (lazyGeneration && generator.ShouldInject(infoContainer)))
            {
                generator.InjectGeneratedCode(infoContainer);
                return true;
            }

            return false;
        }
    }

    public class CommandExecuteCodeGenerator : CommandInstant<CommandExecuteCodeGenerator, bool>
    {
        private readonly BISDInfoContainer infoContainer;
        private readonly bool lazyGeneration;
        private readonly ICodeGenerator generator;

        public CommandExecuteCodeGenerator(BISDInfoContainer infoContainer, ICodeGenerator generator, bool lazyGeneration)
        {
            this.infoContainer = infoContainer;
            this.lazyGeneration = lazyGeneration;
            this.generator = generator;
        }

        protected override bool GetResult()
        {
            if (!lazyGeneration
                ||
                (lazyGeneration && generator.ShouldInject(infoContainer)))
            {
                generator.InjectGeneratedCode(infoContainer);
                return true;
            }

            return false;
        }
    }
}
