using System;

namespace ObjectPrinting
{
    public class TypeConfig<TOwner,TType> : ITypeConfig<TOwner>
    {
        private PrintingConfig<TOwner> printingConfig;
        public TypeConfig(PrintingConfig<TOwner> config) => printingConfig = config;
        public PrintingConfig<TOwner> Use(Func<TType, string> func)
        {
            ((IPrintingConfig) printingConfig).TypeSerialize[typeof(TType)] = p => func((TType) p);
            return printingConfig;
        }

        PrintingConfig<TOwner> ITypeConfig<TOwner>.ParentConfig => printingConfig;
    }
    public interface ITypeConfig<TOwner>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}
